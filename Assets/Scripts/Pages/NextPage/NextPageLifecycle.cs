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
        if(_guid!= null)
        {
            Debug.Log($"NextPageLifecycle Guid: {_guid.GuidInt}");
        }
        else
        {
            Debug.LogError($"{nameof(_guid)} is null");
        }
    }

    protected override UniTask WillPushEnterAsync(CancellationToken cancellationToken)
    {
        var nextModel = new NextPageModel(_parameter);
        _view.SetView(nextModel);
        _view.SetGuid(_guid.GuidInt);
        
        
        return UniTask.CompletedTask;
    }

    public override void DidPushEnter()
    {
        base.DidPushEnter();

        _view.OnClickAddCommentButton.Subscribe(_=>
        {
            var comment = _view.AddComment(_guid.GuidInt.ToString());
            if (comment != null)
            {
                Debug.Log("Comment created successfully.");
                // You can add additional logic here if needed, e.g., sending the comment to a server or saving it locally.
            }
            else
            {
                Debug.LogWarning("Comment creation failed.");
            }
        });
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