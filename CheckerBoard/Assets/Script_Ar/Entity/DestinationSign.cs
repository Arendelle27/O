using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
    public class DestinationSign : Entity
    {
        [SerializeField, LabelText("目的地提示牌的位置"), ReadOnly]
        public Vector2Int pos;

        private void Start()
        {
            this.LookToCamera();
        }

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="pos"></param>
        public void SetInfo(Plot plot)
        {
            this.pos= plot.pos;
            Vector3 v2= plot.transform.position + new Vector3(0, 0, ParameterConfig.entityHigh);
            this.transform.position = v2;

            this.gameObject.SetActive(true);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}

