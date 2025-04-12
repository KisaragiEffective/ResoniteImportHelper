#if RIH_HAS_AAO
using KisaragiMarine.ResoniteImportHelper.Transform.Environment.Common;
using UnityEditor;
using UnityEngine;

namespace KisaragiMarine.ResoniteImportHelper.Transform.Environment.AAO
{
    /// <summary>
    /// UniGLTF に AAO MergeMesh したアバターを食わせるとメッシュが大幅に壊れる現象を回避するためのパス
    /// </summary>
    public sealed class DisableMeshMerge : IPlatformDependantPreprocessor
    {
        public GameObject Preprocess(GameObject modifiableRoot)
        {
            var traceAndOptimizeComponents = modifiableRoot.GetComponents<Anatawa12.AvatarOptimizer.TraceAndOptimize>();

            foreach (var tao in traceAndOptimizeComponents)
            {
                var handle = new SerializedObject(tao);
                handle.FindProperty("mergeSkinnedMesh").boolValue = false;
                handle.ApplyModifiedPropertiesWithoutUndo();
            }

            foreach (var mergeComponent in modifiableRoot.GetComponents<Anatawa12.AvatarOptimizer.MergeSkinnedMesh>())
            {
                Object.DestroyImmediate(mergeComponent);
            }

            return modifiableRoot;
            // var mergeComponents = root.GetComponents<>();
        }
    }
}

#endif
