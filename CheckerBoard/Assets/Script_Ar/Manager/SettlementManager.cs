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
        [SerializeField, LabelText("所有的人类聚落"),ReadOnly]
        public Dictionary<Vector2Int,HumanSettlement> humanSettlements = new Dictionary<Vector2Int, HumanSettlement>();

        [SerializeField, LabelText("所有的机械聚落"), ReadOnly]
        public Dictionary<Vector2Int, RobotSettlement> robotSettlements =new Dictionary<Vector2Int, RobotSettlement>();

        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {
            this.EliminateSettlements();
        }

        public void Restart()
        {
            this.Init();
            MainThreadDispatcher.StartUpdateMicroCoroutine(GetSettlements());
        }

        public void ReadArchive()
        {
            this.Init();
            MainThreadDispatcher.StartUpdateMicroCoroutine(ReadSettlements());
        }

        /// <summary>
        /// 生成聚落
        /// </summary>
        /// <returns></returns>
        IEnumerator GetSettlements()
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2Int v2 = GetRandomPos();
                this.GetSettlement(i >= 3, v2);
                yield return null;
            }
        }
        /// <summary>
        /// 读取聚落协程
        /// </summary>
        /// <returns></returns>
        IEnumerator ReadSettlements()
        {
            foreach (var settlementData in ArchiveManager.archive.settlementData)
            {
                this.GetSettlement(settlementData.isHumanSettlement, settlementData.pos);
                yield return null;
            }
        }

        /// <summary>
        /// 生成单个聚落
        /// </summary>
        /// <param name="isHumanSettlement"></param>
        void GetSettlement(bool isHumanSettlement,Vector2Int pos)
        {
            GameObject gameObject = isHumanSettlement ? GameObjectPool.Instance.HumanSettlements.Get() : GameObjectPool.Instance.RobotSettlements.Get();
            gameObject.transform.parent = this.transform;

            var settlement = gameObject.GetComponent<Settlement>();
            Plot plot = PlotManager.Instance.plots[pos];
            settlement.SetInfo(plot);//设置聚落信息
            settlement.eliminateSettlement.Subscribe(sm =>
            {
                this.EliminateSettlement(sm, isHumanSettlement);
            });


            plot.settlement = settlement;


            if (isHumanSettlement)
            {
                this.humanSettlements.Add(pos, settlement as HumanSettlement);
            }
            else
            {
                this.robotSettlements.Add(pos, settlement as RobotSettlement);
            }
        }
        /// <summary>
        /// 获得随机位置
        /// </summary>
        /// <returns></returns>
        Vector2Int GetRandomPos()
        {
            //try
            //{
                int x = 0;
                int y = 0;
                Vector2Int v2 = Vector2Int.zero;
                do
                {
                    x = Random.value > 0.5f ? Random.Range(-5, -1) : Random.Range(2, 6);
                    y = Random.value > 0.5f ? Random.Range(-3, -1) : Random.Range(2, 4);
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
        /// 消除聚落
        /// </summary>
        /// <param name="aimSettlement"></param>
        /// <param name="isHumeSettlement"></param>
        void EliminateSettlement(Settlement aimSettlement,bool isHumeSettlement)
        {
            PlotManager.Instance.plots[aimSettlement.pos].settlement = null;
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
        ///  与聚落触发事件
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
                case Event_Type.交易:
                    settlement.Transaction();
                    break;
                case Event_Type.战斗:
                    settlement.Confrontation();
                    break;
                case Event_Type.正常:
                    settlement.Normal();
                    break;
            }
        }
    }
}