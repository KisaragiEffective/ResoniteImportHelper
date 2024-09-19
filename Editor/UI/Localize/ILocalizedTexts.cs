using System.Diagnostics.CodeAnalysis;

namespace ResoniteImportHelper.UI.Localize
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
        
        string OpenInFileSystemLabel();
        string ProcessedAvatarLabel();
        string ProcessedAvatarTooltip();
        string InvokeVRCSDKPreprocessorLabel();
        string InvokeVRCSDKPreprocessorTooltip();
        string NDMFManualBakeLabel();
        string NDMFManualBakeTooltip();
    }
}