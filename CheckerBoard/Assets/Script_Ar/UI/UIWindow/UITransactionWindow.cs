using MANAGER;
using Managers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using UILIST;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UITransactionWindow : UIWindow
{
    [SerializeField, LabelText("�����б�"), Tooltip("����ͳ����б�")]
    public TabView tabView;

    [SerializeField, LabelText("���װ�ť"), Tooltip("���װ�ť")]
    List<Button> TransactionButtons;
    //0Ϊ����1Ϊ����,2Ϊ������Դˢ��ˢ�£�3Ϊ�ر�

    [SerializeField, LabelText("��ѡ�еĽ�����ƷID"), ReadOnly]
    UITransactionItem transactionObjectSelected;//��ѡ�еĽ���UI����

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
                Debug.Log("δѡ���κ���Ʒ");
                return;
            }
            UITransactionAmountWindow uITransactionAmountWindow = UIManager.Instance.Show<UITransactionAmountWindow>();
            uITransactionAmountWindow.SetInfo(this.transactionObjectSelected.id,true);
        });
        this.TransactionButtons[1].OnClickAsObservable().Subscribe(_ =>
        {
            if (this.transactionObjectSelected == null)
            {
                Debug.Log("δѡ���κ���Ʒ");
                return;
            }

            UITransactionAmountWindow uITransactionAmountWindow = UIManager.Instance.Show<UITransactionAmountWindow>();
            uITransactionAmountWindow.SetInfo(this.transactionObjectSelected.id, false);
        });
        this.TransactionButtons[2].onClick.AddListener(() =>
        {
            if (EventAreaManager.Instance.ReduceCoolingRoundBySpend(this.transactionObjectSelected.id))
            {
                this.UpdateBuildingList(0);
            }
        });
        this.TransactionButtons[3].onClick.AddListener(() =>
        {
            this.OnCloseClick();
        });

    }

    void OnEnable()
    {
        for(int i = 0;i<tabView.tabPages.Length;i++)
        {
            this.UpdateBuildingList(i);
        }
    }

    /// <summary>
    /// �����л�����
    /// </summary>
    /// <param name="index"></param>
    void OnTabSelect(int index)
    {
        this.transactionObjectSelected = null;
    }

    /// <summary>
    /// ѡ����Ҫ������߳��۵���Ʒ
    /// </summary>
    /// <param name="item"></param>
    public void OnTransactionItemSelected(ListView.ListViewItem item)
    {
        UITransactionItem transactionItem = item as UITransactionItem;
        this.transactionObjectSelected = transactionItem;
    }

    /// <summary>
    /// ��ս����б�UI
    /// </summary>
    void ClearBuildingList(int sort)
    {
        this.tabView.tabPages[sort].RemoveAll();
    }

    /// <summary>
    /// ��ʼ����Ʒ�б�UI
    /// </summary>
    public void UpdateBuildingList(int sort)//0Ϊ����1Ϊ����
    {
        ClearBuildingList(sort);
        foreach (var tD in (EventAreaManager.Instance.selectedEventArea as Settle).transactionDefines.Values)
        {
            GameObject gO;
            if(tD.PurchaseOrSell==sort)
            {
                gO= (sort==0)?GameObjectPool.Instance.UICommodityItems.Get(): GameObjectPool.Instance.UIGoodItems.Get();
                gO.transform.SetParent(this.tabView.tabPages[sort].content);//�ڽ����б��һҳ����
                var ui = gO.GetComponent<UITransactionItem>();
                ui.SetInfo(tD);//���ý���UIItem��Ϣ
                this.tabView.tabPages[sort].AddItem(ui);
            }
        }
    }
}
