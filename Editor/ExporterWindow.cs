using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static ResoniteImportHelper.Editor.ExternalServiceStatus;

namespace ResoniteImportHelper.Editor {
    internal class ExporterWindow : EditorWindow
    {
        [MenuItem("Tools/Resonite Import Helper")]
        private static void OnOpen()
        {
            GetWindow<ExporterWindow>().Show();
        }

        private void CreateGUI()
        {
            var rootObject = new ObjectField("処理する対象のアバター") { objectType = typeof(GameObject) };
            rootVisualElement.Add(rootObject);
            // ReSharper disable once InconsistentNaming
            var doRunVRCSDK3APreprocessors = CreatePreprocessorToggleCheckbox();
            rootVisualElement.Add(doRunVRCSDK3APreprocessors);
            // ReSharper disable once InconsistentNaming
            var doNDMFManualBake = CreateNDMFManualBakeCheckbox(doRunVRCSDK3APreprocessors);
            rootVisualElement.Add(doNDMFManualBake);
            var destination = new ObjectField("処理したアバター") { objectType = typeof(GameObject) };
            destination.RegisterValueChangedCallback(ev =>
            {
                destination.SetValueWithoutNotify(ev.previousValue);
            });

            var run = new Button(() =>
            {
                destination.SetValueWithoutNotify(BusinessLogic.PerformConversion(
                        rootObject.value as GameObject,
                        doRunVRCSDK3APreprocessors.value,
                        doNDMFManualBake.value
                    )
                );
            });
            run.Add(new Label("処理を開始"));
            run.SetEnabled(false);
            rootObject.RegisterValueChangedCallback(ev =>
            {
                run.SetEnabled(ev.newValue != null);
            });
            rootVisualElement.Add(run);
            rootVisualElement.Add(CreateHorizontalLine());
            rootVisualElement.Add(destination);
        }

        private static Toggle CreatePreprocessorToggleCheckbox()
        {
            var ret = new Toggle("VRChat SDKのプリプロセッサを走らせる") { value = HasVRCSDK3A };
            ret.SetEnabled(HasVRCSDK3A);
            return ret;
        }

        // ReSharper disable once InconsistentNaming
        private static Toggle CreateNDMFManualBakeCheckbox(Toggle v)
        {
            var ret = new Toggle("NDMF Manual Bake") { value = HasNDMF };
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
