using ENTITY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClashArea : EventArea
{
    public ClashArea(Plot plot):base(plot)
    {
        Debug.Log("��ʩ��ͻ������Ϣ");
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
        Debug.LogFormat("�����ͻ����{0}",this.plot.pos);
    }
}
