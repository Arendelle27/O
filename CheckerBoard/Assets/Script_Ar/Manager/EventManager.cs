using MANAGER;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    //��ǰ�¼�id
    public int CurGameEventId;

    //�¼��ֵ�
    Dictionary<int, GameEvent> gameEventDic = new Dictionary<int, GameEvent>();

    public void AddGameEvent(int eventId)
    {

    }

    /// <summary>
    /// �����¼�
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

        Debug.Log("����ɹ�");
        EventAreaManager.Instance.transactionObjectsStatue[settleSort][commodityId][0]-= amount;//��������
        if (EventAreaManager.Instance.transactionObjectsStatue[settleSort][commodityId][1] == 0)
        {
            //�ȴ�����
            EventAreaManager.Instance.transactionObjectsStatue[settleSort][commodityId][1] = DataManager.TransactionDefines[settleSort][commodityId].CoolingRounds;
        }

        EventAreaManager.Instance.uITransactionWindow.UpdateBuildingList(0);

        if (DataManager.TransactionDefines[settleSort][commodityId].TransactionType == Transaction_Type.��Դ)
        {
            int[] res = new int[3];
            res[DataManager.TransactionDefines[settleSort][commodityId].Subtype] = amount;
            ResourcesManager.Instance.ChangeBuildingResources(res, true);
            ResourcesManager.Instance.ChangeWealth(-DataManager.TransactionDefines[settleSort][commodityId].Price*amount);
        }
        else if (DataManager.TransactionDefines[settleSort][commodityId].TransactionType == Transaction_Type.��ͼ)
        {
            BuildingManager.Instance.bluePrints[DataManager.TransactionDefines[settleSort][commodityId].Subtype]= true;//��ͼ����
        }


    }

    /// <summary>
    /// �����¼�
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

        //EventAreaManager.Instance.transactionObjectsStatue[settleSort][goodId][0]++;//��������
        Debug.Log("�����ɹ�");
    }

    /// <summary>
    /// �׶ν���
    /// </summary>
    public void StageDecision()
    {
        if (RoundManager.Instance.roundNumber % 5 == 0)
        {
            Debug.Log("�׶ν���");
            ResourcesManager.Instance.wealth -= RoundManager.Instance.roundNumber * 10;
            if (ResourcesManager.Instance.wealth < 0)
            {
                Main.Instance.GameOver();
            }
        }
    }
}

