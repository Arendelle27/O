using ENTITY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteriousArea : EventArea
{
    public MysteriousArea(Plot plot) : base(plot)
    {
        Debug.Log("设施遗迹区域信息");
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
        base.WandererEnter();
        Debug.LogFormat("进入遗迹{0}", this.plot.pos);
    }
}
