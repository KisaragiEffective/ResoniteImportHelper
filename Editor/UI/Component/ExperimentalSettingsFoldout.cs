#nullable enable
using KisaragiMarine.ResoniteImportHelper.UI.Localize;
using UnityEngine.UIElements;

namespace KisaragiMarine.ResoniteImportHelper.UI
{
    internal sealed class ExperimentalSettingsFoldout: Foldout
    {
        internal readonly Toggle BakeShadersConfigurationIntoTextures;

        internal readonly Toggle GenerateIntermediateArtifact;

        internal readonly Toggle ApplyRootScale;

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

            ApplyRootScale = new Toggle(lang.ExperimentalSetting_ApplyRootScale())
            {
                tooltip = lang.ExperimentalSetting_ApplyRootScaleTip()
            };

            this.Add(ApplyRootScale);
        }

    }
}
