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

        [SerializeField, LabelText("流浪者当前的板块"), ReadOnly]
        public Plot plot;//流浪者当前的板块

        private void Update()
        {
            this.LookToCamera();
        }
    }
}
