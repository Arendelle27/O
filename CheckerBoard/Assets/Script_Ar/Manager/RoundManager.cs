using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace MANAGER
{
    public class RoundManager:Singleton<RoundManager>
    {
        //当前回合数
        public int roundNumber = 1;

        int stage;

        [SerializeField, LabelText("通过回合解锁板块"), ReadOnly]
        public Subject<int> unlockPlotByRound = new Subject<int>();

        [SerializeField, LabelText("通过回合解锁建筑"), ReadOnly]
        public Subject<int> unlockBuildingByRound = new Subject<int>();

        public RoundManager()
        {
            this.ObserveEveryValueChanged(_ => this.roundNumber).Subscribe(_ =>
            {
                //变化时更新回合数UI
                (UIMain.Instance.uiPanels[1] as UIGamePanel).roundNumber.text =  this.roundNumber.ToString();
            });

        }

        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {
        }

        /// <summary>
        /// 重开
        /// </summary>
        public void Restart()
        {
            this.Init();
            this.roundNumber = 1;
            this.stage = 0;
        }

        /// <summary>
        /// 读档
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
        /// 结束回合
        /// </summary>
        public void RoundOver()
        {
            WandererManager.Instance.RoundOver();//流浪者和探索小队结束回合

            BuildingManager.Instance.RoundOver();//建筑结束回合

            ResourcesManager.Instance.RoundOver(this.roundNumber);//资源结束回合
            /*结算前*/
           this.stage+= EventManager.Instance.StageDecision(this.stage,this.roundNumber);//阶段决策

            /*回合结束*/
            this.roundNumber++;//回合数加1

            /*回合开始*/
            MessageManager.Instance.RoundOver();//刷新信息

            EventAreaManager.Instance.RoundOver();//事件地区结束回合

            EventManager.Instance.RoundOver();//事件结束回合

            CapabilityManager.Instance.RoundOver();//能力结束回合

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
        /// 根据回合条件解锁建筑类型
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
