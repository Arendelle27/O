using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
    public class HumanSettlement : Settlement
    {

        /// <summary>
        /// ����
        /// </summary>
        public  override void Transaction()
        {
            base.Transaction();
            Debug.Log("������佻��");
        }

        /// <summary>
        /// �Կ�
        /// </summary>
        public override void Confrontation()
        {
            base.Confrontation();
            Debug.Log("�������Կ�");
        }
    }

}
