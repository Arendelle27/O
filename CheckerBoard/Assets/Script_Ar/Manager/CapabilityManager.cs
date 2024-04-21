using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MANAGER
{
    public class CapabilityManager : Singleton<CapabilityManager>
    {
        //��������
        public int upgradePoint = 0;
        //�ѹ������������
        public int upgradePointHaveBuy = 0;

        //��ǰ�ȼ�
        public List<int> curLevels = new List<int>(3)
        {
            0,0,0
        };
        //1Ϊ��ǰС�ӵȼ���2Ϊ��ǰ���׵ȼ���3Ϊ��ǰ�ж����ȼ�

        //С����չ��
        public int expendExploratoryAmount = 0;
        //��Ѽ�����ȴ�غ���
        public int freelyReduceCoolingRound = 0;
        //�����ж���
        public int executionAmount= 0;
        //ǿ����������
        public UIStrengthenCapabilityWindow uIStrengthenCapabilityWindow;

        public CapabilityManager()
        {
            this.ObserveEveryValueChanged(_ => this.upgradePoint).Subscribe(_ =>
            {
                //�仯ʱ��������UI
                (UIMain.Instance.uiPanels[1] as UIGamePanel).leaveValue.text = this.upgradePoint.ToString();
                Debug.Log("������������");
            });

            this.ObserveEveryValueChanged(_ => this.expendExploratoryAmount).Subscribe(_ =>
            {
                (UIMain.Instance.uiPanels[3] as UIExtendExpTeamPanel).UpdateUI(this.expendExploratoryAmount);

            });

            //this.uIStrengthenCapabilityWindow = UIManager.Instance.Show<UIStrengthenCapabilityWindow>();
        }

        public void Init()
        {
            if(this.uIStrengthenCapabilityWindow==null)
            {
                this.uIStrengthenCapabilityWindow = UIManager.Instance.Show<UIStrengthenCapabilityWindow>();
            }
            this.expendExploratoryAmount = 0;
        }

        public void Restart()
        {
            this.Init();
            this.upgradePoint = 0;
            this.upgradePointHaveBuy = 0;
            this.freelyReduceCoolingRound = 0;
            for (int i = 0; i < this.curLevels.Count; i++)
            {
                curLevels[i] = 0;
            }
            this.executionAmount = DataManager.ExecutionUpgradeDefines[this.curLevels[2]].ExecutionIncreaseAmount;
        }

        public void ReadArchive()
        {
            this.Init();
            ArchiveManager.CapabilityManagerData eventManagerData = ArchiveManager.archive.capabilityManagerData;

            this.upgradePoint = eventManagerData.upgradePoint;
            this.upgradePointHaveBuy = eventManagerData.upgradePointHaveBuy;
            this.freelyReduceCoolingRound = eventManagerData.freelyReduceCoolingRound;
            this.curLevels = eventManagerData.curLevels;
            this.executionAmount = eventManagerData.executionAmount;
        }

        /// <summary>
        /// ����������
        /// </summary>
        public bool BuyUpgradePoint()
        {
            int cost = DataManager.UpgradePointCostDefines[this.upgradePointHaveBuy].BuyNextCost;
            if(ResourcesManager.Instance.CanBuyUpgradePoint(cost))
            {
                this.upgradePoint++;
                this.upgradePointHaveBuy++;
                this.uIStrengthenCapabilityWindow.UpdateUpgradePointHaveBuy();

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// ��ǿ����
        /// </summary>
        /// <param name="type"></param>
        public void StrengthenCapability(Upgrade_Type type)
        {
            switch (type)
            {
                case Upgrade_Type.С��:
                    if(this.upgradePoint>= DataManager.TeamUpgradeDefines[this.curLevels[0]+1].TeamUpgradeCost)
                    {
                        TeamUpgradeDefine teamUpgradeDefine = DataManager.TeamUpgradeDefines[this.curLevels[0]+1];
                        this.upgradePoint -= teamUpgradeDefine.TeamUpgradeCost;
                        this.expendExploratoryAmount+= teamUpgradeDefine.TeamIncreaseAmount;
                        this.curLevels[0]++;

                        this.uIStrengthenCapabilityWindow.UpdateUpgradePointHaveBuy();

                        PlotManager.Instance.EnterSelectExtendExpTeam(true);//����ѡ����չ̽��С�ӵ�ģʽ
                        UIManager.Instance.Close<UIStrengthenCapabilityWindow>();
                    }
                    break;
                case Upgrade_Type.����:
                    if (this.upgradePoint >= DataManager.TransactionUpgradeDefines[this.curLevels[1] + 1].TransactionUpgradeCost)
                    {
                        TransactionUpgradeDefine transactionUpgradeDefine = DataManager.TransactionUpgradeDefines[this.curLevels[1] + 1];
                        this.upgradePoint -= transactionUpgradeDefine.TransactionUpgradeCost;

                        if(transactionUpgradeDefine.TransactionSpecialEffectDescription!=null)
                        {
                            this.freelyReduceCoolingRound = transactionUpgradeDefine.TransactionSpecialEffectValue;
                        }

                        this.curLevels[1]++;

                        this.uIStrengthenCapabilityWindow.UpdateUpgradePointHaveBuy();
                    }
                    break;
                case Upgrade_Type.�ж���:
                    if (this.upgradePoint >= DataManager.ExecutionUpgradeDefines[this.curLevels[2] + 1].ExecutionUpgradeCost)
                    {
                        ExecutionUpgradeDefine executionUpgradeDefine = DataManager.ExecutionUpgradeDefines[this.curLevels[2] + 1];
                        this.upgradePoint -= executionUpgradeDefine.ExecutionUpgradeCost;

                        int executionAmountIncrease = executionUpgradeDefine.ExecutionIncreaseAmount - this.executionAmount;
                        ResourcesManager.Instance.execution += executionAmountIncrease;

                        this.executionAmount = executionUpgradeDefine.ExecutionIncreaseAmount;
                        this.curLevels[2]++;

                        this.uIStrengthenCapabilityWindow.UpdateUpgradePointHaveBuy();
                    }
                    break;
            }

            this.uIStrengthenCapabilityWindow.UpdateStrengthenCapabilityItemInfo((int)type); //����ǿ��������Ŀ��Ϣ
        }

        /// <summary>
        /// �����ۿۺ�Ľ��׼۸�
        /// </summary>
        /// <param name="transactionDefine"></param>
        /// <param name="isPurchase"></param>
        /// <returns></returns>
        public int TransactionPrice(TransactionDefine transactionDefine,bool isPurchase)//0Ϊ���룬1Ϊ����
        {
            if(isPurchase)
            {
                return transactionDefine.Price * (100 - DataManager.TransactionUpgradeDefines[curLevels[1]].PurchasePriceReduce)/100;
            }
            else
            {
                return transactionDefine.Price * (100 + DataManager.TransactionUpgradeDefines[curLevels[1]].SellPriceIncrease)/100;
            }
            
        }

        /// <summary>
        /// �غϽ���
        /// </summary>
        public void RoundOver()
        {
            if (DataManager.TransactionUpgradeDefines[this.curLevels[1]].TransactionSpecialEffectDescription!=null)
            {
                this.freelyReduceCoolingRound = DataManager.TransactionUpgradeDefines[this.curLevels[1]].TransactionSpecialEffectValue;
            }
            else
            {
                this.freelyReduceCoolingRound = 0;
            }
        }
    }
}
