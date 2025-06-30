using UnityEngine;
using VContainer;
using VContainer.Unity;

public class CommentLifetimeScope : LifetimeScope
{
    [SerializeField] private CommentView _view;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<CommentLifecycle>().AsSelf();
        builder.RegisterComponent(_view);
    }
}
