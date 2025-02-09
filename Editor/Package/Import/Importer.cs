#nullable enable
// TODO: そのうち Newtonsoft.Json も JsonUtility も使わない、新しいJSONパーサーを作る。
//  設計思想:
//  1. 型システムとアナライザーによってスキーマが論理的に間違っているときは絶対にコンパイルエラーになる。
//  2. エンドユーザーが意識するのはジェネリックな関数である serializer.Serialize<T>(T): CharSequence<Encoding> と
//     deserializer.Deserialize<T>(CharSequence<Encoding>): T のみ。
//  3. untyped なパースを行う際は dynamic 型を用い、 JSONの抽象構文木を意識させるAPI設計はしない。
//  4. 相互運用性を最大限高めるため、入出力において UTF-8 と UTF-16 のどちらも対応する。
//  5. 属性を用いたソースジェネレーターによってボイラープレートを最小限にしながらパフォーマンスの良いコードを生成する。
//  6. メモリアロケーションを極力行わない。
//  7. Unity でも動作する C# のサブセットで記述する。

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using ResoniteImportHelper.Editor.Package.Asset.Types;
using ResoniteImportHelper.Package.Import.Metadata;
using ResoniteImportHelper.Package.Import.Stub;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ResoniteImportHelper.Package.Import
{
    [ScriptedImporter(1, "resonitepackage")]
    internal class Importer : ScriptedImporter
    {
        private delegate void OnInitializeGameObject(GameObject target, string deserializedSlotId);

        public override void OnImportAsset(AssetImportContext ctx)
        {
            // TODO: adjust this later
            var rootGameObject = new GameObject();
            ctx.AddObjectToAsset("$$main", rootGameObject);
            ctx.SetMainObject(rootGameObject);

            using var zipArchive = ZipFile.OpenRead(ctx.assetPath);

            var mainRecordEntry = zipArchive.GetEntry("R-Main.record");
            if (mainRecordEntry == null)
            {
                throw new FormatException("ResonitePackage must contain R-Main.record under the archive root.");
            }

            {
                // TODO: validate only -> UTF-8 only JSON parser to faster parse?
                var recordManifest = ReadZipArchiveContentAsUtf8Sequence(mainRecordEntry);
                Debug.Log($"root decoded: {recordManifest}");

                var pd = JsonConvert.DeserializeObject<PartialDescriptor>(recordManifest);
                if (pd is null)
                {
                    throw new FormatException("Failed to deserialize descriptor");
                }

                var manifests = pd.AssetManifest;

                var metadataArchiveEntries = zipArchive.Entries.Where(a => a.FullName.StartsWith("Metadata/")).ToList();
                var mainArchiveEntries = zipArchive.Entries.Where(a => a.FullName.StartsWith("Assets/")).ToList();

                var mainDataTreeEntry =
                    mainArchiveEntries.SingleOrDefault(e => e.Name == pd.AssetUri.Replace("packdb:///", ""));

                if (mainDataTreeEntry is null)
                {
                    throw new FormatException($"Failed to find main DataTreeDirectory: {pd.AssetUri} is not contained by the package.");
                }

                var mainDataTree = ReadZipArchiveContent(mainDataTreeEntry);
                if (!IsValidDataTreeHeader(mainDataTree[0..4]))
                {
                    ctx.LogImportError("The main data tree do not have correct header magic.");
                    throw new System.FormatException("The main data tree do not have correct header magic.");
                }

                Debug.Log("mode");
                // TODO: 本来ここはVarIntで読むのが正しいが、一旦無視
                var compressionMode = mainDataTree[8];
                Stream? stream;
                if (compressionMode == 3)
                {
                    Debug.Log("Main record is compressed by Brotli.");
                    stream = new BrotliStream(new MemoryStream(mainDataTree[9..]),
                        CompressionMode.Decompress);
                } else if (compressionMode == 0)
                {
                    Debug.Log("Main record is not compressed.");
                    stream = new MemoryStream(mainDataTree[9..]);
                }
                else
                {
                    ctx.LogImportWarning($"Main record is compressed by unrecognized format. (kind: {compressionMode}");
                    stream = null;
                }

                if (stream != null)
                {
                    using (stream)
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        using var bsonTree1 = new BsonReader(stream);
#pragma warning restore CS0618 // Type or member is obsolete
                        var s = new JsonSerializer();
                        var rawRoot = s.Deserialize(bsonTree1);
                        var toAdd = ScriptableObject.CreateInstance<MainTreeAsset>();
                        toAdd.name = "DecodedMainRecord";
                        toAdd.text = rawRoot?.ToString() ?? "null";
                        ctx.AddObjectToAsset("DecodedMainRecord", toAdd);
                    }

                    Stream stream2;
                    if (compressionMode == 3)
                    {
                        Debug.Log("Main record is compressed by Brotli.");
                        stream2 = new BrotliStream(new MemoryStream(mainDataTree[9..]),
                            CompressionMode.Decompress);
                    } else if (compressionMode == 0)
                    {
                        Debug.Log("Main record is not compressed.");
                        stream2 = new MemoryStream(mainDataTree[9..]);
                    }
                    else
                    {
                        throw new NotImplementedException("unreachable");
                    }

                    using (stream2)
                    {
                        var graphRoot = DeserializeFromBsonStream<GraphRoot>(stream2);
                        if (graphRoot == null)
                        {
                            ctx.LogImportWarning("root deserialize failed");
                        }
                        else
                        {
                            ConstructBaseHierarchy(graphRoot, rootGameObject, (go, id) =>
                            {
                                Debug.Log($"{go} was initialized! ID: {id}");
                                ctx.AddObjectToAsset($"GameObject_{id}", go);
                            });
                            ImportTexture(ctx, graphRoot, rootGameObject, mainArchiveEntries);
                            ImportMesh(ctx, graphRoot, rootGameObject);
                            ImportMaterial(ctx, graphRoot, rootGameObject);
                            ConstructRenderers(ctx, graphRoot, rootGameObject);
                            var animator = ConstructAnimator(ctx, graphRoot, rootGameObject);
                            ConstructAnimation(ctx, graphRoot, rootGameObject, animator);
                        }
                    }
                }
            }
        }

        private static TValue? DeserializeFromBsonStream<TValue>(Stream stream)
        {
            var s = new JsonSerializer();
#pragma warning disable CS0618 // Type or member is obsolete
            using var bsonTree = new BsonReader(stream);
#pragma warning restore CS0618 // Type or member is obsolete

            var value = s.Deserialize<TValue>(bsonTree);

            return value;
        }

        private static void ConstructBaseHierarchy(GraphRoot root, GameObject rootGo, OnInitializeGameObject onInitializeGameObject)
        {
            Debug.Log($"Software build: {root.VersionNumber}");
            Debug.Log($"Feature Flags: {Prettify(root.FeatureFlags)}");
            Debug.Log($"Types: {Prettify(root.Types)}");
            Debug.Log($"TypeVersions: {Prettify(root.TypeVersions)}");
            Debug.Log($"Object: {root.RootSlot}");
            Debug.Log($"Assets: {Prettify(root.ContainedAssets)}");

            AttachSlotRecursively(new Dictionary<string, int>(), root.RootSlot, rootGo, onInitializeGameObject, true);
        }

        private static void AttachSlotRecursively(Dictionary<string, int> versions, Slot slot, GameObject go, OnInitializeGameObject onInitializeGameObject, bool isRoot)
        {
            var slotName = slot.Name.Data;
            if (versions.TryGetValue(slotName, out var previousVersion))
            {
                var currentVersion = previousVersion + 1;
                Debug.Log($"Duplicated name: {slotName} on ID={slot.ID}; version: {previousVersion} -> {currentVersion}");
                versions[slotName] = currentVersion;
                slotName += $".{currentVersion}";
            }
            else
            {
                versions[slotName] = 0;
            }
            // go.nameはヒエラルキー上で一意である必要があるらしい。知るか！
            go.name = slotName;
            go.transform.SetLocalPositionAndRotation(slot.Position, slot.Rotation);
            go.transform.localScale = slot.Scale;
            if (!isRoot)
            {
                onInitializeGameObject(go, slot.ID);
            }

            foreach (var child in slot.Children)
            {
                var childGo = new GameObject();
                childGo.transform.SetParent(go.transform);
                AttachSlotRecursively(versions, child, childGo, onInitializeGameObject, false);
            }
        }

        private static void ImportTexture(AssetImportContext ctx, GraphRoot root, GameObject rootGo, List<ZipArchiveEntry> assets)
        {
            var staticTexture2DProviderCandidate = root.Types
                .Select((e, i) => (e.Parse(), i))
                .Cast<((AssemblyName Assembly, TypeName typeName) parsed, int i)?>()
                .FirstOrDefault(t => t!.Value.parsed.Assembly.FullName == "FrooxEngine" && t!.Value.parsed.typeName.FullName == "FrooxEngine.StaticTexture2D");

            if (!staticTexture2DProviderCandidate.HasValue)
            {
                ctx.LogImportWarning("This target do not have StaticTexture2D, skipping.");
                return;
            }

            var staticTexture2DProviderIndex = staticTexture2DProviderCandidate.Value.i;

            // TODO: 今後Slotにあるコンポーネントを死ぬほど反復するが、それはO(nm)になって遅くないか？
            //       何らかのデータ構造の導入を検討するべき。

            /*
             TODO:
                Get variants and load largest one.
                -- https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Texture2D.LoadRawTextureData.html
                How can we handle mips=True case?
                * BC3 (w/ C.C.) => DXT5_Crunched
                * BC3 (w/o C.C.) => DXT5
                * BC1 (w/ C.C.) => DXT1_Crunched
                * BC1 (w/o C.C.) => DXT1
                * RawRGBA => RGBA32
                https://www.webtech.co.jp/blog/optpix_labs/format/6993/
                If there's no one, fallback to PNG and upload it with initial properties.
                Considering DXT7 on fallback-ed import?
            */

            ctx.LogImportWarning("This phase (ImportTexture) is not fully-implemented yet.");
        }

        private static void ImportMesh(AssetImportContext ctx, GraphRoot root, GameObject rootGo)
        {
            // TODO: Read Metadata/*.mesh (represented as JSON).
            ctx.LogImportWarning("This phase (ImportMesh) is not implemented yet.");
        }

        private static void ImportMaterial(AssetImportContext ctx, GraphRoot root, GameObject rootGo)
        {
            // TODO: Branch to control whether import as lilToon (or any toon shader that supports Matcap) or Standard Material (to emulate PBS).
            ctx.LogImportWarning("This phase (ImportMaterial) is not implemented yet.");
        }

        private static void ConstructRenderers(AssetImportContext ctx, GraphRoot root, GameObject rootGo)
        {
            // TODO: Read both MeshRenderer and SkinnedMeshRenderer.
            ctx.LogImportWarning("This phase (ConstructRenderers) is not implemented yet.");
        }

        private static Animator ConstructAnimator(AssetImportContext ctx, GraphRoot root, GameObject rootGo)
        {
            var animator = rootGo.AddComponent<Animator>();
            // TODO: read bones from VRIK component.
            ctx.LogImportWarning("This phase (ConstructAnimator) is not fully-implemented yet.");
            return animator;
        }

        private static void ConstructAnimation(AssetImportContext ctx, GraphRoot root, GameObject rootGo, Animator animator)
        {
            ctx.LogImportWarning("This phase(ConstructAnimation) is not implemented yet.");
        }

        private static string Prettify<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
        {
            return string.Join(",\n", dictionary.Select((a) => $"{a.Key}: {(a.Value)}"));
        }

        private static string Prettify<T>(IEnumerable<T> enumerable)
        {
            return string.Join(",\n", enumerable);
        }

        private static bool IsValidDataTreeHeader(ReadOnlySpan<byte> header)
        {
            return header.Length == 4 && header[0] == 'F' && header[1] == 'r' && header[2] == 'D' && header[3] == 'T';
        }

        private static byte[] ReadZipArchiveContent(ZipArchiveEntry entry)
        {
            var content = new byte[entry.Length];
            {
                using var meta = entry.Open();
                using var s = new BufferedStream(meta);

                s.Read(content);
            }

            return content;
        }

        private static string ReadZipArchiveContentAsUtf8Sequence(ZipArchiveEntry entry)
        {
            return Encoding.UTF8.GetString(ReadZipArchiveContent(entry));
        }

        internal sealed class FormatException : Exception
        {
            internal FormatException(string message) : base(message) {}
        }

        [Serializable]
        internal sealed class PartialDescriptor
        {
            [JsonProperty("assetUri")]
            internal string AssetUri;
            [JsonProperty("assetManifest")]
            internal List<AssetStatistic> AssetManifest;
        }

        [Serializable]
        internal sealed class AssetStatistic
        {
            [JsonProperty("hash")]
            internal string Hash;
            [JsonProperty("bytes")]
            internal int Length;
        }
    }
}
