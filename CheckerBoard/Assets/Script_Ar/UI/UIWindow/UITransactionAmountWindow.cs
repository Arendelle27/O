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
    [SerializeField, LabelText("物品名称"), Tooltip("放入物品名称文本")]
    public Text nameText;
    [SerializeField, LabelText("购买或出售"), Tooltip("购买或出售文本")]
    public Text purchaseOrSellText;
    [SerializeField, LabelText("数量输入框"), Tooltip("放入数量输入框")]
    public InputField amountText;
    [SerializeField, LabelText("花费或赚取"), Tooltip("花费或赚取文本")]
    public Text spendOrEarnText;
    [SerializeField, LabelText("价格"), Tooltip("放入价格文本")]
    public Text priceText;
    [SerializeField, LabelText("数量滑动条"), Tooltip("放入数量滑动条")]
    public Slider amountSlider;
    [SerializeField, LabelText("结果文本"), Tooltip("放入结果文本")]
    public Text resultText;
    [SerializeField, LabelText("按键"), Tooltip("放入按键")]
    public List<Button> buttons;
    //0为确认，1为取消
    [SerializeField, LabelText("单价"), ReadOnly]
    int singlePrice;

    [SerializeField, LabelText("数量"), ReadOnly]
    int amount;
    [SerializeField, LabelText("交易物品ID"), ReadOnly]
    int transactionObjecID;
    [SerializeField, LabelText("是否为购买"), ReadOnly]
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

        this.buttons[0].OnClickAsObservable().Subscribe(_ =>//确认
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

        this.buttons[1].OnClickAsObservable().Subscribe(_ =>//取消
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

        if (DataManager.TransactionDefines[settleSort][transactionObjecId].TransactionType == Transaction_Type.资源)
        {
            this.nameText.text = ((Resource_Type)DataManager.TransactionDefines[settleSort][transactionObjecId].Subtype).ToString();//资源类型

            this.singlePrice = DataManager.TransactionDefines[settleSort][transactionObjecId].Price;

        }
        else if (DataManager.TransactionDefines[settleSort][transactionObjecId].TransactionType == Transaction_Type.蓝图)
        {
            this.nameText.text = string.Format("蓝图{0}", DataManager.TransactionDefines[settleSort][transactionObjecId].Subtype);//蓝图

            this.singlePrice = DataManager.TransactionDefines[settleSort][transactionObjecId].Price;
        }

        if (isPurchase)
        {
            this.purchaseOrSellText.text = "购买数量：";
            this.spendOrEarnText.text = "花费：";

            int maxAmount = ResourcesManager.Instance.wealth/ this.singlePrice;
            if (maxAmount > EventAreaManager.Instance.transactionObjectsStatue[settleSort][transactionObjecId][0])
            {
                maxAmount = EventAreaManager.Instance.transactionObjectsStatue[settleSort][transactionObjecId][0];
            }
            this.amountSlider.maxValue = maxAmount;
        }
        else
        {
            this.purchaseOrSellText.text = "出售数量：";
            this.spendOrEarnText.text = "赚取：";

            this.amountSlider.maxValue = ResourcesManager.Instance.buildingResources[DataManager.TransactionDefines[settleSort][transactionObjecId].Subtype];
        }

        this.amount = 0;
    }
}
