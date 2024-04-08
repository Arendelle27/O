using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class UITransactionAmountWindow : UIWindow
{
    [SerializeField, LabelText("��Ʒ����"), Tooltip("������Ʒ�����ı�")]
    public Text nameText;
    [SerializeField, LabelText("��������"), Tooltip("���������ı�")]
    public Text purchaseOrSellText;
    [SerializeField, LabelText("���������"), Tooltip("�������������")]
    public InputField amountText;
    [SerializeField, LabelText("���ѻ�׬ȡ"), Tooltip("���ѻ�׬ȡ�ı�")]
    public Text spendOrEarnText;
    [SerializeField, LabelText("�۸�"), Tooltip("����۸��ı�")]
    public Text priceText;
    [SerializeField, LabelText("����������"), Tooltip("��������������")]
    public Slider amountSlider;
    [SerializeField, LabelText("����ı�"), Tooltip("�������ı�")]
    public Text resultText;
    [SerializeField, LabelText("����"), Tooltip("���밴��")]
    public List<Button> buttons;
    //0Ϊȷ�ϣ�1Ϊȡ��
    [SerializeField, LabelText("����"), ReadOnly]
    int singlePrice;

    [SerializeField, LabelText("����"), ReadOnly]
    int amount;
    [SerializeField, LabelText("������ƷID"), ReadOnly]
    int transactionObjecID;
    [SerializeField, LabelText("�Ƿ�Ϊ����"), ReadOnly]
    bool isPurchase;

    private void Start()
    {
        this.ObserveEveryValueChanged(_=>this.amount)
            .Subscribe(_ =>
            {
                if(this.amountSlider.value != this.amount)
                {
                    this.amountSlider.value = this.amount;
                }
                if(this.amountText.text != this.amount.ToString())
                {
                    this.amountText.text = this.amount.ToString();
                }

                int total = this.amount * this.singlePrice;
                this.priceText.text = total.ToString();
            });

        this.amountText.OnValueChangedAsObservable().Subscribe(_ =>
        {
            int result = int.Parse(this.amountText.text);

            if (result > this.amountSlider.maxValue)
            {
                result = (int)this.amountSlider.maxValue;
            }
            else if (result < 0)
            {
                result = 0;
            }

            if (result != this.amount)
            {
                this.amount = result;
            }

            this.amountText.text = this.amount.ToString();
        });

        this.amountSlider.OnValueChangedAsObservable().Subscribe(_ =>
        {
            this.amount = (int)this.amountSlider.value;
        });

        this.buttons[0].OnClickAsObservable().Subscribe(_ =>//ȷ��
        {
            if(this.amount!=0)
            {
                if(this.isPurchase)
                {
                    EventManager.Instance.Purchase(this.transactionObjecID, this.amount);
                }
                else
                {
                    EventManager.Instance.Sell(this.transactionObjecID, this.amount);
                }
            }
            this.OnCloseClick();
        });

        this.buttons[1].OnClickAsObservable().Subscribe(_ =>//ȡ��
        {
            this.OnCloseClick();
        });
    }

    public void SetInfo(int transactionObjecId,bool isPurchase)
    {
        this.transactionObjecID = transactionObjecId;
        this.isPurchase = isPurchase;
        Settle eA = EventAreaManager.Instance.selectedEventArea as Settle;
        int settleSort = 0;
        if (eA.isBlackMarket)
        {
            settleSort = 1;
        }
        else
        {
            settleSort = 0;
        }

        if (DataManager.TransactionDefines[settleSort][transactionObjecId].TransactionType == Transaction_Type.��Դ)
        {
            this.nameText.text = ((Resource_Type)DataManager.TransactionDefines[settleSort][transactionObjecId].Subtype).ToString();//��Դ����

            this.singlePrice = DataManager.TransactionDefines[settleSort][transactionObjecId].Price;

        }
        else if (DataManager.TransactionDefines[settleSort][transactionObjecId].TransactionType == Transaction_Type.��ͼ)
        {
            this.nameText.text = string.Format("��ͼ{0}", DataManager.TransactionDefines[settleSort][transactionObjecId].Subtype);//��ͼ

            this.singlePrice = DataManager.TransactionDefines[settleSort][transactionObjecId].Price;
        }

        if (isPurchase)
        {
            this.purchaseOrSellText.text = "����������";
            this.spendOrEarnText.text = "���ѣ�";

            int maxAmount = ResourcesManager.Instance.wealth/ this.singlePrice;
            if (maxAmount > EventAreaManager.Instance.transactionObjectsStatue[settleSort][transactionObjecId][0])
            {
                maxAmount = EventAreaManager.Instance.transactionObjectsStatue[settleSort][transactionObjecId][0];
            }
            this.amountSlider.maxValue = maxAmount;
        }
        else
        {
            this.purchaseOrSellText.text = "����������";
            this.spendOrEarnText.text = "׬ȡ��";

            this.amountSlider.maxValue = ResourcesManager.Instance.buildingResources[DataManager.TransactionDefines[settleSort][transactionObjecId].Subtype];
        }

        this.amount = 0;
    }
}
