using System;

namespace ResoniteImportHelper.UI.Localize
{
    internal sealed class English : ILocalizedTexts
    {
        public string Root() => "Target avatar root";

        public string RootTooltip() =>
            "Specify avatar root. This is usually prefab root, or GameObject where VRCAvatarDescriptor is attached.";

        public string ExportSetting() => "Export settings";

        public string ExportedAvatarLabel() => "Processed avatar";

        public string ExportedAvatarTooltip() => "Processed avatar. Readonly.";

        public string ModelContainsVertexColor() =>
            "Model contains Vertex Color. Import it on Resonite if you want to use.";

        public string Start() => "Start";

        public string UniGLTFIsNotInstalled() => "UniGLTFがプロジェクトにインストールされていません。続行するにはプロジェクトへインストールして下さい。";

        public string OpenInstallationPageForUniGLTF() => "UniGLTFのインストールページを開く";

        public string InstallUniGLTFAutomatically() => "自動的にUniGLTFをインストールする";

        public string OpenInFileSystemLabel() => "Open in file system";
        public string ProcessedAvatarLabel() => "Processed avatar";

        public string ProcessedAvatarTooltip() => "Processed avatar. Readonly.";

        public string InvokeVRCSDKPreprocessorLabel() => "Invoke VRChat SDK Preprocessor";

        public string InvokeVRCSDKPreprocessorTooltip() => "Do you want NDMF or VRCFury to run?";
        
        public string NDMFManualBakeLabel() => "NDMF Manual Bake";

        public string NDMFManualBakeTooltip() => "Do you want NDMF to run?";
    }
}