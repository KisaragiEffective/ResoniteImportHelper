using ResoniteImportHelper.Marker;
using UnityEngine;

namespace ResoniteImportHelper.Transform.Environment.Common
{
    internal interface ISealedLoweredMaterialReference
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>変換されたかもしれない<see cref="Material"/>。</returns>
        [NotPublicAPI]
        public Material GetMaybeConvertedMaterial();

        /// <summary>
        /// 
        /// </summary>
        /// <returns><see cref="LoweredRenderMode"/>。<see cref="LoweredRenderMode.Unknown"/>の場合は自分で計算する必要がある。</returns>
        [NotPublicAPI]
        public LoweredRenderMode GetComputedRenderMode();
    }
    
    internal sealed class NonConvertedMaterial : ISealedLoweredMaterialReference
    {
        private readonly Material _originalMaterial;
        internal NonConvertedMaterial(Material material)
        {
            _originalMaterial = material;
        }

        public Material GetMaybeConvertedMaterial() => _originalMaterial;

        public LoweredRenderMode GetComputedRenderMode() => LoweredRenderMode.Unknown;
    }

    internal readonly struct LoweredMaterialReference : ISealedLoweredMaterialReference
    {
        internal readonly Material ConvertedMaterial;
        internal readonly LoweredRenderMode RenderMode;

        internal LoweredMaterialReference(Material convertedMaterial, LoweredRenderMode renderMode)
        {
            ConvertedMaterial = convertedMaterial;
            RenderMode = renderMode;
        }

        [NotPublicAPI]
        public Material GetMaybeConvertedMaterial() => ConvertedMaterial;

        [NotPublicAPI]
        public LoweredRenderMode GetComputedRenderMode() => RenderMode;
    }
}