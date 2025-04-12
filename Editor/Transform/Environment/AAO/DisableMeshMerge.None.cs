#if !RIH_HAS_AAO
using KisaragiMarine.ResoniteImportHelper.Transform.Environment.Common;
using UnityEngine;

namespace KisaragiMarine.ResoniteImportHelper.Transform.Environment.AAO
{
    /// <summary>
    /// UniGLTF に AAO MergeMesh したアバターを食わせるとメッシュが大幅に壊れる現象を回避するためのパス
    /// </summary>
    public sealed class DisableMeshMerge : IPlatformExpander
    {
        public GameObject PerformEnvironmentDependantShallowCopy(GameObject unmodifiableRoot)
        {
            return unmodifiableRoot;
        }
    }
}
#endif
