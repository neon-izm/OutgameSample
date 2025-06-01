using System.Collections;
using System.Collections.Generic;
using MessagePipe;
using ScreenSystem.Modal;
using ScreenSystem.Page;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

/// <summary>
/// 本当はエッジからのスワイプで出てくるViewを扱うためのもの
/// 一旦仮で素朴なボタン起動にしている
/// </summary>
public class EdgeSwiper : MonoBehaviour
{
    private PageEventPublisher _publisher;

    private ModalManager _modalManager;
    [SerializeField] Button _OpenPageButton;
    [Inject]
    public void Construct(PageEventPublisher publisher,ModalManager modalManager)
    {
        this._publisher = publisher;
        this._modalManager = modalManager;
        if (_modalManager == null)
        {
            Debug.LogError($"{nameof(ModalManager)} is null");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _OpenPageButton.onClick.AsObservable().Subscribe(_ =>
        {
            _publisher.SendPushEvent(new CharacterColorChangePageBuilder());
        }).AddTo(this);
    }

}
