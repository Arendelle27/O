using ENTITY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueArea : EventArea
{

    public ClueArea(Plot plot):base(plot)
    {
        Debug.Log("设施线索区域信息");
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
        Debug.LogFormat("进入剧情区域{0}", this.plot.pos);
    }
}
