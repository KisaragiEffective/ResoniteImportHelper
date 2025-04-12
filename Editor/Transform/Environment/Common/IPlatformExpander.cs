#nullable enable
using UnityEngine;

namespace KisaragiMarine.ResoniteImportHelper.Transform.Environment.Common
{
    /// <summary>
    /// プラットフォーム依存の方法で直接変更を加えてはならない<see cref="UnityEngine.GameObject"/>のシャローコピーを作ったあとに変更を加える。
    /// </summary>
    public interface IPlatformExpander
    {
        public GameObject PerformEnvironmentDependantShallowCopy(GameObject unmodifiableRoot);
    }
}
