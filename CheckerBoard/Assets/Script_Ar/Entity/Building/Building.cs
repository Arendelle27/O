using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
    public class Building : Entity
    {
        [SerializeField, LabelText("建筑的位置"), ReadOnly]
        public Vector2Int pos;

        [SerializeField, LabelText("建筑的类型"), ReadOnly]
        public Building_Type type;

        private void Start()
        {
            this.LookToCamera();
        }

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="plot"></param>
        /// <param name="type"></param>
        public void SetInfo(Plot plot,Building_Type type)
        {
            this.transform.position = new Vector3(plot.pos.x, plot.pos.y,this.transform.position.z);
            this.pos = plot.pos;
            this.type = type;

            this.SR.sprite = ScriptableObjectPool.buildingScriptList[(int)type].sprite;
        }

    }
}
