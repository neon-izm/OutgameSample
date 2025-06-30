
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class FooterLifetimeScope : LifetimeScope
{
    [SerializeField] private FooterView _view;

    protected override void Configure(IContainerBuilder builder)
    {
        Debug.Log("FooterLifetimeScope Configure");
        //builder.Register<GuidCounter>(Lifetime.Singleton);
        builder.RegisterEntryPoint<FooterLifecycle>(Lifetime.Singleton).AsSelf();

        builder.RegisterComponent(_view);
        
    }
}