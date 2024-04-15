using MANAGER;
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

        public int TID;
        [SerializeField, LabelText("机械名称"), ReadOnly]
        public string buildingname;
        [SerializeField, LabelText("描述"), ReadOnly]
        public string description;
        [SerializeField, LabelText("建造时消耗资源"), ReadOnly]
        public int[] resourcesCost = new int[3];
        [SerializeField, LabelText("战力"), ReadOnly]
        public int attack;
        [SerializeField, LabelText("建造时增加机器人敌意值"), ReadOnly]
        public int hostilityToRobot;
        [SerializeField, LabelText("建造时增加人类敌意值"), ReadOnly]
        public int hostilityToHuman;

        private void Start()
        {
            this.LookToCamera();
        }


        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="plot"></param>
        /// <param name="type"></param>
        public virtual void SetInfo(Plot plot,Building_Type type)
        {
            this.transform.position = plot.transform.position + new Vector3(0, 0, ParameterConfig.entityHigh);
            this.pos = plot.pos;
            this.type = type;

            //this.SpendResource();

        }

    }
}
