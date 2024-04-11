using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UILIST;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UICommodityItem : UITransactionItem
{

    [SerializeField, LabelText("��Ʒ״̬"), Tooltip("������Ʒ״̬�ı�")]
    public Text stateText;

    public override void SetInfo(TransactionDefine tD)
    {
        base.SetInfo(tD);
        this.priceText.text = CapabilityManager.Instance.TransactionPrice(tD, true).ToString();

        if (!tD.IsBlackMarket)
        {
            this.countText.text= EventAreaManager.Instance.purchaseObjectsStatue[0][tD.ID][0].ToString();

            if (tD.TransactionType == Transaction_Type.��Դ)//��Դ��
            {
                int cT = EventAreaManager.Instance.purchaseObjectsStatue[0][tD.ID][1];//��ȡ��ȴʱ��
                if (cT > 0)
                {
                    this.stateText.text = string.Format("{0}�����", cT);
                }
                else
                {
                    this.stateText.text = "�л�";
                }
            }
        }
        else
        {
            this.countText.text = EventAreaManager.Instance.purchaseObjectsStatue[1][tD.ID][0].ToString();
            if (tD.TransactionType == Transaction_Type.��Դ)//��Դ��
            {
                int cT = EventAreaManager.Instance.purchaseObjectsStatue[1][tD.ID][1];//��ȡ��ȴʱ��
                if (cT > 0)
                {
                    this.stateText.text = string.Format("{0}�����", cT);
                }
                else
                {
                    this.stateText.text = "�л�";
                }
            }
            else
            {
                if(EventAreaManager.Instance.purchaseObjectsStatue[1][tD.ID][0] != 0)
                {
                    this.stateText.text = "����";
                }
                else
                {
                    this.stateText.text = "����";
                }

            }
        }
    }

}
