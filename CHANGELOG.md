# 変更履歴

## 0.1.20 (2024-02-15)
* fix: UniVRMがインストールされている場合、ブートストラップ時にUniVRMのバージョンを操作する

## 0.1.19 (2024-01-13)
* docs: 一部の文書のマークアップが壊れているのを修正
* docs: RIHを作った経緯を公開
* feat: glTFへ変換したアバターをプロジェクトウィンドウで開くボタン、シーンに追加するボタンを追加
* ci: zizmorによってエラーになるワークフローの記述を修正
* fix: Resoniteによって正面になるように、Unity座標系上においてZ-をデフォルトのアバターの正面とするように
* refactor: C#の名前空間を `ResoniteImportHelper` から `KisaragiMarine.ResoniteImportHelper` へ移動

## 0.1.18 (2024-11-29)
* chore: リリースにプッシュされるVPM互換クライアント用のZIPファイルのパッケージマニフェストの `repository` フィールドがリリースのタグのHEADをパーマリンクで指すように
* fix: VRChat SDKが入っているがlilToonが入っていない環境でコンパイルエラーが起こるのを修正
* docs: 画像の最適化
* docs: 要求するUniGLTFのバージョンへ飛ぶリンクを固定する
* docs: VPM互換クライアントからインストールする方法を追記
* chore(deps): UniGLTFの要求バージョンを0.125.0から0.128.0へ更新
  * 更新後のUIにエラーが表示された場合、そのまま更新できます
  * UniVRMをインストールしている場合、UniVRMは自動で更新されないので手動で更新して下さい
* refactor: コンパイル時に出る警告を削減
* feat: プレハブルートのスケールをアーマチュアに適用する試験的設定を追加
  * 試験的設定なのでオプト・インしないと有効になりません
  * プレハブルートのスケールが2倍、アーマチュアのスケールも2倍なら、アーマチュアのスケールが4倍としてエクスポートされます

## 0.1.17 (2024-11-14)
* docs: 同じプロジェクトに入れても壊れないことを明記
* docs: Missing Scriptをサポート対象から除外
* docs: Missing Shaderをサポート対象から除外
* chore: Microsoft.Unity.Analyzersを導入
* docs: UniGLTFのバージョンを明記
* docs: glTFやNDMFなどへのリンクを追加
* refactor: コンパイル時の警告を一部解決
* fix: 限定された条件下でStandardシェーダーの割当に失敗する現象を修正
* refactor: ロジックをユーティリティクラスへ移動
* feat: **マテリアルの名前を元の名前に近い名前で保存するように**
* refactor: インターフェースを再定義
* refactor: APIの安定性を再定義
* docs: Resoniteが日本において商標の候補として審査されていることを追記
* chore: VRChat Package Manager互換のリリース経路を整備

## 0.1.16 (2024-10-06)
* fix: lilToon以外のMaterialが保存されず、glTFの読み込み時に例外が発生していた問題を修正
* fix: スタンドアロン環境で目のボーンを検知する際に例外が出ないように
* fix: HumanoidアバターでHipボーンが見つからないときに例外を出さないように
* chore: プレハブのオーバーライド検査の信頼度を向上させる準備
* ui: NDMFがインストールされていない時はNDMFにチェックが入らないように
* refactor: 冗長な型を削除
* docs: 貢献方法を明文化
* docs: 対応しているシェーダーの類型を明文化
* chore: GitHub Sponsorsへのリンクをレポジトリに追加

