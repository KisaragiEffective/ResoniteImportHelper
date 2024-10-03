#nullable enable
using ResoniteImportHelper.UI.Localize;
using UnityEngine.UIElements;

namespace ResoniteImportHelper.UI
{
    internal sealed class ExperimentalSettingsFoldout: Foldout
    {
        internal readonly Toggle BakeShadersConfigurationIntoTextures;

        internal readonly Toggle GenerateIntermediateArtifact;

        internal ExperimentalSettingsFoldout(ILocalizedTexts lang)
        {
            this.value = false;
            this.text = lang.ExperimentalSettingRootLabel();
            this.Add(
                new HelpBox(lang.ExperimentalSettingsAreNeverSupported(), HelpBoxMessageType.Warning)
            );
            
            BakeShadersConfigurationIntoTextures = new Toggle(lang.ExperimentalSetting_BakeLilToonLabel())
            {
                tooltip = lang.ExperimentalSetting_BakeLilToonTooltip()
            };
            this.Add(BakeShadersConfigurationIntoTextures);

            GenerateIntermediateArtifact = new Toggle(lang.ExperimentalSetting_GenerateIntermediateArtifactLabel())
            {
                tooltip = lang.ExperimentalSetting_GenerateIntermediateArtifactTooltip()
            };
            
            this.Add(GenerateIntermediateArtifact);
        }

    }
}