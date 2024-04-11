using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UILIST;
using UnityEngine;

public class UIGoodItem : UITransactionItem
{
    public override void SetInfo(TransactionDefine tD)
    {
        base.SetInfo(tD);
        this.priceText.text = CapabilityManager.Instance.TransactionPrice(tD,false).ToString();

        if (!tD.IsBlackMarket)
        {
            this.countText.text = EventAreaManager.Instance.sellObjectsStatue[0][tD.ID].ToString();
        }
        else
        {
            this.countText.text = EventAreaManager.Instance.sellObjectsStatue[1][tD.ID].ToString();
        }
    }
}
