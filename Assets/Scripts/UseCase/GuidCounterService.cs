using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

/// <summary>
/// 仮で作った素朴なサービス
/// インスタンスの同一性を検証できるので、VContainerのConfigure挙動に使うと良い
/// この規模ならServiceとつけてUseCase扱いしなくていい
/// </summary>
public class GuidCounterService : IStartable
{
    private int guid = 0;

    public int GuidInt
    {
        get
        {
            if (guid == 0)
            {
                guid = Guid.NewGuid().GetHashCode();
            }

            return guid;
        }
    }

    public string GetGuidString()
    {
        return Guid.NewGuid().ToString();
    }

    /// <summary>
    /// VContainerのIStartableインターフェースを実装
    /// MonoBehaviourのStart()メソッドに相当する
    /// </summary>
    public void Start()
    {
        Debug.Log($"GuidCounterService Start guid:{GuidInt}");
    }
}
