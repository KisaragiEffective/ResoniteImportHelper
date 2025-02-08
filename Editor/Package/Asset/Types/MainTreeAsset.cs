
using UnityEngine;

namespace ResoniteImportHelper.Editor.Package.Asset.Types
{
    public class MainTreeAsset: ScriptableObject
    {
        public string text;
        public MainTreeAsset(string text)
        {
            this.text = text;
        }
    }
}
