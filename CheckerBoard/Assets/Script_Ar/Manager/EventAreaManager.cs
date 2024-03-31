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
        //���ڵ��¼�����
        public List<Dictionary<Vector2Int,EventArea>> EventAreas = new List<Dictionary<Vector2Int, EventArea>>(4) 
        {
            new Dictionary<Vector2Int, EventArea>(),//����
            new Dictionary<Vector2Int, EventArea>(),//�Կ�
            new Dictionary<Vector2Int, EventArea>(),//�ż�
            new Dictionary<Vector2Int, EventArea>()//����
        };

        //������������ȴ��ȴ
        public Dictionary<int,Dictionary<int,List<int>>> transactionObjectsStatue=new Dictionary<int, Dictionary<int, List<int>>>() 
        {
            {0, new Dictionary<int, List<int>>()},//�ۼ���
            {1, new Dictionary<int, List<int>>()} //����
        };
        //����ֵ,0Ϊʣ��������1Ϊ��ȴʱ��

        //��ͻ����
        public List<ClashArea> clashAreas;

        //��ѡ�е��¼�����
        public EventArea selectedEventArea;
        //���״���
        public UITransactionWindow uITransactionWindow;

        /// <summary>
        /// ��ʼ��
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
        /// ���ɾ���
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
        /// ��ȡ����Э��
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
        /// ���ɵ�������
        /// </summary>
        /// <param name="isHumanSettlement"></param>
        public EventArea GetEventArea(Event_Area_Type type,Plot plot)
        {
            switch (type)
            {
                case Event_Area_Type.����:
                    return new Settle(plot);
                case Event_Area_Type.�Կ�:
                    return new ClashArea(plot);
                case Event_Area_Type.�ż�:
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
        /// ��������¼�����
        /// </summary>
        void EliminateAllEventArea()
        {
            for(int i=0;i<this.EventAreas.Count;i++)
            {
                this.EventAreas[i].Clear();
            }
        }


        /// <summary>
        /// �����¼�����
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="pos"></param>
        void EliminateEventArea(int sort,Vector2Int pos)
        {
            this.EventAreas[sort].Remove(pos);//�Ƴ��¼�����
        }

        /// <summary>
        /// ���ѲƸ�������ȴʱ��
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
                if (DataManager.TransactionDefines[settleSort][transactionObjectId].TransactionType==Transaction_Type.��ͼ)
                {
                    Debug.Log("��ͼ���Ჹ��");
                    return false;
                }
                if(this.transactionObjectsStatue[settleSort][transactionObjectId][0] == DataManager.TransactionDefines[settleSort][transactionObjectId].Amount)
                {
                    Debug.Log("����Ҫ����");
                    return false;
                }


                ResourcesManager.Instance.ChangeWealth(-10);

                this.transactionObjectsStatue[settleSort][transactionObjectId][1] --;
                if (this.transactionObjectsStatue[settleSort][transactionObjectId][1] == 0)
                {
                    this.transactionObjectsStatue[settleSort][transactionObjectId][0] = DataManager.TransactionDefines[settleSort][transactionObjectId].Amount;//����
                }

                return true;
            }
            else
            {
                Debug.Log("��Ǯ����");
                return false;
            }

        }

        /// <summary>
        /// �غϽ���
        /// </summary>
        public void RoundOver()
        {
            foreach (int i in this.transactionObjectsStatue.Keys)
            {
                foreach (int j in this.transactionObjectsStatue[i].Keys)
                {
                    if (DataManager.TransactionDefines[i][j].TransactionType==Transaction_Type.��ͼ)
                    {
                        continue; //��ͼ����Ҫ����
                    }

                    if (this.transactionObjectsStatue[i][j][1] > 0)
                    {
                        this.transactionObjectsStatue[i][j][1]--;//��ȴʱ�����
                        if (this.transactionObjectsStatue[i][j][1]==0)
                        {
                            this.transactionObjectsStatue[i][j][0] = DataManager.TransactionDefines[i][j].Amount;//����
                        }
                    }
                }
            }
        }
    }
}