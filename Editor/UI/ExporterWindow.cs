using System.Linq;
using ResoniteImportHelper.Lint.Pass;
using ResoniteImportHelper.TransFront;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
#if RIH_HAS_VRCSDK3A
using VRC.SDK3.Avatars.Components;
#endif
using static ResoniteImportHelper.UI.ExternalServiceStatus;

namespace ResoniteImportHelper.UI {
    internal class ExporterWindow : EditorWindow
    {
        [MenuItem("Tools/Resonite Import Helper")]
        private static void OnOpen()
        {
            GetWindow<ExporterWindow>().Show();
        }

        // ReSharper disable once ParameterHidesMember
        private static void OnMount(VisualElement rootVisualElement)
        {
            var rootObject = new ObjectField("Target avatar root")
            {
                objectType = typeof(GameObject),
                tooltip = "Specify avatar root. This is usually prefab root, or GameObject where VRCAvatarDescriptor is attached."
            };
            rootVisualElement.Add(rootObject);
            var exportSettingFoldout = new Foldout { text = "Export settings" };
            rootVisualElement.Add(exportSettingFoldout);
            // ReSharper disable once InconsistentNaming
            var doRunVRCSDK3APreprocessors = CreatePreprocessorToggleCheckbox(rootObject);
            exportSettingFoldout.Add(doRunVRCSDK3APreprocessors);
            // ReSharper disable once InconsistentNaming
            var doNDMFManualBake = CreateNDMFManualBakeCheckbox(doRunVRCSDK3APreprocessors);
            exportSettingFoldout.Add(doNDMFManualBake);
            var experimentalSettingsFoldout = CreateExperimentalSettingsFoldout();
            exportSettingFoldout.Add(experimentalSettingsFoldout);
            
            var destination = new ObjectField("Processed avatar")
            {
                objectType = typeof(GameObject),
                tooltip = "Processed avatar. Readonly."
            };
            destination.RegisterCallback<DragUpdatedEvent>(ev => ev.PreventDefault());
            destination.RegisterCallback<DragPerformEvent>(ev => ev.PreventDefault());
            var modelContainsVertexColorNote =
                new HelpBox("Model contains Vertex Color. Import it on Resonite if you want to use.",
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
Please consult familiar person to resolve this warning, or you may want to ignore
-----------------------------
StackTrace:");
                }
            })
            {
                tooltip = "Start"
            };
            run.Add(new Label("Start"));
            run.SetEnabled(false);
#if RIH_HAS_UNI_GLTF
            rootObject.RegisterValueChangedCallback(ev =>
            {
                run.SetEnabled(ev.newValue != null);
            });
#endif
            rootVisualElement.Add(run);
            rootVisualElement.Add(CreateHorizontalLine());
#if !RIH_HAS_UNI_GLTF
            {
                rootVisualElement.Add(
                    new HelpBox("UniGLTFがプロジェクトにインストールされていません。続行するにはプロジェクトへインストールして下さい。", HelpBoxMessageType.Error)
                );
                var button = new Button(() =>
                {
                    Application.OpenURL("https://github.com/vrm-c/UniVRM/releases");
                });
                button.Add(new Label("UniGLTFのインストールページを開く"));
                rootVisualElement.Add(button);
            }
#endif
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
                
                button.Add(new Label("Open in file system"));
                button.SetEnabled(false);
                rootVisualElement.Add(button);
            }
            
            rootVisualElement.Add(modelContainsVertexColorNote);
        }

        private static ExperimentalSettingsFoldout CreateExperimentalSettingsFoldout()
        {
            return new ExperimentalSettingsFoldout();
        }

        private void CreateGUI()
        {
            OnMount(rootVisualElement);
        }

        private static Toggle CreatePreprocessorToggleCheckbox(ObjectField rootObjectField)
        {
            var ret = new Toggle("Invoke VRChat SDK Preprocessor") { value = HasVRCSDK3A, tooltip = "Do you want NDMF or VRCFury to run?" };
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

        // ReSharper disable once InconsistentNaming
        private static Toggle CreateNDMFManualBakeCheckbox(Toggle v)
        {
            var ret = new Toggle("NDMF Manual Bake") { value = HasNDMF, tooltip = "Do you want NDMF to run?" };
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

        // ReSharper disable once InconsistentNaming

        private static VisualElement CreateHorizontalLine() => new()
        {
            style =
            {
                borderTopColor = new StyleColor(new Color(153.0f / 255.0f, 153.0f / 255.0f, 153.0f / 255.0f)),
                borderTopWidth = new StyleFloat(1.0f) 
            }
        };
    }
}
