using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
#if RIH_HAS_LILTOON
using lilToon;
#endif
using ResoniteImportHelper.Allocator;
using ResoniteImportHelper.Marker;
using ResoniteImportHelper.Transform.Environment.Common;
using ResoniteImportHelper.UnityEditorUtility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace ResoniteImportHelper.Transform.Environment.LilToon
{
    internal sealed class LilToonHandler: ISameShaderMaterialTransformPass
    {
        private static ResourceAllocator currentAllocator;
        
        internal LilToonHandler(ResourceAllocator allocator)
        {
            // TODO: キモいのでいつかインスタンス変数に戻す
            LilToonHandler.currentAllocator = allocator;
        }
        
#if RIH_HAS_LILTOON_NEXT
#warning lilToon 2.0.0 is under develop and this Transformer may not able to work or be compiled correctly.
#warning This is not supported yet. Please downgrade lilToon to 1.x series.
#endif
        internal void PerformInlineTransform(GameObject modifiableRoot)
        {
#if !RIH_HAS_LILTOON
            Debug.LogWarning("This project does not have supported version of lilToon, skipping this IPostExpansionTransformer");
            return;
#endif
            Profiler.BeginSample("LilToonHandler.PerformInlineTransform");
            foreach (var renderer in GameObjectRecurseUtility.GetChildrenRecursive(modifiableRoot)
                         .Select(o => 
                             o.TryGetComponent(out SkinnedMeshRenderer smr) ? smr : null
                         ).Where(o => o != null))
            {
                renderer.sharedMaterials = renderer.sharedMaterials.Select(RewriteInline).ToArray();
            }
            Profiler.EndSample();
        }

        private static bool UsesLilToonShader(Material m) => LilToonShaderFamily.Instance.Contains(m.shader);

        private static void PerformBakeTexture(Material m)
        {
            Profiler.BeginSample("LilToonHandler.PerformBakeTexture");
            const int all = 0;
            // Debug.Log("bake");
            var h = MuteDialogIfPossible();
#if RIH_HAS_LILTOON
            #region avoid texture overwrite
            {
                Profiler.BeginSample("LilToonHandler.CopyTextureProperties");
                var props = MaterialEditor.GetMaterialProperties(new Object[] { m });
                foreach (var prop in props.Where(prop => prop.type == MaterialProperty.PropType.Texture).Where(prop => prop.textureValue != null))
                {
                    var x = currentAllocator.Save(prop.textureValue);
                    prop.textureValue = x;
                }
                Profiler.EndSample();
            }
            #endregion
            var inspector = (new global::lilToon.lilToonInspector());
            var inty = inspector.GetType();
            // TODO: 例外ケースのダイアログがアレなので一部を切り貼りするべき？

            #region initialization for lilInspector

            {
                Profiler.BeginSample("LilToonHandler.LilInspectorInitializationStub");
                var props = MaterialEditor.GetMaterialProperties(new Object[] { m });
                var apMethod = inty.GetMethod("AllProperties", BindingFlags.Instance | BindingFlags.NonPublic);
                var lmpProxies = (object[]) apMethod!.Invoke(inspector, Array.Empty<object>());
                var lmpType = lmpProxies.GetType().GetElementType();
                var findPropMethod = lmpType!.GetMethod("FindProperty", BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in lmpProxies) findPropMethod!.Invoke(prop, new object[] { props });
                Profiler.EndSample();
            }
            
            #endregion

            {
                Profiler.BeginSample("LilToonHandler.Reflect-TextureBake");
                
                Profiler.BeginSample("type");
                var ty = inspector.GetType()!;
                Profiler.EndSample();
                
                Profiler.BeginSample("method");
                var method = ty!.GetMethod("TextureBake", BindingFlags.Instance | BindingFlags.NonPublic);
                Profiler.EndSample();
                
                Profiler.BeginSample("invoke");
                method!.Invoke(inspector, new object[] { m, all });
                Profiler.EndSample();
                
                Profiler.EndSample();
            }
#endif
            UnmuteDialog(h);
            Profiler.EndSample();
            // Debug.Log("bake done");
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
            Profiler.BeginSample("SkipDialogDisplayFromLilInspector");
            // Debug.Log("hello!");
            Profiler.BeginSample("frames");
            var frames = new StackTrace(false).GetFrames();
            Profiler.EndSample();
            Profiler.BeginSample("lilFrame");
            var lilToonInspectorBakeCallFrame = frames!.FirstOrDefault(f =>
            {
                if (f == null) return false;
                var m = f.GetMethod();
                // Debug.Log($"  checking {m}");
                var decl = m.DeclaringType;
                return decl!.FullName == "lilToon.lilToonInspector" && m.Name == "TextureBake";
            });
            Profiler.EndSample();
            if (lilToonInspectorBakeCallFrame == null)
            {
                // this call is not what we're looking for;
                return true;
            }
            Profiler.BeginSample("thisAutomationFrame");
            var thisAutomationFrame = frames!.FirstOrDefault(f =>
            {
                if (f == null) return false;
                var m = f.GetMethod();
                // Debug.Log($"  checking {m}");
                var decl = m.DeclaringType;
                return decl!.FullName == typeof(LilToonHandler).FullName && m.Name == nameof(SkipDisplayDialogFromLilInspector);
            });
            Profiler.EndSample();
            Profiler.EndSample();
            
            return thisAutomationFrame == null;
        }

        private static bool SkipSaveDestinationDialog(ref Texture2D __result, Texture2D tex)
        {
            Profiler.BeginSample("SkipSaveDestinationDialog");
            Profiler.BeginSample("frames");
            var frames = new StackTrace(false).GetFrames();
            Profiler.EndSample();
            Profiler.BeginSample("lilFrames");
            var lilToonInspectorBakeCallFrame = frames!.FirstOrDefault(f =>
            {
                if (f == null) return false;
                var m = f.GetMethod();
                // Debug.Log($"  checking {m}");
                var decl = m.DeclaringType;
                return decl!.FullName == "lilToon.lilToonInspector" && m.Name == "TextureBake";
            });
            Profiler.EndSample();
            if (lilToonInspectorBakeCallFrame == null)
            {
                // this call is not what we're looking for;
                return true;
            }
            Profiler.BeginSample("thisAutomationFrame");
            var thisAutomationFrame = frames!.FirstOrDefault(f =>
            {
                if (f == null) return false;
                var m = f.GetMethod();
                // Debug.Log($"  checking {m}");
                var decl = m.DeclaringType;
                return decl!.FullName == typeof(LilToonHandler).FullName && m.Name == nameof(SkipSaveDestinationDialog);
            });
            Profiler.EndSample();

            {
                Profiler.BeginSample("Post");
            
                Profiler.BeginSample("Allocation");
                var persistent = currentAllocator.Save(tex);
                Profiler.EndSample();
            
                Profiler.BeginSample("Log computation");
                Debug.Log($"baking actual: {persistent} / GUID: {AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(persistent))}");
                Profiler.EndSample();
            
                Profiler.BeginSample("ref copy");
                __result = persistent;
                Profiler.EndSample();
            
                Profiler.EndSample();
            }
            
            Profiler.EndSample();

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

        [NotPublicAPI]
        public Material RewriteInline(Material material)
        {
            Profiler.BeginSample("LilToonHandler.RewriteInline");
            Debug.Log($"try rewrite: {material} ({AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(material))})");
            
            if (!UsesLilToonShader(material)) return material;
            
            var variant = currentAllocator.Save(MaterialUtility.CreateVariant(material));
            PerformBakeTexture(variant);
            Profiler.EndSample();
            return variant;

        }
    }
}