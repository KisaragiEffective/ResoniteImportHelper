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
        private static ExportingGltfData WriteGltf(GameObject target, bool containsVertexColors)
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns>Serialized object.</returns>
        internal static ExportInformation WriteGltfToAssetFolder(GameObject target)
        {
#if RIH_HAS_UNI_GLTF
            GameObjectRecurseUtility.EnableAllChildrenWithRenderers(target);
            var containsVertexColors = MeshUtility.GetMeshes(target).Any(m => m.colors32.Length != 0);
            var runIdentifier = $"Run_{DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture)}";
            InitializeTemporalAssetDataDirectory(runIdentifier);

            var gltfAssetRelativePath =
                $"{DestinationFolder}/{runIdentifier}/model.gltf";
            Debug.Log("Absolute path to the Asset: " + Application.dataPath);
            // dataPathはAssetsで終わることに注意！！
            var gltfFilePath = $"{Application.dataPath}/{gltfAssetRelativePath}";

            var data = WriteGltf(target, containsVertexColors);

            #region UniGLTF.GltfExportWindow から引用したGLTFを書き出す処理
            // SPDX-SnippetBegin
            // SPDX-License-Identifier: MIT
            // SPDX-SnippetName: UniGLTF.GltfExportWindow
            // SPFX-SnippetCopyrightText: Copyright (c) 2020 VRM Consortium
            // SPFX-SnippetCopyrightText: Copyright (c) 2018 Masataka SUMI for MToon

            var (json, buffer0) = data.ToGltf(gltfFilePath);
            {
                // write JSON without BOM
                var encoding = new System.Text.UTF8Encoding(false);
                File.WriteAllText(gltfFilePath, json, encoding);
            }

            {
                // write to buffer0 local folder
                var dir = Path.GetDirectoryName(gltfFilePath);
                var bufferPath = Path.Combine(dir, buffer0.uri);
                File.WriteAllBytes(bufferPath, data.BinBytes.ToArray());
            }
            // SPDX-SnippetEnd
            #endregion

            var assetsRelPath = $"Assets/{gltfAssetRelativePath}";
            {
                AssetDatabase.ImportAsset(assetsRelPath);
                AssetDatabase.Refresh();
            }

            var serialized = AssetDatabase.LoadAssetAtPath<GameObject>(assetsRelPath);

            return new ExportInformation(serialized, containsVertexColors);
#else
            throw new Exception("assertion error: UniGLTF is not installed on the project.");
#endif
        }
    }
}