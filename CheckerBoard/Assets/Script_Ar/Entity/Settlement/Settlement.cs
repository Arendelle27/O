using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ENTITY
{
    public class Settlement : Entity
    {
        [SerializeField, LabelText("�����λ��"), ReadOnly]
        public Vector2Int pos;

        [SerializeField, LabelText("����ĵ���ֵ"), ReadOnly]
        public int hotility = 0;

        [SerializeField, LabelText("����Ľ���״̬"), ReadOnly]
        public bool canTransaction = true;

        [SerializeField, LabelText("���������¼�"), ReadOnly]
        public Subject<Settlement> eliminateSettlement = new Subject<Settlement>();

        public virtual void Start()
        {
            this.LookToCamera();
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="pos"></param>
        public void SetInfo(Plot plot)
        {
            this.pos = plot.pos;
            this.transform.position = new Vector3(plot.pos.x, plot.pos.y,this.transform.position.z);

            this.hotility = 0;
            this.canTransaction = true;
        }

        /// <summary>
        /// ���ӵ���ֵ(�Ƿ�ͨ����������)
        /// </summary>
        public virtual void AddHotility(int value)
        {
            this.hotility+=value;
        }

        /// <summary>
        /// ��ͻ
        /// </summary>
        public virtual void Confrontation()
        {

        }

        /// <summary>
        /// ����
        /// </summary>
        public virtual void Transaction()
        {
            if (!this.canTransaction)
                return;

            this.canTransaction = false;

            
        }

        public virtual void Normal()
        {
            Debug.Log("�����¼�");
        }
    }

}
