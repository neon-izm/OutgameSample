using System.Collections;
using System.Collections.Generic;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// 処理速度や設計的にやるべきではないが、外部から動的生成されるデバッグ機能などで
/// 明示的にVContainerのLifetimeScopeを参照してデバッグ実行をすることもできる
/// </summary>
public class ExampleForceResolveDebugPage : DefaultDebugPageBase
{
    protected override string Title => "Example Force Resolve Debug Page";

    public override IEnumerator Initialize()
    {
        // Add a button to this page.
        AddButton("Guid Button", clicked: () =>
        {
            var forceResolvedGuidService = LifetimeScope.Find<RootLifetimeScope>().Container.Resolve<GuidCounterService>();
            if (forceResolvedGuidService != null)
            {
                Debug.Log($"GuidCounterService GuidInt:{forceResolvedGuidService.GuidInt}");
            }
            else
            {
                Debug.LogError("Failed to resolve GuidCounterService from RootLifetimeScope");
            }
        });
        yield break;
    }
}