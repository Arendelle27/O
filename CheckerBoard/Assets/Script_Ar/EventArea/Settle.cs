using ENTITY;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


public class Settle : EventArea
{
    //交易定义
    public Dictionary<int, TransactionDefine> transactionDefines;

    //是否为黑市
    public bool isBlackMarket = false;


    public Settle(Plot plot) : base(plot)
    {
        Debug.Log("设施聚落区域信息");
    }


    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="plot"></param>
    public override void SetInfo(Plot plot)
    {
        base.SetInfo(plot);
        if(plot.plotDefine.CanBuild)
        {
            this.isBlackMarket = false;//能建设的是聚落
            this.transactionDefines = DataManager.TransactionDefines[0];
        }
        else
        {
            this.isBlackMarket = true;//不能建设的是黑市
            this.transactionDefines = DataManager.TransactionDefines[1];
        }
    }

    /// <summary>
    /// 流浪者进入
    /// </summary>
    public override void WandererEnter()
    {
        base.WandererEnter();
        if (this.isBlackMarket)
        {
            Debug.LogFormat("进入黑市{0}", this.plot.pos);
        }
        else
        {
            Debug.LogFormat("进入聚落{0}", this.plot.pos);
        }
    }

}

