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

    public IObservable<Unit> OnClickReturnButton => _returnButton.OnClickAsObservable();
    public IObservable<float> OnSeVolumeChanged => _seVolumeSlider.OnValueChangedAsObservable();
    public IObservable<float> OnBgmVolumeChanged => _bgmVolumeSlider.OnValueChangedAsObservable();
    public IObservable<float> OnVoiceVolumeChanged => _voiceVolumeSlider.OnValueChangedAsObservable();
    
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
