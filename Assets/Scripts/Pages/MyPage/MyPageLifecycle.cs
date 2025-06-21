using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NewDemo.Core.Scripts.UseCase.Settings;
using ScreenSystem.Attributes;
using ScreenSystem.Page;
using UniRx;
using UnityEngine;
using VContainer;

[AssetName("Pages/MyPage")]
public class MyPageLifecycle : LifecyclePageBase
{
    private readonly MyPageView _view;
    private readonly PageEventPublisher _publisher;
    private GuidCounterService _guid;
    
    public class NetworkParameter
    {
        public readonly string Message;

        public NetworkParameter(string message)
        {
            Message = message;
        }
    }

    [Inject]
    public MyPageLifecycle(UserSettingsUseCase userSettingsUseCase, MyPageView view, PageEventPublisher publisher, GuidCounterService guidCounterService) : base(view)
    {
        _view = view;
        _publisher = publisher;
        _guid = guidCounterService;
    }

    protected override UniTask WillPushEnterAsync(CancellationToken cancellationToken)
    {
        // TODO: Initialize page data here
        
        return UniTask.CompletedTask;
    }

    public override void DidPushEnter()
    {
        base.DidPushEnter();

        _view.OnClickReturnButton.Subscribe(_ => _publisher.SendPopEvent());
        _view.SetUserName(_guid.GetGuidString()); 
        // TODO: Subscribe to UI events here
        // Example: _view.OnClickReturnButton.Subscribe(_ => _publisher.SendPopEvent());
        
    }
} 