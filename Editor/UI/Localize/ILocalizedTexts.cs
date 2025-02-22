#nullable enable
using System.Diagnostics.CodeAnalysis;

namespace KisaragiMarine.ResoniteImportHelper.UI.Localize
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal interface ILocalizedTexts
    {
        string Root();
        string RootTooltip();
        string ExportSetting();

        string ModelContainsVertexColor();
        string Start();
        string UniGLTFIsNotInstalled();
        string OpenInstallationPageForUniGLTF();
        string InstallUniGLTFAutomatically();
        string ConflictingVersionOfUniGLTFIsInstalled();

        string OpenInFileSystemLabel();
        string ProcessedAvatarLabel();
        string ProcessedAvatarTooltip();
        string InvokeVRCSDKPreprocessorLabel();
        string InvokeVRCSDKPreprocessorTooltip();
        string NDMFManualBakeLabel();
        string NDMFManualBakeTooltip();

        string ExperimentalSettingRootLabel();
        string ExperimentalSettingsAreNeverSupported();
        string ExperimentalSetting_BakeLilToonLabel();
        string ExperimentalSetting_BakeLilToonTooltip();
        string ExperimentalSetting_GenerateIntermediateArtifactLabel();
        string ExperimentalSetting_GenerateIntermediateArtifactTooltip();
        string ExperimentalSetting_ApplyRootScale();
        string ExperimentalSetting_ApplyRootScaleTip();
        string OpenInAssetWindowLabel();
        string AppendToCurrentSceneLabel();
    }
}