## 0.1.15 (2024-10-03)
* chore: SkinnedMeshRendererのシェイプキーの値の初期値をエクスポートする準備
  * 問題は完全に解決されたわけではありません。
  * 経緯は[issue #109](https://github.com/KisaragiEffective/ResoniteImportHelper/issues/109) で追うことができます
* docs: Constraints関連のワークアラウンドを更新
* chore: `TiedBakeSourceDescriptor` にアイコンが設定されました

## 0.1.15-packaging.2 (2024-09-29)
* build: ビルドが失敗するのを解消

## 0.1.15-packaging.1 (2024-09-29)
* build: 非VRChat環境でビルドが失敗するのを解消

## 0.1.15-packaging.0 (2024-09-29)
* docs: 0.1.13で実装された機能についての注意書きを調整
* chore: ブートストラップ機能でバージョンが違う場合に警告を出すように
    * UniGLTF 0.125.0以外はサポートされていません。
* chore: editorconfigを導入
* chore: Assembly Definitionの冗長な記述を簡略化
* chore: UniGLTFをインストールせずにRIHをインストールできるようにパッケージマニフェストをごまかす
    * これは「RIHの動作にUniGLTFを必要としなくなった」という意味では**ありません**。

## 0.1.14 (2024-09-27)
* perf: マテリアルの変換が高速に
* docs: 関係ない警告をREADMEから除去
* docs: カスタムシェーダーに関するトラブルシューティングを追加
* build: 必要ない`using`文を削除

## 0.1.13 (2024-09-24)
* feat: 半透明のマテリアルを半透明としてエクスポートするように
  * 今までは不透明になっていたため、インポート後に追加の処理が必要でした
* feat: ノーマルマップが存在する場合はエクスポートされるように

## 0.1.12 (2024-09-20)
* feat: エラーなどにより何らかの理由でシーン上にオブジェクトが残った場合専用コンポーネントによりバグの可能性を示唆するように
* refactor: `AvatarTransformService` の処理をメソッドに分割
* feat: ブートストラップ機能を追加
  * Package Managerのウィンドウを開くことは必須ではなくなります
  * docs: READMEを調整
* ui: 簡易的な多言語化を実装
* docs(image): TIFFファイルをKrita形式に置き換え
  * 格納されていたTIFFファイルは、実際にはロスレスではありませんでした。
  * また、ImageMagickなどのサポートが芳しくないためKrita形式に置き換えることとしました。
    * 今後、オリジナルのファイルを開く際はKritaを使用して下さい。
* fix: 目の定義が`Animator`にも`VRCAvatarDescriptor`にもない際に例外が出ないように

## 0.1.11 (2024-09-15)
* fix: マテリアルが白くなる現象を修正
  * 「perf: lilToon変換で生成された全てのMaterial Variantを一度にインポートするように」が取り消されました
* refactor: 不必要なマテリアルを完了前に削除する
* build: Assembly Definitionを修正し、AssetBundleをビルドする時に失敗しないように

## 0.1.10 (2024-09-14)
* ui: メインメニューが表示できない場合があるのを修正

## 0.1.9 (2024-09-13)
* perf: lilToon変換で生成された全てのMaterial Variantを一度にインポートするように
  * この最適化により処理時間が数秒短くなります
* ui: ツール名とバージョン情報を表示するように
* fix: 処理途中のアバターがシーン上に残っていた問題を修正
* chore: Editor上で利用できるパフォーマンスメトリクスを有効化

## 0.1.8 (2024-09-13)
* fix: lilToon変換を行った際にRendererのMaterialを更新するように
* fix: lilToon変換を行った際に元々のMaterialに上書きされないように
* feat: デバッグのためにglTFに変換する前のモデルを出力できるように
  * **試験的機能のため、既定では無効です**
    * Experimental Settings から Generate intermediate prefabを有効にしてオプトインする必要があります
  * この処理を有効にすると処理時間が若干伸びる場合があります
* refactor: interfaceの整理

## 0.1.7 (2024-09-12)
* fix: `MeshFilter`ではなく`MeshRenderer`を有効にするように
* fix(allocator): `Texture2D`が正常に保存できていなかった問題を修正
* fix: アクティブになっていないオブジェクトの下に理解できるレンダラーがある場合、glTFへの変換に含まれるように
* refactor: 一部プラットフォームでしか利用できない可能性のあるクラスの排除
* fix: backlinkを非Prefabの入力でも生成するように
* feat: VRC Avatar Descriptorが存在し、`Animator`に両目のボーンの参照がない場合、VRC ADの参照をコピーするように
  * docs: 上述の機能を記述

## 0.1.6 (2024-09-12)
* fix: lilToon変換を行った時にHarmonyがFormatExceptionをスローし、変換が正常に行われない問題を修正

## 0.1.5 (2024-09-10)
* fix: 頂点カラーの検出がおかしいのを修正
  * この変更により、頂点カラーが全て白の場合はシェーダーが頂点カラーを読み取らないものとして扱います
* feat: lilToonの自動的な焼き込みを追加 (試験的サポート)
  * **試験的サポートのため、既定では無効です**
    * Experimental Settingsでたたまれている項目を開き、明示的に"Bake lilToon's configuration into Texture"のチェックボックスをクリックして下さい
  * プロジェクトにlilToonがない場合、あるいはlilToonが使われていない場合は何も起きません
  * マテリアルのインスペクタから \[全て焼き込み\] を行うのと同様の処理です
  * 各種マップは反映されません
  * VRChat Avatar SDKが存在しない場合、焼き込みの際にダイアログが出ることがあります
* docs: 商標の免責を追加
* docs: Public APIの例外を追加
* log: 非Standardシェーダーを検知した際にログへ出力

## 0.1.4 (2024-09-06)
* fix: [ルーシュカ](https://booth.pm/ja/items/4296675)ちゃんのUpper ChestがVRIKにChestとして認識される問題を修正
* refactor: アセンブリの分割を伴うリストラクチャリング
* docs: Unity 2022.3系列のみのサポート及びバージョニングについて規定を明文化
* docs: "gLTF"をglTFに置き換え

## 0.1.3 (2024-09-06)
* feat: 出力されるglTFの名前を固定せずに元々の`GameObject`から取るように

## 0.1.2 (2024-09-05)
* fix: `DivideVertexBuffer` を `false` に

## 0.1.1 (2024-09-05)
* renovate: 漏れた`.meta`ファイルの追加
* feat: glTFに変換する前のPrefabの参照を保存するように
* fix: 変換前に`SkinnedMeshRenderer`及び`MeshFilter`を有効化するように
* fix: 一時アセットを格納するフォルダは一度だけ作成する
* fix: MissingComponentExceptionを避ける
* feat: モデルの頂点カラーもglTFに含むように
* ui: エクスポート設定を折りたたむ
* renovate: スキーマの誤りを修正
* fix: 一時アセットを格納するフォルダを書き込み前に初期化する
* renovate: 有効化
* docs: 目的及び非目的のセクションで動詞を用いる
* docs: UPMからインストールする方法の補足
* docs: 画像がインラインで表示されているのを修正

## 0.1.0 (2024-08-29)
* package: UPMとしてインストールできるように
* docs: READMEの作成
* ui: 変換したオブジェクトを参照するUIフィールドの変更を禁止
* ui: 変換したオブジェクトが含まれるディレクトリを開くボタンを追加
* ui: ツールチップを英語に
* ui: UniGLTFがインストールされていない場合エラーを出して続行できないように
* sanity: UniGLTFがインストールされていない状態でアバターの変換を行おうとした時にランタイム例外を発生させるように
* ui: 対象のゲームオブジェクトが変更された際、`VRChatAvatarDescriptor` の有無に応じてVRChat SDKのパイプラインを走らせるかどうかを制御するチェックボックスの制御可能状態を切り替えるように
* 初期バージョン
