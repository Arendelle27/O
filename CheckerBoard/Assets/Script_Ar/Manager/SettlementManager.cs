using ENTITY;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

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
        public void Init()
        {
            this.humanSettlements.Clear();
            this.robotSettlements.Clear();

            for(int i=0;i<6;i++)
            {
                this.GetSettlement(i>=3);
            }
        }

        /// <summary>
        /// 生成聚落
        /// </summary>
        /// <param name="isHumanSettlement"></param>
        public void GetSettlement(bool isHumanSettlement)
        {
            GameObject gameObject = isHumanSettlement ? GameObjectPool.Instance.HumanSettlements.Get() : GameObjectPool.Instance.RobotSettlements.Get();
            Instantiate(gameObject, this.transform);

            Vector2Int v2= GetRandomPos();

            var settlement = gameObject.GetComponent<Settlement>();
            settlement.SetInfo(v2);//设置聚落信息
            settlement.eliminateSettlement.Subscribe(sm =>
            {
                this.eliminateSettlement(sm,isHumanSettlement);
            });


            PlotManager.Instance.grids[v2].settlement = settlement;


            if(isHumanSettlement)
            {
                this.humanSettlements.Add(v2, settlement as HumanSettlement);
            }
            else
            {
                this.robotSettlements.Add(v2, settlement as RobotSettlement);
            }
        }
        /// <summary>
        /// 获得随机位置
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

        /// <summary>
        /// 消除聚落
        /// </summary>
        /// <param name="aimSettlement"></param>
        /// <param name="isHumeSettlement"></param>
        void eliminateSettlement(Settlement aimSettlement,bool isHumeSettlement)
        {
            PlotManager.Instance.grids[aimSettlement.pos].settlement = null;
            if(isHumeSettlement)
            {
                humanSettlements.Remove(aimSettlement.pos);
            }
            else
            {
                this.robotSettlements.Remove(aimSettlement.pos);
            }
            (isHumeSettlement ? GameObjectPool.Instance.HumanSettlements : GameObjectPool.Instance.RobotSettlements).Release(aimSettlement.gameObject);
        }
    }
}