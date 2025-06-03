using System.Collections;
using System.Collections.Generic;
using CommonViewParts;
using TMPro;
using UniRx;
using UnityEngine;
using System;

namespace CommonViewParts
{
    public class ScaleSettingView : MonoBehaviour
    {
        public void SetLabel(string label)
        {
            _currentValueLabel.text = label;
        }

        public IObservable<Unit> OnClickDownButton { get; private set; }
        public IObservable<Unit> OnClickUpButton { get; private set; }
        public IObservable<Unit> OnClickCurrentButton { get; private set; }

        [SerializeField] CustomButton _upButton;
        [SerializeField] CustomButton _downButton;
        [SerializeField] CustomButton _currentButton;
        [SerializeField] TextMeshProUGUI _currentValueLabel;

        private void Awake()
        {
            OnClickUpButton = _upButton.OnButtonClicked.AsObservable();
            OnClickDownButton = _downButton.OnButtonClicked.AsObservable();
            OnClickCurrentButton = _currentButton.OnButtonClicked.AsObservable();
        }
    }
}