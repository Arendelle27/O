using ENTITY;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Tilemaps;
using UniRx;
using System;
using UnityEngine.EventSystems;
using Managers;
using UIBUILDING;

namespace MANAGER
{
    public class PlotManager : MonoSingleton<PlotManager>
    {
        //储存当前存在的格子
        public Dictionary<Vector2Int, Plot> plots = new Dictionary<Vector2Int, Plot>();

        [SerializeField, LabelText("地图"), Tooltip("瓦片地图")]
        public Tilemap map;

        [SerializeField, LabelText("地图模式"), ReadOnly]
        public Map_Mode map_Mode = Map_Mode.正常;

        //是否右键选择移动格子中的流浪者
        IDisposable select;
        //是否取消移动格子中的流浪者
        IDisposable cancel;

        [SerializeField, LabelText("当前被选中的板块"), ReadOnly]
        private Plot selectedPlot;//当前被选中的板块

        ///当前被选中的格子
        public Plot SelectedPlot
        {
            get
            {
                return this.selectedPlot;
            }
            set
            {
                if (value != null)
                {
                    this.selectedPlot = value;

                    switch (this.map_Mode)
                    {
                        case Map_Mode.正常:
                            if (this.select != null)
                            {
                                select.Dispose();
                            }

                            if (value.wanderer != null)
                            {
                                this.IsMoveWanaderer(value);
                            }
                            break;
                        case Map_Mode.选择目的地位置:
                            if (value.plot_Type != Plot_Type.未探明&& value.wanderer == null)
                            {
                                this.MoveWanderer(value);
                            }
                            break;
                        case Map_Mode.拓展探索小队:
                            if(DataManager.Instance.levelPromptionAmount>0)
                            {
                                WandererManager.Instance.ExtendExpTeam(value);//拓展探索小队
                            }
                            break;
                    }

                }
            }
        }

        void Start()
        {
            for (int i = -4; i < 5; i++)
                for (int j = -4; j < 5; j++)
                {
                    GetGrid(new Vector2Int(i, j));
                }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            this.SelectedPlot = this.plots[new Vector2Int(0, 0)];
            this.map_Mode = Map_Mode.正常;
        }

        /// <summary>
        /// 重开
        /// </summary>
        public void Restart()
        {
            this.Init();
            for (int i = -4; i < 5; i++)
                for (int j = -4; j < 5; j++)
                {
                    this.plots[new Vector2Int(i, j)].Restart();
                }
        }

        /// <summary>
        /// 读取存档
        /// </summary>
        public void ReadArchive()
        {
            foreach (var plot in ArchiveManager.archive.plotData)
            {
                this.plots[plot.pos].ReadArchive(plot);
            }
        }


        /// <summary>
        /// 在给定的位置产生格子
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void GetGrid(Vector2Int pos)
        {
            GameObject gO = GameObjectPool.Instance.Plots.Get();
            gO.transform.parent = this.transform;
            Vector3Int v3= new Vector3Int(pos.x, pos.y, 0);
            gO.transform.position = this.map.GetCellCenterWorld(v3);

            Plot plot = gO.GetComponent<Plot>();
            plot.pos = pos;

            this.plots.Add(pos, plot);

            plot.clickSelectedSubject.Subscribe(p =>
            {
                if(p.plot_Type==Plot_Type.已探索
                    && this.map_Mode == Map_Mode.正常
                    /*&& p.settlement==null*/)
                {
                    UIManager.Instance.Show<UIBuildingWindow>();//打开建筑UI选择界面
                }
                this.SelectedPlot = p;
            });
        }

        #region 是否移动板块中的流浪者相关方法
        /// <summary>
        /// 是否移动格子中的流浪者
        /// </summary>
        /// <param name="p"></param>
        void IsMoveWanaderer(Plot ini)
        {
            Debug.Log("有流浪者");
            this.select = Observable
                .EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(1))
                .First()
                .Subscribe(_ =>
                {
                    Debug.Log("选择移动点");

                    this.map_Mode = Map_Mode.选择目的地位置;
                    //this.ini = ini;

                    this.CancelMoveWanderer();

                    UIMain.Instance.ChangeToGamePanel(1);//选择落点时关闭UI界面

                    this.select.Dispose();
                });
        }

        /// <summary>
        /// 确认移动流浪者
        /// </summary>
        void MoveWanderer(Plot des)
        {
            this.map_Mode = Map_Mode.正常;
            MainThreadDispatcher.StartUpdateMicroCoroutine( WandererManager.Instance.WandererMoveTo(des));//将流浪者移动到指定的板块

            UIMain.Instance.ChangeToGamePanel(1);//选择完落点打开UI界面

            if (this.cancel!=null)
            {
                this.cancel.Dispose();
            }
        }

        /// <summary>
        /// 是否取消移动流浪者
        /// </summary>
        void CancelMoveWanderer()
        {
            cancel = Observable
                .EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(1))
                .First()
                .Subscribe(_ =>
                {
                    this.map_Mode = Map_Mode.正常;

                    UIMain.Instance.ChangeToGamePanel(1);//取消选择落点后打开UI界面

                    cancel.Dispose();
                });
        }
        #endregion

        /// <summary>
        /// 流浪者进入给定板块
        /// </summary>
        /// <param name="enterPlot"></param>
        public void WanderEnter(Plot enterPlot)
        {
            enterPlot.wanderer = WandererManager.Instance.wanderer;//流浪者进入人类聚落
            SettlementManager.Instance.TriggerEvent(Event_Type.交易,enterPlot.pos);//判断与与聚落触发交易事件

            this.ExpTeamEnterOrLeave(enterPlot, true);//探索小队伴随着流浪者进入
        }


        /// <summary>
        /// 流浪者离开给定板块
        /// </summary>
        /// <param name="leaverPlot"></param>
        public void WanderLeave(Plot leaverPlot)
        {
            leaverPlot.wanderer = null;
            this.ExpTeamEnterOrLeave(leaverPlot, false);//探索小队伴随着流浪者离开
        }

        /// <summary>
        /// 探索小队伴随着流浪者进入或离开
        /// </summary>
        /// <param name="WandererPlot"></param>
        /// <param name="isEnter"></param>
        void ExpTeamEnterOrLeave(Plot WandererPlot,bool isEnter)
        {
            foreach (var expTeam in WandererManager.Instance.exploratoryTeams)
            {
                Vector2Int v2 = new Vector2Int(WandererPlot.pos.x + expTeam.x, WandererPlot.pos.y + expTeam.y);
                if (this.plots.ContainsKey(v2))
                {
                    this.plots[v2].HaveExploratoryTeam = isEnter;
                }
            }
        }

        /// <summary>
        /// 进入选择拓展探索小队模式
        /// </summary>
        /// <param name="isEnter"></param>
        public void EnterSelectExtendExpTeam(bool isEnter)
        {
            this.map_Mode =isEnter? Map_Mode.拓展探索小队:Map_Mode.正常;//切换地图模式

            WandererManager.Instance.wanderer.plot.ShowSelectedColor(isEnter);
            foreach (var expTeam in WandererManager.Instance.exploratoryTeams)
            {
                var v2 = expTeam + WandererManager.Instance.wanderer.plot.pos;
                if (this.plots[v2] != null)
                {
                    this.plots[v2].ShowSelectedColor(isEnter);
                }
            }
        }
    }
}
