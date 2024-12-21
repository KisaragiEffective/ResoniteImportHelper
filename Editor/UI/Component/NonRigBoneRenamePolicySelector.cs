using System.Collections.Generic;
using ResoniteImportHelper.UI.Localize;
using UnityEngine.UIElements;

namespace ResoniteImportHelper.UI.Component
{
    internal enum NonRigBoneRenamePolicy
    {
        Always,
        Never,
    }

    internal sealed class NonRigBoneRenamePolicySelector: DropdownField
    {
        private readonly ILocalizedTexts _lang;

        internal NonRigBoneRenamePolicySelector(ILocalizedTexts lang):
            base(new List<string> {lang.NonRigBoneRenamePolicy_Always(), lang.NonRigBoneRenamePolicy_Never()},
            lang.NonRigBoneRenamePolicy_Always())
        {
            this._lang = lang;
            this.label = lang.NonRigBoneRenameProcessDropdownLabel();
        }

        internal bool DoRename() => this.value == this._lang.NonRigBoneRenamePolicy_Always();
    }
}
