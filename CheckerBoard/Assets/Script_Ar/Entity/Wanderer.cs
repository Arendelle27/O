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

        [SerializeField, LabelText("流浪者的等级"), ReadOnly]
        public int level;//流浪者当前的板块

        private void Start()
        {
            this.ObserveEveryValueChanged(_ => this.level).Subscribe(_ =>
            {
                //变化时更新能量UI
                (UIMain.Instance.uiPanels[1] as UIGamePanel).leaveValue.text = this.level.ToString();
                Debug.Log("等级变化");
            });
        }

        private void OnEnable()
        {
            this.level = 1;
        }

        private void Update()
        {
            this.LookToCamera();
        }
    }
}
