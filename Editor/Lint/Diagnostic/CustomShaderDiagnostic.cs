using ResoniteImportHelper.Marker;
using UnityEngine;

namespace ResoniteImportHelper.Lint.Diagnostic
{
    internal class CustomShaderDiagnostic : IDiagnostic
    {
        internal readonly Material CustomizedShaderUsedMaterial;

        internal CustomShaderDiagnostic(Material customizedShaderUsedMaterial)
        {
            CustomizedShaderUsedMaterial = customizedShaderUsedMaterial;
        }
        
        [NotPublicAPI]
        public string Message()
        {
            return "Resonite does not support Custom Shader";
        }
    }
}
