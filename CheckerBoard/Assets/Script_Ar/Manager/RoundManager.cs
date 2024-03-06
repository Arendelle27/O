using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace MANAGER
{
    public class RoundManager : Singleton<RoundManager>
    {
        //当前回合数
        public int roundNumber = 1;

        public RoundManager()
        {
            this.ObserveEveryValueChanged(_ => this.roundNumber).Subscribe(_ =>
            {
                //变化时更新回合数UI
                UIMain.Instance.gamePanel.roundNumber.text =  this.roundNumber.ToString();
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

            ResourceManager.Instance.RoundOver();//资源结束回合

            WandererManager.Instance.WandererMoveToDestination();
        }
    }
}
