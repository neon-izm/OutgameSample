using System;
using UnityEngine;
using ScreenSystem.Page;
using UnityEngine.UI;
using UniRx;
using TMPro;
using UnityEngine.Serialization;

public class NextPageView : PageViewBase
{
    [SerializeField] private Text _guidText;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private Button _returnButton;

    [SerializeField] private Button _soundSettingButton;
    public IObservable<Unit> OnClickSoundSettingButton => _soundSettingButton.OnClickAsObservable();
    public IObservable<Unit> OnClickReturn => _returnButton.OnClickAsObservable();

    public void SetGuid(int guid)
    {
        _guidText.text = guid.ToString();
    }
    
    
    public void SetView(NextPageModel model)
    {
        _messageText.SetText(model.NextMessage);
    }
}