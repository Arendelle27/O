using ENTITY;
using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventArea
{
    [SerializeField, LabelText("�¼������İ��"), ReadOnly]
    public Plot plot;

    public EventArea(Plot plot)
    {
        this.SetInfo(plot);
    }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="plot"></param>
    public virtual void SetInfo(Plot plot)
    {
        this.plot = plot;
    }

    /// <summary>
    /// �����߽���
    /// </summary>
    public virtual void WandererEnter()
    {
        NpcManager.Instance.NPCAppearUnlock(1, this.plot.plotDefine.ID);
        ChatManager.Instance.ChatConditionUnlock(2, this.plot.plotDefine.ID);//��ӶԻ�
    }

    /// <summary>
    /// �غϽ���
    /// </summary>
    public virtual void RoundOver()
    {

    }
}
