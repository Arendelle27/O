using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MANAGER
{
    public class CapabilityManager : Singleton<CapabilityManager>
    {
        //升级点数
        public int upgradePoint = 0;
        //已购买的升级点数
        public int upgradePointHaveBuy = 0;

        //当前等级
        public List<int> curLevels = new List<int>(3)
        {
            0,0,0
        };
        //1为当前小队等级，2为当前交易等级，3为当前行动力等级

        //小队拓展数
        public int expendExploratoryAmount = 0;
        //免费减少冷却回合数
        public int freelyReduceCoolingRound = 0;
        //消耗行动力
        public int executionAmount= 0;
        //强化能力窗口
        public UIStrengthenCapabilityWindow uIStrengthenCapabilityWindow;

        public CapabilityManager()
        {
            this.ObserveEveryValueChanged(_ => this.upgradePoint).Subscribe(_ =>
            {
                //变化时更新能量UI
                (UIMain.Instance.uiPanels[1] as UIGamePanel).leaveValue.text = this.upgradePoint.ToString();
                Debug.Log("升级点数增加");
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
        /// 购买升级点
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
        /// 增强能力
        /// </summary>
        /// <param name="type"></param>
        public void StrengthenCapability(Upgrade_Type type)
        {
            switch (type)
            {
                case Upgrade_Type.小队:
                    if(this.upgradePoint>= DataManager.TeamUpgradeDefines[this.curLevels[0]+1].TeamUpgradeCost)
                    {
                        TeamUpgradeDefine teamUpgradeDefine = DataManager.TeamUpgradeDefines[this.curLevels[0]+1];
                        this.upgradePoint -= teamUpgradeDefine.TeamUpgradeCost;
                        this.expendExploratoryAmount+= teamUpgradeDefine.TeamIncreaseAmount;
                        this.curLevels[0]++;

                        this.uIStrengthenCapabilityWindow.UpdateUpgradePointHaveBuy();

                        PlotManager.Instance.EnterSelectExtendExpTeam(true);//进入选择扩展探索小队的模式
                        UIManager.Instance.Close<UIStrengthenCapabilityWindow>();
                    }
                    break;
                case Upgrade_Type.交易:
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
                case Upgrade_Type.行动力:
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

            this.uIStrengthenCapabilityWindow.UpdateStrengthenCapabilityItemInfo((int)type); //更新强化能力项目信息
        }

        /// <summary>
        /// 计算折扣后的交易价格
        /// </summary>
        /// <param name="transactionDefine"></param>
        /// <param name="isPurchase"></param>
        /// <returns></returns>
        public int TransactionPrice(TransactionDefine transactionDefine,bool isPurchase)//0为买入，1为卖出
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
        /// 回合结束
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
