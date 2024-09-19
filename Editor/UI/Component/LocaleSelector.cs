using System;
using System.Collections.Generic;
using System.Linq;
using ResoniteImportHelper.UI.Localize;
using UnityEngine.UIElements;

namespace ResoniteImportHelper.UI.Component
{
    internal sealed class LocaleSelector : VisualElement
    {
        private readonly PopupField<LocaleKind> _f;
        private LocaleKind Kind => _f.value;

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
            _f = new PopupField<LocaleKind>(Enum.GetValues(typeof(LocaleKind)).Cast<LocaleKind>().ToList(),
                LocaleKind.English);
            this.Add(_f);
        }

        private enum LocaleKind
        {
            English = 0,
            Japanese = 1,
        }
    }
}