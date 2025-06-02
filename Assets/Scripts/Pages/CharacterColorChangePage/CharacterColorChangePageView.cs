using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
    private ReactiveProperty<Color> _rightHandColorChanged = new ReactiveProperty<Color>();
    private ReactiveProperty<Color> _leftHandColorChanged = new ReactiveProperty<Color>();
    private ReactiveProperty<Color> _rightFootColorChanged = new ReactiveProperty<Color>();
    private ReactiveProperty<Color> _leftFootColorChanged = new ReactiveProperty<Color>();

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
                        Debug.Log($"Right Hand Color{_rightHandColorChanged.Value} ");
                        itemSetSheet.SetColor(_rightHandColorChanged.Value);
                        itemSetSheet._targetColor.Subscribe(
                            x =>
                            {
                                _rightHandColorChanged.Value = x;
                            }
                            ).AddTo(this);
                        break;
                    case 1:
                        //左手
                        itemSetSheet.SetColor(_leftHandColorChanged.Value);
                        itemSetSheet._targetColor.Subscribe(
                            x =>
                            {
                                _leftHandColorChanged.Value =x;
                            }
                        ).AddTo(this);
                        break;
                    case 2:
                        //右足
                        itemSetSheet.SetColor(_rightFootColorChanged.Value);
                        itemSetSheet._targetColor.Subscribe(
                            x =>
                            {
                                _rightFootColorChanged.Value= x;
                            }
                        ).AddTo(this);
                        break;
                    case 3:
                        //左足
                        itemSetSheet.SetColor(_leftFootColorChanged.ToReactiveProperty().Value);
                        itemSetSheet._targetColor.Subscribe(
                            x =>
                            {
                                _leftFootColorChanged.Value = x;
                            }
                        ).AddTo(this);
                        break;
                    default:
                        break;
                }
                
            })
            .AddTo(this);

        Debug.Log("CharacterColorChangePageView created");
        await itemSetTabGroup.InitializeAsync();
        
    }

   
    // 各部位の色を設定
    public void SetRightHandColor(Color color)
    {
        _rightHandColorChanged.SetValueAndForceNotify(color);
    }

    public void SetLeftHandColor(Color color)
    {
        _leftHandColorChanged.SetValueAndForceNotify(color);
    }

    public void SetRightFootColor(Color color)
    {
        _rightFootColorChanged.SetValueAndForceNotify(color);
    }

    public void SetLeftFootColor(Color color)
    {
        _leftFootColorChanged.SetValueAndForceNotify(color);
    }
    private void OnDestroy()
    {
        _rightHandColorChanged?.Dispose();
        _leftHandColorChanged?.Dispose();
        _rightFootColorChanged?.Dispose();
        _leftFootColorChanged?.Dispose();
    }
} 