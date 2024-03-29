using ENTITY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Sirenix.OdinInspector;

namespace MANAGER
{
    public class WandererManager : MonoSingleton<WandererManager>
    {
        //储存当前存在的流浪者
        [SerializeField, LabelText("流浪者"), ReadOnly]
        public Wanderer wanderer;

        [SerializeField, LabelText("储存探索小队的相对于流浪者位置"), ReadOnly]
        public List<Vector2Int> exploratoryTeams;

        ////储存当前存在的目的地提示牌
        //[SerializeField, LabelText("目的地提示牌"), ReadOnly]
        //public DestinationSign destinationSign;

        //储存本次拓展的板块
        [SerializeField, LabelText("本次拓展的板块"), ReadOnly]
        public Stack<Vector2Int> exploredV2 = new Stack<Vector2Int>();

        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {
            this.exploredV2.Clear();

            if (this.wanderer != null)
            {
                this.wanderer.gameObject.SetActive(false);
            }

            //if(this.destinationSign==null)
            //{
            //    GameObject des=Instantiate(GameObjectPool.Instance.DestinationSigns, this.transform);
            //    this.destinationSign = des.GetComponent<DestinationSign>();
            //}
            //this.destinationSign.gameObject.SetActive(false);

        }

        /// <summary>
        /// 重开
        /// </summary>
        public void Restart()
        {
            this.Init();
            this.GetWanderer(PlotManager.Instance.plots[new Vector2Int(0, 0)]);

            this.exploratoryTeams = new List<Vector2Int>() {
                new Vector2Int(1,1),
                new Vector2Int(1,0),
                new Vector2Int(1,-1),
                new Vector2Int(0,1),
                new Vector2Int(0,-1),
                new Vector2Int(-1,1),
                new Vector2Int(-1,0),
                new Vector2Int(-1,-1)
            };
        }

        /// <summary>
        /// 读取存档
        /// </summary>
        public void ReadArchive()
        {
            this.Init();
            this.GetWanderer(PlotManager.Instance.plots[ArchiveManager.archive.wandererData.pos]);
            this.wanderer.level = ArchiveManager.archive.wandererData.level;

            this.exploratoryTeams = ArchiveManager.archive.wandererData.exploratoryTeams;

        }

        /// <summary>
        /// 在给定的板块生成流浪者
        /// </summary>
        /// <param name="plot"></param>
        public void GetWanderer(Plot plot)
        {
            if(this.wanderer==null)
            {
                GameObject go = Instantiate(GameObjectPool.Instance.Wanderer, this.transform);
                Wanderer wanderer = go.GetComponent<Wanderer>();
                this.wanderer = wanderer;
            }
            else
            {
                this.wanderer.gameObject.SetActive(true);
            }

            MainThreadDispatcher.StartUpdateMicroCoroutine( this.WandererMoveTo(plot));
        }

        /// <summary>
        /// 流浪者是否移动到目的地提示牌在的位置
        /// </summary>
        /// <param name="ini"></param>
        //public void WandererMoveToDestination()
        //{
        //    if(this.destinationSign.plot!=null)
        //    {
        //        StartCoroutine(this.WandererMoveTo(this.destinationSign.plot));
        //        this.destinationSign.plot = null;
        //        this.destinationSign.gameObject.SetActive(false);
        //    }
        //}
        

        /// <summary>
        /// 流浪者移动到指定的板块
        /// </summary>
        /// <param name="des"></param>
        public IEnumerator WandererMoveTo(Plot des)
        {

            if (this.wanderer.plot != null)
            {
                PlotManager.Instance.WanderLeave(this.wanderer.plot);
            }

            yield return null;

            this.wanderer.transform.position = des.transform.position - new Vector3(0, 0, 0.3f);
            this.wanderer.plot = des;

            PlotManager.Instance.WanderEnter(des);
            //消耗行动点
        }

        ///// <summary>
        ///// 目的提示牌移动到指定的板块
        ///// </summary>
        ///// <param name="des"></param>
        //public void DestinationSignMoveTo( Plot des)
        //{
        //    this.destinationSign.plot = des;
        //    this.destinationSign.transform.position = des.transform.position-new Vector3(0,0,0.1f);
        //    this.destinationSign.gameObject.SetActive(true);
        //}
        /// <summary>
        /// 流浪者升级
        /// </summary>
        public void Upgrade()
        {
            ResourcesManager.Instance.ChangeWealth(-this.wanderer.level * 10);
            ResourcesManager.Instance.levelPromptionAmount++;
            this.wanderer.level++;
        }

        /// <summary>
        /// 拓展探索小队
        /// </summary>
        /// <param name="aimPlot"></param>
        public void ExtendExpTeam(Plot aimPlot)
        {
            if (aimPlot.HaveExploratoryTeam)
                return;

            foreach (var expTeam in this.exploratoryTeams)
            {
                Vector2Int v2 = aimPlot.pos- this.wanderer.plot.pos;
                if ((v2 - expTeam).magnitude <= 1&& v2.magnitude>0)//判断目标板块是与否探索小队相邻,且不能是流浪者所在的板块
                {
                    this.exploratoryTeams.Add(v2);
                    aimPlot.HaveExploratoryTeam = true;
                    aimPlot.ShowSelectedColor(true);

                    this.exploredV2.Push(v2);//储存本次拓展的板块
                    ResourcesManager.Instance.levelPromptionAmount--;

                    break;
                }
            }
        }

        /// <summary>
        /// 撤销上一次探索小队的拓展
        /// </summary>
        public void WithdrawExpTeam()
        {
            if (this.exploredV2.Count > 0)
            {
                Vector2Int v2 = this.exploredV2.Pop();
                this.exploratoryTeams.Remove(v2);
                Plot aimPlot = PlotManager.Instance.plots[this.wanderer.plot.pos + v2];
                aimPlot.HaveExploratoryTeam = false;
                ResourcesManager.Instance.levelPromptionAmount++;
            }
        }

        //结束回合判定
        public void RoundOver()
        {
            foreach (var expTeam in this.exploratoryTeams)
            {
                Plot aimPlot = PlotManager.Instance.plots[this.wanderer.plot.pos + expTeam];
                aimPlot.TeamExp();
            }
        }
    }
}

