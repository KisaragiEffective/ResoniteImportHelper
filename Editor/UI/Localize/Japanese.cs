namespace ResoniteImportHelper.UI.Localize
{
    internal sealed class Japanese : ILocalizedTexts
    {
        public string Root() => "変換するアバター";

        public string RootTooltip() => "変換するアバターを指定して下さい。";

        public string ExportSetting() => "変換設定";

        public string ModelContainsVertexColor() => "指定したモデルには頂点カラーが含まれています。必要であればResoniteへインポートして下さい。";

        public string Start() => "開始";

        public string UniGLTFIsNotInstalled() => "UniGLTFがプロジェクトにインストールされていません。続行するにはプロジェクトへインストールして下さい。";

        public string OpenInstallationPageForUniGLTF() => "UniGLTFのインストールページを開く";

        public string InstallUniGLTFAutomatically() => "自動的にUniGLTFをインストールする";

        public string OpenInFileSystemLabel() => "エクスプローラーで開く";

        public string ProcessedAvatarLabel() => "変換されたアバター";

        public string ProcessedAvatarTooltip() => "変換されたアバターです。 (読み取り専用)";

        public string InvokeVRCSDKPreprocessorLabel() => "VRChat SDK向けのツールを経由させる";

        public string InvokeVRCSDKPreprocessorTooltip() =>
            "チェックを入れると、NDMFやVRCFuryなど、VRChat Avatar SDKのBuild Pipelineを通過するツールを変換前に動作させます。";

        public string NDMFManualBakeLabel() => "NDMFツールを経由させる";

        public string NDMFManualBakeTooltip() => "チェックを入れると、Non-Destructure Modular Framework及びそのツールを変換前に動作させます。";
    }
}