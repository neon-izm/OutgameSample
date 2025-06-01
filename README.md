# OutgameSample
アウトゲーム設計のサンプルプロジェクトです。

## 動作イメージ

https://github.com/mr-imada/OutgameSample/assets/11347090/e15bc980-fda6-457f-a7b0-93d2f4d4a5bb

以下が行なえます。
- Test PageからNext PageへのPage切り替え
- Test PageでTest Modalの表示
- Test Modalの複数表示
- Test Modal内でApplyを押すことでModalの番号をTest Pageに通知

具体的な実装やコードはAssets/Scriptsを参考にしてください。

## アーキテクチャメモ
- PageBuilderBaseを継承しているのは名前解決+ScreenSystemのSendPushEventでスクリーン表示するため
- LifecyclePageBaseを継承しているものは、他のProjectではPresenterと呼ばれるものに相当します
  - Constructor Injectionで viewやUseCaseをInjectします
- ScreenSystemはUnityScreenNavigatorとVContainerを前提としたボイラープレートコード削減に寄与します
### もっと直接的なPageの作り方
- HogePageBuilderクラスをPageBuilderBase継承で作る(Pure C#)
- HogePageLifecycleクラス（Presenter）をLifecyclePageBaseを継承で作る(Pure C#)
- HogePageLifetimeScope : VContainer.Unity.LifetimeScopeを作ってRegisterComponentをする
  - ここでPresenter(Lifecycle)をViewと結びつけている
- HogePageViewクラスを作ってprefabにアタッチする

## Unity version
Unity2022.3.12f1

