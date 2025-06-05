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
            _userSettingsUseCase.DecreaseHeadScale();
        });
        _view.OnClickHeadUpButton.Subscribe(x =>
        {
            _userSettingsUseCase.IncreaseHeadScale();
        });

        _view.OnClickFootUpButton.Subscribe(x =>
        {
            _userSettingsUseCase.IncreaseFootScale();
        });


        _view.OnClickFootDownButton.Subscribe(x =>
        {
            _userSettingsUseCase.DecreaseFootScale();
        });
    }
}