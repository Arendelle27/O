using ENTITY;
using MANAGER;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClashArea : EventArea
{
    //��ͻ����
    int clashType;
    //0Ϊ���࣬1Ϊ������

    //�Ƿ��������
    bool canExpend;

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
        this.clashType = int.Parse(plot.plotDefine.EventValue);
        this.canExpend = false;

        if (this.plot.wanderer != null)
        {
            this.WandererEnter();
        }
    }


    public override void WandererEnter()
    {
        Debug.LogFormat("�����ͻ����{0}",this.plot.pos);
        MessageManager.Instance.AddMessage(Message_Type.��ͻ, string.Format("��������{0}�ĳ�ͻ��", clashType == 0 ? "����" : "��������"));
        EventManager.Instance.SetConfrontEvent(this.clashType, EventAreaManager.Instance.hotility[this.clashType]+500f,this);//���ó�ͻ�¼�
    }

    /// <summary>
    /// �غϽ���
    /// </summary>
    public override void RoundOver()
    {
        if(!this.canExpend&&RoundManager.Instance.roundNumber>=3)
        {
            this.canExpend = true;
        }

        if(this.canExpend)
        {
            EventAreaManager.Instance.ExpendClashArea(this.plot.pos,this.clashType);//���ų�ͻ����
        }
    }
}
