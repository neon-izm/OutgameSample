using VContainer;
using ScreenSystem.Page;
using ScreenSystem.Modal;
using ScreenSystem.Attributes;
using Cysharp.Threading.Tasks;
using System.Threading;
using MessagePipe;
using NewDemo.Core.Scripts.UseCase.Settings;
using UniRx;
using UnityEngine;
using VContainer.Unity;


public class FooterLifecycle : IStartable
{
    private readonly FooterView _view;
   
    private readonly ModalManager _modalManager;
    private ISubscriber<MessagePipeCounterMessage> _testMessageSubscriber;
    private UserSettingsUseCase _userSettingsUseCase;
    private GuidCounterService _guid;
    
    public FooterLifecycle(FooterView view,  ModalManager modalManager,
        ISubscriber<MessagePipeCounterMessage> testMessageSubscriber, GuidCounterService guidCounterService
        , UserSettingsUseCase userSettingsUseCase
    )
    {
        Debug.Log("ここhよばら");
        _view = view;
      
        _modalManager = modalManager;
        _testMessageSubscriber = testMessageSubscriber;
        _guid = guidCounterService;
        _userSettingsUseCase = userSettingsUseCase;
    }


    public void Start()
    {
        Debug.Log("FooterLifecycle Start");
        if (_guid == null)
        {
            Debug.LogError($"{nameof(_guid)} is null");
        }
        else
        {
            Debug.Log($"{nameof(_guid)} is set to null {_guid.GetGuid}");
        }

        _userSettingsUseCase.FootScale.Subscribe(x =>
        {
            _view.SetValueFoot(x.ToString());
        });
        _userSettingsUseCase.HeadScale.Subscribe(x =>
        {
            _view.SetValueHead(x.ToString());
        });

        _view.OnClickHeadDownButton.Subscribe(x =>
        {
            var current = _userSettingsUseCase.HeadScale.Value;
            current -= 0.1f;
            _userSettingsUseCase.UpdateHeadScale(current);
            _view.SetValueHead(current.ToString());
        });
        _view.OnClickHeadUpButton.Subscribe(x =>
        {
            var current = _userSettingsUseCase.HeadScale.Value;
            current += 0.1f;
            _userSettingsUseCase.UpdateHeadScale(current);
            _view.SetValueHead(current.ToString());
        });

        _view.OnClickFootUpButton.Subscribe(x =>
        {
            var current = _userSettingsUseCase.FootScale.Value;
            current += 0.1f;
            _userSettingsUseCase.UpdateFootScale(current);
            _view.SetValueFoot(current.ToString());
        });


        _view.OnClickFootDownButton.Subscribe(x =>
        {
            var current = _userSettingsUseCase.FootScale.Value;
            current -= 0.1f;
            _userSettingsUseCase.UpdateFootScale(current);
            _view.SetValueFoot(current.ToString());
        });
    }
}