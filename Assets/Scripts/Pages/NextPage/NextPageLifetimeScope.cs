using ScreenSystem.VContainerExtension;
using UnityEngine;
using VContainer;
using VContainer.Unity;
/// <summary>
/// ビューごとのPresenterとViewの紐付け
/// </summary>
public class NextPageLifetimeScope : LifetimeScopeWithParameter<NextPageLifecycle.NetworkParameter>
{
    [SerializeField] private NextPageView _view;

    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        // 複数のPageから参照するUseCaseやServiceはここでRegisterしてはいけない
        //builder.Register<GuidCounter>(Lifetime.Singleton);
        builder.Register<NextPageLifecycle>(Lifetime.Singleton);
        builder.RegisterComponent(_view);
    }
}