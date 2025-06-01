using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 仮で作った素朴なサービス
/// インスタンスの同一性を検証できるので、VContainerのConfigure挙動に使うと良い
/// この規模ならServiceとつけてUseCase扱いしなくていい
/// </summary>
public class GuidCounterService
{
    public int GetGuid
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

    private int guid;
}
