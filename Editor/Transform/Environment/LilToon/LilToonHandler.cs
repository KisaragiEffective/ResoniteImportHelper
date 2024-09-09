using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using lilToon;
using ResoniteImportHelper.Allocator;
using ResoniteImportHelper.Marker;
using ResoniteImportHelper.Transform.Environment.Common;
using ResoniteImportHelper.UnityEditorUtility;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace ResoniteImportHelper.Transform.Environment.LilToon
{
    internal sealed class LilToonHandler: IPostExpansionTransformer
    {
        private readonly ResourceAllocator allocator;
        
        internal LilToonHandler(ResourceAllocator allocator)
        {
            this.allocator = allocator;
        }
        
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
            Debug.Log("bake");
            var h = MuteDialogIfPossible();
#if RIH_HAS_LILTOON
            var inspector = (new global::lilToon.lilToonInspector());
            var inty = inspector.GetType();
            // TODO: 例外ケースのダイアログがアレなので一部を切り貼りするべき？

            #region initialization for lilInspector

            {
                var props = MaterialEditor.GetMaterialProperties(new Object[] { m });
                var apMethod = inty.GetMethod("AllProperties", BindingFlags.Instance | BindingFlags.NonPublic);
                var lmpProxies = (object[]) apMethod!.Invoke(inspector, Array.Empty<object>());
                var lmpType = lmpProxies.GetType().GetElementType();
                var findPropMethod = lmpType!.GetMethod("FindProperty", BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in lmpProxies) findPropMethod!.Invoke(prop, new object[] { props });
            }
            
            #endregion
            
            inspector
                .GetType()!
                .GetMethod("TextureBake", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(inspector, new object[] { m, all });
#elif RIH_HAS_LILTOON_NEXT
            global::lilToon.lilMaterialBaker.TextureBake(m, all);
#endif
            UnmuteDialog(h);
            Debug.Log("bake done");
        }
        
        #region Harmony if possible
        // XXX:
        // lilToon 1.xシリーズ (1.7.3) ではMaterialPropertyを「ヘッドレス」に焼く方法が存在しない。
        // これは開発中の2.x (HEAD: 5ed3507003b3cf92eb1879a92a30408615134781) でも同様である。
        // そのため、私達はHarmonyを用いて焼くメソッドからのダイアログポップアップを抑止する。
        // そうしないと、不必要なエラーが出てきて自動化の妨げになる。

        private static
#if RIH_HAS_HARMONY
        HarmonyLib.Harmony
#else
        object?
#endif
        MuteDialogIfPossible()
        {
#if RIH_HAS_LILTOON_NEXT
            // FIXME
            Debug.Log("lilToon 2.0 is not supported yet. Auto-muting does not work.");
            return null;
#endif
            
#if !RIH_HAS_HARMONY
            Debug.Log("Harmony is unavailable. Auto-muting dialog does not work.");
            return null;
#else
            var h = new HarmonyLib.Harmony(
                "io.github.kisaragieffective.resonite-import-helper.liltoon.headless-bake");

            var warningDialogMethod = typeof(EditorUtility)
                .GetMethod(nameof(EditorUtility.DisplayDialog), new[] { typeof(string), typeof(string), typeof(string) });
            
            h.Patch(
                warningDialogMethod, 
                prefix: new HarmonyLib.HarmonyMethod(typeof(LilToonHandler), nameof(SkipDisplayDialogFromLilInspector))
            );

            var bakeMethod = typeof(lilTextureUtils)
                .GetMethod("SaveTextureToPng", BindingFlags.NonPublic | BindingFlags.Static,
                    null, new[] { typeof(Material), typeof(Texture2D), typeof(string), typeof(string) }, Array.Empty<ParameterModifier>());

            h.Patch(
                bakeMethod,
                prefix: new HarmonyLib.HarmonyMethod(typeof(LilToonHandler), nameof(SkipSaveDestinationDialog))
            );
            
            return h;
#endif
        }

        private static bool SkipDisplayDialogFromLilInspector()
        {
            // Debug.Log("hello!");
            var frames = new StackTrace(false).GetFrames();
            var lilToonInspectorBakeCallFrame = frames!.FirstOrDefault(f =>
            {
                if (f == null) return false;
                var m = f.GetMethod();
                // Debug.Log($"  checking {m}");
                var decl = m.DeclaringType;
                return decl!.FullName == "lilToon.lilToonInspector" && m.Name == "TextureBake";
            });
            if (lilToonInspectorBakeCallFrame == null)
            {
                // this call is not what we're looking for;
                return true;
            }
            var thisAutomationFrame = frames!.FirstOrDefault(f =>
            {
                if (f == null) return false;
                var m = f.GetMethod();
                // Debug.Log($"  checking {m}");
                var decl = m.DeclaringType;
                return decl!.FullName == typeof(LilToonHandler).FullName && m.Name == nameof(SkipDisplayDialogFromLilInspector);
            });
            
            return thisAutomationFrame == null;
        }

        private bool SkipSaveDestinationDialog(ref Texture2D __result, Texture2D tex)
        {
            Debug.Log("hello!");
            var frames = new StackTrace(false).GetFrames();
            var lilToonInspectorBakeCallFrame = frames!.FirstOrDefault(f =>
            {
                if (f == null) return false;
                var m = f.GetMethod();
                // Debug.Log($"  checking {m}");
                var decl = m.DeclaringType;
                return decl!.FullName == "lilToon.lilToonInspector" && m.Name == "TextureBake";
            });
            if (lilToonInspectorBakeCallFrame == null)
            {
                // this call is not what we're looking for;
                return true;
            }
            var thisAutomationFrame = frames!.FirstOrDefault(f =>
            {
                if (f == null) return false;
                var m = f.GetMethod();
                // Debug.Log($"  checking {m}");
                var decl = m.DeclaringType;
                return decl!.FullName == typeof(LilToonHandler).FullName && m.Name == nameof(SkipSaveDestinationDialog);
            });
            
            __result = this.allocator.Save(tex);

            return thisAutomationFrame == null;
        }

        private static void UnmuteDialog(
#if RIH_HAS_HARMONY
            [CanBeNull] HarmonyLib.Harmony
#else
            object?
#endif
                o
        )
        {
            #if RIH_HAS_HARMONY
            o?.UnpatchAll();
            #endif
        }
        #endregion
    }
}