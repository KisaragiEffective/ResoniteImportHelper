using System;
using System.Globalization;
using System.Text.RegularExpressions;
using ResoniteImportHelper.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace ResoniteImportHelper.TransFront
{
    internal static class Entry
    {
        internal static ExportInformation PerformConversion(
            GameObject unmodifiableRoot,
            // ReSharper disable once InconsistentNaming
            bool runVRCSDKPipeline,
            // ReSharper disable once InconsistentNaming
            bool runNDMF,
            bool bakeTexture,
            bool generateIntermediateArtifact
        )
        {
            Profiler.BeginSample("PerformConversion");
            var runIdentifier = $"Run_{DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture)}";
            var rootAlloc = new ResoniteImportHelper.Allocator.ResourceAllocator(InitializeTemporalAssetDataDirectory(runIdentifier));
            var target = Transform.AvatarTransformService.PerformConversionPure(
                unmodifiableRoot,
                runVRCSDKPipeline,
                runNDMF,
                bakeTexture,
                rootAlloc
            );
            
            Debug.Log("Exporting model as glTF");
            var serialized = SerializationService.ExportToAssetFolder(
                new SerializationConfiguration(target, unmodifiableRoot, generateIntermediateArtifact, rootAlloc)
            );
            
            Debug.Log("done");
            // we can remove target because it is cloned in either way.
            Object.DestroyImmediate(target, false);
            Profiler.EndSample();
            return serialized;
        }
        
        private const string DestinationFolder = "ZZZ_TemporalAsset";
        
        private static string InitializeTemporalAssetDataDirectory(string secondaryDirectoryName)
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
                return secondaryDirectoryGUID;
            }
            else
            {
                Debug.LogError("Output directory could not be created");
                throw new Exception("invalid state");
            }
        }
    }
}