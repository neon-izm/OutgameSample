using ScreenSystem.Page;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class NextPageView : PageViewBase
{
    [SerializeField] private Text _guidText;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private Button _returnButton;

    [SerializeField] private Button _soundSettingButton;
    public IObservable<Unit> OnClickSoundSettingButton { get; private set; }
    public IObservable<Unit> OnClickReturn { get; private set; }

    private void Start()
    {
        OnClickSoundSettingButton = _soundSettingButton.OnClickAsObservable();
        OnClickReturn = _returnButton.OnClickAsObservable();
    }

    public void SetGuid(int guid)
    {
        _guidText.text = guid.ToString();
    }
    
    
    public void SetView(NextPageModel model)
    {
        _messageText.SetText(model.NextMessage);
    }
}