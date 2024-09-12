using ResoniteImportHelper.Marker;
using UnityEngine;

namespace ResoniteImportHelper.Transform.Environment.Common
{
    /// <summary>
    /// <see cref="Shader"/> の集合を定義する。
    /// </summary>
    internal interface IShaderFamily
    {
        [NotPublicAPI]
        public bool Contains(Shader shader);

        [NotPublicAPI]
        internal sealed bool Contains(Material material) => this.Contains(material.shader);
    }
}
