using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class SoundSettingPageLifetimeScope : LifetimeScope
{
    [SerializeField] private SoundSettingPageView _view;
    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        //builder.Register<GuidCounter>(Lifetime.Singleton);
        builder.Register<SoundSettingPageLifecycle>(Lifetime.Singleton);
        builder.RegisterComponent(_view);
       
    }
}
