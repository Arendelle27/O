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
                //�仯ʱ����UI
                UIMain.Instance.gamePanel.roundNumber.text =  this.roundNumber.ToString();
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
            WandererManager.Instance.WandererMoveToDestination();
        }
    }
}
