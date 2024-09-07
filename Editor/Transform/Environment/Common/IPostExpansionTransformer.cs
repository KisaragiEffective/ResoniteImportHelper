using ResoniteImportHelper.Marker;
using UnityEngine;

namespace ResoniteImportHelper.Transform.Environment.Common
{
    /// <summary>
    /// <see cref="IPlatformExpander"/>の後にクローンされた<see cref="UnityEngine.GameObject"/>に対してRIHが行う変換を定義する。
    /// </summary>
    internal interface IPostExpansionTransformer
    {
        [NotPublicAPI]
        public void PerformInlineTransform(GameObject modifiableRoot);
    }
}
