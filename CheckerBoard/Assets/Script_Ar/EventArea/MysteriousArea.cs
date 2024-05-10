using ENTITY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteriousArea : EventArea
{
    public MysteriousArea(Plot plot) : base(plot)
    {
        Debug.Log("��ʩ�ż�������Ϣ");
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
        Debug.LogFormat("�����ż�{0}", this.plot.pos);
    }
}
