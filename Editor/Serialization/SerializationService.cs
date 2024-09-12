using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ResoniteImportHelper.Allocator;
using ResoniteImportHelper.Backlink.Component;
using ResoniteImportHelper.UnityEditorUtility;
using MeshUtility = ResoniteImportHelper.UnityEditorUtility.MeshUtility;
using UniGLTF;
using UnityEditor;
using UnityEngine;

namespace ResoniteImportHelper.Serialization
{
    internal static class SerializationService
    {
        private static ExportingGltfData ConstructGltfOnMemory(GameObject target, bool containsVertexColors)
        {
            var data = new ExportingGltfData();
            {
                var exportSettings = new GltfExportSettings
                {
                    // https://github.com/KisaragiEffective/ResoniteImportHelper/issues/29
                    DivideVertexBuffer = false,
                    ExportVertexColor = containsVertexColors
                };

                using var exporter = new gltfExporter(data, exportSettings);
                exporter.Prepare(target);
                exporter.Export();
            }

            return data;
        }
        
        internal static ExportInformation ExportToAssetFolder(SerializationConfiguration config)
        {
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

            var serialized = ExportGltfToAssetFolder(target, containsVertexColors, config.Allocator);
            Debug.Log("backlink: started");
            TryCreateBacklink(config.OriginalMaybePackedObject, config.Allocator);
            Debug.Log("backlink: end");

            return new ExportInformation(serialized, containsVertexColors);
        }

        private static void SerializeIntermediateArtifact(GameObject processedModifiableRoot, ResourceAllocator allocator)
        {
            var path = allocator.BasePath + "/intermediate.prefab";
            PrefabUtility.SaveAsPrefabAsset(processedModifiableRoot, path);
        }
        
        // ReSharper disable once InconsistentNaming
        private static void TryCreateBacklink(GameObject original, ResourceAllocator allocator)
        {
            var source = PrefabUtility.GetCorrespondingObjectFromSource(original);
            if (source == null)
            {
                Debug.Log("backlink: skipping generation: the original object is not a Prefab");
                return;
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

            var hasLocalOverrides = PrefabUtility.HasPrefabInstanceAnyOverrides(original, false);
            GameObject parent;
            if (hasLocalOverrides)
            {
                Debug.Log("backlink: serialization: original object has local modification");
                // SaveAsPrefabAssetは**/*.prefabじゃないと例外を吐く。知るかよ！
                var path = allocator.BasePath + "/serialized_local_modification.prefab";
                var result = PrefabUtility.SaveAsPrefabAsset(original, path, out var success);
                if (!success)
                {
                    Debug.LogWarning("backlink: serialization: SaveAsPrefabAsset was failed");
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="temporary"></param>
        /// <param name="containsVertexColors"></param>
        /// <param name="runIdentifier"></param>
        /// <returns>Serialized object.</returns>
        private static GameObject ExportGltfToAssetFolder(GameObject temporary, bool containsVertexColors, ResourceAllocator allocator)
        {
#if RIH_HAS_UNI_GLTF
            Debug.Log("Absolute path to the Asset: " + Application.dataPath);
            var modelName = temporary.name ?? "model";
            var gltfAssetRelativePath = (allocator.BasePath + "/" + $"{modelName}.gltf")["Assets/".Length..];
            // dataPathはAssetsで終わることに注意！！
            var gltfFilePath = $"{Application.dataPath}/{gltfAssetRelativePath}";

            var onMemoryModelData = ConstructGltfOnMemory(temporary, containsVertexColors);

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

            var assetsRelPath = $"Assets/{gltfAssetRelativePath}";
            {
                AssetDatabase.ImportAsset(assetsRelPath);
                AssetDatabase.Refresh();
            }

            return AssetDatabase.LoadAssetAtPath<GameObject>(assetsRelPath);
#else
            throw new Exception("assertion error: UniGLTF is not installed on the project.");
#endif
        }
    }
}