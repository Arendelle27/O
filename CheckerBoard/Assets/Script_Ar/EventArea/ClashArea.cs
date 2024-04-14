using ENTITY;
using MANAGER;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClashArea : EventArea
{
    //冲突类型
    int clashType;
    //0为人类，1为机器人

    //是否可以扩张
    bool canExpend;

    public ClashArea(Plot plot):base(plot)
    {
        Debug.Log("设施冲突区域信息");
    }

    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="plot"></param>
    public override void SetInfo(Plot plot)
    {
        base.SetInfo(plot);
        this.clashType = int.Parse(plot.plotDefine.EventValue);
        this.canExpend = false;

        if (this.plot.wanderer != null)
        {
            this.WandererEnter();
        }
    }


    public override void WandererEnter()
    {
        Debug.LogFormat("进入冲突区域{0}",this.plot.pos);
        MessageManager.Instance.AddMessage(Message_Type.冲突, string.Format("被卷入了{0}的冲突中", clashType == 0 ? "人们" : "机器人们"));
        EventManager.Instance.SetConfrontEvent(this.clashType, EventAreaManager.Instance.hotility[this.clashType]+500f,this);//设置冲突事件
    }

    /// <summary>
    /// 回合结束
    /// </summary>
    public override void RoundOver()
    {
        if(!this.canExpend&&RoundManager.Instance.roundNumber>=3)
        {
            this.canExpend = true;
        }

        if(this.canExpend)
        {
            EventAreaManager.Instance.ExpendClashArea(this.plot.pos,this.clashType);//扩张冲突区域
        }
    }
}
