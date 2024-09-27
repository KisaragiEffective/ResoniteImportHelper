#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ResoniteImportHelper.ClonedMarker
{
    [CustomEditor(typeof(IntermediateClonedHierarchyMarker))]
    internal sealed class IntermediateClonedHierarchyMarkerEditor: UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            if (TypedTarget().automaticallyAdded)
            {
                root.Add(new HelpBox("If this component is seen, you might encountered error(s). Please report that!",
                    HelpBoxMessageType.Warning));
            }
            else
            {
                root.Add(new HelpBox(
                    "This component is not intended to be manually-added.\n" +
                    "Consider remove this.\n" +
                    "This is not Public API component, and no meaning in general situation.", HelpBoxMessageType.Error));
            }

            // TODO: refactor: get it from Unity Package Manifest
            {
                var b = new Button(() =>
                {
                    Application.OpenURL("https://github.com/KisaragiEffective/ResoniteImportHelper/");
                });
                b.Add(new Label("Open source code repository"));
                root.Add(b);
            }
            {
                var b = new Button(() =>
                {
                    Application.OpenURL("https://github.com/KisaragiEffective/ResoniteImportHelper/issues");
                });
                b.Add(new Label("Open bug tracker"));
                root.Add(b);
            }
            
            root.Add(new ObjectField("Original avatar") { value = TypedTarget().original });
            
            return root;
        }

        private IntermediateClonedHierarchyMarker TypedTarget() => (this.target as IntermediateClonedHierarchyMarker)!;
    }
}
#endif
