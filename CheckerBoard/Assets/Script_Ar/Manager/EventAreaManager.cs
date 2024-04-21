using ENTITY;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using System.Xml.Schema;

namespace MANAGER
{
    public class EventAreaManager : Singleton<EventAreaManager>
    {
        //存在的事件地区
        public List<List<EventArea>> EventAreas = new List<List<EventArea>>(4) 
        {
            new List < EventArea >(),//交易
            new List < EventArea >(),//对抗
            new List < EventArea >(),//遗迹
            new List < EventArea >()//剧情
        };

        //购买数量和冷却
        public Dictionary<int,Dictionary<int,List<int>>> purchaseObjectsStatue=new Dictionary<int, Dictionary<int, List<int>>>() 
        {
            {0, new Dictionary<int, List<int>>()},//聚集地
            {1, new Dictionary<int, List<int>>()} //黑市
        };
        //最后的值,0为剩余数量，1为冷却时间

        //出售数量和冷却
        public Dictionary<int, Dictionary<int, int>> sellObjectsStatue = new Dictionary<int, Dictionary<int, int>>()
        {
            {0, new Dictionary<int, int>()},//聚集地
            {1, new Dictionary<int, int>()} //黑市
        };
        //最后的值,为剩余数量

        //敌意值
        public List<float> hotility=new List<float>(2) {5f,5f};
        //0为人类，1为机器人,基础仇恨值为5

        //被选中的事件地区
        public EventArea selectedEventArea;
        //交易窗口
        public UITransactionWindow uITransactionWindow;

        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {

        }

