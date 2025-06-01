using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using NewDemo.Core.Scripts.Domain.Character.Model;
using NewDemo.Core.Scripts.Domain.Settings.Model;

namespace NewDemo.Core.Scripts.Infrastructure.Settings
{
    /// <summary>
    /// ユーザー設定の永続化を担当するRepository
    /// </summary>
    public sealed class UserSettingsRepository
    {
        private const string SettingsFileName = "user_settings.json";
        private readonly string _settingsFilePath;

        public UserSettingsRepository()
        {
            _settingsFilePath = Path.Combine(Application.persistentDataPath, SettingsFileName);
            Debug.Log($"[UserSettingsRepository] 設定ファイルパス: {_settingsFilePath}");
        }

        /// <summary>
        /// ユーザー設定を読み込み
        /// </summary>
        public async UniTask<UserSettingsData> LoadAsync()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                {
                    Debug.Log("[UserSettingsRepository] 設定ファイルが存在しません。デフォルト設定を使用します。");
                    return CreateDefaultSettings();
                }

                // ファイル読み込みを非同期で実行
                var jsonText = await File.ReadAllTextAsync(_settingsFilePath);
                
                if (string.IsNullOrEmpty(jsonText))
                {
                    Debug.LogWarning("[UserSettingsRepository] 設定ファイルが空です。デフォルト設定を使用します。");
                    return CreateDefaultSettings();
                }

                var settings = JsonUtility.FromJson<UserSettingsData>(jsonText);
                Debug.Log("[UserSettingsRepository] 設定を正常に読み込みました。");
                return settings;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserSettingsRepository] 設定読み込みエラー: {ex.Message}");
                return CreateDefaultSettings();
            }
        }

        /// <summary>
        /// ユーザー設定を保存
        /// </summary>
        public async UniTask SaveAsync(UserSettingsData settings)
        {
            try
            {
                // Timestampを更新
                settings.LastSavedAt = DateTime.Now.ToBinary();
                
                var jsonText = JsonUtility.ToJson(settings, true);
                
                // ディレクトリが存在しない場合は作成
                var directory = Path.GetDirectoryName(_settingsFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // ファイル書き込みを非同期で実行
                await File.WriteAllTextAsync(_settingsFilePath, jsonText);
                
                Debug.Log("[UserSettingsRepository] 設定を正常に保存しました。");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserSettingsRepository] 設定保存エラー: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 設定ファイルを削除
        /// </summary>
        public async UniTask DeleteAsync()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    await UniTask.Run(() => File.Delete(_settingsFilePath));
                    Debug.Log("[UserSettingsRepository] 設定ファイルを削除しました。");
                }
                else
                {
                    Debug.Log("[UserSettingsRepository] 削除対象の設定ファイルが存在しません。");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserSettingsRepository] 設定ファイル削除エラー: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 設定ファイルが存在するかチェック
        /// </summary>
        public bool Exists()
        {
            return File.Exists(_settingsFilePath);
        }

        /// <summary>
        /// 設定ファイルのサイズを取得（デバッグ用）
        /// </summary>
        public long GetFileSize()
        {
            if (!File.Exists(_settingsFilePath))
                return 0;
            
            var fileInfo = new FileInfo(_settingsFilePath);
            return fileInfo.Length;
        }

        /// <summary>
        /// デフォルト設定を作成
        /// </summary>
        private UserSettingsData CreateDefaultSettings()
        {
            return new UserSettingsData
            {
                Character = new CharacterData
                {
                    RightHandColor = Color.white,
                    LeftHandColor = Color.white,
                    RightFootColor = Color.white,
                    LeftFootColor = Color.white,
                    HeadScale = 1.0f,
                    FootScale = 1.0f
                },
                Audio = new NewDemoAudioSettingsData
                {
                    BgmVolume = 1.0f,
                    SeVolume = 1.0f,
                    VoiceVolume = 1.0f,
                    IsBgmMuted = false,
                    IsSeMuted = false,
                    IsVoiceMuted = false
                },
                Version = UserSettingsData.CurrentVersion,
                LastSavedAt = DateTime.Now.ToBinary()
            };
        }
    }

    /// <summary>
    /// ユーザー設定の統合データ構造
    /// </summary>
    [Serializable]
    public struct UserSettingsData
    {
        public const int CurrentVersion = 1;
        
        public CharacterData Character;
        public NewDemoAudioSettingsData Audio;
        public int Version;
        public long LastSavedAt; // DateTime.ToBinary()の値
        
        /// <summary>
        /// 保存日時を取得
        /// </summary>
        public DateTime GetLastSavedDateTime()
        {
            return DateTime.FromBinary(LastSavedAt);
        }
    }
} 