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

        public virtual void SpendResource()
        {

        }

        public virtual void AddHostility()
        {

        }

    }
}
