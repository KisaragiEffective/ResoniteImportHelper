using System.Linq;
using ResoniteImportHelper.Lint.Pass;
using ResoniteImportHelper.TransFront;
using ResoniteImportHelper.UI.Localize;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
#if RIH_HAS_VRCSDK3A
using VRC.SDK3.Avatars.Components;
#endif
using static ResoniteImportHelper.UI.ExternalServiceStatus;
using Button = UnityEngine.UIElements.Button;
using Toggle = UnityEngine.UIElements.Toggle;

namespace ResoniteImportHelper.UI.Component
{
    internal sealed class Body : VisualElement
    {
        private LocaleSelector.LocaleKind _currentLanguage;
        
        internal Body(LocaleSelector ls)
        {
            var lang = ls.GetLanguage();
            
            RenderTo(this, lang);
            ls.PullDown.RegisterValueChangedCallback(ev =>
            {
                if (_currentLanguage == ev.newValue) return;

                _currentLanguage = ev.newValue;

                // rebuild
                this.Clear();
                RenderTo(this, ls.GetLanguage());
            });
        }

        private static void RenderTo(VisualElement rootVisualElement, ILocalizedTexts lang)
        {
            var rootObject = new ObjectField(lang.Root())
            {
                objectType = typeof(GameObject),
                tooltip = lang.RootTooltip()
            };
            rootVisualElement.Add(rootObject);
            var exportSettingFoldout = new Foldout { text = lang.ExportSetting() };
            rootVisualElement.Add(exportSettingFoldout);
            // ReSharper disable once InconsistentNaming
            var doRunVRCSDK3APreprocessors = CreatePreprocessorToggleCheckbox(rootObject, lang);
            exportSettingFoldout.Add(doRunVRCSDK3APreprocessors);
            // ReSharper disable once InconsistentNaming
            var doNDMFManualBake = CreateNDMFManualBakeCheckbox(doRunVRCSDK3APreprocessors, lang);
            exportSettingFoldout.Add(doNDMFManualBake);
            var experimentalSettingsFoldout = CreateExperimentalSettingsFoldout(lang);
            exportSettingFoldout.Add(experimentalSettingsFoldout);
            
            var destination = new ObjectField(lang.ProcessedAvatarLabel())
            {
                objectType = typeof(GameObject),
                tooltip = lang.ProcessedAvatarTooltip()
            };
            destination.RegisterCallback<DragUpdatedEvent>(ev => ev.PreventDefault());
            destination.RegisterCallback<DragPerformEvent>(ev => ev.PreventDefault());
            var modelContainsVertexColorNote =
                new HelpBox(lang.ModelContainsVertexColor(),
                    HelpBoxMessageType.Info)
                {
                    style = { display = DisplayStyle.None }
                };
            
            var run = new Button(() =>
            {
                var result = Entry.PerformConversion(
                    rootObject.value as GameObject,
                    doRunVRCSDK3APreprocessors.value,
                    doNDMFManualBake.value,
                    experimentalSettingsFoldout.BakeShadersConfigurationIntoTextures.value,
                    experimentalSettingsFoldout.GenerateIntermediateArtifact.value
                );
                destination.value = result.SerializedObject;
                modelContainsVertexColorNote.style.display =
                    result.HasVertexColor ? DisplayStyle.Flex : DisplayStyle.None;
                // SerializedObjectはUniGLTFが全てStandardシェーダーに変換しているので
                // Backlinkから元々のオブジェクトを拾ってくる必要がある
                var diags = new CustomShaderDetectionPath().Check(result.LookupBacklink().SerializedParent).ToList();
                Debug.Log($"{diags.Count} custom materials.");
                // TODO: 置き場として微妙
                foreach (var diagnostic in diags)
                {
                    var m = diagnostic.CustomizedShaderUsedMaterial;
                    var o = diagnostic.ReferencedRenderer;
                    
                    Debug.Log($@"custom shader warning: {diagnostic.Message()}
Material: {m}
GameObject: {o.gameObject}
Please consult familiar person to resolve rootVisualElement warning, or you may want to ignore
-----------------------------
StackTrace:");
                }
            })
            {
                tooltip = lang.Start()
            };
            run.Add(new Label(lang.Start()));
            run.SetEnabled(false);
#if RIH_HAS_UNI_GLTF
            rootObject.RegisterValueChangedCallback(ev =>
            {
                run.SetEnabled(ev.newValue != null);
            });
#endif
            rootVisualElement.Add(run);
            rootVisualElement.Add(new HorizontalLine());
            rootVisualElement.Add(new UniGltfInstallPrompt(lang));
            rootVisualElement.Add(destination);
            {
                var button = new Button(() =>
                {
                    EditorUtility.RevealInFinder(AssetDatabase.GetAssetPath(destination.value));
                });
                destination.RegisterValueChangedCallback(ev =>
                {
                    button.SetEnabled(ev.newValue != null);
                });
                
                button.Add(new Label(lang.OpenInFileSystemLabel()));
                button.SetEnabled(false);
                rootVisualElement.Add(button);
            }
            
            rootVisualElement.Add(modelContainsVertexColorNote);
        }
        
        private static Toggle CreatePreprocessorToggleCheckbox(ObjectField rootObjectField, ILocalizedTexts lang)
        {
            var ret = new Toggle(lang.InvokeVRCSDKPreprocessorLabel()) { value = HasVRCSDK3A, tooltip = lang.InvokeVRCSDKPreprocessorTooltip() };
            ret.SetEnabled(HasVRCSDK3A);
#if RIH_HAS_VRCSDK3A
            rootObjectField.RegisterValueChangedCallback(ev =>
            {
                var v = ev.newValue as GameObject;
                if (v == null)
                {
                    return;
                }

                var hasAvatarDescriptor = v.TryGetComponent(out VRCAvatarDescriptor _);
                ret.value = hasAvatarDescriptor;
                ret.SetEnabled(hasAvatarDescriptor);
            });
#endif
            return ret;
        }

        private static ExperimentalSettingsFoldout CreateExperimentalSettingsFoldout(ILocalizedTexts lang)
        {
            return new ExperimentalSettingsFoldout(lang);
        }
        
        // ReSharper disable once InconsistentNaming
        private static Toggle CreateNDMFManualBakeCheckbox(Toggle v, ILocalizedTexts lang)
        {
            var ret = new Toggle(lang.NDMFManualBakeLabel()) { value = HasNDMF, tooltip = lang.NDMFManualBakeTooltip() };
            ret.SetEnabled(Toggleable());
            v.RegisterValueChangedCallback(ev =>
            {
                if (ev.newValue)
                {
                    ret.value = true;
                }
                ret.SetEnabled(Toggleable());
            });
            return ret;

            bool Toggleable() =>
                (v.value, HasNDMF) switch
                {
                    (true, true) => false,
                    (true, false) => false,
                    (false, true) => true,
                    (false, false) => true,
                };
        }
    }
}