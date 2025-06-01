using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using NewDemo.Core.Scripts.Domain.Character.Model;
using NewDemo.Core.Scripts.Domain.Settings.Model;
using NewDemo.Core.Scripts.Infrastructure.Settings;

namespace NewDemo.Core.Scripts.UseCase.Settings
{
    /// <summary>
    /// ユーザー設定の統合管理と永続化を担当するUseCase
    /// </summary>
    public sealed class UserSettingsUseCase : IDisposable
    {
        private readonly Character _character;
        private readonly NewDemoAudioSettings _audioSettings;
        private readonly UserSettingsRepository _repository;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        // 自動保存のタイマー
        private readonly ReactiveProperty<bool> _isDirty = new ReactiveProperty<bool>(false);
        private IDisposable _autoSaveSubscription;

        public UserSettingsUseCase(
            Character character, 
            NewDemoAudioSettings audioSettings, 
            UserSettingsRepository repository)
        {
            _character = character;
            _audioSettings = audioSettings;
            _repository = repository;

            SetupAutoSave();
            SetupChangeDetection();
        }

        // ===== ReactiveProperty公開（UI表示用） =====

        #region キャラクター設定の購読

        /// <summary>
        /// 右手の色（読み取り専用）
        /// </summary>
        public IReadOnlyReactiveProperty<Color> RightHandColor => _character.RightHandColor;

        /// <summary>
        /// 左手の色（読み取り専用）
        /// </summary>
        public IReadOnlyReactiveProperty<Color> LeftHandColor => _character.LeftHandColor;

        /// <summary>
        /// 右足の色（読み取り専用）
        /// </summary>
        public IReadOnlyReactiveProperty<Color> RightFootColor => _character.RightFootColor;

        /// <summary>
        /// 左足の色（読み取り専用）
        /// </summary>
        public IReadOnlyReactiveProperty<Color> LeftFootColor => _character.LeftFootColor;

        /// <summary>
        /// 頭のスケール（読み取り専用）
        /// </summary>
        public IReadOnlyReactiveProperty<float> HeadScale => _character.HeadScale;

        /// <summary>
        /// 足のスケール（読み取り専用）
        /// </summary>
        public IReadOnlyReactiveProperty<float> FootScale => _character.FootScale;

        #endregion

        #region 音声設定の購読

        /// <summary>
        /// BGM音量（読み取り専用）
        /// </summary>
        public IReadOnlyReactiveProperty<float> BgmVolume => _audioSettings.BgmVolume;

        /// <summary>
        /// SE音量（読み取り専用）
        /// </summary>
        public IReadOnlyReactiveProperty<float> SeVolume => _audioSettings.SeVolume;

        /// <summary>
        /// Voice音量（読み取り専用）
        /// </summary>
        public IReadOnlyReactiveProperty<float> VoiceVolume => _audioSettings.VoiceVolume;

        /// <summary>
        /// BGMミュート状態（読み取り専用）
        /// </summary>
        public IReadOnlyReactiveProperty<bool> IsBgmMuted => _audioSettings.IsBgmMuted;

        /// <summary>
        /// SEミュート状態（読み取り専用）
        /// </summary>
        public IReadOnlyReactiveProperty<bool> IsSeMuted => _audioSettings.IsSeMuted;

        /// <summary>
        /// Voiceミュート状態（読み取り専用）
        /// </summary>
        public IReadOnlyReactiveProperty<bool> IsVoiceMuted => _audioSettings.IsVoiceMuted;

        #endregion

        #region 計算プロパティ

        /// <summary>
        /// 実効BGM音量（ミュート状態を考慮）
        /// </summary>
        public IObservable<float> EffectiveBgmVolume => _audioSettings.EffectiveBgmVolume;

        /// <summary>
        /// 実効SE音量（ミュート状態を考慮）
        /// </summary>
        public IObservable<float> EffectiveSeVolume => _audioSettings.EffectiveSeVolume;

        /// <summary>
        /// 実効Voice音量（ミュート状態を考慮）
        /// </summary>
        public IObservable<float> EffectiveVoiceVolume => _audioSettings.EffectiveVoiceVolume;

        #endregion

        /// <summary>
        /// 設定を初期化（アプリ起動時に呼び出し）
        /// </summary>
        public async UniTask InitializeAsync()
        {
            try
            {
                Debug.Log("[UserSettingsUseCase] 設定の初期化を開始");
                
                var userData = await _repository.LoadAsync();
                
                // キャラクター設定を復元
                _character.FromData(userData.Character);
                
                // 音声設定を復元
                _audioSettings.FromData(userData.Audio);
                
                // 復元完了後にダーティフラグをクリア
                _isDirty.Value = false;
                
                Debug.Log($"[UserSettingsUseCase] 設定の初期化完了 (バージョン: {userData.Version}, 最終保存: {userData.GetLastSavedDateTime()})");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserSettingsUseCase] 設定初期化エラー: {ex.Message}");
                // エラー時はデフォルト設定のまま続行
            }
        }

