using System;
using UniRx;
using UnityEngine;

namespace NewDemo.Core.Scripts.Domain.Character.Model
{
    /// <summary>
    /// キャラクターの外見設定を管理するドメインモデル
    /// </summary>
    public sealed class Character
    {
        // 体の部位の色設定
        private readonly ReactiveProperty<Color> _rightHandColor = new ReactiveProperty<Color>(Color.white);
        private readonly ReactiveProperty<Color> _leftHandColor = new ReactiveProperty<Color>(Color.white);
        private readonly ReactiveProperty<Color> _rightFootColor = new ReactiveProperty<Color>(Color.white);
        private readonly ReactiveProperty<Color> _leftFootColor = new ReactiveProperty<Color>(Color.white);

        // 体のスケール設定（50%~150%、10%刻み）
        private readonly ReactiveProperty<float> _headScale = new ReactiveProperty<float>(1.0f);
        private readonly ReactiveProperty<float> _footScale = new ReactiveProperty<float>(1.0f);

        // 公開プロパティ（読み取り専用）
        public IReadOnlyReactiveProperty<Color> RightHandColor => _rightHandColor;
        public IReadOnlyReactiveProperty<Color> LeftHandColor => _leftHandColor;
        public IReadOnlyReactiveProperty<Color> RightFootColor => _rightFootColor;
        public IReadOnlyReactiveProperty<Color> LeftFootColor => _leftFootColor;
        
        public IReadOnlyReactiveProperty<float> HeadScale => _headScale;
        public IReadOnlyReactiveProperty<float> FootScale => _footScale;

        // 変更通知イベント
        public IObservable<Color> OnRightHandColorChanged => _rightHandColor;
        public IObservable<Color> OnLeftHandColorChanged => _leftHandColor;
        public IObservable<Color> OnRightFootColorChanged => _rightFootColor;
        public IObservable<Color> OnLeftFootColorChanged => _leftFootColor;
        public IObservable<float> OnHeadScaleChanged => _headScale;
        public IObservable<float> OnFootScaleChanged => _footScale;

        /// <summary>
        /// 右手の色を設定
        /// </summary>
        public void SetRightHandColor(Color color)
        {
            _rightHandColor.Value = color;
        }

        /// <summary>
        /// 左手の色を設定
        /// </summary>
        public void SetLeftHandColor(Color color)
        {
            _leftHandColor.Value = color;
        }

        /// <summary>
        /// 右足の色を設定
        /// </summary>
        public void SetRightFootColor(Color color)
        {
            _rightFootColor.Value = color;
        }

        /// <summary>
        /// 左足の色を設定
        /// </summary>
        public void SetLeftFootColor(Color color)
        {
            _leftFootColor.Value = color;
        }

        /// <summary>
        /// 頭のスケールを設定（50%~150%、10%刻み）
        /// </summary>
        public void SetHeadScale(float scale)
        {
            var clampedScale = Mathf.Clamp(scale, 0.5f, 1.5f);
            var roundedScale = Mathf.Round(clampedScale * 10f) / 10f;
            _headScale.Value = roundedScale;
        }

        /// <summary>
        /// 足のスケールを設定（50%~150%、10%刻み）
        /// </summary>
        public void SetFootScale(float scale)
        {
            var clampedScale = Mathf.Clamp(scale, 0.5f, 1.5f);
            var roundedScale = Mathf.Round(clampedScale * 10f) / 10f;
            _footScale.Value = roundedScale;
        }

        /// <summary>
        /// 頭のスケールを段階的に増加（+10%）
        /// </summary>
        public void IncreaseHeadScale()
        {
            SetHeadScale(_headScale.Value + 0.1f);
        }

        /// <summary>
        /// 頭のスケールを段階的に減少（-10%）
        /// </summary>
        public void DecreaseHeadScale()
        {
            SetHeadScale(_headScale.Value - 0.1f);
        }

        /// <summary>
        /// 足のスケールを段階的に増加（+10%）
        /// </summary>
        public void IncreaseFootScale()
        {
            SetFootScale(_footScale.Value + 0.1f);
        }

        /// <summary>
        /// 足のスケールを段階的に減少（-10%）
        /// </summary>
        public void DecreaseFootScale()
        {
            SetFootScale(_footScale.Value - 0.1f);
        }

        /// <summary>
        /// すべての設定をデフォルトにリセット
        /// </summary>
        public void ResetToDefault()
        {
            _rightHandColor.Value = Color.white;
            _leftHandColor.Value = Color.white;
            _rightFootColor.Value = Color.white;
            _leftFootColor.Value = Color.white;
            _headScale.Value = 1.0f;
            _footScale.Value = 1.0f;
        }

        /// <summary>
        /// 現在の設定をデータオブジェクトとして取得
        /// </summary>
        public CharacterData ToData()
        {
            return new CharacterData
            {
                RightHandColor = _rightHandColor.Value,
                LeftHandColor = _leftHandColor.Value,
                RightFootColor = _rightFootColor.Value,
                LeftFootColor = _leftFootColor.Value,
                HeadScale = _headScale.Value,
                FootScale = _footScale.Value
            };
        }

        /// <summary>
        /// データオブジェクトから設定を復元
        /// </summary>
        public void FromData(CharacterData data)
        {
            _rightHandColor.Value = data.RightHandColor;
            _leftHandColor.Value = data.LeftHandColor;
            _rightFootColor.Value = data.RightFootColor;
            _leftFootColor.Value = data.LeftFootColor;
            SetHeadScale(data.HeadScale);
            SetFootScale(data.FootScale);
        }
    }

    /// <summary>
    /// キャラクター設定のデータ転送オブジェクト
    /// </summary>
    [Serializable]
    public struct CharacterData
    {
        public Color RightHandColor;
        public Color LeftHandColor;
        public Color RightFootColor;
        public Color LeftFootColor;
        public float HeadScale;
        public float FootScale;
    }
} 