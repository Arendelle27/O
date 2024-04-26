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
    //[SerializeField, LabelText("���׵ȼ��ı�"), Tooltip("��ʾ���׵ȼ��ı�")]
    //public Text transactionLevelText;

    [SerializeField, LabelText("�����б�"), Tooltip("����ͳ����б�")]
    public TabView tabView;

    [SerializeField, LabelText("������ȴ�غϰ�ť"), Tooltip("���������ȴ�غϰ�ť")]
    Button reduceCoolingRoundButton;

    [SerializeField, LabelText("������ȴ�غϰ�ť"), Tooltip("����������ȴ�غϰ�ť")]
    Button resetRoundButton;

    [SerializeField, LabelText("�رհ�ť"), Tooltip("����رհ�ť")]
    Button closeButton;

    [SerializeField, LabelText("������ȴ�غ������ı�"), Tooltip("��ʾ������ȴ�غ������ı�")]
    public Text reduceCoolingRoundBySpendText;

    [SerializeField, LabelText("��ѡ�еĽ�����ƷID"), ReadOnly]
    UITransactionItem transactionObjectSelected;//��ѡ�еĽ���UI����

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
                Debug.Log("δѡ���κ���Ʒ");
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
                Debug.Log("δѡ���κ���Ʒ");
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

    void UpdateReduceCoolingRoundBySpendText()
    {
        if(CapabilityManager.Instance.freelyReduceCoolingRound>0)
        {
            reduceCoolingRoundBySpendText.text = "���";
        }
        else
        {
            reduceCoolingRoundBySpendText.text = "50�ռ��";
        }
    }
}