        /// <summary>
        /// 重开
        /// </summary>
        public void Restart()
        {
            this.Init();
            foreach (int i in DataManager.TransactionDefines.Keys)
            {
                foreach (int j in DataManager.TransactionDefines[i].Keys)
                {
                    if (DataManager.TransactionDefines[i][j].PurchaseOrSell == 0)
                    {
                        if (this.purchaseObjectsStatue[i].ContainsKey(j))
                        {
                            this.purchaseObjectsStatue[i][j][0] = DataManager.TransactionDefines[i][j].Amount;
                            this.purchaseObjectsStatue[i][j][1] = 0;
                        }
                        else
                        {
                            this.purchaseObjectsStatue[i].Add(j, new List<int>(2) { DataManager.TransactionDefines[i][j].Amount, 0 });
                        }
                    }
                    else
                    {
                        if (this.sellObjectsStatue[i].ContainsKey(j))
                        {
                            this.sellObjectsStatue[i][j] = DataManager.TransactionDefines[i][j].Amount;
                        }
                        else
                        {
                            this.sellObjectsStatue[i].Add(j, DataManager.TransactionDefines[i][j].Amount);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 读档
        /// </summary>
        public void ReadArchive()
        {
            this.Init();
            ArchiveManager.EventAreaManagerData eventAreaManagerData = ArchiveManager.archive.eventAreaManagerData;

            for(int i=0;i<eventAreaManagerData.purchaseObjectStatueDatas.Count;i++)
            {
                foreach(var purchaseObjectStatueData in eventAreaManagerData.purchaseObjectStatueDatas[i].purchaseObjectStatueDatas)
                {
                    if (this.purchaseObjectsStatue[i].ContainsKey(purchaseObjectStatueData.purchaseObjectId))
                    {
                        this.purchaseObjectsStatue[i][purchaseObjectStatueData.purchaseObjectId] = purchaseObjectStatueData.purchaseObjectStatue;
                    }
                    else
                    {
                        this.purchaseObjectsStatue[i].Add(purchaseObjectStatueData.purchaseObjectId, purchaseObjectStatueData.purchaseObjectStatue);
                    }
                }
            }

            for (int i = 0; i < eventAreaManagerData.sellObjectsStatueDatas.Count; i++)
            {
                foreach (var sellObjectStatueData in eventAreaManagerData.sellObjectsStatueDatas[i].sellObjectStatueDatas)
                {
                    if (this.sellObjectsStatue[i].ContainsKey(sellObjectStatueData.sellObjectId))
                    {
                        this.sellObjectsStatue[i][sellObjectStatueData.sellObjectId] = sellObjectStatueData.sellObjectStatue;
                    }
                    else
                    {
                        this.sellObjectsStatue[i].Add(sellObjectStatueData.sellObjectId, sellObjectStatueData.sellObjectStatue);
                    }
                }
            }

            this.hotility = eventAreaManagerData.hotility;
        }

        public void GameOver()
        {
            this.EliminateAllEventArea();
            for(int i=0;i<this.hotility.Count;i++)
            {
                this.hotility[i] = 5f;
            }
        }   

        /// <summary>
        /// 生成单个聚落
        /// </summary>
        /// <param name="isHumanSettlement"></param>
        public EventArea GetEventArea(Event_Area_Type type,Plot plot)
        {
            switch (type)
            {
                case Event_Area_Type.交易:
                    Settle settle = new Settle(plot);
                    this.AddEventArea(0, settle);
                    return settle;
                case Event_Area_Type.对抗:
                    ClashArea clashArea = new ClashArea(plot);
                    this.AddEventArea(1, clashArea);
                    return clashArea;
                case Event_Area_Type.遗迹:
                    return new MysteriousArea(plot);
                default:
                    return new ClueArea(plot);
            }
        }


            //try
            //{

            //}
            //catch (System.Exception e)
            //{
            //    return Vector2Int.zero;
            //}

        /// <summary>
        /// 添加事件地区
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="eventArea"></param>
        public void AddEventArea(int sort,EventArea eventArea)
        {
            this.EventAreas[sort].Add(eventArea);
        }
        
        /// <summary>
        /// 清除所有事件地区
        /// </summary>
        void EliminateAllEventArea()
        {
            for(int i=0;i<this.EventAreas.Count;i++)
            {
                this.EventAreas[i].Clear();
            }
        }


        /// <summary>
        /// 消除事件地区
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="pos"></param>
        public void EliminateEventArea(int sort,EventArea eventArea)
        {
            this.EventAreas[sort].Remove(eventArea);//移除事件地区
        }

        /// <summary>
        /// 花费财富减少冷却时间
        /// </summary>
        /// <param name="transactionObjectId"></param>
        /// <returns></returns>
        public bool ReduceCoolingRoundBySpend(int transactionObjectId)
        {
            Settle eA = this.selectedEventArea as Settle;
            int settleSort = 0;
            if (eA.isBlackMarket)
            {
                settleSort = 1;
            }
            else
            {
                settleSort = 0;
            }
            if (DataManager.TransactionDefines[settleSort][transactionObjectId].TransactionType == Transaction_Type.蓝图)
            {
                Debug.Log("蓝图不会补货");
                MessageManager.Instance.AddMessage(Message_Type.交易, string.Format("蓝图不会补货"));
                return false;
            }
            if (this.purchaseObjectsStatue[settleSort][transactionObjectId][0] == DataManager.TransactionDefines[settleSort][transactionObjectId].Amount)
            {
                Debug.Log("不需要补货");
                MessageManager.Instance.AddMessage(Message_Type.交易, string.Format("不需要补货"));
                return false;
            }

            if (CapabilityManager.Instance.freelyReduceCoolingRound>0)
            {
                CapabilityManager.Instance.freelyReduceCoolingRound--;

                this.ReduceCoolingRound(settleSort, transactionObjectId);

                return true;
            }

            if(ResourcesManager.Instance.wealth>=50)
            {
                ResourcesManager.Instance.ChangeWealth(-50);

                this.ReduceCoolingRound(settleSort, transactionObjectId);

                return true;
            }
            else
            {
                MessageManager.Instance.AddMessage(Message_Type.交易, string.Format("金钱不足够补货"));
                return false;
            }
        }

        /// <summary>
        /// 补货
        /// </summary>
        /// <param name="settleSort"></param>
        /// <param name="transactionObjectId"></param>
        void ReduceCoolingRound(int settleSort,int transactionObjectId)
        {
            this.purchaseObjectsStatue[settleSort][transactionObjectId][1]--;
            MessageManager.Instance.AddMessage(Message_Type.交易, string.Format("{0}商品{1}加快一天补货", settleSort == 1 ? "黑市" : "聚落", (Resource_Type)DataManager.TransactionDefines[settleSort][transactionObjectId].Subtype));
            if (this.purchaseObjectsStatue[settleSort][transactionObjectId][1] == 0)
            {
                this.purchaseObjectsStatue[settleSort][transactionObjectId][0] = DataManager.TransactionDefines[settleSort][transactionObjectId].Amount;//补货
                MessageManager.Instance.AddMessage(Message_Type.交易, string.Format("{0}商品{1}补货", settleSort == 1 ? "黑市" : "聚落", (Resource_Type)DataManager.TransactionDefines[settleSort][transactionObjectId].Subtype));
            }
        }

        /// <summary>
        /// 增减敌意值
        /// </summary>
        /// <param name="settleSort"></param>
        /// <param name="value"></param>
        /// <param name="isAdd"></param>
        public void AddOrSubtractHotility(int settleSort,float value,bool isAdd)
        {
            if(isAdd)
            {
                if (this.hotility[settleSort] >= 100f)
                {
                    return;
                }
                this.hotility[settleSort] += value;

                if (this.hotility[settleSort] > 100f)//敌意值不会超过100
                {
                    this.hotility[settleSort] = 100f;
                }
            }
            else
            {
                if (this.hotility[settleSort] <= 1f)
                {
                    return;
                }
                this.hotility[settleSort] -= value;
                if (this.hotility[settleSort] < 1f)
                {
                    this.hotility[settleSort] = 1f;
                }
            }
        }

        /// <summary>
        /// 扩张冲突区域
        /// </summary>
        /// <param name="pos"></param>
        public void ExpendClashArea(Vector2Int pos,int clashType)
        {
            List<Vector2Int> relativePos= new List<Vector2Int>() { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };
            foreach(Vector2Int rP in relativePos)
            {
                Vector2Int newPos = pos + rP;
                if (!PlotManager.Instance.plots.ContainsKey(newPos))//不在地图上
                    continue;
                if(PlotManager.Instance.plotTypeSepical.ContainsKey(newPos))//特殊地块
                    continue;
                if (PlotManager.Instance.plots[newPos].eventArea==null)
                {
                    int n = Random.Range(0, 100);
                    if (n < 10)
                    {
                        PlotManager.Instance.plots[newPos].ChangeToClashType(clashType);
                    }
                }

            }
        }

        /// <summary>
        /// 回合结束
        /// </summary>
        public void RoundOver()
        {
            //补货
            foreach (int i in this.purchaseObjectsStatue.Keys)
            {
                foreach (int j in this.purchaseObjectsStatue[i].Keys)
                {
                    if (DataManager.TransactionDefines[i][j].TransactionType==Transaction_Type.蓝图)
                    {
                        continue; //蓝图不需要补货
                    }

                    if (this.purchaseObjectsStatue[i][j][1] > 0)
                    {
                        this.purchaseObjectsStatue[i][j][1]--;//冷却时间减少
                        if (this.purchaseObjectsStatue[i][j][1]==0)
                        {
                            this.purchaseObjectsStatue[i][j][0] = DataManager.TransactionDefines[i][j].Amount;//补货
                            MessageManager.Instance.AddMessage(Message_Type.交易, string.Format("{0}商品{1}补货", i==1 ? "黑市" : "聚落", (Resource_Type)DataManager.TransactionDefines[i][j].Subtype));
                        }
                    }
                }
            }

            //出售
            foreach (int i in DataManager.TransactionDefines.Keys)
            {
                foreach (int j in DataManager.TransactionDefines[i].Keys)
                {
                    this.sellObjectsStatue[i][j] = DataManager.TransactionDefines[i][j].Amount;
                }
            }

            List<ClashArea> clashAreas = new List<ClashArea>();

            foreach(ClashArea cA in this.EventAreas[1])//对抗区域
            {
                clashAreas.Add(cA);
            }

            foreach (ClashArea cA in clashAreas)
            {
                cA.RoundOver();
            }

        }
    }
}