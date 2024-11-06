#nullable enable
using UnityEngine;

namespace ResoniteImportHelper.Lint.Diagnostic
{
    internal class CustomShaderDiagnostic : IDiagnostic
    {
        internal readonly Material CustomizedShaderUsedMaterial;
        internal readonly Renderer ReferencedRenderer;

        internal CustomShaderDiagnostic(Material customizedShaderUsedMaterial, Renderer referencedRenderer)
        {
            CustomizedShaderUsedMaterial = customizedShaderUsedMaterial;
            ReferencedRenderer = referencedRenderer;
        }
        public string Message()
        {
            return "Resonite does not support Custom Shader";
        }
    }
}
