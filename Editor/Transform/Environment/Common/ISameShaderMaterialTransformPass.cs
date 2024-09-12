using ResoniteImportHelper.Marker;
using ResoniteImportHelper.UnityEditorUtility;
using UnityEngine;

namespace ResoniteImportHelper.Transform.Environment.Common
{
    /// <summary>
    /// <see cref="Material.shader"/> を変更しない<see cref="Material"/>の書き換えを定義する。
    /// </summary>
    internal interface ISameShaderMaterialTransformPass
    {
        [NotPublicAPI]
        public Material RewriteInline(Material material);

        internal sealed Material Rewrite(Material material) => RewriteInline(MaterialUtility.CreateVariant(material));
    }
}
