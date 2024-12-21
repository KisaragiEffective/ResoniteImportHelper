#nullable enable
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

        public string UniGLTFIsNotInstalled() => "UniGLTF is not installed on the Project. Please install to continue.";

        public string OpenInstallationPageForUniGLTF() => "Open installation page";

        public string InstallUniGLTFAutomatically() => "install UniGLTF automatically";

        public string ConflictingVersionOfUniGLTFIsInstalled() =>
            "UniGLTF is installed, but its version does not match our requirement.";

        public string OpenInFileSystemLabel() => "Open in file system";
        public string ProcessedAvatarLabel() => "Processed avatar";

        public string ProcessedAvatarTooltip() => "Processed avatar. Readonly.";

        public string InvokeVRCSDKPreprocessorLabel() => "Invoke VRChat SDK Preprocessor";

        public string InvokeVRCSDKPreprocessorTooltip() => "Do you want NDMF or VRCFury to run?";

        public string NDMFManualBakeLabel() => "NDMF Manual Bake";

        public string NDMFManualBakeTooltip() => "Do you want NDMF to run?";
        public string ExperimentalSettingRootLabel() => "Experimental Settings";

        public string ExperimentalSettingsAreNeverSupported() =>
            "Experimental Settings may disappear at anytime, and break your avatar!\nUse at your own peril and wisely.";

        public string ExperimentalSetting_BakeLilToonLabel() => "Bake lilToon's configuration into Texture";

        public string ExperimentalSetting_BakeLilToonTooltip() =>
            "Do you want to bake configuration such as AlphaMask?";

        public string ExperimentalSetting_GenerateIntermediateArtifactLabel() => "Generate intermediate prefab";

        public string ExperimentalSetting_GenerateIntermediateArtifactTooltip() =>
            "Do you want to intermediate artifacts to debug?\nIt will be persisted under the same directory.";

        public string ExperimentalSetting_ApplyRootScale() => "Apply root scale to Armature";

        public string ExperimentalSetting_ApplyRootScaleTip() =>
            "Do you want to multiply Armature scale by root scale?";

        public string OpenInAssetWindowLabel() => "Open in Project window";

        public string AppendToCurrentSceneLabel() => "Append to current Scene";
    }
}
