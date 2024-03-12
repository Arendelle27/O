using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ENTITY
{
    public class Settlement : Entity
    {
        [SerializeField, LabelText("聚落的位置"), ReadOnly]
        public Vector2Int pos;

        [SerializeField, LabelText("消除聚落事件"), ReadOnly]
        public Subject<Settlement> eliminateSettlement = new Subject<Settlement>();

        [SerializeField, LabelText("聚落的敌意值"), ReadOnly]
        public int hotility = 0;

        [SerializeField, LabelText("聚落的交易状态"), ReadOnly]
        public bool canTransaction = true;

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="pos"></param>
        public void SetInfo(Plot plot)
        {
            this.pos = plot.pos;
            this.transform.position = new Vector3(plot.pos.x, plot.pos.y,this.transform.position.z);

            this.hotility = 0;
            this.canTransaction = true;
        }

        private void Update()
        {
            this.LookToCamera();
        }

        /// <summary>
        /// 增加敌意值(是否通过交易增加)
        /// </summary>
        public virtual void AddHotility(bool isTransaction)
        {
        }

        /// <summary>
        /// 冲突
        /// </summary>
        public virtual void Confrontation()
        {

        }

        /// <summary>
        /// 交易
        /// </summary>
        public virtual void Transaction()
        {
            if (!this.canTransaction)
                return;

            this.canTransaction = false;

            this.AddHotility(true);
            
        }

        public virtual void Normal()
        {
            Debug.Log("正常事件");
        }
    }

}
