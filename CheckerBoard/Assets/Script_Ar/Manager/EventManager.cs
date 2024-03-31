using MANAGER;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    //当前事件id
    public int CurGameEventId;

    //事件字典
    Dictionary<int, GameEvent> gameEventDic = new Dictionary<int, GameEvent>();

    public void AddGameEvent(int eventId)
    {

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
        EventAreaManager.Instance.transactionObjectsStatue[settleSort][commodityId][0]-= amount;//数量减少
        if (EventAreaManager.Instance.transactionObjectsStatue[settleSort][commodityId][1] == 0)
        {
            //等待补货
            EventAreaManager.Instance.transactionObjectsStatue[settleSort][commodityId][1] = DataManager.TransactionDefines[settleSort][commodityId].CoolingRounds;
        }

        EventAreaManager.Instance.uITransactionWindow.UpdateBuildingList(0);

        if (DataManager.TransactionDefines[settleSort][commodityId].TransactionType == Transaction_Type.资源)
        {
            int[] res = new int[3];
            res[DataManager.TransactionDefines[settleSort][commodityId].Subtype] = amount;
            ResourcesManager.Instance.ChangeBuildingResources(res, true);
            ResourcesManager.Instance.ChangeWealth(-DataManager.TransactionDefines[settleSort][commodityId].Price*amount);
        }
        else if (DataManager.TransactionDefines[settleSort][commodityId].TransactionType == Transaction_Type.蓝图)
        {
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

        int resType = DataManager.TransactionDefines[settleSort][goodId].Subtype;
        int[] res = new int[3];
        res[resType] = amount;
        ResourcesManager.Instance.ChangeBuildingResources(res, false);
        ResourcesManager.Instance.ChangeWealth(DataManager.TransactionDefines[settleSort][goodId].Price * amount);

        //EventAreaManager.Instance.transactionObjectsStatue[settleSort][goodId][0]++;//数量增加
        Debug.Log("卖出成功");
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
                Main.Instance.GameOver();
            }
        }
    }
}

