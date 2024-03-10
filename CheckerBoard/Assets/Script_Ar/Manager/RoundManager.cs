using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace MANAGER
{
    public class RoundManager : Singleton<RoundManager>
    {
        //��ǰ�غ���
        public int roundNumber = 1;

        public RoundManager()
        {
            this.ObserveEveryValueChanged(_ => this.roundNumber).Subscribe(_ =>
            {
                //�仯ʱ���»غ���UI
                (UIMain.Instance.uiPanels[1] as UIGamePanel).roundNumber.text =  this.roundNumber.ToString();
            });
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public void Init()
        {
            this.roundNumber = 1;
        }

        /// <summary>
        /// �����غ�
        /// </summary>
        public void RoundOver()
        {
            this.roundNumber++;//�غ�����1

            BuildingManager.Instance.RoundOver();//���������غ�

            DataManager.Instance.RoundOver();//��Դ�����غ�

            WandererManager.Instance.WandererMoveToDestination();
        }
    }
}