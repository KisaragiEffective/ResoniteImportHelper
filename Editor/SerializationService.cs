using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UniGLTF;
using UnityEditor;
using UnityEngine;

namespace ResoniteImportHelper.Editor
{
    internal static class SerializationService
    {
        private static ExportingGltfData ConstructGltfOnMemory(GameObject target, bool containsVertexColors)
        {
            var data = new ExportingGltfData();
            {
                var exportSettings = new GltfExportSettings
                {
                    // ???
                    DivideVertexBuffer = true,
                    ExportVertexColor = containsVertexColors
                };

                using var exporter = new gltfExporter(data, exportSettings);
                exporter.Prepare(target);
                exporter.Export();
            }

            return data;
        }
        private const string DestinationFolder = "ZZZ_TemporalAsset";
        
        private static void InitializeTemporalAssetDataDirectory(string secondaryDirectoryName)
        {
            #region sanity check
            {
                if (!new Regex("^[^*:\\/]+$").IsMatch(secondaryDirectoryName))
                {
                    throw new ArgumentException("shall not be empty nor contains special character",
                        nameof(secondaryDirectoryName));
                }
            }
            #endregion
            // System.GuidではなくUnityEditor.GUIDであることに注意
            if (AssetDatabase.GUIDFromAssetPath($"Assets/{DestinationFolder}").Empty())
            {
                // ReSharper disable once InconsistentNaming
                var maybeNewGUID = AssetDatabase.CreateFolder("Assets", DestinationFolder);
                if (maybeNewGUID != "")
                {
                    Debug.Log($"Temporal asset folder was created. GUID: {maybeNewGUID}");
                }
            }

            // ReSharper disable once InconsistentNaming
            var secondaryDirectoryGUID = AssetDatabase.CreateFolder($"Assets/{DestinationFolder}", secondaryDirectoryName);

            if (secondaryDirectoryGUID != "")
            {
                Debug.Log($"Output directory's GUID: {secondaryDirectoryGUID}");
            }
            else
            {
                Debug.LogError("Output directory could not be created");
                throw new Exception("invalid state");
            }
        }
        
        internal static ExportInformation ExportToAssetFolder(GameObject target)
        {
            GameObjectRecurseUtility.EnableAllChildrenWithRenderers(target);
            var containsVertexColors = MeshUtility.GetMeshes(target).Any(m => m.colors32.Length != 0);
            var runIdentifier = $"Run_{DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture)}";
            InitializeTemporalAssetDataDirectory(runIdentifier);

            var serialized = ExportGltfToAssetFolder(target, containsVertexColors, runIdentifier);

            return new ExportInformation(serialized, containsVertexColors);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="temporary"></param>
        /// <param name="containsVertexColors"></param>
        /// <param name="runIdentifier"></param>
        /// <returns>Serialized object.</returns>
        private static GameObject ExportGltfToAssetFolder(GameObject temporary, bool containsVertexColors, string runIdentifier)
        {
#if RIH_HAS_UNI_GLTF
            Debug.Log("Absolute path to the Asset: " + Application.dataPath);
            var gltfAssetRelativePath =
                $"{DestinationFolder}/{runIdentifier}/model.gltf";
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