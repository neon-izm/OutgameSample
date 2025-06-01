using UnityEngine;
using VContainer;
using VContainer.Unity;

public class CharacterColorChangePageLifetimeScope : LifetimeScope
{
    [SerializeField] private CharacterColorChangePageView _characterColorChangePageView;
    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        builder.Register<CharacterColorChangePageLifecycle>(Lifetime.Singleton);
        builder.RegisterInstance(_characterColorChangePageView);
    }
}
