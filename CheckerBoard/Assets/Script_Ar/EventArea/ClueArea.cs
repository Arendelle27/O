using ENTITY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueArea : EventArea
{

    public ClueArea(Plot plot):base(plot)
    {
        Debug.Log("��ʩ����������Ϣ");
    }
    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="plot"></param>
    public override void SetInfo(Plot plot)
    {
        base.SetInfo(plot);
    }

    public override void WandererEnter()
    {
        base.WandererEnter();
        Debug.LogFormat("�����������{0}", this.plot.pos);
    }
}
