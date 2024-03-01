using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ENTITY
{
    public class Wanderer : Entity
    {
        [SerializeField, LabelText("ID"), ReadOnly]
        public int id;


        private void Update()
        {
            this.LookToCamera();
        }
    }
}
