using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CommonViewParts
{
    /// <summary>
    /// 選択すると拡縮するようなボタンView
    /// </summary>
    [RequireComponent(typeof(CustomButton))]
    public class ScaledCommonButtonView : MonoBehaviour
    {
        private const float DefaultScale = 1f;
        private const float PressedScale = 0.9f;
        
        private const float ActiveImageAlpha = 1f;
        private const float InactiveImageAlpha = 0.5f;
        
        private CustomButton _button;
        [SerializeField] private Image _image;

        private void Start()
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }
            _button = GetComponent<CustomButton>();
            
            _button.OnButtonPressed
                .Subscribe(_ => SetScale(PressedScale))
                .AddTo(this.gameObject);

            _button.OnButtonReleased
                .Subscribe(_ => SetScale(DefaultScale))
                .AddTo(this.gameObject);
            
            _button.IsActiveRP
                .Subscribe(SetButtonActive)
                .AddTo(this.gameObject);
        }

        private void SetScale(float scale)
        {
            _image.rectTransform.localScale = Vector3.one * scale;
        }

        private void SetButtonActive(bool isActive)
        {
            float alpha = isActive ? ActiveImageAlpha : InactiveImageAlpha;
            _image.color = new Color(1, 1, 1, alpha);
        }
        
    }
}