using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MANAGER;
using System.Resources;

public class UIPersuadePanel : UIConfrontEventPanel
{
    [SerializeField, LabelText("劝说花费空间币显示和输入"), Tooltip("放入劝说花费空间币显示和输入文本")]
    public InputField persuadeCostCurrencyText;
    [SerializeField, LabelText("劝说花费空间币滑块"), Tooltip("放入劝说花费空间币滑块")]
    public Slider persuadeCostCurrencySlider;

    [SerializeField, LabelText("劝说降低敌意值奖励"), Tooltip("放入劝说降低敌意值奖励显示文本")]
    public Text persuadeSuccessSubtractHotility;

    [SerializeField, LabelText("劝说成功资源1奖励"), Tooltip("放入劝说成功资源1奖励显示文本")]
    public Text persuadeSuccessRewardResource1;

    [SerializeField, LabelText("劝说成功资源2奖励"), Tooltip("放入劝说成功资源2奖励显示文本")]
    public Text persuadeSuccessRewardResource2;

    [SerializeField, LabelText("劝说花费空间币"), ReadOnly]
    int persuadeCostCurrencyValue;


    protected override void Start()
    {
        base.Start();

        this.ObserveEveryValueChanged(_ => this.persuadeCostCurrencyValue)
    .Subscribe(_ =>
    {
        if (this.persuadeCostCurrencySlider.value != this.persuadeCostCurrencyValue)
        {
            this.persuadeCostCurrencySlider.value = this.persuadeCostCurrencyValue;
        }
        if (this.persuadeCostCurrencyText.text != this.persuadeCostCurrencyValue.ToString())
        {
            this.persuadeCostCurrencyText.text = this.persuadeCostCurrencyValue.ToString();
        }
        this.CalculateRates();
    });

        this.persuadeCostCurrencyText.OnValueChangedAsObservable().Subscribe(_ =>
        {
            int result = int.Parse(this.persuadeCostCurrencyText.text);

            if (result > this.persuadeCostCurrencySlider.maxValue)
            {
                result = (int)this.persuadeCostCurrencySlider.maxValue;
            }
            else if (result < 0)
            {
                result = 0;
            }

            if (result != this.persuadeCostCurrencyValue)
            {
                this.persuadeCostCurrencyValue = result;
            }

            this.persuadeCostCurrencyText.text = this.persuadeCostCurrencyValue.ToString();
        });

        this.persuadeCostCurrencySlider.OnValueChangedAsObservable().Subscribe(_ =>
        {
            this.persuadeCostCurrencyValue = (int)this.persuadeCostCurrencySlider.value;
        });

        this.insureButton.OnClickAsObservable().Subscribe(_ =>
        {
            if(!EventManager.Instance.Persudate(this.successRate,this.persuadeCostCurrencyValue))
            {
                int rate= EventManager.Instance.calculateRates(0);
                EventManager.Instance.Confront(rate);
            }
            UIManager.Instance.Close<UIConfrontEventWindow>();
        }).AddTo(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void CalculateRates()
    {
        base.CalculateRates();
        int rate=EventManager.Instance.calculateRates(1,this.persuadeCostCurrencyValue);
        if(this.successRate!=rate)
        {
            this.successRate=rate;
        }
    }

    protected override void SetInfo()
    {
        base.SetInfo();
        this.persuadeCostCurrencySlider.maxValue=ResourcesManager.Instance.wealth;
        this.persuadeCostCurrencyValue = 0;

        this.persuadeSuccessSubtractHotility.text= string.Format("{0}",this.curConfrontDefine.PersuadeRewardReduceHostilityDescription);

        if(this.curConfrontDefine.PersuadeRewardResource1Description != "")
        {
            this.persuadeSuccessRewardResource1.text = string.Format("{0}{1}",this.curConfrontDefine.PersuadeRewardResource1Description,Resource_Type.断裂电缆);
            this.persuadeSuccessRewardResource1.gameObject.SetActive(true);
        }
        else
        {
            this.persuadeSuccessRewardResource1.gameObject.SetActive(false);
        }

        if(this.curConfrontDefine.PersuadeRewardResource2Description != "")
        {
            this.persuadeSuccessRewardResource2.text = string.Format("{0}{1}",this.curConfrontDefine.PersuadeRewardResource2Description, Resource_Type.废弃金属);
            this.persuadeSuccessRewardResource2.gameObject.SetActive(true);
        }
        else
        {
            this.persuadeSuccessRewardResource2.gameObject.SetActive(false);
        }

    }
}
