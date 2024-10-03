#nullable enable
using System;
using System.Linq;
using ResoniteImportHelper.UI.Localize;
using UnityEngine.UIElements;

namespace ResoniteImportHelper.UI.Component
{
    internal sealed class LocaleSelector : VisualElement
    {
        internal readonly PopupField<LocaleKind> PullDown;
        private LocaleKind Kind => PullDown.value;

        internal ILocalizedTexts GetLanguage()
        {
            return Kind switch
            {
                LocaleKind.English => new English(),
                LocaleKind.Japanese => new Japanese(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        internal LocaleSelector()
        {
            PullDown = new PopupField<LocaleKind>(Enum.GetValues(typeof(LocaleKind)).Cast<LocaleKind>().ToList(),
                LocaleKind.English);
            this.Add(PullDown);
        }

        internal enum LocaleKind
        {
            English = 0,
            Japanese = 1,
        }
    }
}