#nullable enable
using ResoniteImportHelper.Marker;
using ResoniteImportHelper.Transform.Environment.Common;
using UnityEngine;

namespace ResoniteImportHelper.Transform.Environment.LilToon
{
    internal sealed class LilToonShaderFamily: IShaderFamily
    {
        internal static readonly LilToonShaderFamily Instance = new();
        
        private LilToonShaderFamily() {}
        
        [NotPublicAPI]
        public bool Contains(Shader shader)
        {
            // FIXME: this is fuzzy
            return shader.name.StartsWith("Hidden/lil");
        }
    }
}