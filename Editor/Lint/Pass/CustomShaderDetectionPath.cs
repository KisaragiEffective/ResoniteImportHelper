using System.Collections.Generic;
using System.Linq;
using ResoniteImportHelper.Lint.Diagnostic;
using ResoniteImportHelper.Marker;
using ResoniteImportHelper.UnityEditorUtility;
using UnityEngine;

namespace ResoniteImportHelper.Lint.Pass
{
    internal class CustomShaderDetectionPath : ILintPass<CustomShaderDiagnostic>
    {
        [NotPublicAPI]
        public IEnumerable<CustomShaderDiagnostic> Check(GameObject unmodifiableRoot)
        {
            return GameObjectRecurseUtility
                .GetChildrenRecursive(unmodifiableRoot)
                .Select(ExtractMaterials)
                .Where(r => r.Item2 != null)
                .Where(r => r.Item1.Any(NonStandardMaterial))
                .SelectMany(t => t.Item1.Select(m => new CustomShaderDiagnostic(m, t.Item2)));
        }

        private static (IEnumerable<Material>, Renderer?) ExtractMaterials(GameObject o)
        {
            return ExtractMaterials<SkinnedMeshRenderer>(o)
                   ?? ExtractMaterials<MeshRenderer>(o)
                   ?? (Enumerable.Empty<Material>(), null);
        }

        private static (IEnumerable<Material>, Renderer)? ExtractMaterials<TRenderer>(GameObject o) where TRenderer: Renderer
        {
            return o.TryGetComponent<TRenderer>(out var r) ? (r.sharedMaterials, r) : null;
        }

        private static bool NonStandardMaterial(Material m)
        {
            var shader = m.shader;
            // Debug.Log(shader);
            return !AssetDatabasePlusPlus.IsUnityEngineBuiltinObject(shader) || shader.name != "Standard";
        }
    }
}
