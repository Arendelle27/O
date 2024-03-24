using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
    public class Building : Entity
    {
        [SerializeField, LabelText("������λ��"), ReadOnly]
        public Vector2Int pos;

        [SerializeField, LabelText("����������"), ReadOnly]
        public Building_Type type;

        public int TID;
        [SerializeField, LabelText("����ʱ������Դ"), ReadOnly]
        public int[] resourcesCost = new int[3];
        [SerializeField, LabelText("ս��"), ReadOnly]
        public int attack;
        [SerializeField, LabelText("����ʱ���ӻ����˵���ֵ"), ReadOnly]
        public int hostilityToRobot;
        [SerializeField, LabelText("����ʱ�����������ֵ"), ReadOnly]
        public int hostilityToHuman;

        private void Start()
        {
            this.LookToCamera();
        }


        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="plot"></param>
        /// <param name="type"></param>
        public virtual void SetInfo(Plot plot,Building_Type type)
        {
            this.transform.position = new Vector3(plot.pos.x, plot.pos.y,this.transform.position.z);
            this.pos = plot.pos;
            this.type = type;

            this.SpendResource();
            this.AddHostility();

        }

        /// <summary>
        /// ����������Դ
        /// </summary>
        public void SpendResource()
        {
            int[] cost = new int[3] { -this.resourcesCost[0], -this.resourcesCost[1], -this.resourcesCost[2] };
            ResourcesManager.Instance.ChangeBuildingResources(cost);
        }

        /// <summary>
        /// ���ӵ���ֵ
        /// </summary>
        public void AddHostility()
        {
            if (SettlementManager.Instance.humanSettlements.ContainsKey(this.pos))
            {
                int hostility = this.hostilityToHuman;
                SettlementManager.Instance.humanSettlements[this.pos].AddHotility(hostility);
            }
            else if (SettlementManager.Instance.robotSettlements.ContainsKey(this.pos))
            {
                int hostility = this.hostilityToRobot;
                SettlementManager.Instance.robotSettlements[this.pos].AddHotility(hostility);
            }
        }

    }
}
