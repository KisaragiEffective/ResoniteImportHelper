using UnityEngine;

namespace ResoniteImportHelper.UnityEditorUtility
{
    internal static class MaterialUtility
    {
        internal static Material CreateVariant(Material parent)
        {
            Debug.Log($"create material variant for {parent.name}");
            return new Material(parent)
            {
                parent = parent
            };
        }
    }
}