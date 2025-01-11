#nullable enable
using KisaragiMarine.ResoniteImportHelper.Allocator;
using UnityEngine;

namespace KisaragiMarine.ResoniteImportHelper.Transform.Environment.Common
{
    internal interface ISealedLoweredMaterialReference
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns>変換されたかもしれない<see cref="Material"/>。</returns>
        public Material GetMaybeConvertedMaterial();

        /// <summary>
        ///
        /// </summary>
        /// <returns>存在する場合はそれ、ない場合はnull</returns>
        public InMemory<Material>? GetAllocationJob();

        /// <summary>
        ///
        /// </summary>
        /// <returns><see cref="LoweredRenderMode"/>。<see cref="LoweredRenderMode.Unknown"/>の場合は自分で計算する必要がある。</returns>
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
        public InMemory<Material>? GetAllocationJob() => null;

        public LoweredRenderMode GetComputedRenderMode() => LoweredRenderMode.Unknown;
    }

    internal readonly struct LoweredMaterialReference : ISealedLoweredMaterialReference
    {
        internal readonly InMemory<Material> ToBeAllocatedMaterial;
        internal readonly LoweredRenderMode RenderMode;

        internal LoweredMaterialReference(InMemory<Material> toBeAllocatedMaterial, LoweredRenderMode renderMode)
        {
            ToBeAllocatedMaterial = toBeAllocatedMaterial;
            RenderMode = renderMode;
        }

        public Material GetMaybeConvertedMaterial() => ToBeAllocatedMaterial.InMemoryValue;

        public InMemory<Material>? GetAllocationJob()
        {
            return ToBeAllocatedMaterial;
        }

        public LoweredRenderMode GetComputedRenderMode() => RenderMode;
    }
}
