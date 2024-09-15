# 変更履歴
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
