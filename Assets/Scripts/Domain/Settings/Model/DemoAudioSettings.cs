using System;
using UniRx;
using UnityEngine;

namespace NewDemo.Core.Scripts.Domain.Settings.Model
{
    /// <summary>
    /// 音声設定を管理するドメインモデル
    /// </summary>
    public sealed class DemoAudioSettings
    {
        // 音量設定（0.0f ~ 1.0f）
        private readonly ReactiveProperty<float> _bgmVolume = new ReactiveProperty<float>(1.0f);
        private readonly ReactiveProperty<float> _seVolume = new ReactiveProperty<float>(1.0f);
        private readonly ReactiveProperty<float> _voiceVolume = new ReactiveProperty<float>(1.0f);

        // ミュート状態
        private readonly ReactiveProperty<bool> _isBgmMuted = new ReactiveProperty<bool>(false);
        private readonly ReactiveProperty<bool> _isSeMuted = new ReactiveProperty<bool>(false);
        private readonly ReactiveProperty<bool> _isVoiceMuted = new ReactiveProperty<bool>(false);

        // 公開プロパティ（読み取り専用）
        public IReadOnlyReactiveProperty<float> BgmVolume => _bgmVolume;
        public IReadOnlyReactiveProperty<float> SeVolume => _seVolume;
        public IReadOnlyReactiveProperty<float> VoiceVolume => _voiceVolume;
        
        public IReadOnlyReactiveProperty<bool> IsBgmMuted => _isBgmMuted;
        public IReadOnlyReactiveProperty<bool> IsSeMuted => _isSeMuted;
        public IReadOnlyReactiveProperty<bool> IsVoiceMuted => _isVoiceMuted;

        // 実効音量（ミュート状態を考慮した実際の音量）
        public IObservable<float> EffectiveBgmVolume => 
            _bgmVolume.CombineLatest(_isBgmMuted, (volume, isMuted) => isMuted ? 0f : volume);
        public IObservable<float> EffectiveSeVolume => 
            _seVolume.CombineLatest(_isSeMuted, (volume, isMuted) => isMuted ? 0f : volume);
        public IObservable<float> EffectiveVoiceVolume => 
            _voiceVolume.CombineLatest(_isVoiceMuted, (volume, isMuted) => isMuted ? 0f : volume);

        // 変更通知イベント
        public IObservable<float> OnBgmVolumeChanged => _bgmVolume;
        public IObservable<float> OnSeVolumeChanged => _seVolume;
        public IObservable<float> OnVoiceVolumeChanged => _voiceVolume;
        public IObservable<bool> OnBgmMuteChanged => _isBgmMuted;
        public IObservable<bool> OnSeMuteChanged => _isSeMuted;
        public IObservable<bool> OnVoiceMuteChanged => _isVoiceMuted;

        /// <summary>
        /// BGM音量を設定（0.0f ~ 1.0f）
        /// </summary>
        public void SetBgmVolume(float volume)
        {
            _bgmVolume.Value = Mathf.Clamp01(volume);
        }

        /// <summary>
        /// SE音量を設定（0.0f ~ 1.0f）
        /// </summary>
        public void SetSeVolume(float volume)
        {
            _seVolume.Value = Mathf.Clamp01(volume);
        }

        /// <summary>
        /// Voice音量を設定（0.0f ~ 1.0f）
        /// </summary>
        public void SetVoiceVolume(float volume)
        {
            _voiceVolume.Value = Mathf.Clamp01(volume);
        }

        /// <summary>
        /// BGMミュート状態を設定
        /// </summary>
        public void SetBgmMute(bool isMuted)
        {
            _isBgmMuted.Value = isMuted;
        }

        /// <summary>
        /// SEミュート状態を設定
        /// </summary>
        public void SetSeMute(bool isMuted)
        {
            _isSeMuted.Value = isMuted;
        }

        /// <summary>
        /// Voiceミュート状態を設定
        /// </summary>
        public void SetVoiceMute(bool isMuted)
        {
            _isVoiceMuted.Value = isMuted;
        }

        /// <summary>
        /// BGMミュート状態をトグル
        /// </summary>
        public void ToggleBgmMute()
        {
            _isBgmMuted.Value = !_isBgmMuted.Value;
        }

        /// <summary>
        /// SEミュート状態をトグル
        /// </summary>
        public void ToggleSeMute()
        {
            _isSeMuted.Value = !_isSeMuted.Value;
        }

        /// <summary>
        /// Voiceミュート状態をトグル
        /// </summary>
        public void ToggleVoiceMute()
        {
            _isVoiceMuted.Value = !_isVoiceMuted.Value;
        }

        /// <summary>
        /// すべてのミュート状態をトグル
        /// </summary>
        public void ToggleAllMute()
        {
            var anyMuted = _isBgmMuted.Value || _isSeMuted.Value || _isVoiceMuted.Value;
            var newMuteState = !anyMuted;
            
            _isBgmMuted.Value = newMuteState;
            _isSeMuted.Value = newMuteState;
            _isVoiceMuted.Value = newMuteState;
        }

        /// <summary>
        /// すべての設定をデフォルトにリセット
        /// </summary>
        public void ResetToDefault()
        {
            _bgmVolume.Value = 1.0f;
            _seVolume.Value = 1.0f;
            _voiceVolume.Value = 1.0f;
            _isBgmMuted.Value = false;
            _isSeMuted.Value = false;
            _isVoiceMuted.Value = false;
        }

        /// <summary>
        /// 現在の設定をデータオブジェクトとして取得
        /// </summary>
        public NewDemoAudioSettingsData ToData()
        {
            return new NewDemoAudioSettingsData
            {
                BgmVolume = _bgmVolume.Value,
                SeVolume = _seVolume.Value,
                VoiceVolume = _voiceVolume.Value,
                IsBgmMuted = _isBgmMuted.Value,
                IsSeMuted = _isSeMuted.Value,
                IsVoiceMuted = _isVoiceMuted.Value
            };
        }

        /// <summary>
        /// データオブジェクトから設定を復元
        /// </summary>
        public void FromData(NewDemoAudioSettingsData data)
        {
            SetBgmVolume(data.BgmVolume);
            SetSeVolume(data.SeVolume);
            SetVoiceVolume(data.VoiceVolume);
            _isBgmMuted.Value = data.IsBgmMuted;
            _isSeMuted.Value = data.IsSeMuted;
            _isVoiceMuted.Value = data.IsVoiceMuted;
        }

        /// <summary>
        /// 指定された音声チャンネルの実効音量を取得
        /// </summary>
        public float GetEffectiveVolume(AudioChannel channel)
        {
            return channel switch
            {
                AudioChannel.BGM => _isBgmMuted.Value ? 0f : _bgmVolume.Value,
                AudioChannel.SE => _isSeMuted.Value ? 0f : _seVolume.Value,
                AudioChannel.Voice => _isVoiceMuted.Value ? 0f : _voiceVolume.Value,
                _ => 0f
            };
        }
    }

    /// <summary>
    /// 音声設定のデータ転送オブジェクト
    /// </summary>
    [Serializable]
    public struct NewDemoAudioSettingsData
    {
        public float BgmVolume;
        public float SeVolume;
        public float VoiceVolume;
        public bool IsBgmMuted;
        public bool IsSeMuted;
        public bool IsVoiceMuted;
    }

    /// <summary>
    /// 音声チャンネルの種類
    /// </summary>
    public enum AudioChannel
    {
        BGM,
        SE,
        Voice
    }
} 