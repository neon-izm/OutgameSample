using System;
using System.Collections;
using System.Collections.Generic;
using NewDemo.Core.Scripts.Domain.Settings.Model;
using ScreenSystem.Page;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettingPageView : PageViewBase
{
    
    [SerializeField] private Button _returnButton;
    [SerializeField] private Slider _seVolumeSlider;
    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Slider _voiceVolumeSlider;

    public IObservable<Unit> OnClickReturnButton { get; private set; }
    public IObservable<float> OnSeVolumeChanged { get; private set; }
    public IObservable<float> OnBgmVolumeChanged { get; private set; }
    public IObservable<float> OnVoiceVolumeChanged { get; private set; }

    public void SetSeVolume(float seVolume)
    {
        _seVolumeSlider.value = seVolume;
    }

    public void SetBgmVolume(float volume)
    {
        _bgmVolumeSlider.value = volume;
    }

    public void SetVoice(float volume)
    {
        _voiceVolumeSlider.value = volume;
    }

    private void Start()
    {
        OnClickReturnButton = _returnButton.OnClickAsObservable();
        OnSeVolumeChanged = _seVolumeSlider.OnValueChangedAsObservable();
        OnBgmVolumeChanged = _bgmVolumeSlider.OnValueChangedAsObservable();
        OnVoiceVolumeChanged = _voiceVolumeSlider.OnValueChangedAsObservable();
    }
}
