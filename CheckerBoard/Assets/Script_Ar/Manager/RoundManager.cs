using Managers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            ArchiveManager.RoundManagerData roundManagerData = ArchiveManager.archive.roundManagerData;
            this.roundNumber = roundManagerData.roundNumber;
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

            //NpcManager.Instance.RoundOver(this.roundNumber);//npc�����غ�
            /*����ǰ*/
            //this.stage+= EventManager.Instance.StageDecision(this.stage,this.roundNumber);//�׶ξ���

            QuestManager.Instance.QuestEndByRound(this.roundNumber);//�׶ξ���

            /*�غϽ���*/
            this.roundNumber++;//�غ�����1
            NpcManager.Instance.RoundOver(this.roundNumber);//npc�����غ�
            SoundManager.Instance.RoundStart(this.roundNumber);//���ֿ�ʼ�غ�

            ChatManager.Instance.RoundStart(this.roundNumber);//���쿪ʼ�غ�

            if(this.roundNumber>30)
            {
                return;
            }
            /*�غϿ�ʼ*/
            MessageManager.Instance.RoundOver();//ˢ����Ϣ

            NpcManager.Instance.RoundStart(this.roundNumber);//npc��ʼ�غ�

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
    }
}
