using VContainer;
using VContainer.Unity;
using UnityEngine;

public class MyPageLifetimeScope : LifetimeScope
{
    [SerializeField] private MyPageView _view;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<MyPageLifecycle>(Lifetime.Singleton);
        builder.RegisterComponent(_view);
    }
}
