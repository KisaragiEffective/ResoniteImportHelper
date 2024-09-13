using System.Collections.Generic;
using System.Linq;
using ResoniteImportHelper.Marker;
using UnityEngine;

#nullable enable
namespace ResoniteImportHelper.Transform.Environment.Common
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
            Material from,
            IEnumerable<Material>? disposableMaterialVariants
        ) => disposableMaterialVariants == null ? new NotModifiedTag(from) : new NotModifiedTag(from, disposableMaterialVariants);

        /// <summary>
        /// 変換されたことを示す<see cref="IMaterialConversionResult"/>を返す。
        /// </summary>
        /// <param name="from">変換前</param>
        /// <param name="to">変換後</param>
        /// <param name="disposableMaterialVariants"><see cref="DisposableMaterialVariants"/>で破棄される可能性がある<see cref="Material"/></param>
        /// <returns></returns>
        internal static IMaterialConversionResult Modified(
            Material from,
            Material to,
            IEnumerable<Material>? disposableMaterialVariants
        ) => disposableMaterialVariants == null ? new ModifiedTag(from, to) : new ModifiedTag(from, to, disposableMaterialVariants);

        /// <summary>
        /// Materialの変換後に処理後にまとめて破壊しても良い<see cref="Material"/> Variantを列挙する。
        /// このメソッドは呼び出しごとに同じ集合を返さなければならないが、その順序は問われない。
        /// </summary>
        /// <returns></returns>
        [NotPublicAPI]
        public IEnumerable<Material> DisposableMaterialVariants();

        [NotPublicAPI]
        public Material GetInput();

        [NotPublicAPI]
        public Material GetOutcome();

        [NotPublicAPI]
        public bool HasModified();

        internal sealed class NotModifiedTag: IMaterialConversionResult
        {
            private readonly Material _from;
            private readonly IEnumerable<Material> _disposableMaterialVariants;

            internal NotModifiedTag(Material from)
            {
                this._from = from;
                this._disposableMaterialVariants = Enumerable.Empty<Material>();
            }

            internal NotModifiedTag(Material from, IEnumerable<Material> disposableMaterialVariants)
            {
                this._from = from;
                this._disposableMaterialVariants = disposableMaterialVariants;
            }

            [NotPublicAPI]
            public IEnumerable<Material> DisposableMaterialVariants() => _disposableMaterialVariants;

            [NotPublicAPI]
            public Material GetInput() => _from;

            [NotPublicAPI]
            public Material GetOutcome() => _from;

            [NotPublicAPI]
            public bool HasModified() => false;
        }
        
        internal sealed class ModifiedTag: IMaterialConversionResult
        {
            private readonly Material _from;
            private readonly Material _to;
            private readonly IEnumerable<Material> _disposableMaterialVariants;

            internal ModifiedTag(Material from, Material to)
            {
                this._from = from;
                this._to = to;
                this._disposableMaterialVariants = Enumerable.Empty<Material>();
            }
            
            internal ModifiedTag(Material from, Material to, IEnumerable<Material> disposableMaterialVariants)
            {
                this._from = from;
                this._to = to;
                this._disposableMaterialVariants = disposableMaterialVariants;
            }

            [NotPublicAPI]
            public IEnumerable<Material> DisposableMaterialVariants() => _disposableMaterialVariants;

            [NotPublicAPI]
            public Material GetInput() => _from;

            [NotPublicAPI]
            public Material GetOutcome() => _to;
            
            [NotPublicAPI]
            public bool HasModified() => true;
        }
    }
}