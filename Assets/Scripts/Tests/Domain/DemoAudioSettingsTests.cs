using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UniRx;
using NewDemo.Core.Scripts.Domain.Settings.Model;

namespace NewDemo.Tests.Domain
{
    /// <summary>
    /// NewDemoAudioSettingsクラスのユニットテスト
    /// </summary>
    public class DemoAudioSettingsTests
    {
        private DemoAudioSettings _audioSettings;

        [SetUp]
        public void SetUp()
        {
            _audioSettings = new DemoAudioSettings();
        }

        [TearDown]
        public void TearDown()
        {
            _audioSettings = null;
        }

        #region 音量設定のテスト

        [Test]
        [TestCase(0.0f)]   // 最小値
        [TestCase(0.5f)]   // 中間値
        [TestCase(1.0f)]   // 最大値
        public void SetBgmVolume_有効な値_正しく設定される(float volume)
        {
            // Act
            _audioSettings.SetBgmVolume(volume);

            // Assert
            Assert.AreEqual(volume, _audioSettings.BgmVolume.Value, 0.001f);
        }

        [Test]
        [TestCase(-0.1f, 0.0f)]  // 下限を下回る値 -> 0にクランプ
        [TestCase(1.1f, 1.0f)]   // 上限を上回る値 -> 1にクランプ
        [TestCase(2.0f, 1.0f)]   // 大きく上回る値 -> 1にクランプ
        public void SetBgmVolume_範囲外の値_クランプされる(float input, float expected)
        {
            // Act
            _audioSettings.SetBgmVolume(input);

            // Assert
            Assert.AreEqual(expected, _audioSettings.BgmVolume.Value, 0.001f);
        }

        [Test]
        public void SetSeVolume_有効な値_正しく設定される()
        {
            // Arrange
            const float testVolume = 0.7f;

            // Act
            _audioSettings.SetSeVolume(testVolume);

            // Assert
            Assert.AreEqual(testVolume, _audioSettings.SeVolume.Value, 0.001f);
        }

        [Test]
        public void SetVoiceVolume_有効な値_正しく設定される()
        {
            // Arrange
            const float testVolume = 0.3f;

            // Act
            _audioSettings.SetVoiceVolume(testVolume);

            // Assert
            Assert.AreEqual(testVolume, _audioSettings.VoiceVolume.Value, 0.001f);
        }

        #endregion

        #region ミュート機能のテスト

        [Test]
        public void SetBgmMute_True_ミュート状態になる()
        {
            // Act
            _audioSettings.SetBgmMute(true);

            // Assert
            Assert.IsTrue(_audioSettings.IsBgmMuted.Value);
        }

        [Test]
        public void SetBgmMute_False_ミュート解除される()
        {
            // Arrange
            _audioSettings.SetBgmMute(true);

            // Act
            _audioSettings.SetBgmMute(false);

            // Assert
            Assert.IsFalse(_audioSettings.IsBgmMuted.Value);
        }

        [Test]
        public void ToggleBgmMute_初期状態から_ミュートされる()
        {
            // Act
            _audioSettings.ToggleBgmMute();

            // Assert
            Assert.IsTrue(_audioSettings.IsBgmMuted.Value);
        }

        [Test]
        public void ToggleBgmMute_ミュート状態から_ミュート解除される()
        {
            // Arrange
            _audioSettings.SetBgmMute(true);

            // Act
            _audioSettings.ToggleBgmMute();

            // Assert
            Assert.IsFalse(_audioSettings.IsBgmMuted.Value);
        }

        [Test]
        public void ToggleSeMute_動作確認()
        {
            // Arrange
            var initialState = _audioSettings.IsSeMuted.Value;

            // Act
            _audioSettings.ToggleSeMute();

            // Assert
            Assert.AreEqual(!initialState, _audioSettings.IsSeMuted.Value);
        }

        [Test]
        public void ToggleVoiceMute_動作確認()
        {
            // Arrange
            var initialState = _audioSettings.IsVoiceMuted.Value;

            // Act
            _audioSettings.ToggleVoiceMute();

            // Assert
            Assert.AreEqual(!initialState, _audioSettings.IsVoiceMuted.Value);
        }

