#nullable enable
using ResoniteImportHelper.Marker;
using UnityEngine;

namespace ResoniteImportHelper.Transform.Environment.Common
{
    /// <summary>
    /// プラットフォーム依存の方法で直接変更を加えてはならない<see cref="UnityEngine.GameObject"/>のシャローコピーを作ったあとに変更を加える。
    /// </summary>
    internal interface IPlatformExpander
    {
        [NotPublicAPI]
        public GameObject PerformEnvironmentDependantShallowCopy(GameObject unmodifiableRoot);
    }
}
