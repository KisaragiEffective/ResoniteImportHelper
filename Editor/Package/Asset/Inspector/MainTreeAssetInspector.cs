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
            root.Add(new Button(() =>
            {
                var x = Path.GetTempFileName();
                File.WriteAllText(x, (this.target as MainTreeAsset)!.text);
                Debug.Log($"Write full text on {x}.");
            }));

            root.Add(new TextField() { multiline = true, value = (this.target as MainTreeAsset)!.text.Substring(0, 5000) });

            return root;
        }
    }
}
