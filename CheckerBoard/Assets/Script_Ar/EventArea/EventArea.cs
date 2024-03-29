using ENTITY;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventArea
{
    [SerializeField, LabelText("事件地区的板块"), ReadOnly]
    public Plot plot;

    public EventArea(Plot plot)
    {
        this.SetInfo(plot);
    }

    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="plot"></param>
    public virtual void SetInfo(Plot plot)
    {
        this.plot = plot;
    }

    /// <summary>
    /// 流浪者进入
    /// </summary>
    public virtual void WandererEnter()
    {

    }
}