        /// <summary>
        /// 設定を手動保存
        /// </summary>
        public async UniTask SaveAsync()
        {
            try
            {
                var userData = new UserSettingsData
                {
                    Character = _character.ToData(),
                    Audio = _audioSettings.ToData(),
                    Version = UserSettingsData.CurrentVersion,
                    LastSavedAt = DateTime.Now.ToBinary()
                };

                await _repository.SaveAsync(userData);
                _isDirty.Value = false;
                
                Debug.Log("[UserSettingsUseCase] 設定を手動保存しました");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserSettingsUseCase] 手動保存エラー: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 設定をリセット（デフォルト値に戻す）
        /// </summary>
        public async UniTask ResetToDefaultAsync()
        {
            try
            {
                Debug.Log("[UserSettingsUseCase] 設定をデフォルトにリセット");
                
                _character.ResetToDefault();
                _audioSettings.ResetToDefault();
                
                await SaveAsync();
                
                Debug.Log("[UserSettingsUseCase] デフォルトリセット完了");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserSettingsUseCase] デフォルトリセットエラー: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 設定ファイルを完全削除
        /// </summary>
        public async UniTask DeleteSettingsAsync()
        {
            try
            {
                await _repository.DeleteAsync();
                
                // メモリ上の設定もリセット
                _character.ResetToDefault();
                _audioSettings.ResetToDefault();
                _isDirty.Value = false;
                
                Debug.Log("[UserSettingsUseCase] 設定ファイルを削除しました");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserSettingsUseCase] 設定削除エラー: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 設定の概要情報を取得
        /// </summary>
        public SettingsSummary GetSummary()
        {
            return new SettingsSummary
            {
                FileExists = _repository.Exists(),
                FileSize = _repository.GetFileSize(),
                IsDirty = _isDirty.Value,
                CharacterData = _character.ToData(),
                AudioData = _audioSettings.ToData()
            };
        }

        // ===== UseCase経由での操作メソッド（推奨アプローチ） =====

        #region キャラクター設定操作

        /// <summary>
        /// 右手の色を設定
        /// </summary>
        public void UpdateRightHandColor(Color color)
        {
            _character.SetRightHandColor(color);
        }

        /// <summary>
        /// 左手の色を設定
        /// </summary>
        public void UpdateLeftHandColor(Color color)
        {
            _character.SetLeftHandColor(color);
        }

        /// <summary>
        /// 右足の色を設定
        /// </summary>
        public void UpdateRightFootColor(Color color)
        {
            _character.SetRightFootColor(color);
        }

        /// <summary>
        /// 左足の色を設定
        /// </summary>
        public void UpdateLeftFootColor(Color color)
        {
            _character.SetLeftFootColor(color);
        }

        /// <summary>
        /// 頭のスケールを設定
        /// </summary>
        public void UpdateHeadScale(float scale)
        {
            _character.SetHeadScale(scale);
        }

        /// <summary>
        /// 足のスケールを設定
        /// </summary>
        public void UpdateFootScale(float scale)
        {
            _character.SetFootScale(scale);
        }

        /// <summary>
        /// 頭のスケールを増加
        /// </summary>
        public void IncreaseHeadScale()
        {
            _character.IncreaseHeadScale();
        }

        /// <summary>
        /// 頭のスケールを減少
        /// </summary>
        public void DecreaseHeadScale()
        {
            _character.DecreaseHeadScale();
        }

        /// <summary>
        /// 足のスケールを増加
        /// </summary>
        public void IncreaseFootScale()
        {
            _character.IncreaseFootScale();
        }

        /// <summary>
        /// 足のスケールを減少
        /// </summary>
        public void DecreaseFootScale()
        {
            _character.DecreaseFootScale();
        }

        /// <summary>
        /// 指定された体の部位の色を設定
        /// </summary>
        public void UpdateBodyPartColor(BodyPart part, Color color)
        {
            switch (part)
            {
                case BodyPart.RightHand:
                    UpdateRightHandColor(color);
                    break;
                case BodyPart.LeftHand:
                    UpdateLeftHandColor(color);
                    break;
                case BodyPart.RightFoot:
                    UpdateRightFootColor(color);
                    break;
                case BodyPart.LeftFoot:
                    UpdateLeftFootColor(color);
                    break;
                default:
                    Debug.LogWarning($"[UserSettingsUseCase] 未対応の体の部位: {part}");
                    break;
            }
        }

        /// <summary>
        /// キャラクター設定の一括更新
        /// </summary>
        public void UpdateCharacterSettings(Color handColor, Color footColor, float headScale, float footScale)
        {
            _character.SetRightHandColor(handColor);
            _character.SetLeftHandColor(handColor);
            _character.SetRightFootColor(footColor);
            _character.SetLeftFootColor(footColor);
            _character.SetHeadScale(headScale);
            _character.SetFootScale(footScale);
        }

        #endregion

        #region 音声設定操作

        /// <summary>
        /// BGM音量を設定
        /// </summary>
        public void UpdateBgmVolume(float volume)
        {
            _audioSettings.SetBgmVolume(volume);
        }

        /// <summary>
        /// SE音量を設定
        /// </summary>
        public void UpdateSeVolume(float volume)
        {
            _audioSettings.SetSeVolume(volume);
        }

        /// <summary>
        /// Voice音量を設定
        /// </summary>
        public void UpdateVoiceVolume(float volume)
        {
            _audioSettings.SetVoiceVolume(volume);
        }

        /// <summary>
        /// BGMミュート状態を設定
        /// </summary>
        public void UpdateBgmMute(bool isMuted)
        {
            _audioSettings.SetBgmMute(isMuted);
        }

        /// <summary>
        /// SEミュート状態を設定
        /// </summary>
        public void UpdateSeMute(bool isMuted)
        {
            _audioSettings.SetSeMute(isMuted);
        }

        /// <summary>
        /// Voiceミュート状態を設定
        /// </summary>
        public void UpdateVoiceMute(bool isMuted)
        {
            _audioSettings.SetVoiceMute(isMuted);
        }

        /// <summary>
        /// BGMミュート状態をトグル
        /// </summary>
        public void ToggleBgmMute()
        {
            _audioSettings.ToggleBgmMute();
        }

        /// <summary>
        /// SEミュート状態をトグル
        /// </summary>
        public void ToggleSeMute()
        {
            _audioSettings.ToggleSeMute();
        }

        /// <summary>
        /// Voiceミュート状態をトグル
        /// </summary>
        public void ToggleVoiceMute()
        {
            _audioSettings.ToggleVoiceMute();
        }

        /// <summary>
        /// すべてのミュート状態をトグル
        /// </summary>
        public void ToggleAllMute()
        {
            _audioSettings.ToggleAllMute();
        }

        /// <summary>
        /// 指定された音声チャンネルの音量を設定
        /// </summary>
        public void UpdateAudioVolume(AudioChannel channel, float volume)
        {
            switch (channel)
            {
                case AudioChannel.BGM:
                    UpdateBgmVolume(volume);
                    break;
                case AudioChannel.SE:
                    UpdateSeVolume(volume);
                    break;
                case AudioChannel.Voice:
                    UpdateVoiceVolume(volume);
                    break;
                default:
                    Debug.LogWarning($"[UserSettingsUseCase] 未対応の音声チャンネル: {channel}");
                    break;
            }
        }

        /// <summary>
        /// 指定された音声チャンネルのミュート状態を設定
        /// </summary>
        public void UpdateAudioMute(AudioChannel channel, bool isMuted)
        {
            switch (channel)
            {
                case AudioChannel.BGM:
                    UpdateBgmMute(isMuted);
                    break;
                case AudioChannel.SE:
                    UpdateSeMute(isMuted);
                    break;
                case AudioChannel.Voice:
                    UpdateVoiceMute(isMuted);
                    break;
                default:
                    Debug.LogWarning($"[UserSettingsUseCase] 未対応の音声チャンネル: {channel}");
                    break;
            }
        }

        /// <summary>
        /// 音声設定の一括更新
        /// </summary>
        public void UpdateAudioSettings(float bgmVolume, float seVolume, float voiceVolume, bool isBgmMuted, bool isSeMuted, bool isVoiceMuted)
        {
            _audioSettings.SetBgmVolume(bgmVolume);
            _audioSettings.SetSeVolume(seVolume);
            _audioSettings.SetVoiceVolume(voiceVolume);
            _audioSettings.SetBgmMute(isBgmMuted);
            _audioSettings.SetSeMute(isSeMuted);
            _audioSettings.SetVoiceMute(isVoiceMuted);
        }

        /// <summary>
        /// すべての音量を一括設定
        /// </summary>
        public void UpdateAllVolumes(float volume)
        {
            _audioSettings.SetBgmVolume(volume);
            _audioSettings.SetSeVolume(volume);
            _audioSettings.SetVoiceVolume(volume);
        }

        #endregion

        #region 便利メソッド

        /// <summary>
        /// ランダムなキャラクター設定を生成
        /// </summary>
        public void RandomizeCharacter()
        {
            var randomColors = new Color[]
            {
                Color.red, Color.green, Color.blue, Color.yellow,
                Color.cyan, Color.magenta, Color.white, Color.gray
            };
            
            var handColor = randomColors[UnityEngine.Random.Range(0, randomColors.Length)];
            var footColor = randomColors[UnityEngine.Random.Range(0, randomColors.Length)];
            var headScale = UnityEngine.Random.Range(0.5f, 1.5f);
            var footScale = UnityEngine.Random.Range(0.5f, 1.5f);
            
            UpdateCharacterSettings(handColor, footColor, headScale, footScale);
        }

        /// <summary>
        /// ランダムな音声設定を生成
        /// </summary>
        public void RandomizeAudio()
        {
            var bgmVolume = UnityEngine.Random.Range(0f, 1f);
            var seVolume = UnityEngine.Random.Range(0f, 1f);
            var voiceVolume = UnityEngine.Random.Range(0f, 1f);
            var isBgmMuted = UnityEngine.Random.Range(0, 2) == 1;
            var isSeMuted = UnityEngine.Random.Range(0, 2) == 1;
            var isVoiceMuted = UnityEngine.Random.Range(0, 2) == 1;
            
            UpdateAudioSettings(bgmVolume, seVolume, voiceVolume, isBgmMuted, isSeMuted, isVoiceMuted);
        }

        #endregion

        // ===== 内部処理（既存コード） =====

        /// <summary>
        /// 変更検知の設定
        /// </summary>
        private void SetupChangeDetection()
        {
            // キャラクター設定の変更を監視
            _character.OnRightHandColorChanged.Subscribe(_ => MarkDirty()).AddTo(_disposables);
            _character.OnLeftHandColorChanged.Subscribe(_ => MarkDirty()).AddTo(_disposables);
            _character.OnRightFootColorChanged.Subscribe(_ => MarkDirty()).AddTo(_disposables);
            _character.OnLeftFootColorChanged.Subscribe(_ => MarkDirty()).AddTo(_disposables);
            _character.OnHeadScaleChanged.Subscribe(_ => MarkDirty()).AddTo(_disposables);
            _character.OnFootScaleChanged.Subscribe(_ => MarkDirty()).AddTo(_disposables);

            // 音声設定の変更を監視
            _audioSettings.OnBgmVolumeChanged.Subscribe(_ => MarkDirty()).AddTo(_disposables);
            _audioSettings.OnSeVolumeChanged.Subscribe(_ => MarkDirty()).AddTo(_disposables);
            _audioSettings.OnVoiceVolumeChanged.Subscribe(_ => MarkDirty()).AddTo(_disposables);
            _audioSettings.OnBgmMuteChanged.Subscribe(_ => MarkDirty()).AddTo(_disposables);
            _audioSettings.OnSeMuteChanged.Subscribe(_ => MarkDirty()).AddTo(_disposables);
            _audioSettings.OnVoiceMuteChanged.Subscribe(_ => MarkDirty()).AddTo(_disposables);
        }

        /// <summary>
        /// 自動保存の設定（変更から3秒後に自動保存）
        /// </summary>
        private void SetupAutoSave()
        {
            _autoSaveSubscription = _isDirty
                .Where(isDirty => isDirty)
                .Throttle(TimeSpan.FromSeconds(3.0f))
                .Subscribe(async _ =>
                {
                    try
                    {
                        await SaveAsync();
                        Debug.Log("[UserSettingsUseCase] 自動保存が実行されました");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[UserSettingsUseCase] 自動保存エラー: {ex.Message}");
                    }
                });
        }

        /// <summary>
        /// 変更をマーク
        /// </summary>
        private void MarkDirty()
        {
            _isDirty.Value = true;
        }

        public void Dispose()
        {
            _autoSaveSubscription?.Dispose();
            _disposables?.Dispose();
        }
    }

    /// <summary>
    /// 設定の概要情報
    /// </summary>
    public struct SettingsSummary
    {
        public bool FileExists;
        public long FileSize;
        public bool IsDirty;
        public CharacterData CharacterData;
        public NewDemoAudioSettingsData AudioData;
    }

    /// <summary>
    /// 体の部位
    /// </summary>
    public enum BodyPart
    {
        RightHand,
        LeftHand,
        RightFoot,
        LeftFoot
    }
} 