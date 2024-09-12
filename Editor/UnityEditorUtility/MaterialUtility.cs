using UnityEngine;

namespace ResoniteImportHelper.UnityEditorUtility
{
    internal static class MaterialUtility
    {
        internal static Material CreateVariant(Material parent)
        {
            return new Material(parent)
            {
                parent = parent
            };
        }
    }
}