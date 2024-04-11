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
    [SerializeField, LabelText("����ѡ������ı�"), Tooltip("��ʾ����ѡ������ı�")]
    public Text UIStrengthenCapabilityItemTitleText;
    [SerializeField, LabelText("�����ȼ��ı�"), Tooltip("��ʾ�����ȼ��ı�")]
    public Text LevelText;
    [SerializeField, LabelText("��ǰ�����ı�"), Tooltip("��ʾ��ǰ�����ı�")]
    public Text curCapabilityText;
    [SerializeField, LabelText("�����������ı�"), Tooltip("��ʾ�����������ı�")]
    public Text nextCapabilityText;
    [SerializeField, LabelText("��������"), Tooltip("��������")]
    public Button upgradeButton;
    [SerializeField, LabelText("������Ҫ�����ı�"), Tooltip("��ʾ������Ҫ�����ı�")]
    public Text upgradePointCost;

    [SerializeField, LabelText("��������"), ReadOnly]
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
    /// ������Ϣ
    /// </summary>
    /// <param name="sort"></param>
    public void SetInfo(int sort=-1)
    {
        if(sort!=-1)
        {
            this.UpgradeType = (Upgrade_Type)sort;//������������
        }

        this.LevelText.text = CapabilityManager.Instance.curLevels[(int)this.UpgradeType].ToString();//���������ȼ�

        switch (this.UpgradeType)
        {
            case Upgrade_Type.С��:
                this.SetInfoTeam();
                break;
            case Upgrade_Type.����:
                this.SetInfoTransaction();

                break;
            case Upgrade_Type.�ж���:
                this.SetInfoExecution();
                break;
        }

    }

    /// <summary>
    /// ������ϢΪС��
    /// </summary>
    void SetInfoTeam()
    {
        this.UIStrengthenCapabilityItemTitleText.text = "̽��С��";
        TeamUpgradeDefine curTeamUpgradeDefine = DataManager.TeamUpgradeDefines[CapabilityManager.Instance.curLevels[0]];
        this.curCapabilityText.text = string.Format("̽��С������:{0}", curTeamUpgradeDefine.TeamIncreaseAmount);

        if (DataManager.TeamUpgradeDefines.ContainsKey(CapabilityManager.Instance.curLevels[0] + 1))
        {
            TeamUpgradeDefine nextTeamUpgradeDefine = DataManager.TeamUpgradeDefines[CapabilityManager.Instance.curLevels[0]+1];
            this.nextCapabilityText.text = string.Format("̽��С������:{0}", nextTeamUpgradeDefine.TeamIncreaseAmount);
            this.upgradePointCost.text = nextTeamUpgradeDefine.TeamUpgradeCost.ToString();
            this.upgradeButton.gameObject.SetActive(true);
        }
        else
        {
            this.nextCapabilityText.text = string.Format("�Ѿ�������ߵȼ�");
            this.upgradeButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ������ϢΪ����
    /// </summary>
    void SetInfoTransaction()
    {
        this.UIStrengthenCapabilityItemTitleText.text = "����";
        TransactionUpgradeDefine transactionUpgradeDefine = DataManager.TransactionUpgradeDefines[CapabilityManager.Instance.curLevels[1]];
        
        string curSpeicalEffect = "";
        if(transactionUpgradeDefine.TransactionSpecialEffectDescription!=null)
        {
            curSpeicalEffect =string.Format("\r\n{0}", transactionUpgradeDefine.TransactionSpecialEffectDescription);
        }

        this.curCapabilityText.text = string.Format("������Ʒ�۸񽵵�{0}%\r\n������Ʒ�۸�����{1}%{2}", transactionUpgradeDefine.PurchasePriceReduce, transactionUpgradeDefine.PurchasePriceReduce,curSpeicalEffect);
        if (DataManager.TransactionUpgradeDefines.ContainsKey(CapabilityManager.Instance.curLevels[1] + 1))
        {
            TransactionUpgradeDefine nextTransactionUpgradeDefine = DataManager.TransactionUpgradeDefines[CapabilityManager.Instance.curLevels[1] + 1];
            string nextSpeicalEffect = "";
            if (nextTransactionUpgradeDefine.TransactionSpecialEffectDescription != null)
            {
                nextSpeicalEffect = string.Format("\r\n{0}", nextTransactionUpgradeDefine.TransactionSpecialEffectDescription);
            }
            this.nextCapabilityText.text = string.Format("������Ʒ�۸񽵵�{0}%\r\n������Ʒ�۸�����{1}%{2}", nextTransactionUpgradeDefine.PurchasePriceReduce, nextTransactionUpgradeDefine.PurchasePriceReduce,nextSpeicalEffect);
            this.upgradePointCost.text = nextTransactionUpgradeDefine.TransactionUpgradeCost.ToString();
            this.upgradeButton.gameObject.SetActive(true);
        }
        else
        {
            this.nextCapabilityText.text = string.Format("�Ѿ�������ߵȼ�");
            this.upgradeButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ������ϢΪ�ж���
    /// </summary>
    void SetInfoExecution()
    {
        this.UIStrengthenCapabilityItemTitleText.text = "�ж���";
        ExecutionUpgradeDefine executionUpgradeDefine = DataManager.ExecutionUpgradeDefines[CapabilityManager.Instance.curLevels[2]];
        this.curCapabilityText.text = string.Format("�ж�������:{0}", executionUpgradeDefine.ExecutionIncreaseAmount);
        if (DataManager.ExecutionUpgradeDefines.ContainsKey(CapabilityManager.Instance.curLevels[2] + 1))
        {
            ExecutionUpgradeDefine nextExecutionUpgradeDefine = DataManager.ExecutionUpgradeDefines[CapabilityManager.Instance.curLevels[2] + 1];
            this.nextCapabilityText.text = string.Format("�ж�������:{0}", nextExecutionUpgradeDefine.ExecutionIncreaseAmount);
            this.upgradePointCost.text = nextExecutionUpgradeDefine.ExecutionUpgradeCost.ToString();
            this.upgradeButton.gameObject.SetActive(true);
        }
        else
        {
            this.nextCapabilityText.text = string.Format("�Ѿ�������ߵȼ�");
            this.upgradeButton.gameObject.SetActive(false);
        }
    }
}
