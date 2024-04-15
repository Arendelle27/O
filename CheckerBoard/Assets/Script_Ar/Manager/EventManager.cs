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

    //�¼��ֵ�
    //Dictionary<int, GameEvent> gameEventDic = new Dictionary<int, GameEvent>();

    //��ǰ�Կ��¼�
    public ConfrontDefine curConfrontEvent;
    //�����ǰ�Կ��¼����ڳ�ͻ�����У���¼��ǰ��ͻ����
    public ClashArea curClashArea;

    /// <summary>
    /// ���öԿ��¼�
    /// </summary>
    /// <param name="index"></param>
    public void SetConfrontEvent(int settleSort, float hotility,ClashArea curClashArea=null)
    {
        this.curConfrontEvent = DataManager.ConfrontDefines[settleSort][this.CalculateConfrontLevel(hotility)-1];
        this.curClashArea = curClashArea;
        UIManager.Instance.Show<UIConfrontEventWindow>();
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
        EventAreaManager.Instance.purchaseObjectsStatue[settleSort][commodityId][0]-= amount;//��������
        if (EventAreaManager.Instance.purchaseObjectsStatue[settleSort][commodityId][1] == 0)
        {
            //�ȴ�����
            EventAreaManager.Instance.purchaseObjectsStatue[settleSort][commodityId][1] = DataManager.TransactionDefines[settleSort][commodityId].CoolingRounds;
        }

        EventAreaManager.Instance.uITransactionWindow.UpdateBuildingList(0);

        int totalMoney = CapabilityManager.Instance.TransactionPrice( DataManager.TransactionDefines[settleSort][commodityId],true) * amount;
        ResourcesManager.Instance.ChangeWealth(-totalMoney);
        EventAreaManager.Instance.AddOrSubtractHotility(0, totalMoney / 200,true);//����ֵ����

        if (DataManager.TransactionDefines[settleSort][commodityId].TransactionType == Transaction_Type.��Դ)
        {
            MessageManager.Instance.AddMessage(Message_Type.����, string.Format("������{1}��{0}",(Resource_Type)commodityId, amount));
            int[] res = new int[3];
            res[DataManager.TransactionDefines[settleSort][commodityId].Subtype] = amount;
            ResourcesManager.Instance.ChangeBuildingResources(res, true);

        }
        else if (DataManager.TransactionDefines[settleSort][commodityId].TransactionType == Transaction_Type.��ͼ)
        {
            MessageManager.Instance.AddMessage(Message_Type.����, string.Format("��������ͼ{0}", commodityId));
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

        EventAreaManager.Instance.sellObjectsStatue[settleSort][goodId] -= amount;//��������

        EventAreaManager.Instance.uITransactionWindow.UpdateBuildingList(1);

        int resType = DataManager.TransactionDefines[settleSort][goodId].Subtype;
        int[] res = new int[3];
        res[resType] = amount;
        ResourcesManager.Instance.ChangeBuildingResources(res, false);
        int totalMoney= CapabilityManager.Instance.TransactionPrice(DataManager.TransactionDefines[settleSort][goodId],false) * amount;
        ResourcesManager.Instance.ChangeWealth(totalMoney);
        EventAreaManager.Instance.AddOrSubtractHotility(0, totalMoney/200, true);//����ֵ����

        //EventAreaManager.Instance.transactionObjectsStatue[settleSort][goodId][0]++;//��������
        MessageManager.Instance.AddMessage(Message_Type.����, string.Format("������{1}��{0}", (Resource_Type)goodId, amount));
        Debug.Log("�����ɹ�");
    }

    /// <summary>
    /// ����ɹ���
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
            case 0://�Կ��ɹ�����
                rate= cD.ConfrontBaseWinningRate+2*(BuildingManager.Instance.totalAttack-cD.DemandForce);
                break;
            case 1://Ȱ˵�ɹ�����
                rate= (int)(cD.PersuadeBaseWinningRate + cD.Level * costCurrency / EventAreaManager.Instance.hotility[this.curConfrontEvent.SettleType]);
                break;
            default://���ܳɹ�����
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
    /// �Կ�
    /// </summary>
    /// <param name="successRate"></param>
    public void Confront(int successRate)
    {
        ConfrontDefine cD = this.curConfrontEvent;
        int n=Random.Range(1, 101);
        if(n<=successRate)
        {
            Debug.Log("�Կ��ɹ�");
            int gainCurrency = Random.Range(cD.VictoryRewardCurrencyMin, cD.VictoryRewardCurrencyMax+1);
            ResourcesManager.Instance.ChangeWealth(gainCurrency);
            int[] gainResources = new int[3] {0,0,0};
            string gainresource1 = "";
            string gainresource2 = "";
            string gainresource3 = "";
            if(cD.VictoryRewardResource1Description!=null)
            {
                gainResources[0]= Random.Range(cD.VictoryRewardResource1Min, cD.VictoryRewardResource1Max + 1);
                gainresource1 = string.Format("��{0}��{1}", gainResources[0], Resource_Type.���ѵ���);
            }
            if(cD.VictoryRewardResource2Description!=null)
            {
                gainResources[1] = Random.Range(cD.VictoryRewardResource2Min, cD.VictoryRewardResource2Max + 1);
                gainresource2 = string.Format("��{0}��{1}", gainResources[1], Resource_Type.��������);
            }
            if(cD.VictoryRewardResource3Description!=null)
            {
                gainResources[2] = Random.Range(cD.VictoryRewardResource3Min, cD.VictoryRewardResource3Max + 1);
                gainresource3 = string.Format("��{0}��{1}", gainResources[2], Resource_Type.Ӱ��оƬ);
            }
            ResourcesManager.Instance.ChangeBuildingResources(gainResources, true);

            string resolveClash = this.removeCurClashArea();
                
            MessageManager.Instance.AddMessage(Message_Type.��ͻ, string.Format("ȡ����ս����ʤ����{4}�����{0}ö�ռ��{1}{2}{3}",gainCurrency,gainresource1,gainresource2,gainresource3,resolveClash));
        }
        else
        {
            Debug.Log("�Կ�ʧ��");
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
                            lostBuilding += "��";
                        }
                        lostBuilding += string.Format("λ��({0},{1})��{2}", building.pos.x,building.pos.y ,building.type);
                    }
                    else
                    {
                        break;
                    }
                }
                if(lostBuilding!="")
                {
                    lostBuilding = ","+lostBuilding+"���ݻ�";
                }
            }
            MessageManager.Instance.AddMessage(Message_Type.��ͻ,string.Format( "��ս����ʧ����ʧȥ��{0}%�Ŀռ��{1}", cD.FailurePenaltyCurrency,lostBuilding));
        }
    }

    /// <summary>
    /// ˵��
    /// </summary>
    /// <param name="successRate"></param>
    public bool Persudate(int successRate,int costCurrency)
    {
        ConfrontDefine cD = this.curConfrontEvent;
        string costCurrencyStr = "";
        if(costCurrency>0)
        {
            ResourcesManager.Instance.ChangeWealth(-costCurrency);
            costCurrencyStr = string.Format("������{0}�ռ��,", costCurrency);
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
                gainresource1 = string.Format(",���{0}��{1}", gainResources[0], Resource_Type.���ѵ���);
            }
            if (cD.PersuadeRewardResource2Description != null)
            {
                gainResources[1] = Random.Range(cD.PersuadeRewardResource2Min, cD.PersuadeRewardResource2Max + 1);
                if(gainresource1=="")
                {
                    gainresource2 = string.Format(",���{0}��{1}", gainResources[1], Resource_Type.��������);
                }
                else
                {
                    gainresource2 = string.Format("��{0}��{1}", gainResources[1], Resource_Type.��������);
                }
            }
            ResourcesManager.Instance.ChangeBuildingResources(gainResources, true);

            string resolveClash= this.removeCurClashArea();

            MessageManager.Instance.AddMessage(Message_Type.��ͻ, string.Format("{4}�ɹ�˵���˶Է�{3}��{0}��̬����������{1}{2}", cD.SettleType==0?"����":"��������", gainresource1, gainresource2, resolveClash,costCurrencyStr));
            return true;
        }
        else
        {
            if(costCurrency>0)
            {
                MessageManager.Instance.AddMessage(Message_Type.��ͻ, string.Format("{0}�Է���û�����㣬����ӭս", costCurrencyStr));
            }
            else
            {
                MessageManager.Instance.AddMessage(Message_Type.��ͻ, string.Format("�Է���������˵��������ӭս"));
            }
            return false;
        }
    }

    /// <summary>
    /// ������ͻ����
    /// </summary>
    /// <returns></returns>
    string removeCurClashArea()
    {
        string resolveClash = "";
        if (curClashArea != null)
        {
            resolveClash = string.Format(",�����λ�ڣ�{0},{1}���ĳ�ͻ", this.curClashArea.plot.pos.x, this.curClashArea.plot.pos.y);
            this.curClashArea.plot.ChangeToNormalType();
            EventAreaManager.Instance.EliminateEventArea(1, this.curClashArea);
            this.curClashArea = null;
        }
        return resolveClash;
    }

    /// <summary>
    /// ����
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
                addHostility = string.Format(",{0}�ĵ�������", cD.SettleType == 0 ? "����" : "��������");
            }
            MessageManager.Instance.AddMessage(Message_Type.��ͻ, string.Format("�ɹ��ӻ���������{0}", addHostility));
            return true;
        }
        else
        {
            MessageManager.Instance.AddMessage(Message_Type.��ͻ, string.Format("û���ӵ�������ӭս"));
            return false;
        }
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
                MainThreadDispatcher.StartUpdateMicroCoroutine(Main.Instance.GameOver());
            }
        }
    }

    /// <summary>
    /// ����Կ��ȼ�
    /// </summary>
    /// <param name="hotility"></param>
    /// <returns></returns>
    int CalculateConfrontLevel(float hotility)//���ݵ���ֵ����ȼ�����ֵ
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
    /// �غϽ���
    /// </summary>
    public void RoundOver()
    {
        //�����Կ��¼�
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
            MessageManager.Instance.AddMessage(Message_Type.��ͻ, string.Format("��{0}Ϯ����",settleSort==0?"����":"��������"));
        }
    }
}

