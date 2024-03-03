using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
    public class DestinationSign : Entity
    {
        [SerializeField, LabelText("目的地提示牌的位置"), ReadOnly]
        public Plot plot;


        private void Update()
        {
            this.LookToCamera();
        }
    }
}

