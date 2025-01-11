#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
#if RIH_HAS_LILTOON
using lilToon;
#endif
using KisaragiMarine.ResoniteImportHelper.Allocator;
using KisaragiMarine.ResoniteImportHelper.Transform.Environment.Common;
using KisaragiMarine.ResoniteImportHelper.UnityEditorUtility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace KisaragiMarine.ResoniteImportHelper.Transform.Environment.LilToon
{
    internal sealed class LilToonHandler: IMaterialTextureBakePass, ICustomShaderLowerPass
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
        internal void BakeTextureFront(GameObject modifiableRoot)
        {
#if !RIH_HAS_LILTOON
            Debug.LogWarning("This project does not have supported version of lilToon, skipping this IPostExpansionTransformer");
            return;
#endif
            Profiler.BeginSample("LilToonHandler.PerformInlineTransform");
            Profiler.BeginSample("Inline transform");

            Profiler.BeginSample("MuteDialog");
            // Harmonyのパッチが遅いので一度だけやる
            var h = MuteDialogIfPossible();
            Profiler.EndSample();

            foreach (var renderer in RendererUtility.GetConvertibleRenderersInChildren(modifiableRoot))
            {
                var conversionResults = renderer.sharedMaterials.Select(BakeTextureWithCache).ToList();
                renderer.sharedMaterials = conversionResults.Select(r =>
                {
                    var x = r.AllocatedConvertedMaterial();
                    return x != null ? currentAllocator.Save(x.Value) : r.GetOutcome();
                }).ToArray();
            }

            UnmuteDialog(h);
            Profiler.EndSample();

            Profiler.EndSample();
        }

        private static bool UsesLilToonShader(Material m) => LilToonShaderFamily.Instance.Contains(m.shader);

        private static bool _isInspectorInitialized = false;
        private static MethodInfo? _bakeMethod;

        private static void PerformBakeTexture(Material m)
        {
            Profiler.BeginSample("LilToonHandler.PerformBakeTexture");
            const int all = 0;
            // Debug.Log("bake");
#if RIH_HAS_LILTOON
            #region avoid texture overwrite
            {
                Profiler.BeginSample("LilToonHandler.CopyTextureProperties");
                var props = MaterialEditor.GetMaterialProperties(new Object[] { m });
                foreach (var prop in props.Where(prop => prop.type == MaterialProperty.PropType.Texture).Where(prop => prop.textureValue != null))
                {
                    var x = currentAllocator.SaveAmbiguously(prop.textureValue);
                    prop.textureValue = x;
                }
                Profiler.EndSample();
            }
            #endregion
            Profiler.BeginSample(".ctor");
            var inspector = new lilToonInspector();
            Profiler.EndSample();
            var inspectorType = inspector.GetType();
            // TODO: 例外ケースのダイアログがアレなので一部を切り貼りするべき？

            #region initialization for lilInspector

            {
                Profiler.BeginSample("LilToonHandler.LilInspectorInitializationStub");
                var props = MaterialEditor.GetMaterialProperties(new Object[] { m });
                var apMethod = inspectorType.GetMethod("AllProperties", BindingFlags.Instance | BindingFlags.NonPublic);
                var lmpProxies = (object[]) apMethod!.Invoke(inspector, Array.Empty<object>());
                var lmpType = lmpProxies.GetType().GetElementType();
                var findPropMethod = lmpType!.GetMethod("FindProperty", BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in lmpProxies) findPropMethod!.Invoke(prop, new object[] { props });
                Profiler.EndSample();
            }

            #endregion

            {
                Profiler.BeginSample("LilToonHandler.Reflect-TextureBake");

                Profiler.BeginSample("method");
                _bakeMethod ??= inspectorType.GetMethod("TextureBake", BindingFlags.Instance | BindingFlags.NonPublic);
                Profiler.EndSample();

                Profiler.BeginSample("invoke");
                _bakeMethod!.Invoke(inspector, new object[] { m, all });
                Profiler.EndSample();

                Profiler.EndSample();
            }
#endif
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
            Profiler.BeginSample("Harmony .ctor");
            var h = new HarmonyLib.Harmony(
                "io.github.kisaragieffective.resonite-import-helper.liltoon.headless-bake");
            Profiler.EndSample();
#if RIH_HAS_LILTOON
            Profiler.BeginSample("Mute warnings");
            Profiler.BeginSample("Get Method");
            var warningDialogMethod = typeof(EditorUtility)
                .GetMethod(nameof(EditorUtility.DisplayDialog), new[] { typeof(string), typeof(string), typeof(string) });
            Profiler.EndSample();

            Profiler.BeginSample("Construct method");
            var prefix =
                new HarmonyLib.HarmonyMethod(typeof(LilToonHandler), nameof(SkipDisplayDialogFromLilInspector));
            Profiler.EndSample();

            Profiler.BeginSample("Patch");
            h.Patch(
                warningDialogMethod,
                prefix
            );
            Profiler.EndSample();
            Profiler.EndSample();

            Profiler.BeginSample("Mute file destination dialog");
            Profiler.BeginSample("Get Method");
            var bakeMethod = typeof(lilTextureUtils)
                .GetMethod("SaveTextureToPng", BindingFlags.NonPublic | BindingFlags.Static,
                    null, new[] { typeof(Material), typeof(Texture2D), typeof(string), typeof(string) }, Array.Empty<ParameterModifier>());
            Profiler.EndSample();

            Profiler.BeginSample("Patch");
            h.Patch(
                bakeMethod,
                prefix: new HarmonyLib.HarmonyMethod(typeof(LilToonHandler), nameof(SkipSaveDestinationDialog))
            );
            Profiler.EndSample();
            Profiler.EndSample();
#else
            Debug.LogWarning("This project does not have lilToon as an Unity-managed package.");
#endif // RIH_HAS_LILTOON
            return h;
#endif // RIH_HAS_HARMONY
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
                var persistent = currentAllocator.SaveAmbiguously(tex);
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

        private readonly Dictionary<Material, IMaterialConversionResult> _bakedMaterialCache = new();

        public IMaterialConversionResult BakeTextureWithCache(Material material)
        {
            if (_bakedMaterialCache.TryGetValue(material, out var baked))
            {
                return baked;
            }

            var v = BakeTextureInline(material);
            _bakedMaterialCache.Add(material, v);

            return v;
        }

        private static IMaterialConversionResult BakeTextureInline(Material material)
        {
            Profiler.BeginSample("LilToonHandler.RewriteInline");
            Debug.Log($"try rewrite: {material} ({AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(material))})");

            if (!UsesLilToonShader(material)) return IMaterialConversionResult.NotModified(material);

            var variant = MaterialUtility.CreateVariant(material);
            PerformBakeTexture(variant);

            IMaterialConversionResult ret;
            if (MaterialUtility.HasAnyOverride(variant))
            {
                Debug.Log("this iteration produces override.");
                ret = IMaterialConversionResult.Modified(material, variant);
            }
            else
            {
                ret = IMaterialConversionResult.NotModified(material);
            }

            Profiler.EndSample();
            return ret;
        }

        private static IEnumerable<T> Single<T>(T obj)
        {
            yield return obj;
        }

        private static readonly int BumpMap = Shader.PropertyToID("_BumpMap");
        private static readonly int StandardShaderMixtureMode = Shader.PropertyToID("_Mode");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

        public ISealedLoweredMaterialReference LowerInline(Material m)
        {
            if (!UsesLilToonShader(m)) return new NonConvertedMaterial(m);

            var standardMaterial = MaterialUtility.CreateSimilarStandardMaterialFromCustomizedOne(m);

            LoweredRenderMode mode;
            var mainTexture = m.mainTexture;
            if (mainTexture != null)
            {
                Debug.Log($"typeof mainTexture: {mainTexture.GetType()}");
                var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mainTexture));
                Debug.Log($"typeof importer: {importer.GetType()}");

                if (importer is TextureImporter ti)
                {
                    var hasAlpha = ti.alphaSource == TextureImporterAlphaSource.FromInput;
                    var isNonOpaqueShader = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(m.shader)) !=
                                            "efa77a80ca0344749b4f19fdd5891cbe";
                    Debug.Log($"Test for {mainTexture}: import: {hasAlpha}, isNonOpaque: {isNonOpaqueShader}");
                    var givenAlpha = hasAlpha && isNonOpaqueShader;
                    mode = givenAlpha ? LoweredRenderMode.Blend : LoweredRenderMode.Opaque;

                    standardMaterial.SetOverrideTag("RenderType", givenAlpha ? "Transparent" : "");
                    if (givenAlpha)
                    {
                        standardMaterial.renderQueue = 3000;
                    }
                }
                else
                {
                    mode = LoweredRenderMode.Blend;
                    standardMaterial.SetOverrideTag("RenderType", "Transparent");
                    standardMaterial.renderQueue = 3000;
                }
            }
            else
            {
                mode = LoweredRenderMode.Opaque;
            }

            // set NormalMap
            standardMaterial.SetTexture(BumpMap, m.GetTexture(BumpMap));

            return new LoweredMaterialReference(new InMemory<Material>(standardMaterial), mode);
        }
    }
}
