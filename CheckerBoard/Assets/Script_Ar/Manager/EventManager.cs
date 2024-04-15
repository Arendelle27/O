using ENTITY;
using MANAGER;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{

    //事件字典
    //Dictionary<int, GameEvent> gameEventDic = new Dictionary<int, GameEvent>();

    //当前对抗事件
    public ConfrontDefine curConfrontEvent;
    //如果当前对抗事件是在冲突区域中，记录当前冲突区域
    public ClashArea curClashArea;

    /// <summary>
    /// 设置对抗事件
    /// </summary>
    /// <param name="index"></param>
    public void SetConfrontEvent(int settleSort, float hotility,ClashArea curClashArea=null)
    {
        this.curConfrontEvent = DataManager.ConfrontDefines[settleSort][this.CalculateConfrontLevel(hotility)-1];
        this.curClashArea = curClashArea;
        UIManager.Instance.Show<UIConfrontEventWindow>();
    }


    /// <summary>
    /// 购买事件
    /// </summary>
    /// <param name="commodityId"></param>
    public void Purchase(int commodityId,int amount)
    {
        Settle eA= EventAreaManager.Instance.selectedEventArea as Settle;
        int settleSort =0;
        if(eA.isBlackMarket)
        {
            settleSort = 1;
        }
        else
        {
            settleSort = 0;
        }

        Debug.Log("购买成功");
        EventAreaManager.Instance.purchaseObjectsStatue[settleSort][commodityId][0]-= amount;//数量减少
        if (EventAreaManager.Instance.purchaseObjectsStatue[settleSort][commodityId][1] == 0)
        {
            //等待补货
            EventAreaManager.Instance.purchaseObjectsStatue[settleSort][commodityId][1] = DataManager.TransactionDefines[settleSort][commodityId].CoolingRounds;
        }

        EventAreaManager.Instance.uITransactionWindow.UpdateBuildingList(0);

        int totalMoney = CapabilityManager.Instance.TransactionPrice( DataManager.TransactionDefines[settleSort][commodityId],true) * amount;
        ResourcesManager.Instance.ChangeWealth(-totalMoney);
        EventAreaManager.Instance.AddOrSubtractHotility(0, totalMoney / 200,true);//敌意值增加

        if (DataManager.TransactionDefines[settleSort][commodityId].TransactionType == Transaction_Type.资源)
        {
            MessageManager.Instance.AddMessage(Message_Type.交易, string.Format("购买了{1}个{0}",(Resource_Type)commodityId, amount));
            int[] res = new int[3];
            res[DataManager.TransactionDefines[settleSort][commodityId].Subtype] = amount;
            ResourcesManager.Instance.ChangeBuildingResources(res, true);

        }
        else if (DataManager.TransactionDefines[settleSort][commodityId].TransactionType == Transaction_Type.蓝图)
        {
            MessageManager.Instance.AddMessage(Message_Type.交易, string.Format("购买了蓝图{0}", commodityId));
            BuildingManager.Instance.bluePrints[DataManager.TransactionDefines[settleSort][commodityId].Subtype]= true;//蓝图解锁
        }

    }

    /// <summary>
    /// 卖出事件
    /// </summary>
    /// <param name="goodId"></param>
    public void Sell(int goodId,int amount)
    {
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

        EventAreaManager.Instance.sellObjectsStatue[settleSort][goodId] -= amount;//数量减少

        EventAreaManager.Instance.uITransactionWindow.UpdateBuildingList(1);

        int resType = DataManager.TransactionDefines[settleSort][goodId].Subtype;
        int[] res = new int[3];
        res[resType] = amount;
        ResourcesManager.Instance.ChangeBuildingResources(res, false);
        int totalMoney= CapabilityManager.Instance.TransactionPrice(DataManager.TransactionDefines[settleSort][goodId],false) * amount;
        ResourcesManager.Instance.ChangeWealth(totalMoney);
        EventAreaManager.Instance.AddOrSubtractHotility(0, totalMoney/200, true);//敌意值增加

        //EventAreaManager.Instance.transactionObjectsStatue[settleSort][goodId][0]++;//数量增加
        MessageManager.Instance.AddMessage(Message_Type.交易, string.Format("卖出了{1}个{0}", (Resource_Type)goodId, amount));
        Debug.Log("卖出成功");
    }

    /// <summary>
    /// 计算成功率
    /// </summary>
    /// <param name="copeSort"></param>
    /// <param name="settleSort"></param>
    /// <returns></returns>
    public int calculateRates(int copeSort,int costCurrency=0)
    {
        ConfrontDefine cD = this.curConfrontEvent;
        int rate = 0;
        switch (copeSort)
        {
            case 0://对抗成功概率
                rate= cD.ConfrontBaseWinningRate+2*(BuildingManager.Instance.totalAttack-cD.DemandForce);
                break;
            case 1://劝说成功概率
                rate= (int)(cD.PersuadeBaseWinningRate + cD.Level * costCurrency / EventAreaManager.Instance.hotility[this.curConfrontEvent.SettleType]);
                break;
            default://逃跑成功概率
                rate= cD.EscapeRate;
                break;
        }
        if(rate>100)
        {
            rate = 100;
        }
        else if(rate<0)
        {
            rate = 0;
        }
        return rate;
    }

    /// <summary>
    /// 对抗
    /// </summary>
    /// <param name="successRate"></param>
    public void Confront(int successRate)
    {
        ConfrontDefine cD = this.curConfrontEvent;
        int n=Random.Range(1, 101);
        if(n<=successRate)
        {
            Debug.Log("对抗成功");
            int gainCurrency = Random.Range(cD.VictoryRewardCurrencyMin, cD.VictoryRewardCurrencyMax+1);
            ResourcesManager.Instance.ChangeWealth(gainCurrency);
            int[] gainResources = new int[3] {0,0,0};
            string gainresource1 = "";
            string gainresource2 = "";
            string gainresource3 = "";
            if(cD.VictoryRewardResource1Description!=null)
            {
                gainResources[0]= Random.Range(cD.VictoryRewardResource1Min, cD.VictoryRewardResource1Max + 1);
                gainresource1 = string.Format("、{0}个{1}", gainResources[0], Resource_Type.断裂电缆);
            }
            if(cD.VictoryRewardResource2Description!=null)
            {
                gainResources[1] = Random.Range(cD.VictoryRewardResource2Min, cD.VictoryRewardResource2Max + 1);
                gainresource2 = string.Format("、{0}个{1}", gainResources[1], Resource_Type.废弃金属);
            }
            if(cD.VictoryRewardResource3Description!=null)
            {
                gainResources[2] = Random.Range(cD.VictoryRewardResource3Min, cD.VictoryRewardResource3Max + 1);
                gainresource3 = string.Format("、{0}个{1}", gainResources[2], Resource_Type.影像芯片);
            }
            ResourcesManager.Instance.ChangeBuildingResources(gainResources, true);

            string resolveClash = this.removeCurClashArea();
                
            MessageManager.Instance.AddMessage(Message_Type.冲突, string.Format("取得了战斗的胜利，{4}获得了{0}枚空间币{1}{2}{3}",gainCurrency,gainresource1,gainresource2,gainresource3,resolveClash));
        }
        else
        {
            Debug.Log("对抗失败");
            int lostCurrency = ResourcesManager.Instance.wealth* cD.FailurePenaltyCurrency/100;
            ResourcesManager.Instance.ChangeWealth(-lostCurrency);
            string lostBuilding = "";
            if(cD.FailurePenaltyBuildingDescription!=null)
            {
                for (int i = 0; i < cD.FailurePenaltyBuilding; i++)
                {
                    int gB = BuildingManager.Instance.gatheringBuildings.Count;
                    int pB = BuildingManager.Instance.productionBuildings.Count;
                    int bB = BuildingManager.Instance.battleBuildings.Count;
                    int tB = gB + pB + bB;
                    if(tB>0)
                    {
                        Building building = null;
                        int bc = Random.Range(0, tB);
                        if (bc < gB)
                        {
                            building = BuildingManager.Instance.gatheringBuildings.ElementAt(bc).Value;
                            BuildingManager.Instance.RemoveBuilding(0, building);
                        }
                        else if (bc < gB + pB)
                        {
                            building = BuildingManager.Instance.productionBuildings.ElementAt(bc-gB).Value;
                            BuildingManager.Instance.RemoveBuilding(1, building);
                        }
                        else
                        {
                            building = BuildingManager.Instance.battleBuildings.ElementAt(bc - gB-pB).Value;
                            BuildingManager.Instance.RemoveBuilding(3, building);
                        }
                        if(lostBuilding!="")
                        {
                            lostBuilding += "、";
                        }
                        lostBuilding += string.Format("位于({0},{1})的{2}", building.pos.x,building.pos.y ,building.type);
                    }
                    else
                    {
                        break;
                    }
                }
                if(lostBuilding!="")
                {
                    lostBuilding = ","+lostBuilding+"被摧毁";
                }
            }
            MessageManager.Instance.AddMessage(Message_Type.冲突,string.Format( "在战斗中失利，失去了{0}%的空间币{1}", cD.FailurePenaltyCurrency,lostBuilding));
        }
    }

    /// <summary>
    /// 说服
    /// </summary>
    /// <param name="successRate"></param>
    public bool Persudate(int successRate,int costCurrency)
    {
        ConfrontDefine cD = this.curConfrontEvent;
        string costCurrencyStr = "";
        if(costCurrency>0)
        {
            ResourcesManager.Instance.ChangeWealth(-costCurrency);
            costCurrencyStr = string.Format("花费了{0}空间币,", costCurrency);
        }

        int n = Random.Range(1, 101);
        if (n <= successRate)
        {
            int reduceHoility = cD.PersuadeRewardReduceHostility;
            EventAreaManager.Instance.AddOrSubtractHotility(cD.SettleType, reduceHoility, false);

            int[] gainResources = new int[3] { 0, 0, 0 };
            string gainresource1 = "";
            string gainresource2 = "";
            if (cD.PersuadeRewardResource1Description != null)
            {
                gainResources[0] = Random.Range(cD.PersuadeRewardResource1Min, cD.PersuadeRewardResource1Max + 1);
                gainresource1 = string.Format(",获得{0}个{1}", gainResources[0], Resource_Type.断裂电缆);
            }
            if (cD.PersuadeRewardResource2Description != null)
            {
                gainResources[1] = Random.Range(cD.PersuadeRewardResource2Min, cD.PersuadeRewardResource2Max + 1);
                if(gainresource1=="")
                {
                    gainresource2 = string.Format(",获得{0}个{1}", gainResources[1], Resource_Type.废弃金属);
                }
                else
                {
                    gainresource2 = string.Format("、{0}个{1}", gainResources[1], Resource_Type.废弃金属);
                }
            }
            ResourcesManager.Instance.ChangeBuildingResources(gainResources, true);

            string resolveClash= this.removeCurClashArea();

            MessageManager.Instance.AddMessage(Message_Type.冲突, string.Format("{4}成功说服了对方{3}，{0}的态度有所改善{1}{2}", cD.SettleType==0?"人们":"机器人们", gainresource1, gainresource2, resolveClash,costCurrencyStr));
            return true;
        }
        else
        {
            if(costCurrency>0)
            {
                MessageManager.Instance.AddMessage(Message_Type.冲突, string.Format("{0}对方并没有满足，被迫迎战", costCurrencyStr));
            }
            else
            {
                MessageManager.Instance.AddMessage(Message_Type.冲突, string.Format("对方不想听你说话，被迫迎战"));
            }
            return false;
        }
    }

    /// <summary>
    /// 消除冲突区域
    /// </summary>
    /// <returns></returns>
    string removeCurClashArea()
    {
        string resolveClash = "";
        if (curClashArea != null)
        {
            resolveClash = string.Format(",解决了位于（{0},{1}）的冲突", this.curClashArea.plot.pos.x, this.curClashArea.plot.pos.y);
            this.curClashArea.plot.ChangeToNormalType();
            EventAreaManager.Instance.EliminateEventArea(1, this.curClashArea);
            this.curClashArea = null;
        }
        return resolveClash;
    }

    /// <summary>
    /// 逃跑
    /// </summary>
    /// <param name="successRate"></param>
    /// <returns></returns>
    public bool Escape(int successRate)
    {
        ConfrontDefine cD = this.curConfrontEvent;
        int n = Random.Range(1, 101);
        if (n <= successRate)
        {
            string addHostility = "";
            if(cD.EscapeAddHostilityDescription!=null)
            {
                int reduceHoility = cD.EscapeAddHostility;
                EventAreaManager.Instance.AddOrSubtractHotility(cD.SettleType, reduceHoility, true);
                addHostility = string.Format(",{0}的敌意增加", cD.SettleType == 0 ? "人们" : "机器人们");
            }
            MessageManager.Instance.AddMessage(Message_Type.冲突, string.Format("成功从混乱中脱身{0}", addHostility));
            return true;
        }
        else
        {
            MessageManager.Instance.AddMessage(Message_Type.冲突, string.Format("没能逃掉，被迫迎战"));
            return false;
        }
    }

    /// <summary>
    /// 阶段结算
    /// </summary>
    public void StageDecision()
    {
        if (RoundManager.Instance.roundNumber % 5 == 0)
        {
            Debug.Log("阶段结算");
            ResourcesManager.Instance.wealth -= RoundManager.Instance.roundNumber * 10;
            if (ResourcesManager.Instance.wealth < 0)
            {
                MainThreadDispatcher.StartUpdateMicroCoroutine(Main.Instance.GameOver());
            }
        }
    }

    /// <summary>
    /// 计算对抗等级
    /// </summary>
    /// <param name="hotility"></param>
    /// <returns></returns>
    int CalculateConfrontLevel(float hotility)//根据敌意值计算等级敌意值
    {
        int[] levelRate;
        float confrontLevelParameter = hotility * RoundManager.Instance.roundNumber;
        if (confrontLevelParameter < 300f)
        {
            levelRate = new int[3] { 100, 100, 100 };
        }
        else if (confrontLevelParameter < 1000f)
        {
            levelRate = new int[3] { 70, 100, 100 };
        }
        else if (confrontLevelParameter < 2200f)
        {
            levelRate = new int[3] { 30, 70, 100 };
        }
        else
        {
            levelRate = new int[3] { 10, 30, 100 };
        }

        int confrontLevel;
        int n = Random.Range(1, 101);
        if (n <= levelRate[0])
        {
            confrontLevel = 1;
        }
        else if (n <= levelRate[1])
        {
            confrontLevel = 2;
        }
        else
        {
            confrontLevel = 3;
        }
        return confrontLevel;
    }

    /// <summary>
    /// 回合结束
    /// </summary>
    public void RoundOver()
    {
        //产生对抗事件
        List<float> hotility = EventAreaManager.Instance.hotility;
        float total = hotility[0]+ hotility[1];
        int rate = 0;
        if(total<20f)
        {
        }
        else if(total<60f)
        {
            rate = 5;
        }
        else if(total<100f)
        {
            rate = 20;
        }
        else if(total<180f)
        {
            rate = 55;
        }
        else
        {
            rate = 99;
        }
        int n = Random.Range(1, 101);
        if(n<=rate)
        {
            int settleSort;
            float r = Random.Range(0,total);
            if (r < hotility[0])
            {
                settleSort = 0;
            }
            else
            {
                settleSort = 1;
            }

            this.SetConfrontEvent(settleSort, hotility[settleSort]);
            MessageManager.Instance.AddMessage(Message_Type.冲突, string.Format("被{0}袭击了",settleSort==0?"人们":"机器人们"));
        }
    }
}

