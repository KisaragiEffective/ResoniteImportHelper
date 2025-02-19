#nullable enable
using System.Collections.Generic;
using KisaragiMarine.ResoniteImportHelper.Allocator;
using UnityEngine;

#nullable enable
namespace KisaragiMarine.ResoniteImportHelper.Transform.Environment.Common
{
    internal interface IMaterialConversionResult
    {
        /// <summary>
        /// 変換されていないことを示す<see cref="IMaterialConversionResult"/>を返す。
        /// </summary>
        /// <param name="from">変換前</param>
        /// <param name="disposableMaterialVariants"><see cref="DisposableMaterialVariants"/>で破棄される可能性がある<see cref="Material"/></param>
        /// <returns></returns>
        internal static IMaterialConversionResult NotModified(
            Material from
        ) => new NotModifiedTag(from);

        /// <summary>
        /// 変換されたことを示す<see cref="IMaterialConversionResult"/>を返す。
        /// </summary>
        /// <param name="from">変換前</param>
        /// <param name="to">変換後</param>
        /// <param name="disposableMaterialVariants"><see cref="DisposableMaterialVariants"/>で破棄される可能性がある<see cref="Material"/></param>
        /// <returns></returns>
        internal static IMaterialConversionResult Modified(
            Material from,
            Material to
        ) => new ModifiedTag(from, to);

        public InMemory<Material>? AllocatedConvertedMaterial();

        public Material GetInput();

        public Material GetOutcome();

        public bool HasModified();

        internal sealed class NotModifiedTag: IMaterialConversionResult
        {
            private readonly Material _from;

            internal NotModifiedTag(Material from)
            {
                this._from = from;
            }

            public InMemory<Material>? AllocatedConvertedMaterial() => null;

            public Material GetInput() => _from;

            public Material GetOutcome() => _from;

            public bool HasModified() => false;
        }

        internal sealed class ModifiedTag: IMaterialConversionResult
        {
            private readonly Material _from;
            private readonly Material _to;

            internal ModifiedTag(Material from, Material to)
            {
                this._from = from;
                this._to = to;
            }

            internal ModifiedTag(Material from, Material to, IEnumerable<Material> disposableMaterialVariants)
            {
                this._from = from;
                this._to = to;
            }

            public InMemory<Material>? AllocatedConvertedMaterial() => new InMemory<Material>(this._to);

            public Material GetInput() => _from;

            public Material GetOutcome() => _to;
            public bool HasModified() => true;
        }
    }
}
