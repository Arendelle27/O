using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
    public class HumanSettlement : Settlement
    {

        /// <summary>
        /// 交易
        /// </summary>
        public  override void Transaction()
        {
            base.Transaction();
            Debug.Log("人类聚落交易");
        }

        /// <summary>
        /// 对抗
        /// </summary>
        public override void Confrontation()
        {
            base.Confrontation();
            Debug.Log("人类聚落对抗");
        }
    }

}
