using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Extensions.Unity;
using UnityEngine;
using VContainer;

public sealed class DebugSheetController : MonoBehaviour
{
    GuidCounterService _guidCounterService;

    [Inject]
    public void Construct(GuidCounterService guidCounterService)
    {
        _guidCounterService = guidCounterService;
    }
    private void Start()
    {
        // Get or create the root page.
        var rootPage = DebugSheet.Instance.GetOrCreateInitialPage();
        // Add a link transition to the ExampleDebugPage.
        rootPage.AddPageLinkButton<ExampleDebugPage>("ExampleDebugPage",
            onLoad: page => page.page.Init(_guidCounterService));
        rootPage.AddPageLinkButton<SystemInfoDebugPage>(nameof(SystemInfo));
        rootPage.AddPageLinkButton<ApplicationDebugPage>(nameof(Application));
        rootPage.AddPageLinkButton<TimeDebugPage>(nameof(Time));
        rootPage.AddPageLinkButton<QualitySettingsDebugPage>(nameof(QualitySettings));
        rootPage.AddPageLinkButton<ScreenDebugPage>(nameof(Screen));
        rootPage.AddPageLinkButton<InputDebugPage>(nameof(Input));
       
    }
}