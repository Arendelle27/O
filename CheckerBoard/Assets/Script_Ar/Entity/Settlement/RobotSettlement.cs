using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
    public class RobotSettlement : Settlement
    {
        public override void AddHotility(bool isTransaction)
        {
            if(isTransaction)
            {
                this.hotility += 10;//ͨ����������
                Debug.Log("��е����ͨ���������ӵ���ֵ");
            }
            else
            {
                this.hotility += 20;//ͨ����������
                Debug.Log("��е����ͨ���������ӵ���ֵ");
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        public override void Transaction()
        {
            base.Transaction();
            Debug.Log("��е���佻��");
        }

        /// <summary>
        /// �Կ�
        /// </summary>
        public override void Confrontation()
        {
            base.Confrontation();
            Debug.Log("��е����Կ�");
        }
    }

}
