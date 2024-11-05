#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ResoniteImportHelper.Editor.Package.Import.Stub;
using ResoniteImportHelper.Package.Import.Deserialize.Bitmap;
using ResoniteImportHelper.Package.Import.Deserialize.Mesh;
using ResoniteImportHelper.Package.Import.Deserialize.Metadata;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ResoniteImportHelper.Package.Import
{
    [ScriptedImporter(1, "resonitepackage")]
    internal class Importer : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            // TODO: adjust this later
            var root = new GameObject();
            ctx.AddObjectToAsset("$$main", root);
            ctx.SetMainObject(root);

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

                var manifests = JsonConvert.DeserializeObject<PartialDescriptor>(recordManifest)!.AssetManifest;

                var metadataArchiveEntries = zipArchive.Entries.Where(a => a.FullName.StartsWith("Metadata/")).ToList();
                var mainArchiveEntries = zipArchive.Entries.Where(a => a.FullName.StartsWith("Assets/")).ToList();

                foreach (var manifest in manifests)
                {
                    var hash = manifest.Hash;
                    // 拡張子を推定できないのでStartsWithでごまかす
                    var correspondingMetadataEntry = metadataArchiveEntries.SingleOrDefault(x => x.FullName.StartsWith($"Metadata/{hash}"));

                    if (correspondingMetadataEntry == null)
                    {
                        throw new FormatException($"ResonitePackage must contain Metadata for hash({hash}), but it's missing.");
                    }

                    var correspondingAssetEntry = mainArchiveEntries.SingleOrDefault(e => e.Name == hash);
                    if (correspondingAssetEntry == null)
                    {
                        throw new FormatException($"ResonitePackage must contain Asset for hash({hash}), but it's missing.");
                    }

                    Debug.Log($"extension is {correspondingMetadataEntry.Name.Split('.').Last()}");

                    var metaStr = ReadZipArchiveContentAsUtf8Sequence(correspondingMetadataEntry);

                    if (TryReadAs(metaStr, out MeshMetadata? meshMetadata))
                    {
                        var m = new Mesh();

                        // TODO: configure
                        m.bounds = new Bounds();
                        m.name = hash;

                        ctx.AddObjectToAsset(hash, m);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    } else if (TryReadAs(metaStr, out BitmapMetadata bitmapMetadata))
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    {
                        var t = new Texture2D((int) bitmapMetadata!.Width, (int) bitmapMetadata.Height);

                        t.LoadImage(ReadZipArchiveContent(correspondingAssetEntry));

                        var toAdd = new GameObject($"TextureHolder_{hash}")
                        {
                            transform =
                            {
                                parent = root.transform
                            }
                        };

                        var c = toAdd.AddComponent<StaticTexture2D>();
                        c.texture = t;

                        ctx.AddObjectToAsset(hash, toAdd);
                    }

                    Debug.Log($"meta for hash({hash}): \n{metaStr}");
                }
            }
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

        private string ReadZipArchiveContentAsUtf8Sequence(ZipArchiveEntry entry)
        {
            return Encoding.UTF8.GetString(ReadZipArchiveContent(entry));
        }

        private bool TryReadAs<T>(string metadataJson, out T? meta) where T: IMetadata<IAssetTag>
        {
            var meshMeta = JsonConvert.DeserializeObject<T>(metadataJson);
            if (meshMeta == null)
            {
                meta = default;
                return false;
            }

            meta = meshMeta;
            return true;
        }

        internal sealed class FormatException : Exception
        {
            internal FormatException(string message) : base(message) {}
        }

        [Serializable]
        internal sealed class PartialDescriptor
        {
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
