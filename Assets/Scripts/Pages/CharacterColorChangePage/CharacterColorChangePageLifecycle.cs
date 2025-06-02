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

[AssetName("CharacterColorChangePage")]
public class CharacterColorChangePageLifecycle : LifecyclePageBase
{
    private readonly CharacterColorChangePageView _view;
    private readonly PageEventPublisher _publisher;
    private UserSettingsUseCase _userSettingsUseCase;
    private GuidCounterService _guid;

    [Inject]
    public CharacterColorChangePageLifecycle(UserSettingsUseCase userSettingsUseCase, CharacterColorChangePageView view, PageEventPublisher publisher, GuidCounterService guidCounterService) : base(view)
    {
        _view = view;
        _publisher = publisher;
        _guid = guidCounterService;
        _userSettingsUseCase = userSettingsUseCase;
       
        _userSettingsUseCase.RightHandColor.Subscribe(x =>
        {
            _view.SetRightHandColor(x);
        });
        _userSettingsUseCase.LeftHandColor.Subscribe(x =>
        {
            _view.SetLeftHandColor(x);
        });
        _userSettingsUseCase.RightFootColor.Subscribe(x=>_view.SetRightFootColor(x));
        _userSettingsUseCase.LeftFootColor.Subscribe(x=>_view.SetLeftFootColor(x));
        
    }

    protected override UniTask WillPushEnterAsync(CancellationToken cancellationToken)
    {
        Debug.Log($"WillPushEnterAsync called RightHandColor:{_userSettingsUseCase.RightHandColor.Value}");
        
        // 現在の設定値を画面に反映
        _view.SetRightHandColor(_userSettingsUseCase.RightHandColor.Value);
        _view.SetLeftHandColor(_userSettingsUseCase.LeftHandColor.Value);
        _view.SetRightFootColor(_userSettingsUseCase.RightFootColor.Value);
        _view.SetLeftFootColor(_userSettingsUseCase.LeftFootColor.Value);
        
        return UniTask.CompletedTask;
    }

    public override void DidPushEnter()
    {
        base.DidPushEnter();

        // 戻るボタン
        _view.OnClickReturnButton.Subscribe(_ => _publisher.SendPopEvent());
       
        // 各色変更イベントをUserSettingsUseCaseに連携
        _view.OnRightHandColorChanged.Subscribe(color => _userSettingsUseCase.UpdateRightHandColor(color));
        _view.OnLeftHandColorChanged.Subscribe(color => _userSettingsUseCase.UpdateLeftHandColor(color));
        _view.OnRightFootColorChanged.Subscribe(color => _userSettingsUseCase.UpdateRightFootColor(color));
        _view.OnLeftFootColorChanged.Subscribe(color => _userSettingsUseCase.UpdateLeftFootColor(color));
       
        /*
        // リセットボタン（オプション）
        _view.OnClickResetButton.Subscribe(_ =>
        {
            _userSettingsUseCase.UpdateRightHandColor(Color.white);
            _userSettingsUseCase.UpdateLeftHandColor(Color.white);
            _userSettingsUseCase.UpdateRightFootColor(Color.white);
            _userSettingsUseCase.UpdateLeftFootColor(Color.white);

            // UI側も更新
            _view.SetRightHandColor(Color.white);
            _view.SetLeftHandColor(Color.white);
            _view.SetRightFootColor(Color.white);
            _view.SetLeftFootColor(Color.white);

        });
        */

    }
} 