using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace MANAGER
{
    public class RoundManager:Singleton<RoundManager>
    {
        //��ǰ�غ���
        public int roundNumber = 1;

        int stage;

        [SerializeField, LabelText("ͨ���غϽ������"), ReadOnly]
        public Subject<int> unlockPlotByRound = new Subject<int>();

        [SerializeField, LabelText("ͨ���غϽ�������"), ReadOnly]
        public Subject<int> unlockBuildingByRound = new Subject<int>();

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
        void Init()
        {
        }

        /// <summary>
        /// �ؿ�
        /// </summary>
        public void Restart()
        {
            this.Init();
            this.roundNumber = 1;
            this.stage = 0;
        }

        /// <summary>
        /// ����
        /// </summary>
        public void ReadArchive()
        {
            this.Init();
            //this.roundNumber = ArchiveManager.archive.roundNumber;
        }

        public void GameOver()
        {
            this.roundNumber = 0;
        }

        /// <summary>
        /// �����غ�
        /// </summary>
        public void RoundOver()
        {
            WandererManager.Instance.RoundOver();//�����ߺ�̽��С�ӽ����غ�

            BuildingManager.Instance.RoundOver();//���������غ�

            ResourcesManager.Instance.RoundOver(this.roundNumber);//��Դ�����غ�
            /*����ǰ*/
           this.stage+= EventManager.Instance.StageDecision(this.stage,this.roundNumber);//�׶ξ���

            /*�غϽ���*/
            this.roundNumber++;//�غ�����1

            /*�غϿ�ʼ*/
            MessageManager.Instance.RoundOver();//ˢ����Ϣ

            EventAreaManager.Instance.RoundOver();//�¼����������غ�

            EventManager.Instance.RoundOver();//�¼������غ�

            CapabilityManager.Instance.RoundOver();//���������غ�

            if (this.unlockPlotByRound!=null)
            {
                this.unlockPlotByRound.OnNext(this.roundNumber);
            }
            if(this.unlockBuildingByRound!=null)
            {
                this.unlockBuildingByRound.OnNext(this.roundNumber);
            }

        }

        /// <summary>
        /// ���ݻغ�����������������
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool UnlockBuildingTypeByResource(int amount)
        {
            if(this.roundNumber==amount)
            {
                return true;
            }
            return false;
        }
    }
}
