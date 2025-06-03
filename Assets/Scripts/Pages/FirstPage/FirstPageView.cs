using System;
using UnityEngine;
using ScreenSystem.Page;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class FirstPageView : PageViewBase
{
    [SerializeField] private Text _guidText;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private TextMeshProUGUI _modalCountText;
    [SerializeField] private Button _nextPageButton;
    [SerializeField] private Button _nextModalButton;
    [SerializeField] private Button _colorPageButton;

    [SerializeField] private Text _settingsValueText;
    public IObservable<Unit> OnClickPage { get; private set; }
    public IObservable<Unit> OnClickModal { get; private set; }

    public IObservable<Unit> OnClickColorPage { get; private set; }

    private void Start()
    {
        OnClickPage = _nextPageButton.OnClickAsObservable();
        OnClickModal = _nextModalButton.OnClickAsObservable();
        OnClickColorPage = _colorPageButton.OnClickAsObservable();
    }

    public void SetView(FirstPageModel model)
    {
        _messageText.SetText(model.FirstPageMessage);
        UpdateModalCount(0);
    }

    public void SetGuidInt(int guid)
    {
        _guidText.text = guid.ToString();
    }

    public void SetSettingsValue(string value)
    {
        _settingsValueText.text = value;
    }
    public void UpdateModalCount(int count)
    {
        _modalCountText.SetText($"Modal Count: {count}");
    }
}