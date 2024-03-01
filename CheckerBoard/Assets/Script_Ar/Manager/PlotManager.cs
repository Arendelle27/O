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
        public Dictionary<Vector2Int, Plot> grids = new Dictionary<Vector2Int, Plot>();

        [SerializeField, LabelText("地图"), Tooltip("瓦片地图")]
        public Tilemap map;

        //是否右键选择移动格子中的流浪者
        IDisposable select;
        //是否取消移动格子中的流浪者
        IDisposable cancel;

        [SerializeField, LabelText("是否移动板块中的流浪者"),ReadOnly]
        bool isMove;//是否移动格子中的流浪者
        [SerializeField, LabelText("出发点的板块"), ReadOnly]
        Plot ini;//出发点的板块



        ///当前被选中的格子
        Plot SelectedPlot
        {
            set
            {
                if (value != null)
                {
                    if(value.wanderer!=null)
                    {
                        this.IsMoveWanaderer(value);
                    }
                    else
                    {
                        if(this.select!=null)
                        {
                            select.Dispose();
                        }
                        if (this.isMove)
                        {
                            this.MoveWanderer(value);
                        }
                    }
                }
            }
        }

        void Awake()
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
            this.SelectedPlot = this.grids[new Vector2Int(0, 0)];
            for (int i = -4; i < 5; i++)
                for (int j = -4; j < 5; j++)
                {
                    this.grids[new Vector2Int(i, j)].ChangeType(Plot_Type.未探明);
                }

            this.grids[new Vector2Int(0,0)].ChangeType(Plot_Type.已开发);
        }

        

        /// <summary>
        /// 在给定的位置产生格子
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void GetGrid(Vector2Int pos)
        {
            GameObject gO = Instantiate(GameObjectPool.Instance.Plots.Get(), this.transform);
            Vector3Int v3= new Vector3Int(pos.x, pos.y, 0);
            gO.transform.position = this.map.GetCellCenterWorld(v3);

            Plot plot = gO.GetComponent<Plot>();
            plot.pos = pos;
            this.grids.Add(pos, plot);

            plot.Selected.Subscribe(v2 =>
            {
                if(this.grids[v2].plot_Type==Plot_Type.已探索)
                {
                    UIManager.Instance.Show<UIBuilding>();//打开建筑UI选择界面
                }
                else
                {
                    UIManager.Instance.Close<UIBuilding>();//关闭建筑UI选择界面
                }
                this.SelectedPlot = this.grids[v2];
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
                    Debug.Log("选择了");
                    this.isMove = true;
                    this.ini = ini;

                    this.CancelMoveWanderer();
                });
        }

        /// <summary>
        /// 确认移动流浪者
        /// </summary>
        void MoveWanderer(Plot des)
        {
            this.isMove = false;
            WandererManager.Instance.WandererMoveTo(this.ini, des);
            if(this.cancel!=null)
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
                    this.isMove = false;
                    cancel.Dispose();
                });
        }
        #endregion

        #region 改变板块类型
        /// <summary>
        /// 改变板块类型
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="plot_Type"></param>
        public void PlotsChangeType(Vector2Int pos, Plot_Type plot_Type)
        {
            switch (plot_Type)
            {
                case Plot_Type.可探索:
                    this.PlotsChangeToCanDiscover(pos);
                    break;
                case Plot_Type.已探索:
                    this.PlotsChangeToDiscovered_Plot(pos);
                    break;
                case Plot_Type.已开发:
                    this.PlotsChangeToDeveloped_Plot(pos);
                    break;
            }
        }
        /// <summary>
        /// 使周围一圈板块转为可探索
        /// </summary>
        /// <param name="pos"></param>
        void PlotsChangeToCanDiscover(Vector2Int pos)
        {
            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                {
                    int x= pos.x + i;
                    int y= pos.y + j;
                    if (x == pos.x && y == pos.y)
                        continue;

                    Vector2Int v2 = new Vector2Int(x, y);
                    if (this.grids.ContainsKey(v2))
                    {
                        if (this.grids[v2].plot_Type==Plot_Type.未探明)
                        {
                            this.grids[v2].ChangeType(Plot_Type.可探索);
                        }
                    }

                }
        }
        /// <summary>
        /// 当前板块转为已探明
        /// </summary>
        /// <param name="pos"></param>
        void PlotsChangeToDiscovered_Plot(Vector2Int pos)
        {
            if (grids[pos].plot_Type == Plot_Type.可探索)
            {
                this.grids[pos].ChangeType(Plot_Type.已探索);
            }

        }
        /// <summary>
        /// 当前板块转为已开发
        /// </summary>
        /// <param name="pos"></param>
        void PlotsChangeToDeveloped_Plot(Vector2Int pos)
        {
            if (grids[pos].plot_Type==Plot_Type.已探索)
            {
                this.grids[pos].ChangeType(Plot_Type.已开发);
            }
        }
        #endregion
    }
}