        [Test]
        public void ToggleAllMute_すべて非ミュート状態から_すべてミュートされる()
        {
            // Arrange
            _audioSettings.SetBgmMute(false);
            _audioSettings.SetSeMute(false);
            _audioSettings.SetVoiceMute(false);

            // Act
            _audioSettings.ToggleAllMute();

            // Assert
            Assert.IsTrue(_audioSettings.IsBgmMuted.Value);
            Assert.IsTrue(_audioSettings.IsSeMuted.Value);
            Assert.IsTrue(_audioSettings.IsVoiceMuted.Value);
        }

        [Test]
        public void ToggleAllMute_一部ミュート状態から_すべて非ミュートされる()
        {
            // Arrange
            _audioSettings.SetBgmMute(true);
            _audioSettings.SetSeMute(false);
            _audioSettings.SetVoiceMute(false);

            // Act
            _audioSettings.ToggleAllMute();

            // Assert
            Assert.IsFalse(_audioSettings.IsBgmMuted.Value);
            Assert.IsFalse(_audioSettings.IsSeMuted.Value);
            Assert.IsFalse(_audioSettings.IsVoiceMuted.Value);
        }

        #endregion

        #region 実効音量のテスト

        [Test]
        public void GetEffectiveVolume_BGM_非ミュート時_設定音量が返される()
        {
            // Arrange
            const float testVolume = 0.8f;
            _audioSettings.SetBgmVolume(testVolume);
            _audioSettings.SetBgmMute(false);

            // Act
            var effectiveVolume = _audioSettings.GetEffectiveVolume(AudioChannel.BGM);

            // Assert
            Assert.AreEqual(testVolume, effectiveVolume, 0.001f);
        }

        [Test]
        public void GetEffectiveVolume_BGM_ミュート時_0が返される()
        {
            // Arrange
            const float testVolume = 0.8f;
            _audioSettings.SetBgmVolume(testVolume);
            _audioSettings.SetBgmMute(true);

            // Act
            var effectiveVolume = _audioSettings.GetEffectiveVolume(AudioChannel.BGM);

            // Assert
            Assert.AreEqual(0f, effectiveVolume, 0.001f);
        }

        [Test]
        public void GetEffectiveVolume_SE_動作確認()
        {
            // Arrange
            const float testVolume = 0.6f;
            _audioSettings.SetSeVolume(testVolume);
            _audioSettings.SetSeMute(false);

            // Act
            var effectiveVolume = _audioSettings.GetEffectiveVolume(AudioChannel.SE);

            // Assert
            Assert.AreEqual(testVolume, effectiveVolume, 0.001f);
        }

        [Test]
        public void GetEffectiveVolume_Voice_動作確認()
        {
            // Arrange
            const float testVolume = 0.4f;
            _audioSettings.SetVoiceVolume(testVolume);
            _audioSettings.SetVoiceMute(false);

            // Act
            var effectiveVolume = _audioSettings.GetEffectiveVolume(AudioChannel.Voice);

            // Assert
            Assert.AreEqual(testVolume, effectiveVolume, 0.001f);
        }

        #endregion

        #region データ変換のテスト

        [Test]
        public void ToData_設定済みデータ_正しくデータオブジェクトに変換される()
        {
            // Arrange
            _audioSettings.SetBgmVolume(0.8f);
            _audioSettings.SetSeVolume(0.6f);
            _audioSettings.SetVoiceVolume(0.4f);
            _audioSettings.SetBgmMute(true);
            _audioSettings.SetSeMute(false);
            _audioSettings.SetVoiceMute(true);

            // Act
            var data = _audioSettings.ToData();

            // Assert
            Assert.AreEqual(0.8f, data.BgmVolume, 0.001f);
            Assert.AreEqual(0.6f, data.SeVolume, 0.001f);
            Assert.AreEqual(0.4f, data.VoiceVolume, 0.001f);
            Assert.IsTrue(data.IsBgmMuted);
            Assert.IsFalse(data.IsSeMuted);
            Assert.IsTrue(data.IsVoiceMuted);
        }

