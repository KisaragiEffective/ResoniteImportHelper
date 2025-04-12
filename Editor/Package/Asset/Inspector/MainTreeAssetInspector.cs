using System.IO;
using ResoniteImportHelper.Editor.Package.Asset.Types;
using UnityEngine;
using UnityEngine.UIElements;
using CustomInspector = UnityEditor.CustomEditor;
using UserMadeInspector = UnityEditor.Editor;

namespace ResoniteImportHelper.Package.Asset.Inspector
{
    [CustomInspector(typeof(MainTreeAsset))]
    public class MainTreeAssetInspector: UserMadeInspector
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            root.Add(new Label("Content"));
            var b = new Button(() =>
            {
                var x = Path.GetTempFileName();
                File.WriteAllText(x, (this.target as MainTreeAsset)!.text);
                Debug.Log($"Wrote full text temporary file located in {x}.");
            });
            b.Add(new Label("Write whole text to temporary file"));
            root.Add(b);

            root.Add(new TextField() { multiline = true, value = (this.target as MainTreeAsset)!.text.Substring(0, 5000) });

            return root;
        }
    }
}
