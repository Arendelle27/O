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
    public class SettlementManager : MonoSingleton<SettlementManager>
    {
        [SerializeField, LabelText("���е��������"),ReadOnly]
        public Dictionary<Vector2Int,HumanSettlement> humanSettlements = new Dictionary<Vector2Int, HumanSettlement>();

        [SerializeField, LabelText("���еĻ�е����"), ReadOnly]
        public Dictionary<Vector2Int, RobotSettlement> robotSettlements =new Dictionary<Vector2Int, RobotSettlement>();

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public void Init()
        {
            this.EliminateSettlements();

            MainThreadDispatcher.StartUpdateMicroCoroutine(GetSettlements());
        }

        /// <summary>
        /// ���ɾ���
        /// </summary>
        /// <returns></returns>
        IEnumerator GetSettlements()
        {
            for (int i = 0; i < 6; i++)
            {
                GetSettlement(i >= 3);
                yield return null;
            }
        }

        /// <summary>
        /// ���ɵ�������
        /// </summary>
        /// <param name="isHumanSettlement"></param>
        public void GetSettlement(bool isHumanSettlement)
        {
            GameObject gameObject = isHumanSettlement ? GameObjectPool.Instance.HumanSettlements.Get() : GameObjectPool.Instance.RobotSettlements.Get();
            gameObject.transform.parent = this.transform;

            Vector2Int v2 = GetRandomPos();

            var settlement = gameObject.GetComponent<Settlement>();
            Plot plot = PlotManager.Instance.grids[v2];
            settlement.SetInfo(plot);//���þ�����Ϣ
            settlement.eliminateSettlement.Subscribe(sm =>
            {
                this.EliminateSettlement(sm, isHumanSettlement);
            });


            plot.settlement = settlement;


            if (isHumanSettlement)
            {
                this.humanSettlements.Add(v2, settlement as HumanSettlement);
            }
            else
            {
                this.robotSettlements.Add(v2, settlement as RobotSettlement);
            }
        }
        /// <summary>
        /// ������λ��
        /// </summary>
        /// <returns></returns>
        public Vector2Int GetRandomPos()
        {
            //try
            //{
                int x = 0;
                int y = 0;
                Vector2Int v2 = Vector2Int.zero;
                do
                {
                    x = Random.value > 0.5f ? Random.Range(-4, -1) : Random.Range(2, 5);
                    y = Random.value > 0.5f ? Random.Range(-4, -1) : Random.Range(2, 5);
                    v2 = new Vector2Int(x, y);
                }
                while (this.humanSettlements.ContainsKey(v2) || this.robotSettlements.ContainsKey(v2));
                return v2;
            //}
            //catch (System.Exception e)
            //{
            //    return Vector2Int.zero;
            //}
        }

        void EliminateSettlements()
        {
            for (int i = 0; i < humanSettlements.Count;)
            {
                var item = humanSettlements.ElementAt(i);
                this.EliminateSettlement(item.Value, true);
            }
            for (int i = 0; i < robotSettlements.Count;)
            {
                var item = robotSettlements.ElementAt(i);
                this.EliminateSettlement(item.Value, false);
            }
        }


        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="aimSettlement"></param>
        /// <param name="isHumeSettlement"></param>
        void EliminateSettlement(Settlement aimSettlement,bool isHumeSettlement)
        {
            PlotManager.Instance.grids[aimSettlement.pos].settlement = null;
            if (isHumeSettlement)
            {
                GameObjectPool.Instance.HumanSettlements.Release(aimSettlement.gameObject);
                this.humanSettlements.Remove(aimSettlement.pos);
            }
            else
            {
                GameObjectPool.Instance.RobotSettlements.Release(aimSettlement.gameObject);
                this.robotSettlements.Remove(aimSettlement.pos);
            }
        }

        /// <summary>
        ///  ����䴥���¼�
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pos"></param>
        public void TriggerEvent(Event_Type type, Vector2Int pos)
        {
            Settlement settlement = null;
            if(this.humanSettlements.ContainsKey(pos))
            {
                settlement = this.humanSettlements[pos];
            }
            else if (this.robotSettlements.ContainsKey(pos))
            {
                settlement = this.robotSettlements[pos];
            }

            if(settlement==null)
            {
                return;
            }

            switch (type)
            {
                case Event_Type.����:
                    settlement.Transaction();
                    break;
                case Event_Type.ս��:
                    settlement.Confrontation();
                    break;
                case Event_Type.����:
                    settlement.Normal();
                    break;
            }
        }
    }
}