using VContainer;
using ScreenSystem.Page;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using ScreenSystem.Attributes;
using UnityEngine;

[AssetName("NextPage")]
public class NextPageLifecycle : LifecyclePageBase
{
    private readonly NextPageView _view;
    private readonly PageEventPublisher _publisher;
    private readonly NetworkParameter _parameter;

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
    public NextPageLifecycle(NextPageView view, PageEventPublisher publisher, NetworkParameter parameter,GuidCounterService guidCounterService) : base(view)
    {
        _view = view;
        _publisher = publisher;
        _parameter = parameter;
        _guid = guidCounterService;
    }

    protected override UniTask WillPushEnterAsync(CancellationToken cancellationToken)
    {
        var NextModel = new NextPageModel(_parameter);
        _view.SetView(NextModel);
        _view.SetGuid(_guid.GuidInt);
        return UniTask.CompletedTask;
    }

    public override void DidPushEnter()
    {
        base.DidPushEnter();

        _view.OnClickSoundSettingButton.Subscribe(_ =>
        {
            _publisher.SendPushEvent(new SoundSettingPageBuilder());
            Debug.Log("OnClickNextButton:"+_guid.GuidInt);
        });
        _view.OnClickReturn.Subscribe(_ =>
        {
            _publisher.SendPopEvent();
        });
    }
}