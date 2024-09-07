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
                .SelectMany(ExtractMaterials)
                .Where(NonStandardMaterial)
                .Select(m => new CustomShaderDiagnostic(m));
        }

        private static IEnumerable<Material> ExtractMaterials(GameObject o)
        {
            return (o.TryGetComponent<SkinnedMeshRenderer>(out var smr) ? smr.sharedMaterials : null) 
                   ?? (o.TryGetComponent<MeshRenderer>(out var mr) ? mr.sharedMaterials : null) 
                   ?? Enumerable.Empty<Material>();
        }

        private static bool NonStandardMaterial(Material m)
        {
            var shader = m.shader;
            // Debug.Log(shader);
            return !AssetDatabasePlusPlus.IsUnityEngineBuiltinObject(shader) || shader.name != "Standard";
        }
    }
}
