using ENTITY;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


public class Settle : EventArea
{
    //���׶���
    public Dictionary<int, TransactionDefine> transactionDefines;

    [SerializeField, LabelText("�Ƿ�Ϊ����"), ReadOnly]
    public bool isBlackMarket = false;

    [SerializeField, LabelText("����ĵ���ֵ"), ReadOnly]
    public int hotility = 0;


    public Settle(Plot plot) : base(plot)
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
        if(plot.plotDefine.CanBuild)
        {
            this.isBlackMarket = false;//�ܽ�����Ǿ���
            this.transactionDefines = DataManager.TransactionDefines[0];
        }
        else
        {
            this.isBlackMarket = true;//���ܽ�����Ǻ���
            this.transactionDefines = DataManager.TransactionDefines[1];
        }

        this.hotility = 0;
    }

    /// <summary>
    /// �����߽���
    /// </summary>
    public override void WandererEnter()
    {
        if (this.isBlackMarket)
        {
            Debug.LogFormat("�������{0}", this.plot.pos);
        }
        else
        {
            Debug.LogFormat("�������{0}", this.plot.pos);
        }
    }

    /// <summary>
    /// ���ӵ���ֵ(�Ƿ�ͨ����������)
    /// </summary>
    public void AddHotility(int value)
    {
        this.hotility += value;
    }

}

