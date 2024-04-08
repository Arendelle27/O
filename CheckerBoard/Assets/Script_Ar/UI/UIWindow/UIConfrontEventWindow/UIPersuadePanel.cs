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
    [SerializeField, LabelText("Ȱ˵���ѿռ����ʾ������"), Tooltip("����Ȱ˵���ѿռ����ʾ�������ı�")]
    public InputField persuadeCostCurrencyText;
    [SerializeField, LabelText("Ȱ˵���ѿռ�һ���"), Tooltip("����Ȱ˵���ѿռ�һ���")]
    public Slider persuadeCostCurrencySlider;

    [SerializeField, LabelText("Ȱ˵���͵���ֵ����"), Tooltip("����Ȱ˵���͵���ֵ������ʾ�ı�")]
    public Text persuadeSuccessSubtractHotility;

    [SerializeField, LabelText("Ȱ˵�ɹ���Դ1����"), Tooltip("����Ȱ˵�ɹ���Դ1������ʾ�ı�")]
    public Text persuadeSuccessRewardResource1;

    [SerializeField, LabelText("Ȱ˵�ɹ���Դ2����"), Tooltip("����Ȱ˵�ɹ���Դ2������ʾ�ı�")]
    public Text persuadeSuccessRewardResource2;

    [SerializeField, LabelText("Ȱ˵���ѿռ��"), ReadOnly]
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
            this.persuadeSuccessRewardResource1.text = string.Format("{0}{1}",this.curConfrontDefine.PersuadeRewardResource1Description,Resource_Type.���ѵ���);
            this.persuadeSuccessRewardResource1.gameObject.SetActive(true);
        }
        else
        {
            this.persuadeSuccessRewardResource1.gameObject.SetActive(false);
        }

        if(this.curConfrontDefine.PersuadeRewardResource2Description != "")
        {
            this.persuadeSuccessRewardResource2.text = string.Format("{0}{1}",this.curConfrontDefine.PersuadeRewardResource2Description, Resource_Type.��������);
            this.persuadeSuccessRewardResource2.gameObject.SetActive(true);
        }
        else
        {
            this.persuadeSuccessRewardResource2.gameObject.SetActive(false);
        }

    }
}
