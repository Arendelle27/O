using MANAGER;
using Managers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UILIST;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UITransactionWindow : UIWindow
{
    //[SerializeField, LabelText("交易等级文本"), Tooltip("显示交易等级文本")]
    //public Text transactionLevelText;

    [SerializeField, LabelText("交易列表"), Tooltip("购买和出售列表")]
    public TabView tabView;

    [SerializeField, LabelText("减少冷却回合按钮"), Tooltip("放入减少冷却回合按钮")]
    Button reduceCoolingRoundButton;

    [SerializeField, LabelText("重置冷却回合按钮"), Tooltip("放入重置冷却回合按钮")]
    Button resetRoundButton;

    [SerializeField, LabelText("关闭按钮"), Tooltip("放入关闭按钮")]
    Button closeButton;

    [SerializeField, LabelText("减少冷却回合消耗文本"), Tooltip("显示减少冷却回合消耗文本")]
    public Text reduceCoolingRoundBySpendText;

    [SerializeField, LabelText("被选中的交易物品ID"), ReadOnly]
    UITransactionItem transactionObjectSelected;//被选中的建筑UI类型

    private void Start()
    {
        tabView.OnTabSelect = this.OnTabSelect;

        for(int i=0;i<tabView.tabPages.Length;i++)
        {
            tabView.tabPages[i].onItemSelected += this.OnTransactionItemSelected;
        }
        //this.tabView.tabPages[0].onItemSelected += this.OnTransactionItemSelected;

        this.reduceCoolingRoundButton.OnClickAsObservable().Subscribe(_ =>
        {
            if(this.transactionObjectSelected == null)
            {
                Debug.Log("未选中任何商品");
                return;
            }

            UICommodityItem uIcommodityItem = this.transactionObjectSelected as UICommodityItem;
            if(uIcommodityItem==null)
            {
                return;
            }

            if (EventAreaManager.Instance.ReduceCoolingRoundBySpend(uIcommodityItem.id))
            {
                this.UpdateBuildingList(0);
                this.UpdateReduceCoolingRoundBySpendText();
            }
        });

        this.resetRoundButton.OnClickAsObservable().Subscribe(_ =>
        {
            if (this.transactionObjectSelected == null)
            {
                Debug.Log("未选中任何商品");
                return;
            }

            UIGoodItem uIGoodItem = this.transactionObjectSelected as UIGoodItem;
            if (uIGoodItem == null)
            {
                return;
            }

            if (EventAreaManager.Instance.ResetCoolingRoundBySpend(uIGoodItem.id))
            {
                this.UpdateBuildingList(1);
                this.UpdateReduceCoolingRoundBySpendText();
            }
        });

        this.closeButton.OnClickAsObservable().Subscribe(_ =>
        {
            this.OnCloseClick();
        });

    }

    void OnEnable()
    {
        //this.transactionLevelText.text = CapabilityManager.Instance.curLevels[1].ToString();
        for (int i = 0;i<tabView.tabPages.Length;i++)
        {
            this.UpdateBuildingList(i);
        }
    }

    /// <summary>
    /// 按下切换按键
    /// </summary>
    /// <param name="index"></param>
    void OnTabSelect(int index)
    {
        this.transactionObjectSelected = null;
    }

    /// <summary>
    /// 选中想要购买或者出售的商品
    /// </summary>
    /// <param name="item"></param>
    public void OnTransactionItemSelected(ListView.ListViewItem item)
    {
        UITransactionItem transactionItem = item as UITransactionItem;
        this.transactionObjectSelected = transactionItem;
    }

    /// <summary>
    /// 清空建筑列表UI
    /// </summary>
    void ClearBuildingList(int sort)
    {
        this.tabView.tabPages[sort].RemoveAll();
    }

    /// <summary>
    /// 初始化商品列表UI
    /// </summary>
    public void UpdateBuildingList(int sort)//0为购买，1为出售
    {
        ClearBuildingList(sort);
        foreach (var tD in (EventAreaManager.Instance.selectedEventArea as Settle).transactionDefines.Values)
        {
            GameObject gO;
            if(tD.PurchaseOrSell==sort)
            {
                gO= (sort==0)?GameObjectPool.Instance.UICommodityItems.Get(): GameObjectPool.Instance.UIGoodItems.Get();
                gO.transform.SetParent(this.tabView.tabPages[sort].content);//在建筑列表第一页生成
                var ui = gO.GetComponent<UITransactionItem>();
                ui.SetInfo(tD);//设置建筑UIItem信息
                this.tabView.tabPages[sort].AddItem(ui);
            }
        }
    }

    void UpdateReduceCoolingRoundBySpendText()
    {
        if(CapabilityManager.Instance.freelyReduceCoolingRound>0)
        {
            reduceCoolingRoundBySpendText.text = "免费";
        }
        else
        {
            reduceCoolingRoundBySpendText.text = "50空间币";
        }
    }
}
