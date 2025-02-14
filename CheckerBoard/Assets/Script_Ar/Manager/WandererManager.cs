using ENTITY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Sirenix.OdinInspector;
using System.Linq;
using DG.Tweening;

namespace MANAGER
{
    public class WandererManager : MonoSingleton<WandererManager>
    {
        //储存当前存在的流浪者
        [SerializeField, LabelText("流浪者"), ReadOnly]
        public Wanderer wanderer;

        [SerializeField, LabelText("储存探索小队的相对于流浪者位置"), ReadOnly]
        public Dictionary<Vector2Int,GameObject> exploratoryTeams=new Dictionary<Vector2Int, GameObject>();

        [SerializeField, LabelText("目的地提示牌"), ReadOnly]
        public DestinationSign destinationSign;


        //储存本次拓展的板块
        [SerializeField, LabelText("本次拓展的板块"), ReadOnly]
        public Stack<Vector2Int> exploredV2 = new Stack<Vector2Int>();


        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {

            if (this.destinationSign == null)
            {
                this.destinationSign = GameObjectPool.Instance.DestinationSign.GetComponent<DestinationSign>();
                this.destinationSign.transform.SetParent(this.transform);
            }
            this.destinationSign.gameObject.SetActive(false);

        }

        /// <summary>
        /// 重开
        /// </summary>
        public void Restart()
        {
            this.Init();
            this.GetExplotryTeam(new Vector2Int(0, 0));
            this.GetWanderer(PlotManager.Instance.plots[new Vector2Int(0, 0)]);
        }

        /// <summary>
        /// 读取存档
        /// </summary>
        public void ReadArchive()
        {
            this.Init();
            ArchiveManager. WandererManagerData wandererManagerData = ArchiveManager.archive.wandererManagerData;
            foreach (var item in wandererManagerData.exploratoryTeams)
            {
                this.GetExplotryTeam(item);
            }
            this.GetWanderer(PlotManager.Instance.plots[wandererManagerData.wandererData.pos]);

        }

        public void GameOver()
        {
            for (int i = 0; i < exploratoryTeams.Count;)
            {
                var item = this.exploratoryTeams.ElementAt(i);
                this.RemoveExplotryTeam(item.Key);
            }

            if (this.wanderer != null)
            {
                this.wanderer.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 在给定的板块生成流浪者
        /// </summary>
        /// <param name="plot"></param>
        public void GetWanderer(Plot plot)
        {
            if(this.wanderer==null)
            {
                GameObject go = GameObjectPool.Instance.Wanderer;
                go.transform.SetParent(this.transform);
                go.gameObject.SetActive(true);
                Wanderer wanderer = go.GetComponent<Wanderer>();
                this.wanderer = wanderer;
            }
            else
            {
                this.wanderer.gameObject.SetActive(true);
            }
            //this.wanderer.transform.position = plot.transform.position + new Vector3(0, 0, ParameterConfig.entityHigh);
            MainThreadDispatcher.StartUpdateMicroCoroutine( this.WandererMoveTo(plot));
        }

        /// <summary>
        /// 生成探索小队
        /// </summary>
        /// <param name="pos"></param>
        public void GetExplotryTeam(Vector2Int pos)
        {
            GameObject gO = GameObjectPool.Instance.ExploratoryTeams.Get();
            this.exploratoryTeams.Add(pos, gO);
            if (this.wanderer!=null&&this.wanderer.gameObject.activeSelf)
            {
                pos += this.wanderer.plot.pos;
                PlotManager.Instance.ExpendPlot(this.wanderer.plot.pos);
            }
            gO.transform.position = PlotManager.Instance.plots[pos].transform.position;
            gO.transform.SetParent(this.transform);

            PlotManager.Instance.AddOrRemoveExpTeamPlot(pos,true);
        }

        /// <summary>
        /// 消除探索小队
        /// </summary>
        /// <param name="pos"></param>
        public void RemoveExplotryTeam(Vector2Int pos)
        {
            GameObjectPool.Instance.ExploratoryTeams.Release(this.exploratoryTeams[pos].gameObject);
            this.exploratoryTeams.Remove(pos);

            Vector2Int v2=pos+this.wanderer.plot.pos;
            PlotManager.Instance.AddOrRemoveExpTeamPlot(v2, false);
        }

        /// <summary>
        /// 流浪者移动到指定的板块
        /// </summary>
        /// <param name="amiPlot"></param>
        public IEnumerator WandererMoveTo(Plot amiPlot)
        {

            if (this.wanderer.plot != null)
            {
                PlotManager.Instance.WanderLeave(this.wanderer.plot);
            }

            yield return null;
            Tweener tw = this.wanderer.transform.DOMove(amiPlot.transform.position + new Vector3(0, ParameterConfig.entityForward, ParameterConfig.entityHigh), 0.3f);
            tw.OnComplete(() =>
            {
                this.wanderer.plot = amiPlot;

                PlotManager.Instance.WanderEnter(amiPlot);
                PlotManager.Instance.EnterMoveWanderer(false);
                if (NoviceGuideManager.Instance.isGuideStage[1])//是否处于新手指引阶段
                {
                    NoviceGuideManager.Instance.NoviceGuideStage++;
                }
            });


        }


        /// <summary>
        /// 拓展探索小队
        /// </summary>
        /// <param name="aimPlot"></param>
        public void ExtendExpTeam(Plot aimPlot)
        {
            if (aimPlot.HaveExploratoryTeam||aimPlot.wanderer!=null)
                return;

            foreach (var expTeam in this.exploratoryTeams)
            {
                Vector2Int v2 = aimPlot.pos- this.wanderer.plot.pos;
                if ((v2 - expTeam.Key).magnitude <= 1)//判断目标板块是与否探索小队相邻,且不能是流浪者所在的板块
                {
                    this.GetExplotryTeam(v2);

                    this.exploredV2.Push(v2);//储存本次拓展的板块
                    CapabilityManager.Instance.expendExploratoryAmount--;

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
                this.RemoveExplotryTeam(v2);
                CapabilityManager.Instance.expendExploratoryAmount++;
            }
        }

        //结束回合判定
        public void RoundOver()
        {
            foreach (var expTeam in this.exploratoryTeams.Keys)
            {
                Vector2Int pos = this.wanderer.plot.pos + expTeam;
                if (PlotManager.Instance.plots.ContainsKey(pos))
                {
                    PlotManager.Instance.plots[pos].TeamExp();
                }
            }
        }
    }
}

