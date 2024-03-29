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
            this.transform.position = new Vector3(plot.pos.x, plot.pos.y,this.transform.position.z);
            this.pos = plot.pos;
            this.type = type;

            //this.SpendResource();

            this.AddHostility();
        }

        /// <summary>
        /// 增加敌意值
        /// </summary>
        public void AddHostility()
        {
            //增加聚落板块的敌意
            if (EventAreaManager.Instance.EventAreas[0].ContainsKey(pos))//如果该板块为聚落
            {
                (EventAreaManager.Instance.EventAreas[0][pos] as Settle).AddHotility(this.hostilityToHuman);
            }
        }

    }
}
