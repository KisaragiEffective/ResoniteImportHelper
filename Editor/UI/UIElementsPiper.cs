#nullable enable
using UnityEngine.UIElements;

namespace KisaragiMarine.ResoniteImportHelper.UI
{
    internal static class UIElementsPiper
    {
        internal static T AddAndReturn<T>(this VisualElement parent, T element) where T : VisualElement
        {
            parent.Add(element);
            return element;
        }
    }
}
