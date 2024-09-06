using ResoniteImportHelper.Serialization;
using UnityEngine;

namespace ResoniteImportHelper.TransFront
{
    internal static class Entry
    {
        internal static ExportInformation PerformConversion(
            GameObject unmodifiableRoot,
            // ReSharper disable once InconsistentNaming
            bool runVRCSDKPipeline,
            // ReSharper disable once InconsistentNaming
            bool runNDMF
        )
        {
            var target = Transform.AvatarTransformService.PerformConversionPure(unmodifiableRoot, runVRCSDKPipeline, runNDMF);
            
            Debug.Log("Exporting model as glTF");
            var serialized = SerializationService.ExportToAssetFolder(
                new SerializationConfiguration(target, unmodifiableRoot)
            );
            
            Debug.Log("done");
            // we can remove target because it is cloned in either way.
            Object.DestroyImmediate(target, false);
            return serialized;
        }
    }
}