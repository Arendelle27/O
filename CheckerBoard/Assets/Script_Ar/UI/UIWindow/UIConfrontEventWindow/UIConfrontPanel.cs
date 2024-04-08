using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MANAGER;

public class UIConfrontPanel : UIConfrontEventPanel
{
    [SerializeField, LabelText("�ɹ��ռ�ҽ���"), Tooltip("����ɹ��ռ�ҽ�����ʾ�ı�")]
    public Text victoryRewardCurrency;
    [SerializeField, LabelText("�ɹ���Դ1����"), Tooltip("����ɹ���Դ1������ʾ�ı�")]
    public Text victoryRewardResource1;
    [SerializeField, LabelText("�ɹ���Դ2����"), Tooltip("����ɹ���Դ2������ʾ�ı�")]
    public Text victoryRewardResource2;
    [SerializeField, LabelText("�ɹ���Դ3����"), Tooltip("����ɹ���Դ3������ʾ�ı�")]
    public Text victoryRewardResource3;
    [SerializeField, LabelText("ʧ�ܿռ�ҳͷ�"), Tooltip("����ʧ�ܿռ�ҳͷ���ʾ�ı�")]
    public Text failurePenaltyCurrency;
    [SerializeField, LabelText("ʧ�ܻ�е�ͷ�"), Tooltip("����ʧ�ܻ�е�ͷ���ʾ�ı�")]
    public Text failurePenaltyBuilding;

    protected override void Start()
    {
        base.Start();
        this.insureButton.OnClickAsObservable().Subscribe(_ =>
        {
            EventManager.Instance.Confront(this.successRate);
            UIManager.Instance.Close<UIConfrontEventWindow>();
        });
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void CalculateRates()
    {
        base.CalculateRates();
        int rate=EventManager.Instance.calculateRates(0);
        if(this.successRate!=rate)
        {
            this.successRate=rate;
        }
    }

    protected override void SetInfo()
    {
        base.SetInfo();
        this.victoryRewardCurrency.text = string.Format("{0}�ռ��",this.curConfrontDefine.VictoryRewardCurrencyDescription);

        if(this.curConfrontDefine.VictoryRewardResource1Description!="")
        {
            this.victoryRewardResource1.text = string.Format("{0}{1}",this.curConfrontDefine.VictoryRewardResource1Description,Resource_Type.���ѵ���);
            this.victoryRewardResource1.gameObject.SetActive(true);
        }
        else
        {
            this.victoryRewardResource1.gameObject.SetActive(false);
        }

        if(this.curConfrontDefine.VictoryRewardResource2Description!="")
        {
            this.victoryRewardResource2.text = string.Format("{0}{1}",this.curConfrontDefine.VictoryRewardResource2Description,Resource_Type.��������);
            this.victoryRewardResource2.gameObject.SetActive(true);
        }
        else
        {
            this.victoryRewardResource2.gameObject.SetActive(false);
        }

        if(this.curConfrontDefine.VictoryRewardResource3Description!="")
        {
            this.victoryRewardResource3.text = string.Format("{0}{1}",this.curConfrontDefine.VictoryRewardResource3Description,Resource_Type.Ӱ��оƬ);
            this.victoryRewardResource3.gameObject.SetActive(true);
        }
        else
        {
            this.victoryRewardResource3.gameObject.SetActive(false);
        }

        this.failurePenaltyCurrency.text = string.Format("��ʧ{0}�ռ��",this.curConfrontDefine.FailurePenaltyCurrencyDescription);

        if(this.curConfrontDefine.FailurePenaltyBuildingDescription!="")
        {
            this.failurePenaltyBuilding.text = string.Format("�ݻ�{0}",this.curConfrontDefine.FailurePenaltyBuildingDescription);
            this.failurePenaltyBuilding.gameObject.SetActive(true);
        }
        else
        {
            this.failurePenaltyBuilding.gameObject.SetActive(false);
        }
    }
}
