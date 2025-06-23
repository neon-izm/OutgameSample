using UnityEngine;
using UnityEngine.SceneManagement;
using UnityScreenNavigator.Runtime.Core.Page;
using VContainer.Unity;
using System;
using ScreenSystem.Page;
using System.Collections.Generic;
using System.Linq;
using ScreenSystem.Attributes;
using UnityEngine.Assertions;

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

    public bool IsTopPage(IPageBuilder pageBuilder, out Page topPage)
    {
        var logTopPage = GetPages().LastOrDefault();
        Assert.IsFalse(logTopPage == null || logTopPage.Identifier == GetAssetName(pageBuilder));
        topPage = logTopPage;
        return logTopPage.Identifier == GetAssetName(pageBuilder);
    }

    /// <summary>
    /// ページのリストをスタック順に取得する
    /// </summary>
    /// <returns>ページのリスト</returns>
    public List<Page> GetPages()
    {
        var ret = _pageContainer.OrderedPagesIds.Select(id => _pageContainer.Pages[id]).ToList();
        return ret;
    }

    /// <summary>
    /// ページのIDを取得する.
    /// UsePrefabName as Idの場合はページのPrefabNameを返す.
    /// </summary>
    /// <returns>ページのID</returns>
    public List<string> GetPageIds()
    {
        return GetPages().Select(page => page.Identifier).ToList();
    }
    
    private string GetAssetName( IPageBuilder pageBuilder)
    {
        var lifecyclePageType = pageBuilder.GetType().BaseType.GetGenericArguments().FirstOrDefault(genericType => genericType.IsSubclassOf(typeof(LifecyclePageBase)));
        var nameAttribute = Attribute.GetCustomAttribute(lifecyclePageType, typeof(AssetNameAttribute)) as AssetNameAttribute;
        return nameAttribute?.PrefabName;
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

    /// <summary>
    /// ページをPopする.
    /// ページが見つからなかった場合は何もしない.
    /// </summary>
    public void PopPage(){
        if(_pageContainer.IsInTransition || _pageContainer.OrderedPagesIds.Count <= 1){
            return;
        }
        _publisher.SendPopEvent();
    }

    /// <summary>
    /// ページをPushする.すでにそのページがある場合はそこまでPopして戻る
    /// 現在のページがそのページの場合は何もしない.
    /// </summary>
    public void PushOrPopPage(IPageBuilder pageBuilder)
    {
        if(_pageContainer.IsInTransition){
            return;
        }
        var assetName = GetAssetName(pageBuilder);
        if(assetName == null){
            return;
        }
        var positionFromEnd = GetPageIdentifer(assetName);
        if (positionFromEnd >= 1) // 見つからなかった場合はPush
        {
            for(int i = positionFromEnd; 0<i; i--){
                _publisher.SendPopEvent();
            }
        }
        else if (positionFromEnd == 0)
        {
            return;
        }
        else
        {
            Debug.Log("Push: " + pageBuilder.ToString());
            _publisher.SendPushEvent(pageBuilder);
        }
    }
}