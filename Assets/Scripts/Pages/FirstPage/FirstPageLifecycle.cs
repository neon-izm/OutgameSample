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

[AssetName("FirstPage")]
public class FirstPageLifecycle : LifecyclePageBase
{
    private readonly FirstPageView _view;
    private readonly PageEventPublisher _publisher;
    private readonly ModalManager _modalManager;
    private readonly NextPageUseCase _nextPageUseCase;
    private ISubscriber<MessagePipeCounterMessage> _testMessageSubscriber;
    private UserSettingsUseCase _userSettingsUseCase;
    private GuidCounterService _guid;
    [Inject]
    public FirstPageLifecycle(FirstPageView view, PageEventPublisher publisher, ModalManager modalManager, NextPageUseCase nextPageUseCase,
        ISubscriber<MessagePipeCounterMessage> testMessageSubscriber,GuidCounterService guidCounterService
        ,    UserSettingsUseCase userSettingsUseCase
        ) : base(view) 
    {
        _view = view;
        _publisher = publisher;
        _modalManager = modalManager;
        _nextPageUseCase = nextPageUseCase;
        _testMessageSubscriber = testMessageSubscriber;
        _guid = guidCounterService;
        _userSettingsUseCase = userSettingsUseCase;
        _userSettingsUseCase.InitializeAsync().Forget();
    }

    protected override UniTask WillPushEnterAsync(CancellationToken cancellationToken)
    {
        var testModel = new FirstPageModel();
        _view.SetView(testModel);
        _view.SetGuidInt(_guid.GuidInt);
       
        return UniTask.CompletedTask;
    }

    public override void DidPopEnter()
    {
        base.DidPopEnter();
        // 戻るで表示された時にも値は更新されて欲しい
        var jsonText = JsonUtility.ToJson(_userSettingsUseCase.GetSummary(), true);

        Debug.Log(jsonText);
        _view.SetSettingsValue(jsonText);
    }

    public override void DidPushEnter()
    {
        base.DidPushEnter();
        var jsonText = JsonUtility.ToJson(_userSettingsUseCase.GetSummary(), true);

        Debug.Log(jsonText);
        _view.SetSettingsValue(jsonText);

        _view.OnClickMyPage.Subscribe(_ =>
        {
            _publisher.SendPushEvent(new MyPageBuilder());
        });

        _view.OnClickPage.Subscribe(_ => UniTask.Void(async () =>
        {
            // 通信を行い、通信結果を渡して次の画面を開く
            var parameter = await _nextPageUseCase.DoConnect(cancellationToken: ExitCancellationToken);
            _publisher.SendPushEvent(new NextPageBuilder(parameter));
        }));
        _view.OnClickColorPage.Subscribe(_ =>
        {
            _publisher.SendPushEvent(new CharacterColorChangePageBuilder(true,true));

        });
        _view.OnClickModal.Subscribe(_ =>
        {
            // 通信を行わずにパラメータだけを渡して次の画面を開く
            var countParameter = new TestModalLifecycle.CountParameter(1);
            _modalManager.Push(new TestModalBuilder(countParameter), cancellationToken: ExitCancellationToken).Forget();
        });
        
        
        _testMessageSubscriber.Subscribe(m =>
        {
            _view.UpdateModalCount(m.Count);
        }).AddTo(DisposeCancellationToken);
    }
}