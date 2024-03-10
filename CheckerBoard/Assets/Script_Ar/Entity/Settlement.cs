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

        public Subject<Settlement> eliminateSettlement = new Subject<Settlement>();

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="pos"></param>
        public void SetInfo(Vector2Int pos)
        {
            this.pos = pos;
            this.transform.position = new Vector3(pos.x, pos.y,this.transform.position.z);
        }

        private void Update()
        {
            this.LookToCamera();
        }
    }

}
