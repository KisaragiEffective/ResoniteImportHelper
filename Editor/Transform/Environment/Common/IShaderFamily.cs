#nullable enable
using UnityEngine;

namespace KisaragiMarine.ResoniteImportHelper.Transform.Environment.Common
{
    /// <summary>
    /// <see cref="Shader"/> の集合を定義する。
    /// </summary>
    internal interface IShaderFamily
    {
        public bool Contains(Shader shader);

        internal sealed bool Contains(Material material) => this.Contains(material.shader);
    }
}
