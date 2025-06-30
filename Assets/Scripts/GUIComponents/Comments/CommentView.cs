using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CommentView : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] TMPro.TextMeshProUGUI _text;
    
    [SerializeField] Text _buttonText;
    public IObservable<Unit> OnClickButton { get; private set; }

    public void SetButtonText(string text)
    {
        _buttonText.text = text;
    }
    public void SetText(string text)
    {
        _text.text = text;
    }
    private void Awake()
    {
       OnClickButton = _button.OnClickAsObservable();
    }
}
