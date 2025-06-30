using System;
using System.Collections;
using System.Collections.Generic;
using NewDemo.Core.Scripts.Domain.Settings.Model;
using ScreenSystem.Page;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MyPageView : PageViewBase
{
    [SerializeField] private Text _userNameText;
    [SerializeField] private Button _returnButton;

    [SerializeField] private Button _soundSettingsButton;

    // TODO: Add other UI components here
    // Example: [SerializeField] private Button _settingsButton;

    public void SetUserName(string userName)
    {
        _userNameText.text = userName;
    }

    public IObservable<Unit> OnClickSoundSettingsButton { get; private set; }
    public IObservable<Unit> OnClickReturnButton { get; private set; }
    // TODO: Add other observable properties here
    // Example: public IObservable<Unit> OnClickSettingsButton { get; private set; }

    // TODO: Add public methods for updating UI here
    // Example:
    // public void SetUserName(string userName)
    // {
    //     _userNameText.text = userName;
    // }

    private void Awake()
    {
        OnClickReturnButton = _returnButton.OnClickAsObservable();
        
        // TODO: Initialize other observables here
        OnClickSoundSettingsButton = _soundSettingsButton.OnClickAsObservable();
        // Example: OnClickSettingsButton = _settingsButton.OnClickAsObservable();
    }
} 