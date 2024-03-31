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
        public List<Dictionary<Vector2Int,EventArea>> EventAreas = new List<Dictionary<Vector2Int, EventArea>>(4) 
        {
            new Dictionary<Vector2Int, EventArea>(),//交易
            new Dictionary<Vector2Int, EventArea>(),//对抗
            new Dictionary<Vector2Int, EventArea>(),//遗迹
            new Dictionary<Vector2Int, EventArea>()//剧情
        };

        //交易数量和冷却冷却
        public Dictionary<int,Dictionary<int,List<int>>> transactionObjectsStatue=new Dictionary<int, Dictionary<int, List<int>>>() 
        {
            {0, new Dictionary<int, List<int>>()},//聚集地
            {1, new Dictionary<int, List<int>>()} //黑市
        };
        //最后的值,0为剩余数量，1为冷却时间

        //冲突地区
        public List<ClashArea> clashAreas;

        //被选中的事件地区
        public EventArea selectedEventArea;
        //交易窗口
        public UITransactionWindow uITransactionWindow;

        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {
            this.EliminateAllEventArea();

        }

        public void Restart()
        {
            this.Init();
            foreach (int i in DataManager.TransactionDefines.Keys)
            {
                foreach (int j in DataManager.TransactionDefines[i].Keys)
                {
                    if (DataManager.TransactionDefines[i][j].PurchaseOrSell == 0)
                    {
                        if (this.transactionObjectsStatue[i].ContainsKey(j))
                        {
                            this.transactionObjectsStatue[i][j][0] = DataManager.TransactionDefines[i][j].Amount;
                            this.transactionObjectsStatue[i][j][1] = 0;
                        }
                        else
                        {
                            this.transactionObjectsStatue[i].Add(j, new List<int>(2) { DataManager.TransactionDefines[i][j].Amount, 0 });
                        }
                    }
                }
            }
            //MainThreadDispatcher.StartUpdateMicroCoroutine(GetSettlements());
        }

        public void ReadArchive()
        {
            //this.Init();
            //MainThreadDispatcher.StartUpdateMicroCoroutine(ReadSettlements());
        }

        /// <summary>
        /// 生成聚落
        /// </summary>
        /// <returns></returns>
        //IEnumerator GetSettlements()
        //{
        //    for (int i = 0; i < 6; i++)
        //    {
        //        Vector2Int v2 = GetRandomPos();
        //        this.GetSettlement(i >= 3, v2);
        //        yield return null;
        //    }
        //}
        /// <summary>
        /// 读取聚落协程
        /// </summary>
        /// <returns></returns>
        //IEnumerator ReadSettlements()
        //{
        //    foreach (var settlementData in ArchiveManager.archive.settlementData)
        //    {
        //        this.GetSettlement(settlementData.isHumanSettlement, settlementData.pos);
        //        yield return null;
        //    }
        //}

        /// <summary>
        /// 生成单个聚落
        /// </summary>
        /// <param name="isHumanSettlement"></param>
        public EventArea GetEventArea(Event_Area_Type type,Plot plot)
        {
            switch (type)
            {
                case Event_Area_Type.交易:
                    return new Settle(plot);
                case Event_Area_Type.对抗:
                    return new ClashArea(plot);
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
        void EliminateEventArea(int sort,Vector2Int pos)
        {
            this.EventAreas[sort].Remove(pos);//移除事件地区
        }

        /// <summary>
        /// 花费财富减少冷却时间
        /// </summary>
        /// <param name="transactionObjectId"></param>
        /// <returns></returns>
        public bool ReduceCoolingRoundBySpend(int transactionObjectId)
        {
            if(ResourcesManager.Instance.wealth>=50)
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
                if (DataManager.TransactionDefines[settleSort][transactionObjectId].TransactionType==Transaction_Type.蓝图)
                {
                    Debug.Log("蓝图不会补货");
                    return false;
                }
                if(this.transactionObjectsStatue[settleSort][transactionObjectId][0] == DataManager.TransactionDefines[settleSort][transactionObjectId].Amount)
                {
                    Debug.Log("不需要补货");
                    return false;
                }


                ResourcesManager.Instance.ChangeWealth(-10);

                this.transactionObjectsStatue[settleSort][transactionObjectId][1] --;
                if (this.transactionObjectsStatue[settleSort][transactionObjectId][1] == 0)
                {
                    this.transactionObjectsStatue[settleSort][transactionObjectId][0] = DataManager.TransactionDefines[settleSort][transactionObjectId].Amount;//补货
                }

                return true;
            }
            else
            {
                Debug.Log("金钱不足");
                return false;
            }

        }

        /// <summary>
        /// 回合结束
        /// </summary>
        public void RoundOver()
        {
            foreach (int i in this.transactionObjectsStatue.Keys)
            {
                foreach (int j in this.transactionObjectsStatue[i].Keys)
                {
                    if (DataManager.TransactionDefines[i][j].TransactionType==Transaction_Type.蓝图)
                    {
                        continue; //蓝图不需要补货
                    }

                    if (this.transactionObjectsStatue[i][j][1] > 0)
                    {
                        this.transactionObjectsStatue[i][j][1]--;//冷却时间减少
                        if (this.transactionObjectsStatue[i][j][1]==0)
                        {
                            this.transactionObjectsStatue[i][j][0] = DataManager.TransactionDefines[i][j].Amount;//补货
                        }
                    }
                }
            }
        }
    }
}