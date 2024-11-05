#nullable enable
using ResoniteImportHelper.Marker;
using UnityEngine;

namespace ResoniteImportHelper.Transform.Environment.Common
{
    /// <summary>
    /// <see cref="Material.shader"/> を変更せずに<see cref="Material"/>が参照するテクスチャを
    /// 他の被参照テクスチャに重ね合わせるパスを定義する。
    /// </summary>
    internal interface IMaterialTextureBakePass
    {
        [NotPublicAPI]
        public IMaterialConversionResult BakeTextureWithCache(Material material);
    }
}
