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

        [SerializeField, LabelText("通过回合解锁板块"), ReadOnly]
        public Subject<int> unLoadByRound = new Subject<int>();

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
        }

        /// <summary>
        /// 读档
        /// </summary>
        public void ReadArchive()
        {
            this.Init();
            this.roundNumber = ArchiveManager.archive.roundNumber;
        }

        /// <summary>
        /// 结束回合
        /// </summary>
        public void RoundOver()
        {


            BuildingManager.Instance.RoundOver();//建筑结束回合

            ResourcesManager.Instance.RoundOver();//资源结束回合

            this.StageDecision();//阶段决策

            this.roundNumber++;//回合数加1

            //下一回合开始
            this.unLoadByRound.OnNext(this.roundNumber);

            SettlementManager.Instance.TriggerEvent(Event_Type.正常,WandererManager.Instance.wanderer.plot.pos);

            SettlementManager.Instance.TriggerEvent(Event_Type.战斗, WandererManager.Instance.wanderer.plot.pos);
        }

        /// <summary>
        /// 阶段结算
        /// </summary>
        void StageDecision()
        {
            if(this.roundNumber%5==0)
            {
                Debug.Log("阶段结算");
                ResourcesManager.Instance.wealth-=roundNumber*10;
                if(ResourcesManager.Instance.wealth<0)
                {
                    Main.Instance.GameOver();
                }
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
