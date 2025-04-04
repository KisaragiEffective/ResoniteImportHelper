#nullable enable
using System.IO;
using System.Linq;
using KisaragiMarine.ResoniteImportHelper.Allocator;
using KisaragiMarine.ResoniteImportHelper.Backlink.Component;
using KisaragiMarine.ResoniteImportHelper.UnityEditorUtility;
using MeshUtility = KisaragiMarine.ResoniteImportHelper.UnityEditorUtility.MeshUtility;
#if RIH_HAS_UNI_GLTF
using UniGLTF;
#else
using ExportingGltfData = System.Object;
#endif
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace KisaragiMarine.ResoniteImportHelper.Serialization
{
    internal static class SerializationService
    {
        private static ExportingGltfData ConstructGltfOnMemory(GameObject target, bool containsVertexColors)
        {
            #if RIH_HAS_UNI_GLTF
            Profiler.BeginSample("ConstructGltfOnMemory");
            var data = new ExportingGltfData();
            {
                var exportSettings = new GltfExportSettings
                {
                    // https://github.com/KisaragiEffective/ResoniteImportHelper/issues/29
                    DivideVertexBuffer = false,
                    ExportVertexColor = containsVertexColors,
                    // https://github.com/KisaragiEffective/ResoniteImportHelper/issues/242
                    InverseAxis = Axes.X
                };

                Profiler.BeginSample(".ctor");
                using var exporter = new gltfExporter(data, exportSettings);
                Profiler.EndSample();

                Profiler.BeginSample("Prepare");
                exporter.Prepare(target);
                Profiler.EndSample();

                Profiler.BeginSample("Export");
                exporter.Export();
                Profiler.EndSample();
            }
            Profiler.EndSample();

            return data;
            #else
            throw new System.Exception("assertion error");
#endif
        }

        internal static ExportInformation ExportToAssetFolder(SerializationConfiguration config)
        {
            Profiler.BeginSample("ExportToAssetFolder");

            var target = config.ProcessingTemporaryObjectRoot;
            GameObjectRecurseUtility.EnableAllChildrenWithRenderers(target);
            if (config.GenerateIntermediateArtifact)
            {
                SerializeIntermediateArtifact(target, config.Allocator);
            }
            var containsVertexColors = MeshUtility.GetMeshes(target).Any(m =>
            {
                var t = m.colors32;
                // if there are completely none => immediately false
                // if all of them is white => *assumes* there are effectively none
                return t.Any(c => c.r != 255 && c.g != 255 && c.b != 255 && c.a != 255);
            });

            var serialized = ExportGltfToAssetFolder(target, containsVertexColors, config.Allocator, true);
            Debug.Log("backlink: started");
            TryCreateBacklink(config.OriginalMaybePackedObject, config.Allocator);
            Debug.Log("backlink: end");

            Debug.Log("PostGLTF: done. reloading");
            AssetDatabase.Refresh();

            Profiler.EndSample();
            return new ExportInformation(serialized.LoadFromAssetDatabase(), containsVertexColors);
        }

        private static void SerializeIntermediateArtifact(GameObject processedModifiableRoot, ResourceAllocator allocator)
        {
            var path = allocator.BasePath + "/intermediate.prefab";
            PrefabUtility.SaveAsPrefabAsset(processedModifiableRoot, path);
        }

        // ReSharper disable once InconsistentNaming
        private static void TryCreateBacklink(GameObject original, ResourceAllocator allocator)
        {
            // SaveAsPrefabAssetは**/*.prefabじゃないと例外を吐く。知るかよ！
            var serializedLocalModificationPath = allocator.BasePath + "/serialized_local_modification.prefab";

            Profiler.BeginSample("TryCreateBacklink");

            var source = PrefabUtility.GetCorrespondingObjectFromSource(original);
            if (source == null)
            {
                // 後続のやつが失敗するので、PrefabじゃなくてもPrefabに仕立て上げる
                // *効率的に*unpackした元を知るすべはないので、接続はしない
                // (やろうと思えばできますが、果たしてそれは嬉しいでしょうか？)
                Debug.Log("backlink: serializing unpacked (or wild) asset as local modification");
                source = PrefabUtility.SaveAsPrefabAsset(original, serializedLocalModificationPath, out var success);
                if (!success)
                {
                    Debug.LogWarning("backlink: serialization: SaveAsPrefabAsset was failed");
                    Profiler.EndSample();
                    return;
                }
            }

            {
                var path = AssetDatabase.GetAssetPath(source) switch
                {
                    "" => "empty??",
                    null => "null",
                    var other => other,
                };

                Debug.Log($"backlink: path to Prefab: {path}");
            }

            bool hasLocalOverrides;

            {
                var oldRootPosition = original.transform.position;
                // 原点にないとPosition Constraintなどの座標を参照するコンポーネントの値が
                // 演算誤差によって本来の設定とは異なるためオーバーライドしたとして扱われてしまう
                original.transform.position = Vector3.zero;
                /*
                TODO: もうちょっとうまくやる。
                例えば位置を戻しただけではPos. ConstraintのOffsetをオーバーライドしたという
                勘違いは治らない。
                */
                hasLocalOverrides = PrefabUtility.HasPrefabInstanceAnyOverrides(original, false);
                // 検査が済んだら戻す
                original.transform.position = oldRootPosition;
            }

            GameObject parent;
            if (hasLocalOverrides)
            {
                Debug.Log("backlink: serialization: original object has local modification");
                var result = PrefabUtility.SaveAsPrefabAsset(original, serializedLocalModificationPath, out var success);
                if (!success)
                {
                    Debug.LogWarning("backlink: serialization: SaveAsPrefabAsset was failed");
                    Profiler.EndSample();
                    return;
                }

                parent = result;
            }
            else
            {
                Debug.Log("backlink: reusing pre-existing prefab: original object has no local modification");
                parent = source;
            }

            Debug.Log("backlink: create ScriptableObject");
            var o = ScriptableObject.CreateInstance<TiedBakeSourceDescriptor>();
            o.SerializedParent = parent;

            Debug.Log("backlink: finalize");
            AssetDatabase.CreateAsset(o, allocator.BasePath + "/tied.asset");
            Profiler.EndSample();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="temporary"></param>
        /// <param name="containsVertexColors"></param>
        /// <param name="runIdentifier"></param>
        /// <param name="allocator"></param>
        /// <param name="alignInitialMorphValues"></param>
        /// <returns>Lazily-loaded glTF as a <see cref="GameObject"/>.</returns>
        private static DelayedReference<GameObject> ExportGltfToAssetFolder(
            GameObject temporary,
            bool containsVertexColors,
            ResourceAllocator allocator,
            bool alignInitialMorphValues
        )
        {
            Profiler.BeginSample("ExportGltfToAssetFolder");
#if RIH_HAS_UNI_GLTF
            Debug.Log("Absolute path to the Asset: " + Application.dataPath);
            var modelName = temporary.name ?? "model";
            var gltfAssetRelativePath = (allocator.BasePath + "/" + $"{modelName}.gltf")["Assets/".Length..];
            // dataPathはAssetsで終わることに注意！！
            var gltfFilePath = $"{Application.dataPath}/{gltfAssetRelativePath}";

            var onMemoryModelData = ConstructGltfOnMemory(temporary, containsVertexColors);

            Profiler.BeginSample("Serialization");
            #region UniGLTF.GltfExportWindow から引用したGLTFを書き出す処理
            // SPDX-SnippetBegin
            // SPDX-License-Identifier: MIT
            // SPDX-SnippetName: UniGLTF.GltfExportWindow
            // SPFX-SnippetCopyrightText: Copyright (c) 2020 VRM Consortium
            // SPFX-SnippetCopyrightText: Copyright (c) 2018 Masataka SUMI for MToon

            var (json, buffer0) = onMemoryModelData.ToGltf(gltfFilePath);
            {
                // write JSON without BOM
                var encoding = new System.Text.UTF8Encoding(false);
                File.WriteAllText(gltfFilePath, json, encoding);
            }

            {
                // write to buffer0 local folder
                var dir = Path.GetDirectoryName(gltfFilePath);
                var bufferPath = Path.Combine(dir, buffer0.uri);
                File.WriteAllBytes(bufferPath, onMemoryModelData.BinBytes.ToArray());
            }
            // SPDX-SnippetEnd
            #endregion
            Profiler.EndSample();

            Profiler.BeginSample("Import and Refresh");
            var assetsRelPath = $"Assets/{gltfAssetRelativePath}";
            {
                AssetDatabase.ImportAsset(assetsRelPath);
                AssetDatabase.Refresh();
            }
            Profiler.EndSample();

            {
                var postGltf = new PostGltfService(File.ReadAllText(assetsRelPath));

                if (alignInitialMorphValues)
                {
                    postGltf.AlignInitialMorphValues(temporary);
                }
            }

            Profiler.EndSample();
            return new DelayedReference<GameObject>(AssetDatabase.AssetPathToGUID(assetsRelPath));
#else
            throw new System.Exception("assertion error: UniGLTF is not installed on the project.");
#endif
        }
    }
}
