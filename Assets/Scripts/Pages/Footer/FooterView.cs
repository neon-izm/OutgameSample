using System;
using CommonViewParts;
using UnityEngine;
using ScreenSystem.Page;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class FooterView : MonoBehaviour
{
   public IObservable<Unit> OnClickFootDownButton { get; private set; }
   public IObservable<Unit> OnClickFootUpButton { get; private set; }

   public IObservable<Unit> OnClickHeadDownButton { get; private set; }
   public IObservable<Unit> OnClickHeadUpButton { get; private set; }

   public void SetValueFoot(string text)
   {
      _scaleSettingViewFoot.SetLabel(text);
   }

   public void SetValueHead(string text)
   {
      _scaleSettingViewHead.SetLabel(text);
   }
   
   [SerializeField] private ScaleSettingView _scaleSettingViewHead;
   [SerializeField] private ScaleSettingView _scaleSettingViewFoot;

   private void Awake()
   {
      OnClickFootUpButton = _scaleSettingViewFoot.OnClickUpButton;
      OnClickFootDownButton = _scaleSettingViewFoot.OnClickDownButton;
      OnClickHeadUpButton = _scaleSettingViewHead.OnClickUpButton;
      OnClickHeadDownButton = _scaleSettingViewHead.OnClickDownButton;
   }
}