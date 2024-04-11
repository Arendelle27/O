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
    [SerializeField, LabelText("���׵ȼ��ı�"), Tooltip("��ʾ���׵ȼ��ı�")]
    public Text transactionLevelText;

    [SerializeField, LabelText("�����б�"), Tooltip("����ͳ����б�")]
    public TabView tabView;

    [SerializeField, LabelText("���װ�ť"), Tooltip("���װ�ť")]
    List<Button> TransactionButtons;
    //0Ϊ����1Ϊ����,2Ϊ������Դˢ��ˢ�£�3Ϊ�ر�

    [SerializeField, LabelText("������ȴ�غ������ı�"), Tooltip("��ʾ������ȴ�غ������ı�")]
    public Text reduceCoolingRoundBySpendText;

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
            reduceCoolingRoundBySpendText.text = "����ѡ�е���Ʒһ��Ĳ���ʱ��";
        }
        else
        {
            reduceCoolingRoundBySpendText.text = "����50�ռ�Ҽ���ѡ�е���Ʒһ��Ĳ���ʱ��";
        }
    }
}
