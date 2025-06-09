using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Demo.Subsystem.GUIComponents
{
    /// <summary>
    /// 素朴に作ったToastのView
    /// UniTaskなどにも依存しない作りにしておいたので、別PJで挙動を作ったりしてください
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class ToastElement : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text = default;
        private const float AlphaZero = 0f;
        /// <summary>
        /// フェードイン、フェードアウトの秒数、仮で0.3秒としています
        /// </summary>
        private const float FadeDurationSeconds = 0.3f;
        private const float AlphaOne = 1f;
        private CanvasGroup _canvasGroup;

        public void Show(string message, float durationSeconds = 3.0f, Action onClosed = null)
        {
            var rect = GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }

            this.text.text = message;

            StartCoroutine(ShowCoroutine(durationSeconds, onClosed));
        }

        IEnumerator ShowCoroutine(float durationSeconds = 3.0f, Action onClosed = null)
        {
            float alpha = AlphaZero;

            if (FadeDurationSeconds > 0f)
            {
                float startTime = Time.time;

                //Anim start
                while (!Mathf.Approximately(alpha, AlphaOne))
                {
                    alpha = Mathf.Lerp(AlphaZero, AlphaOne, (Time.time - startTime) / FadeDurationSeconds);
                    _canvasGroup.alpha = alpha;

                    yield return null;
                }
            }

            yield return new WaitForSeconds(durationSeconds - FadeDurationSeconds * 2);
            if (FadeDurationSeconds > 0f)
            {
                float endTime = Time.time;

                //Anim start
                while (!Mathf.Approximately(alpha, AlphaZero))
                {
                    alpha = Mathf.Lerp(AlphaOne, AlphaZero, (Time.time - endTime) / FadeDurationSeconds);
                    _canvasGroup.alpha = alpha;

                    yield return null;
                }
            }

            onClosed?.Invoke();
            Destroy(this.gameObject);
        }
    }
}