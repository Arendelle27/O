using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MANAGER;

public class UIConfrontPanel : UIConfrontEventPanel
{
    [SerializeField, LabelText("成功空间币奖励"), Tooltip("放入成功空间币奖励显示文本")]
    public Text victoryRewardCurrency;
    [SerializeField, LabelText("成功资源1奖励"), Tooltip("放入成功资源1奖励显示文本")]
    public Text victoryRewardResource1;
    [SerializeField, LabelText("成功资源2奖励"), Tooltip("放入成功资源2奖励显示文本")]
    public Text victoryRewardResource2;
    [SerializeField, LabelText("成功资源3奖励"), Tooltip("放入成功资源3奖励显示文本")]
    public Text victoryRewardResource3;
    [SerializeField, LabelText("失败空间币惩罚"), Tooltip("放入失败空间币惩罚显示文本")]
    public Text failurePenaltyCurrency;
    [SerializeField, LabelText("失败机械惩罚"), Tooltip("放入失败机械惩罚显示文本")]
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
        this.victoryRewardCurrency.text = string.Format("{0}空间币",this.curConfrontDefine.VictoryRewardCurrencyDescription);

        if(this.curConfrontDefine.VictoryRewardResource1Description!="")
        {
            this.victoryRewardResource1.text = string.Format("{0}{1}",this.curConfrontDefine.VictoryRewardResource1Description,Resource_Type.断裂电缆);
            this.victoryRewardResource1.gameObject.SetActive(true);
        }
        else
        {
            this.victoryRewardResource1.gameObject.SetActive(false);
        }

        if(this.curConfrontDefine.VictoryRewardResource2Description!="")
        {
            this.victoryRewardResource2.text = string.Format("{0}{1}",this.curConfrontDefine.VictoryRewardResource2Description,Resource_Type.废弃金属);
            this.victoryRewardResource2.gameObject.SetActive(true);
        }
        else
        {
            this.victoryRewardResource2.gameObject.SetActive(false);
        }

        if(this.curConfrontDefine.VictoryRewardResource3Description!="")
        {
            this.victoryRewardResource3.text = string.Format("{0}{1}",this.curConfrontDefine.VictoryRewardResource3Description,Resource_Type.影像芯片);
            this.victoryRewardResource3.gameObject.SetActive(true);
        }
        else
        {
            this.victoryRewardResource3.gameObject.SetActive(false);
        }

        this.failurePenaltyCurrency.text = string.Format("损失{0}空间币",this.curConfrontDefine.FailurePenaltyCurrencyDescription);

        if(this.curConfrontDefine.FailurePenaltyBuildingDescription!="")
        {
            this.failurePenaltyBuilding.text = string.Format("摧毁{0}",this.curConfrontDefine.FailurePenaltyBuildingDescription);
            this.failurePenaltyBuilding.gameObject.SetActive(true);
        }
        else
        {
            this.failurePenaltyBuilding.gameObject.SetActive(false);
        }
    }
}
