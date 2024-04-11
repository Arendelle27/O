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

    [SerializeField, LabelText("商品状态"), Tooltip("放入商品状态文本")]
    public Text stateText;

    public override void SetInfo(TransactionDefine tD)
    {
        base.SetInfo(tD);
        this.priceText.text = CapabilityManager.Instance.TransactionPrice(tD, true).ToString();

        if (!tD.IsBlackMarket)
        {
            this.countText.text= EventAreaManager.Instance.purchaseObjectsStatue[0][tD.ID][0].ToString();

            if (tD.TransactionType == Transaction_Type.资源)//资源类
            {
                int cT = EventAreaManager.Instance.purchaseObjectsStatue[0][tD.ID][1];//获取冷却时间
                if (cT > 0)
                {
                    this.stateText.text = string.Format("{0}天后补满", cT);
                }
                else
                {
                    this.stateText.text = "有货";
                }
            }
        }
        else
        {
            this.countText.text = EventAreaManager.Instance.purchaseObjectsStatue[1][tD.ID][0].ToString();
            if (tD.TransactionType == Transaction_Type.资源)//资源类
            {
                int cT = EventAreaManager.Instance.purchaseObjectsStatue[1][tD.ID][1];//获取冷却时间
                if (cT > 0)
                {
                    this.stateText.text = string.Format("{0}天后补满", cT);
                }
                else
                {
                    this.stateText.text = "有货";
                }
            }
            else
            {
                if(EventAreaManager.Instance.purchaseObjectsStatue[1][tD.ID][0] != 0)
                {
                    this.stateText.text = "可售";
                }
                else
                {
                    this.stateText.text = "已售";
                }

            }
        }
    }

}
