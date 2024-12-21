#nullable enable
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
        public string ConflictingVersionOfUniGLTFIsInstalled() => "UniGLTFはインストールされていますが、指定されたバージョンにマッチしません！";

        public string OpenInFileSystemLabel() => "エクスプローラーで開く";

        public string ProcessedAvatarLabel() => "変換されたアバター";

        public string ProcessedAvatarTooltip() => "変換されたアバターです。 (読み取り専用)";

        public string InvokeVRCSDKPreprocessorLabel() => "VRChat SDK向けのツールを経由させる";

        public string InvokeVRCSDKPreprocessorTooltip() =>
            "チェックを入れると、NDMFやVRCFuryなど、VRChat Avatar SDKのBuild Pipelineを通過するツールを変換前に動作させます。";

        public string NDMFManualBakeLabel() => "NDMFツールを経由させる";

        public string NDMFManualBakeTooltip() => "チェックを入れると、Non-Destructure Modular Framework及びそのツールを変換前に動作させます。";
        public string ExperimentalSettingRootLabel() => "試験的機能";

        public string ExperimentalSettingsAreNeverSupported() =>
            "試験的機能は安定して保証することが存在されません。つまり、いつ消えてもおかしくないということです。\n" +
            "加えて、有効にした場合変換結果が壊れる可能性のある設定も含まれるかもしれません。\n" +
            "あなたが何を行っているのか正確に理解している場合のみこれらのオプションを調整して下さい。";

        public string ExperimentalSetting_BakeLilToonLabel() => "lilToonの設定を一枚のテクスチャーに焼く";

        public string ExperimentalSetting_BakeLilToonTooltip() =>
            "チェックを入れると、変換結果に含まれるテクスチャにおいてlilToonの色調設定やアルファマスクなどを焼き込みます。";

        public string ExperimentalSetting_GenerateIntermediateArtifactLabel() => "中間プレハブを生成する";

        public string ExperimentalSetting_GenerateIntermediateArtifactTooltip() =>
            "チェックを入れると、デバッグのために中間アーティファクトを生成します。中間アーティファクトはglTFと同じディレクトリに生成されます。";

        public string ExperimentalSetting_ApplyRootScale() => "ルートのスケールをアーマチュアに適用する";

        public string ExperimentalSetting_ApplyRootScaleTip() =>
            "チェックを入れると、ルートに設定されているスケールをアーマチュアのスケールに乗算し、ルートのスケールを(1, 1, 1)にします。";

        public string NonRigBoneRenameProcessDropdownLabel() => "<NoIK> の付与";

        public string NonRigBoneRenamePolicy_Always() => "常に付与する";

        public string NonRigBoneRenamePolicy_Never() => "常に付与しない";
    }
}
