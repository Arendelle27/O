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
    [SerializeField, LabelText("交易等级文本"), Tooltip("显示交易等级文本")]
    public Text transactionLevelText;

    [SerializeField, LabelText("交易列表"), Tooltip("购买和出售列表")]
    public TabView tabView;

    [SerializeField, LabelText("交易按钮"), Tooltip("交易按钮")]
    List<Button> TransactionButtons;
    //0为购买，1为出售,2为消耗资源刷新刷新，3为关闭

    [SerializeField, LabelText("减少冷却回合消耗文本"), Tooltip("显示减少冷却回合消耗文本")]
    public Text reduceCoolingRoundBySpendText;

    [SerializeField, LabelText("被选中的交易物品ID"), ReadOnly]
    UITransactionItem transactionObjectSelected;//被选中的建筑UI类型

    private void Start()
    {
        tabView.OnTabSelect = this.OnTabSelect;

        foreach (var listView in this.tabView.tabPages)
        {
            listView.onItemSelected += this.OnTransactionItemSelected;
        }

        this.TransactionButtons[0].OnClickAsObservable().Subscribe(_ =>
        {
            if(this.transactionObjectSelected ==null)
            {
                Debug.Log("未选中任何商品");
                return;
            }
            UITransactionAmountWindow uITransactionAmountWindow = UIManager.Instance.Show<UITransactionAmountWindow>();
            uITransactionAmountWindow.SetInfo(this.transactionObjectSelected.id,true);
        });
        this.TransactionButtons[1].OnClickAsObservable().Subscribe(_ =>
        {
            if (this.transactionObjectSelected == null)
            {
                Debug.Log("未选中任何商品");
                return;
            }

            UITransactionAmountWindow uITransactionAmountWindow = UIManager.Instance.Show<UITransactionAmountWindow>();
            uITransactionAmountWindow.SetInfo(this.transactionObjectSelected.id, false);
        });
        this.TransactionButtons[2].OnClickAsObservable().Subscribe(_ =>
        {
            if (EventAreaManager.Instance.ReduceCoolingRoundBySpend(this.transactionObjectSelected.id))
            {
                this.UpdateBuildingList(0);
                this.UpdateReduceCoolingRoundBySpendText();
            }
        });
        this.TransactionButtons[3].onClick.AddListener(() =>
        {
            this.OnCloseClick();
        });

    }

    void OnEnable()
    {
        this.transactionLevelText.text = CapabilityManager.Instance.curLevels[1].ToString();
        for (int i = 0;i<tabView.tabPages.Length;i++)
        {
            this.UpdateBuildingList(i);
        }
        this.UpdateReduceCoolingRoundBySpendText();
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
            reduceCoolingRoundBySpendText.text = "减少选中的商品一天的补货时间";
        }
        else
        {
            reduceCoolingRoundBySpendText.text = "花费50空间币减少选中的商品一天的补货时间";
        }
    }
}