        [Test]
        public void FromData_有効なデータ_正しく設定される()
        {
            // Arrange
            var data = new NewDemoAudioSettingsData
            {
                BgmVolume = 0.9f,
                SeVolume = 0.7f,
                VoiceVolume = 0.5f,
                IsBgmMuted = false,
                IsSeMuted = true,
                IsVoiceMuted = false
            };

            // Act
            _audioSettings.FromData(data);

            // Assert
            Assert.AreEqual(0.9f, _audioSettings.BgmVolume.Value, 0.001f);
            Assert.AreEqual(0.7f, _audioSettings.SeVolume.Value, 0.001f);
            Assert.AreEqual(0.5f, _audioSettings.VoiceVolume.Value, 0.001f);
            Assert.IsFalse(_audioSettings.IsBgmMuted.Value);
            Assert.IsTrue(_audioSettings.IsSeMuted.Value);
            Assert.IsFalse(_audioSettings.IsVoiceMuted.Value);
        }

        #endregion

        #region リセット機能のテスト

        [Test]
        public void ResetToDefault_カスタマイズ済み_デフォルト値に戻る()
        {
            // Arrange
            _audioSettings.SetBgmVolume(0.5f);
            _audioSettings.SetSeVolume(0.3f);
            _audioSettings.SetVoiceVolume(0.7f);
            _audioSettings.SetBgmMute(true);
            _audioSettings.SetSeMute(true);
            _audioSettings.SetVoiceMute(true);

            // Act
            _audioSettings.ResetToDefault();

            // Assert
            Assert.AreEqual(1.0f, _audioSettings.BgmVolume.Value, 0.001f);
            Assert.AreEqual(1.0f, _audioSettings.SeVolume.Value, 0.001f);
            Assert.AreEqual(1.0f, _audioSettings.VoiceVolume.Value, 0.001f);
            Assert.IsFalse(_audioSettings.IsBgmMuted.Value);
            Assert.IsFalse(_audioSettings.IsSeMuted.Value);
            Assert.IsFalse(_audioSettings.IsVoiceMuted.Value);
        }

        #endregion

        #region ReactivePropertyの動作テスト

        [UnityTest]
        public IEnumerator EffectiveBgmVolume_音量とミュート状態変更_正しく計算される()
        {
            // Arrange
            var receivedVolumes = new List<float>();
            _audioSettings.EffectiveBgmVolume.Subscribe(volume => receivedVolumes.Add(volume));

            // Act 1: 音量変更
            _audioSettings.SetBgmVolume(0.8f);
            yield return null;

            // Act 2: ミュート
            _audioSettings.SetBgmMute(true);
            yield return null;

            // Act 3: ミュート解除
            _audioSettings.SetBgmMute(false);
            yield return null;

            // Assert
            Assert.That(receivedVolumes.Count, Is.GreaterThanOrEqualTo(3));
            Assert.AreEqual(1.0f, receivedVolumes[0], 0.001f); // 初期値
            Assert.AreEqual(0.8f, receivedVolumes[1], 0.001f); // 音量変更後
            Assert.AreEqual(0.0f, receivedVolumes[2], 0.001f); // ミュート時
            Assert.AreEqual(0.8f, receivedVolumes[3], 0.001f); // ミュート解除後
        }

        [UnityTest]
        public IEnumerator OnBgmVolumeChanged_音量変更_イベント発火される()
        {
            // Arrange
            var eventFired = false;
            var receivedVolume = 0f;
            _audioSettings.OnBgmVolumeChanged.Subscribe(volume =>
            {
                eventFired = true;
                receivedVolume = volume;
            });

            // Act
            _audioSettings.SetBgmVolume(0.5f);
            yield return null;

            // Assert
            Assert.IsTrue(eventFired);
            Assert.AreEqual(0.5f, receivedVolume, 0.001f);
        }

        [UnityTest]
        public IEnumerator OnBgmMuteChanged_ミュート状態変更_イベント発火される()
        {
            // Arrange
            var eventFired = false;
            var receivedMuteState = false;
            _audioSettings.OnBgmMuteChanged.Subscribe(isMuted =>
            {
                eventFired = true;
                receivedMuteState = isMuted;
            });

            // Act
            _audioSettings.SetBgmMute(true);
            yield return null;

            // Assert
            Assert.IsTrue(eventFired);
            Assert.IsTrue(receivedMuteState);
        }

        #endregion
    }
} 