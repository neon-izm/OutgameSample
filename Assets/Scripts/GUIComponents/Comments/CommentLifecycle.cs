using System;
using System.Collections;
using System.Collections.Generic;
using Demo.Subsystem;
using NewDemo.Core.Scripts.UseCase.Settings;
using ScreenSystem.Page;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class CommentLifecycle : IStartable
{
    private GuidCounterService _guidCounterService;
    private CommentView _view;
    [Inject]
    public CommentLifecycle(GuidCounterService guidCounterService, CommentView view)
    {
        Debug.Log("CommentLifecycle Constructor");
        _guidCounterService = guidCounterService;
        _view = view;
    }
    
    private string _guidString= String.Empty;

    // Start is called before the first frame update
    public void Start()
    {
        _view.SetText(_guidCounterService.GuidInt.ToString());
        _view.OnClickButton.Subscribe(_view =>
        {
            if (string.IsNullOrEmpty(_guidString))
            {
                _guidString = _guidCounterService.GetGuidString();
            }
            Toast.QueueToast(_guidString);
        });
    }

}
