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

        [SerializeField, LabelText("�����ߵ�ǰ�İ��"), ReadOnly]
        public Plot plot;//�����ߵ�ǰ�İ��

        [SerializeField, LabelText("�����ߵĵȼ�"), ReadOnly]
        public int level;//�����ߵ�ǰ�İ��

        private void Start()
        {
            this.ObserveEveryValueChanged(_ => this.level).Subscribe(_ =>
            {
                //�仯ʱ��������UI
                (UIMain.Instance.uiPanels[1] as UIGamePanel).leaveValue.text = this.level.ToString();
                Debug.Log("�ȼ��仯");
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
