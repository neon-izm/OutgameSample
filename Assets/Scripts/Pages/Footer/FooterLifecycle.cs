using VContainer;
using ScreenSystem.Page;
using ScreenSystem.Modal;
using ScreenSystem.Attributes;
using Cysharp.Threading.Tasks;
using System.Threading;
using Demo.Subsystem;
using MessagePipe;
using NewDemo.Core.Scripts.UseCase.Settings;
using UniRx;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Page;
using VContainer.Unity;


public class FooterLifecycle : IStartable
{
    private readonly FooterView _view;
    private PageContainer _pageContainer;
    private readonly ModalManager _modalManager;
    private ISubscriber<MessagePipeCounterMessage> _testMessageSubscriber;
    private UserSettingsUseCase _userSettingsUseCase;
    private GuidCounterService _guid;
    private PageEventPublisher _publisher;
    private PageRoutingService _pageRoutingService;
    public FooterLifecycle(FooterView view,  ModalManager modalManager,
        ISubscriber<MessagePipeCounterMessage> testMessageSubscriber, GuidCounterService guidCounterService
        , UserSettingsUseCase userSettingsUseCase
        , PageEventPublisher publisher
        , PageRoutingService pageRoutingService
    )
    {
        Debug.Log("ここhよばら");
        _view = view;
      
        _modalManager = modalManager;
        _testMessageSubscriber = testMessageSubscriber;
        _guid = guidCounterService;
        _userSettingsUseCase = userSettingsUseCase;
        _pageRoutingService = pageRoutingService;
        if(publisher != null    )
        {
            _publisher = publisher;
        }
        else
        {
           Debug.LogError("PageEventPublisher is null");
        }

     
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
            Debug.Log($"{nameof(_guid)} is set to null {_guid.GuidInt}");
        }
        _view.OnClickMyPage.Subscribe(x =>
        {
            _pageRoutingService.PushOrPopPage(new MyPageBuilder());
        });
        _view.OnClickFirstPage.Subscribe(x =>
        {
            _pageRoutingService.PushOrPopPage(new FirstPageBuilder());
        });

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

        _view.OnClickDebugShowOageIds.Subscribe(x =>
        {
            Debug.Log("PageNames: " + string.Join(", ", _pageRoutingService.GetPageNames()));
            Toast.QueueToast(string.Join(", ", _pageRoutingService.GetPageNames()));
        });
    }

}