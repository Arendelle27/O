using ENTITY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClashArea : EventArea
{
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

    }

    public override void WandererEnter()
    {
        Debug.LogFormat("进入冲突区域{0}",this.plot.pos);
    }
}
