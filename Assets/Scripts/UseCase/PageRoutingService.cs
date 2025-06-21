using UnityEngine;
using UnityEngine.SceneManagement;
using UnityScreenNavigator.Runtime.Core.Page;
using VContainer.Unity;
using System;
using ScreenSystem.Page;
using System.Collections.Generic;

public class PageRoutingService : IStartable, IDisposable, IPageContainerCallbackReceiver
{
    private static PageContainer _pageContainer;
    private PageEventPublisher _publisher;

    private GuidCounterService _guidCounterService;

    public PageRoutingService(PageEventPublisher publisher, GuidCounterService guidCounterService)
    {
        _publisher = publisher;
        _guidCounterService = guidCounterService;
    }

    public void Start()
    {
        if (_guidCounterService == null)
        {
            Debug.LogError("GuidCounterService is not found");
        }

        if (PageContainer.Instances.Count > 0)
        {
            _pageContainer = PageContainer.Instances[0];
            _pageContainer.AddCallbackReceiver(this);
        }
        else
        {
            Debug.LogError("PageContainer is not found");
        }
    }

    public void Dispose()
    {
        if (_pageContainer != null)
        {
            _pageContainer.RemoveCallbackReceiver(this);
        }

        _pageContainer = null;
    }

    // IPageContainerCallbackReceiver implementations
    public void BeforePush(Page enterPage, Page exitPage)
    {
        Debug.Log($"BeforePush: entering {enterPage?.Identifier}, exiting {exitPage?.Identifier}");
    }

    public void AfterPush(Page enterPage, Page exitPage)
    {
        Debug.Log($"AfterPush: entered {enterPage?.Identifier}, exited {exitPage?.Identifier}");
    }

    public void BeforePop(Page enterPage, Page exitPage)
    {
        Debug.Log($"BeforePop: entering {enterPage?.Identifier}, exiting {exitPage?.Identifier}");
    }

    public void AfterPop(Page enterPage, Page exitPage)
    {
        Debug.Log($"AfterPop: entered {enterPage?.Identifier}, exited {exitPage?.Identifier}");
    }

    public List<string> GetPageNames()
    {
        var pageNames = new List<string>();
        var pageIds = _pageContainer.OrderedPagesIds;
        for (int i = pageIds.Count - 1; i >= 0; i--)
        {
            var pageId = pageIds[i];
            var page = _pageContainer.Pages[pageId];
            pageNames.Add(page.Identifier);
        }
        return pageNames;
    }


    /// <summary>
    /// ページの位置を取得する
    /// ページが見つからなかった場合は-1を返す
    /// ページが見つかった場合は末尾から何個目かを返す(Popするときに使用)
    /// 該当ページが末尾の場合は0を返す
    /// </summary>
    /// <param name="pageName">ページ名</param>
    /// <returns>ページの位置(末尾から何個目か)</returns>
    public static int GetPageIdentifer(string pageName)
    {
        var pageIds = _pageContainer.OrderedPagesIds;
        for (int i = pageIds.Count - 1; i >= 0; i--)
        {
            var pageId = pageIds[i];
            var page = _pageContainer.Pages[pageId];
            if (page.Identifier == pageName)
            {
                return pageIds.Count - 1 - i; // 末尾から何個目か
            }
        }

        return -1; // 見つからなかった場合 -> Pushする
    }

    public void PushOrPopPage(string pageName)
    {
        var positionFromEnd = GetPageIdentifer(pageName);
        if (positionFromEnd >= 1) // 見つからなかった場合はPush
        {
            _pageContainer.Pop(true, positionFromEnd);
        }
        else if (positionFromEnd == 0)
        {
            return;
        }
        else
        {
            Debug.Log("Push: " + pageName);
            _pageContainer.Push(pageName, true, true, null, true, null);
        }
    }
}