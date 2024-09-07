using System.Linq;
using System.Reflection;
using ResoniteImportHelper.Marker;
using ResoniteImportHelper.Transform.Environment.Common;
using ResoniteImportHelper.UnityEditorUtility;
using UnityEngine;

namespace ResoniteImportHelper.Transform.Environment.LilToon
{
    internal sealed class LilToonHandler: IPostExpansionTransformer
    {
#if RIH_HAS_LILTOON_NEXT
#warning lilToon 2.0.0 is under develop and this Transformer may not able to work or be compiled correctly.
#warning The support is limited, and is under experimental state.
#endif
        [NotPublicAPI]
        public void PerformInlineTransform(GameObject modifiableRoot)
        {
#if !RIH_HAS_LILTOON && !RIH_HAS_LILTOON_NEXT
            Debug.LogWarning("This project does not have lilToon, skipping this IPostExpansionTransformer");
            return;
#endif
            foreach (var material in GameObjectRecurseUtility.GetChildrenRecursive(modifiableRoot)
                         .SelectMany(o => o.TryGetComponent(out SkinnedMeshRenderer smr) ? smr.sharedMaterials : Enumerable.Empty<Material>())
                         .Where(UsesLilToonShader))
            {
                PerformBakeTexture(material);
            }
        }

        private static bool UsesLilToonShader(Material m)
        {
            // FIXME: this is fuzzy
            return m.shader.name.StartsWith("Hidden/lil");
        }

        private static void PerformBakeTexture(Material m)
        {
            const int all = 0;
            
#if RIH_HAS_LILTOON
            var inspector = (new global::lilToon.lilToonInspector());
            // TODO: 例外ケースのダイアログがアレなので一部を切り貼りするべき？
            inspector
                .GetType()!
                .GetMethod("TextureBake", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(inspector, new object[] { m, all });
#elif RIH_HAS_LILTOON_NEXT
            global::lilToon.lilMaterialBaker.TextureBake(m, all);
#endif
        }
    }
}