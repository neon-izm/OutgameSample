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

[AssetName("SoundSettingPage")]
public class SoundSettingPageLifecycle : LifecyclePageBase
{
    private readonly SoundSettingPageView _view;
    private readonly PageEventPublisher _publisher;
    private UserSettingsUseCase _userSettingsUseCase;
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
    public SoundSettingPageLifecycle(UserSettingsUseCase userSettingsUseCase, SoundSettingPageView view, PageEventPublisher publisher, GuidCounterService guidCounterService) : base(view)
    {
        _view = view;
        _publisher = publisher;
        _guid = guidCounterService;
        _userSettingsUseCase = userSettingsUseCase;
    }

    protected override UniTask WillPushEnterAsync(CancellationToken cancellationToken)
    {
        
        _view.SetSeVolume(_userSettingsUseCase.SeVolume.Value);
        _view.SetBgmVolume(_userSettingsUseCase.BgmVolume.Value);
        _view.SetVoice(_userSettingsUseCase.VoiceVolume.Value);
        
        return UniTask.CompletedTask;
    }

    public override void DidPushEnter()
    {
        base.DidPushEnter();

        _view.OnClickReturnButton.Subscribe(_ => _publisher.SendPopEvent());
        _view.OnBgmVolumeChanged.Subscribe(x => _userSettingsUseCase.UpdateBgmVolume(x) );
        _view.OnSeVolumeChanged.Subscribe(x => _userSettingsUseCase.UpdateSeVolume(x));
        _view.OnVoiceVolumeChanged.Subscribe(x => _userSettingsUseCase.UpdateVoiceVolume(x));
        
    }
}