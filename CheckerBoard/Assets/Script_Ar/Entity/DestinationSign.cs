using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ENTITY
{
    public class DestinationSign : Entity
    {
        [SerializeField, LabelText("Ŀ�ĵ���ʾ�Ƶ�λ��"), ReadOnly]
        public Plot plot;


        private void Update()
        {
            this.LookToCamera();
        }
    }
}

