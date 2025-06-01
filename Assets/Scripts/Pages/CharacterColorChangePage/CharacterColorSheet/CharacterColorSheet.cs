using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Sheet;

public class CharacterColorSheet : Sheet
{
    
    [SerializeField] Slider _redSlider;
    [SerializeField] Slider _greenSlider;
    [SerializeField] Slider _blueSlider;
    
    [SerializeField] Image _targetGraphic;
    public ReactiveProperty<Color> _targetColor;

    public void SetColor(Color color)
    {
        _targetGraphic.color = color;
        _redSlider.value = color.r;
        _greenSlider.value = color.g;
        _blueSlider.value = color.b;
    }
    // Start is called before the first frame update
    void Start()
    {
        Observable
            .CombineLatest(
                _redSlider.OnValueChangedAsObservable(),
                _greenSlider.OnValueChangedAsObservable(),
                _blueSlider.OnValueChangedAsObservable()
            )
            .Select(colorList => new Color(colorList[0], colorList[1], colorList[2]))
            .Subscribe(color =>
            {
                _targetColor.SetValueAndForceNotify(color);
                _targetGraphic.color = color;
            });
    }
    

}
