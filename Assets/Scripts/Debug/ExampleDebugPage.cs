using System.Collections;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using VContainer;

/// <summary>
/// note:DefaultDebugPageBaseは内部的にMonoBehaviourを継承している
/// </summary>
public sealed class ExampleDebugPage : DefaultDebugPageBase
{
    protected override string Title { get; } = "Example Debug Page";
    
    GuidCounterService _guidCounterService;
    
    public void Init(GuidCounterService guidCounterService)
    {
        _guidCounterService = guidCounterService;
        Debug.Log("ExampleDebugPage Injected");
    }
    public override IEnumerator Initialize()
    {
        // Add a button to this page.
        AddButton("Example Button", clicked: () => { Debug.Log("Clicked"); });
        if (_guidCounterService != null)
        {
            Debug.Log($"ExampleDebugPage Initialize{_guidCounterService.GuidInt}");
        }
        else
        {
            Debug.LogError($"ExampleDebugPage Initialize Failed to inject GuidCounterService");
        }

        yield break;
    }
}