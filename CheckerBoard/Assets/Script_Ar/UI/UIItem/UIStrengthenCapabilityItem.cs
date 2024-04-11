using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UILIST;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIStrengthenCapabilityItem : ListView.ListViewItem
{
    [SerializeField, LabelText("升级选项标题文本"), Tooltip("显示升级选项标题文本")]
    public Text UIStrengthenCapabilityItemTitleText;
    [SerializeField, LabelText("能力等级文本"), Tooltip("显示能力等级文本")]
    public Text LevelText;
    [SerializeField, LabelText("当前能力文本"), Tooltip("显示当前能力文本")]
    public Text curCapabilityText;
    [SerializeField, LabelText("升级后能力文本"), Tooltip("显示升级后能力文本")]
    public Text nextCapabilityText;
    [SerializeField, LabelText("升级按键"), Tooltip("升级按键")]
    public Button upgradeButton;
    [SerializeField, LabelText("升级需要点数文本"), Tooltip("显示升级需要点数文本")]
    public Text upgradePointCost;

    [SerializeField, LabelText("升级类型"), ReadOnly]
    public Upgrade_Type UpgradeType;

    private void Start()
    {
        this.upgradeButton.OnClickAsObservable().Subscribe(_ =>
        {
            CapabilityManager.Instance.StrengthenCapability(this.UpgradeType);
        });
    }

    public override void OnPointerClick(PointerEventData eventData)
    {

    }

    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="sort"></param>
    public void SetInfo(int sort=-1)
    {
        if(sort!=-1)
        {
            this.UpgradeType = (Upgrade_Type)sort;//设置升级类型
        }

        this.LevelText.text = CapabilityManager.Instance.curLevels[(int)this.UpgradeType].ToString();//设置能力等级

        switch (this.UpgradeType)
        {
            case Upgrade_Type.小队:
                this.SetInfoTeam();
                break;
            case Upgrade_Type.交易:
                this.SetInfoTransaction();

                break;
            case Upgrade_Type.行动力:
                this.SetInfoExecution();
                break;
        }

    }

    /// <summary>
    /// 设置信息为小队
    /// </summary>
    void SetInfoTeam()
    {
        this.UIStrengthenCapabilityItemTitleText.text = "探索小队";
        TeamUpgradeDefine curTeamUpgradeDefine = DataManager.TeamUpgradeDefines[CapabilityManager.Instance.curLevels[0]];
        this.curCapabilityText.text = string.Format("探索小队数量:{0}", curTeamUpgradeDefine.TeamIncreaseAmount);

        if (DataManager.TeamUpgradeDefines.ContainsKey(CapabilityManager.Instance.curLevels[0] + 1))
        {
            TeamUpgradeDefine nextTeamUpgradeDefine = DataManager.TeamUpgradeDefines[CapabilityManager.Instance.curLevels[0]+1];
            this.nextCapabilityText.text = string.Format("探索小队数量:{0}", nextTeamUpgradeDefine.TeamIncreaseAmount);
            this.upgradePointCost.text = nextTeamUpgradeDefine.TeamUpgradeCost.ToString();
            this.upgradeButton.gameObject.SetActive(true);
        }
        else
        {
            this.nextCapabilityText.text = string.Format("已经到达最高等级");
            this.upgradeButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 设置信息为交易
    /// </summary>
    void SetInfoTransaction()
    {
        this.UIStrengthenCapabilityItemTitleText.text = "交易";
        TransactionUpgradeDefine transactionUpgradeDefine = DataManager.TransactionUpgradeDefines[CapabilityManager.Instance.curLevels[1]];
        
        string curSpeicalEffect = "";
        if(transactionUpgradeDefine.TransactionSpecialEffectDescription!=null)
        {
            curSpeicalEffect =string.Format("\r\n{0}", transactionUpgradeDefine.TransactionSpecialEffectDescription);
        }

        this.curCapabilityText.text = string.Format("购买物品价格降低{0}%\r\n出售物品价格提升{1}%{2}", transactionUpgradeDefine.PurchasePriceReduce, transactionUpgradeDefine.PurchasePriceReduce,curSpeicalEffect);
        if (DataManager.TransactionUpgradeDefines.ContainsKey(CapabilityManager.Instance.curLevels[1] + 1))
        {
            TransactionUpgradeDefine nextTransactionUpgradeDefine = DataManager.TransactionUpgradeDefines[CapabilityManager.Instance.curLevels[1] + 1];
            string nextSpeicalEffect = "";
            if (nextTransactionUpgradeDefine.TransactionSpecialEffectDescription != null)
            {
                nextSpeicalEffect = string.Format("\r\n{0}", nextTransactionUpgradeDefine.TransactionSpecialEffectDescription);
            }
            this.nextCapabilityText.text = string.Format("购买物品价格降低{0}%\r\n出售物品价格提升{1}%{2}", nextTransactionUpgradeDefine.PurchasePriceReduce, nextTransactionUpgradeDefine.PurchasePriceReduce,nextSpeicalEffect);
            this.upgradePointCost.text = nextTransactionUpgradeDefine.TransactionUpgradeCost.ToString();
            this.upgradeButton.gameObject.SetActive(true);
        }
        else
        {
            this.nextCapabilityText.text = string.Format("已经到达最高等级");
            this.upgradeButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 设置信息为行动力
    /// </summary>
    void SetInfoExecution()
    {
        this.UIStrengthenCapabilityItemTitleText.text = "行动力";
        ExecutionUpgradeDefine executionUpgradeDefine = DataManager.ExecutionUpgradeDefines[CapabilityManager.Instance.curLevels[2]];
        this.curCapabilityText.text = string.Format("行动力上限:{0}", executionUpgradeDefine.ExecutionIncreaseAmount);
        if (DataManager.ExecutionUpgradeDefines.ContainsKey(CapabilityManager.Instance.curLevels[2] + 1))
        {
            ExecutionUpgradeDefine nextExecutionUpgradeDefine = DataManager.ExecutionUpgradeDefines[CapabilityManager.Instance.curLevels[2] + 1];
            this.nextCapabilityText.text = string.Format("行动力上限:{0}", nextExecutionUpgradeDefine.ExecutionIncreaseAmount);
            this.upgradePointCost.text = nextExecutionUpgradeDefine.ExecutionUpgradeCost.ToString();
            this.upgradeButton.gameObject.SetActive(true);
        }
        else
        {
            this.nextCapabilityText.text = string.Format("已经到达最高等级");
            this.upgradeButton.gameObject.SetActive(false);
        }
    }
}
