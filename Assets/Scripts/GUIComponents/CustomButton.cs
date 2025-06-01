using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace CommonViewParts
{
    /// <summary>
    /// ボタンのraycast当たり判定スケール
    /// </summary>
    public enum RaycastScale
    {
        /// <summary>100%（デフォルト）</summary>
        Scale100 = 100,
        /// <summary>110%</summary>
        Scale110 = 110,
        /// <summary>120%</summary>
        Scale120 = 120,
        /// <summary>140%</summary>
        Scale140 = 140
    }

    /// <summary>
    /// 汎用ボタン
    /// </summary>
    [RequireComponent(typeof(ObservableEventTrigger))]
    public class CustomButton : MonoBehaviour
    {
        [SerializeField]
        private RaycastScale _raycastScale = RaycastScale.Scale100;
        
        private float _buttonRaycastScale = 1f;
        
        private ObservableEventTrigger _observableEventTrigger;

        /// <summary>
        /// ボタンクリック時
        /// </summary>
        public IObservable<Unit> OnButtonClicked => _observableEventTrigger
            .OnPointerClickAsObservable().AsUnitObservable().Where(_ => _isActiveRP.Value);
        
        /// <summary>
        /// ボタンを押した時
        /// </summary>
        public IObservable<Unit> OnButtonPressed => _observableEventTrigger
            .OnPointerDownAsObservable().AsUnitObservable().Where(_ => _isActiveRP.Value);
        
        /// <summary>
        /// ボタンを離した時
        /// </summary>
        public IObservable<Unit> OnButtonReleased => _observableEventTrigger
            .OnPointerUpAsObservable().AsUnitObservable().Where(_ => _isActiveRP.Value);
        
        /// <summary>
        /// ボタンの領域にカーソルが入った時
        /// </summary>
        public IObservable<Unit> OnButtonEntered => _observableEventTrigger
            .OnPointerEnterAsObservable().AsUnitObservable().Where(_ => _isActiveRP.Value);
        
        /// <summary>
        /// ボタンの領域からカーソルが出た時
        /// </summary>
        public IObservable<Unit> OnButtonExited => _observableEventTrigger
            .OnPointerExitAsObservable().AsUnitObservable().Where(_ => _isActiveRP.Value);

        /// <summary>
        /// ボタンのアクティブ状態を保持するReactiveProperty
        /// </summary>
        public IReadOnlyReactiveProperty<bool> IsActiveRP => _isActiveRP;
        
        private readonly ReactiveProperty<bool> _isActiveRP = new(true);

        protected virtual void OnDestroy()
        {
            _isActiveRP.Dispose();
        }

        protected virtual void Awake()
        {
            _observableEventTrigger = GetComponent<ObservableEventTrigger>();
            
            // Raycast Scaleを適用
            ApplyRaycastScale();
        }

        /// <summary>
        /// ボタンのアクティブ状態を取得する
        /// </summary>
        public bool GetIsActive() => _isActiveRP.Value;

        /// <summary>
        /// アクティブ状態を変更する
        /// </summary>
        public void SetActive(bool isActive)
        {
            _isActiveRP.Value = isActive;
        }
        
        /// <summary>
        /// Raycast Scaleの設定値を取得
        /// </summary>
        public RaycastScale GetRaycastScale() => _raycastScale;
        
        /// <summary>
        /// Raycast Scaleを設定
        /// </summary>
        public void SetRaycastScale(RaycastScale scale)
        {
            _raycastScale = scale;
            ApplyRaycastScale();
        }
        
        /// <summary>
        /// 設定されたRaycast Scaleを適用
        /// </summary>
        private void ApplyRaycastScale()
        {
            _buttonRaycastScale = (float)_raycastScale / 100f;
            
            if (_raycastScale != RaycastScale.Scale100)
            {
                var additionalScale = _buttonRaycastScale - 1f;
                TryChangeRaycastPaddings(additionalScale);
            }
        }
        
        private void TryChangeRaycastPaddings( float additionalScale)
        {
            var image = GetComponent<Image>();
            if (image == null)
            {
                return;
            }
            
            // Otherwise we will strugle with raycasting the sticker we want
            if (additionalScale > 0)
            {
                // Ex. we have sticker with sizes 200x200 with 0.5 additional scale.
                // So it becomes 300x300
                var rectTransform = image.transform as RectTransform;

                var W = rectTransform.sizeDelta.x; // Ex. 300
                var H = rectTransform.sizeDelta.y; // ...

                var originalW = W / (1 + additionalScale); // Ex. 200
                var originalH = H / (1 + additionalScale); // ...

                var paddingW = (W - originalW) / 2; // (300 - 200) / 2 = 50
                var paddingH = (H - originalH) / 2; // ...

                image.raycastPadding = new Vector4(paddingW, paddingH, paddingW, paddingH);
            }
        }
    }
}
