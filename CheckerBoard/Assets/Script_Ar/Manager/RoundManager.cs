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
        public void Init()
        {
            this.roundNumber = 1;
        }

        /// <summary>
        /// 结束回合
        /// </summary>
        public void RoundOver()
        {

            this.roundNumber++;//回合数加1

            BuildingManager.Instance.RoundOver();//建筑结束回合

            DataManager.Instance.RoundOver();//资源结束回合

            this.StageDecision();//阶段决策

            //下一回合开始

            SettlementManager.Instance.TriggerEvent(Event_Type.正常,WandererManager.Instance.wanderer.plot.pos);

            SettlementManager.Instance.TriggerEvent(Event_Type.战斗, WandererManager.Instance.wanderer.plot.pos);
        }

        void StageDecision()
        {
            if(this.roundNumber%5==0)
            {
                Debug.Log("阶段结算");
                DataManager.Instance.wealth-=roundNumber*10;
                if(DataManager.Instance.wealth<0)
                {
                    Main.Instance.GameOver();
                }
            }
        }
    }
}
