#nullable enable
using KisaragiMarine.ResoniteImportHelper.UI.Component;
using UnityEditor;
using UnityEngine.UIElements;

namespace KisaragiMarine.ResoniteImportHelper.UI {
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
            rootVisualElement.Add(new WindowHeader());
            var localeSelector = new LocaleSelector();
            // var sessionStrider = new SessionStrider();

            rootVisualElement.Add(localeSelector);
            rootVisualElement.Add(new HorizontalLine());
            rootVisualElement.Add(new Body(localeSelector));
        }

        private void CreateGUI()
        {
            OnMount(rootVisualElement);
        }
    }
}
