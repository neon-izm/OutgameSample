using System;
using System.Collections;
using System.Collections.Generic;
using Demo.Subsystem.GUIComponents.TabGroup;
using ScreenSystem.Page;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorChangePageView : PageViewBase
{
    [Header("UI Elements")]
    [SerializeField] private Button _returnButton;
    //[SerializeField] private Button _resetButton;
    public TabGroup itemSetTabGroup;

    [Header("Preset Colors")]
    [SerializeField] private Color[] _presetColors = new Color[]
    {
        Color.white, Color.red, Color.green, Color.blue, 
        Color.yellow, Color.magenta, Color.cyan, Color.black
    };

    // 各部位の色変更時のイベント
    private readonly Subject<Color> _rightHandColorChanged = new Subject<Color>();
    private readonly Subject<Color> _leftHandColorChanged = new Subject<Color>();
    private readonly Subject<Color> _rightFootColorChanged = new Subject<Color>();
    private readonly Subject<Color> _leftFootColorChanged = new Subject<Color>();

    // イベント公開プロパティ
    public IObservable<Unit> OnClickReturnButton => _returnButton.OnClickAsObservable();
    //public IObservable<Unit> OnClickResetButton => _resetButton.OnClickAsObservable();
    
    public IObservable<Color> OnRightHandColorChanged => _rightHandColorChanged.AsObservable();
    public IObservable<Color> OnLeftHandColorChanged => _leftHandColorChanged.AsObservable();
    public IObservable<Color> OnRightFootColorChanged => _rightFootColorChanged.AsObservable();
    public IObservable<Color> OnLeftFootColorChanged => _leftFootColorChanged.AsObservable();

    private async void Start()
    {
        
        itemSetTabGroup.OnTabLoaded
            .Subscribe(x =>
            {
                Debug.Log(x.Sheet.name + " is loaded");
                
                var itemSetSheet = (CharacterColorSheet)x.Sheet;

                switch (x.Index)
                {
                    case 0:
                        //右手
                        itemSetSheet.SetColor(OnRightHandColorChanged.ToReadOnlyReactiveProperty().Value);
                        itemSetSheet._targetColor.Subscribe(
                            x =>
                            {
                                _rightHandColorChanged.OnNext(x);
                            }
                            ).AddTo(this);
                        break;
                    case 1:
                        //左手
                        itemSetSheet.SetColor(OnLeftHandColorChanged.ToReadOnlyReactiveProperty().Value);
                        itemSetSheet._targetColor.Subscribe(
                            x =>
                            {
                                _leftHandColorChanged.OnNext(x);
                            }
                        ).AddTo(this);
                        break;
                    case 2:
                        //右足
                        itemSetSheet.SetColor(OnRightFootColorChanged.ToReadOnlyReactiveProperty().Value);
                        itemSetSheet._targetColor.Subscribe(
                            x =>
                            {
                                _rightFootColorChanged.OnNext(x);
                            }
                        ).AddTo(this);
                        break;
                    case 3:
                        //左足
                        itemSetSheet.SetColor(OnLeftFootColorChanged.ToReadOnlyReactiveProperty().Value);
                        itemSetSheet._targetColor.Subscribe(
                            x =>
                            {
                                _leftFootColorChanged.OnNext(x);
                            }
                        ).AddTo(this);
                        break;
                    default:
                        break;
                }
                
            })
            .AddTo(this);

        await itemSetTabGroup.InitializeAsync();
        // 簡易的な色選択実装（ランダムカラー）
        // 実際の実装では、ColorPickerやパレットUIを使用
        /*
        _rightHandColorButton.OnClickAsObservable().Subscribe(_ => 
        {
            var randomColor = _presetColors[UnityEngine.Random.Range(0, _presetColors.Length)];
            SetRightHandColor(randomColor);
            _rightHandColorChanged.OnNext(randomColor);
        });
        
        _leftHandColorButton.OnClickAsObservable().Subscribe(_ => 
        {
            var randomColor = _presetColors[UnityEngine.Random.Range(0, _presetColors.Length)];
            SetLeftHandColor(randomColor);
            _leftHandColorChanged.OnNext(randomColor);
        });
        
        _rightFootColorButton.OnClickAsObservable().Subscribe(_ => 
        {
            var randomColor = _presetColors[UnityEngine.Random.Range(0, _presetColors.Length)];
            SetRightFootColor(randomColor);
            _rightFootColorChanged.OnNext(randomColor);
        });
        
        _leftFootColorButton.OnClickAsObservable().Subscribe(_ => 
        {
            var randomColor = _presetColors[UnityEngine.Random.Range(0, _presetColors.Length)];
            SetLeftFootColor(randomColor);
            _leftFootColorChanged.OnNext(randomColor);
        });
        */
    }

   
    // 各部位の色を設定
    public void SetRightHandColor(Color color)
    {
        _rightHandColorChanged.OnNext(color);
    }

    public void SetLeftHandColor(Color color)
    {
        _leftHandColorChanged.OnNext(color);
    }

    public void SetRightFootColor(Color color)
    {
        _rightFootColorChanged.OnNext(color);
    }

    public void SetLeftFootColor(Color color)
    {
        _leftFootColorChanged.OnNext(color);
    }
    private void OnDestroy()
    {
        _rightHandColorChanged?.Dispose();
        _leftHandColorChanged?.Dispose();
        _rightFootColorChanged?.Dispose();
        _leftFootColorChanged?.Dispose();
    }
} 