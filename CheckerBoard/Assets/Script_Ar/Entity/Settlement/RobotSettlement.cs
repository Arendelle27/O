using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
    public class RobotSettlement : Settlement
    {

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