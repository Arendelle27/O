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
        public List<List<EventArea>> EventAreas = new List<List<EventArea>>(4) 
        {
            new List < EventArea >(),//����
            new List < EventArea >(),//�Կ�
            new List < EventArea >(),//�ż�
            new List < EventArea >()//����
        };

        //������������ȴ
        public Dictionary<int,Dictionary<int,List<int>>> purchaseObjectsStatue=new Dictionary<int, Dictionary<int, List<int>>>() 
        {
            {0, new Dictionary<int, List<int>>()},//�ۼ���
            {1, new Dictionary<int, List<int>>()} //����
        };
        //����ֵ,0Ϊʣ��������1Ϊ��ȴʱ��

        //������������ȴ
        public Dictionary<int, Dictionary<int, int>> sellObjectsStatue = new Dictionary<int, Dictionary<int, int>>()
        {
            {0, new Dictionary<int, int>()},//�ۼ���
            {1, new Dictionary<int, int>()} //����
        };
        //����ֵ,Ϊʣ������

        //����ֵ
        public List<float> hotility=new List<float>(2) {5f,5f};
        //0Ϊ���࣬1Ϊ������,�������ֵΪ5

        //��ѡ�е��¼�����
        public EventArea selectedEventArea;
        //���״���
        public UITransactionWindow uITransactionWindow;

        /// <summary>
        /// ��ʼ��
        /// </summary>
        void Init()
        {

        }

        /// <summary>
        /// �ؿ�
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
        /// ����
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
        /// ���ɵ�������
        /// </summary>
        /// <param name="isHumanSettlement"></param>
        public EventArea GetEventArea(Event_Area_Type type,Plot plot)
        {
            switch (type)
            {
                case Event_Area_Type.����:
                    Settle settle = new Settle(plot);
                    this.AddEventArea(0, settle);
                    return settle;
                case Event_Area_Type.�Կ�:
                    ClashArea clashArea = new ClashArea(plot);
                    this.AddEventArea(1, clashArea);
                    return clashArea;
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
        /// ����¼�����
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="eventArea"></param>
        public void AddEventArea(int sort,EventArea eventArea)
        {
            this.EventAreas[sort].Add(eventArea);
        }
        
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
        public void EliminateEventArea(int sort,EventArea eventArea)
        {
            this.EventAreas[sort].Remove(eventArea);//�Ƴ��¼�����
        }

        /// <summary>
        /// ���ѲƸ�������ȴʱ��
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
            if (DataManager.TransactionDefines[settleSort][transactionObjectId].TransactionType == Transaction_Type.��ͼ)
            {
                Debug.Log("��ͼ���Ჹ��");
                MessageManager.Instance.AddMessage(Message_Type.����, string.Format("��ͼ���Ჹ��"));
                return false;
            }
            if (this.purchaseObjectsStatue[settleSort][transactionObjectId][0] == DataManager.TransactionDefines[settleSort][transactionObjectId].Amount)
            {
                Debug.Log("����Ҫ����");
                MessageManager.Instance.AddMessage(Message_Type.����, string.Format("����Ҫ����"));
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
                MessageManager.Instance.AddMessage(Message_Type.����, string.Format("��Ǯ���㹻����"));
                return false;
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="settleSort"></param>
        /// <param name="transactionObjectId"></param>
        void ReduceCoolingRound(int settleSort,int transactionObjectId)
        {
            this.purchaseObjectsStatue[settleSort][transactionObjectId][1]--;
            MessageManager.Instance.AddMessage(Message_Type.����, string.Format("{0}��Ʒ{1}�ӿ�һ�첹��", settleSort == 1 ? "����" : "����", (Resource_Type)DataManager.TransactionDefines[settleSort][transactionObjectId].Subtype));
            if (this.purchaseObjectsStatue[settleSort][transactionObjectId][1] == 0)
            {
                this.purchaseObjectsStatue[settleSort][transactionObjectId][0] = DataManager.TransactionDefines[settleSort][transactionObjectId].Amount;//����
                MessageManager.Instance.AddMessage(Message_Type.����, string.Format("{0}��Ʒ{1}����", settleSort == 1 ? "����" : "����", (Resource_Type)DataManager.TransactionDefines[settleSort][transactionObjectId].Subtype));
            }
        }

        /// <summary>
        /// ��������ֵ
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

                if (this.hotility[settleSort] > 100f)//����ֵ���ᳬ��100
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
        /// ���ų�ͻ����
        /// </summary>
        /// <param name="pos"></param>
        public void ExpendClashArea(Vector2Int pos,int clashType)
        {
            List<Vector2Int> relativePos= new List<Vector2Int>() { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };
            foreach(Vector2Int rP in relativePos)
            {
                Vector2Int newPos = pos + rP;
                if (!PlotManager.Instance.plots.ContainsKey(newPos))//���ڵ�ͼ��
                    continue;
                if(PlotManager.Instance.plotTypeSepical.ContainsKey(newPos))//����ؿ�
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
        /// �غϽ���
        /// </summary>
        public void RoundOver()
        {
            //����
            foreach (int i in this.purchaseObjectsStatue.Keys)
            {
                foreach (int j in this.purchaseObjectsStatue[i].Keys)
                {
                    if (DataManager.TransactionDefines[i][j].TransactionType==Transaction_Type.��ͼ)
                    {
                        continue; //��ͼ����Ҫ����
                    }

                    if (this.purchaseObjectsStatue[i][j][1] > 0)
                    {
                        this.purchaseObjectsStatue[i][j][1]--;//��ȴʱ�����
                        if (this.purchaseObjectsStatue[i][j][1]==0)
                        {
                            this.purchaseObjectsStatue[i][j][0] = DataManager.TransactionDefines[i][j].Amount;//����
                            MessageManager.Instance.AddMessage(Message_Type.����, string.Format("{0}��Ʒ{1}����", i==1 ? "����" : "����", (Resource_Type)DataManager.TransactionDefines[i][j].Subtype));
                        }
                    }
                }
            }

            //����
            foreach (int i in DataManager.TransactionDefines.Keys)
            {
                foreach (int j in DataManager.TransactionDefines[i].Keys)
                {
                    this.sellObjectsStatue[i][j] = DataManager.TransactionDefines[i][j].Amount;
                }
            }

            List<ClashArea> clashAreas = new List<ClashArea>();

            foreach(ClashArea cA in this.EventAreas[1])//�Կ�����
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