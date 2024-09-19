using UnityEngine;
using UnityEngine.UIElements;

namespace ResoniteImportHelper.UI
{
    internal sealed class HorizontalLine : VisualElement
    {
        internal HorizontalLine()
        {
            this.style.borderTopColor = new StyleColor(new Color(153.0f / 255.0f, 153.0f / 255.0f, 153.0f / 255.0f));
            this.style.borderTopWidth = new StyleFloat(1.0f);
        }
    }
}