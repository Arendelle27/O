using MANAGER;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ArchiveManager;

namespace ENTITY
{
    public class Plot : Entity
    {
        [SerializeField, LabelText("格子定义"), ReadOnly]
        public PlotDefine plotDefine;

        //[SerializeField, LabelText("格子图像"), Tooltip("不同状态的格子的图像")]
        //public List<Sprite> plot_Sps = new List<Sprite>();

        [SerializeField, LabelText("板块状态"), ReadOnly]
        public Plot_Statue plot_Statue;

        [SerializeField, LabelText("板块类型"), ReadOnly]
        public int plotType;
        //0为资源类，1为事件类

        //临时 当前颜色
        public Color curColor;
        //被选中时的颜色
        public Color selectedColor=Color.blue;

        [SerializeField, LabelText("板块类型"), Tooltip("显示数字")]
        public Text figure;

        [SerializeField, LabelText("位置"), ReadOnly]
        public Vector2Int pos;

        [SerializeField, LabelText("流浪者"), ReadOnly]
        public Wanderer wanderer;
        [SerializeField, LabelText("建筑"), ReadOnly]
        public Building building;
        [SerializeField, LabelText("聚落"), ReadOnly]
        public Settlement settlement;

        [SerializeField, LabelText("是否有探索小队"), ReadOnly]
        private bool haveExploratoryTeam;//是否有探索小队
        public bool HaveExploratoryTeam
        {
            get
            {
                return this.haveExploratoryTeam;
            }
            set
            {
                this.haveExploratoryTeam = value;
                this.ExpTeamEnter(value);
            }
        }

        [SerializeField, LabelText("是否是第一次探索"), ReadOnly]
        public bool isFirstExplored;

        [SerializeField, LabelText("资源数量"), ReadOnly]
        public List<int> buildingResources = new List<int>() { 0, 0 };
        //0为资源类型，1为资源数量
        //资源类型  1，2，3，0为无资源

        [SerializeField, LabelText("是否可以进入"), ReadOnly]
        public bool canEnter;

        #region 事件
        [SerializeField, LabelText("鼠标点击板块"), ReadOnly]
        public Subject<Plot> clickSelectedSubject = new Subject<Plot>();

        [SerializeField, LabelText("鼠标进入板块"), ReadOnly]
        public Subject<Vector2Int> enterSelectedSubject = new Subject<Vector2Int>();

        [SerializeField, LabelText("通过板块解锁板块"), ReadOnly]
        public Subject<int> unLoadByPlot=new Subject<int>();
        #endregion

        private void Start()
        {
            this.ObserveEveryValueChanged(_ => this.building).Subscribe(_ =>
            {
                if (this.building != null)
                {
                    this.BuildConstruction(true);
                }
                else
                {
                    this.BuildConstruction(false);
                }
            });

            this.ObserveEveryValueChanged(_ => this.wanderer).Subscribe(_ =>
            {
                if(this.wanderer!=null)
                {
                    this.WandererEnter(true);
                }
                else
                {
                    this.WandererEnter(false);
                }
            });


            this.ObserveEveryValueChanged(_ => this.curColor).Subscribe(_ =>
            {
                if(this.SR.color!=this.selectedColor)
                {
                    this.SR.color = this.curColor;
                }
            });
        }

        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {
            this.wanderer = null;
            this.building = null;
            this.HaveExploratoryTeam = false;

        }

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="plotdefine"></param>
        public void SetInfo(PlotDefine plotdefine,bool canEnter=true)
        {
            this.plotDefine= plotdefine;
            this.plotType = plotdefine.Type;
            this.figure.text = plotdefine.ID.ToString();

            if (this.plotType == 0)
            {
                this.buildingResources[0]= plotdefine.ResourceType;
                this.buildingResources[1] = plotdefine.ResourceTotal;
            }

            this.Init();
            ChangeType(Plot_Statue.未探明);
            this.isFirstExplored = true;
            this.canEnter = canEnter;
        }

        /// <summary>
        /// 读档
        /// </summary>
        /// <param name="plotData"></param>
        public void ReadArchive(PlotData plotData)
        {
            this.Init();
            this.ChangeType(plotData.plotType);
        }

        #region 改变格子状态
        /// <summary>
        /// 随格子类型改变而改变
        /// </summary>
        /// <param name="plot_Type"></param>
        void ChangeType(Plot_Statue plot_Type)
        {
            switch (plot_Type)
            {
                case Plot_Statue.未探明:
                    ChangeToUndiscoverd();
                    break;
                case Plot_Statue.可探索:
                    ChangeToCanDisCover();
                    break;
                case Plot_Statue.已探索:
                    ChangeToDiscovered();
                    break;
                case Plot_Statue.已开发:
                    ChangeToDeveloped();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 改变为未探明状态
        /// </summary>
        void ChangeToUndiscoverd()
        {
            this.plot_Statue = Plot_Statue.未探明;
            this.curColor = Color.black;
        }

        /// <summary>
        /// 改变为可探明状态
        /// </summary>
        void ChangeToCanDisCover()
        {
            this.plot_Statue = Plot_Statue.可探索;
            this.curColor = Color.red;
        }

        /// <summary>
        /// 改变为已探明状态
        /// </summary>
        void ChangeToDiscovered()
        {
            this.plot_Statue = Plot_Statue.已探索;
            this.curColor = Color.yellow;
        }

        /// <summary>
        /// 改变为已开发状态
        /// </summary>
        void ChangeToDeveloped()
        {
            this.plot_Statue = Plot_Statue.已开发;
            this.curColor = Color.green;
        }
        #endregion

        /// <summary>
        /// 流浪者进入或离开
        /// </summary>
        /// <param name="isEnter"></param>
        void WandererEnter(bool isEnter)
        {
            if(isEnter)
            {
                if(this.plot_Statue==Plot_Statue.可探索||this.plot_Statue==Plot_Statue.未探明)
                {
                    this.ChangeType(Plot_Statue.已探索);
                }
                if(isFirstExplored)///第一次探索
                {
                    this.unLoadByPlot.OnNext(this.plotDefine.ID);//通过格子解锁格子
                    ResourcesManager.Instance.ChangeBuildingResources(new int[3] {1,1,1});

                    this.isFirstExplored = false;
                }
            }
            //else
            //{
            //    if(this.plot_Type==Plot_Type.已探索)
            //    {
            //        this.ChangeType(Plot_Type.未探明);
            //    }
            //}
        }

        /// <summary>
        /// 建成或者拆除建筑
        /// </summary>
        /// <param name="isbuild"></param>
        void BuildConstruction(bool isbuild)
        {
            if(isbuild)
            {
                this.ChangeType(Plot_Statue.已开发);
            }
            else
            {
                if(this.plot_Statue==Plot_Statue.已开发)
                {
                    this.ChangeType(Plot_Statue.已探索);
                }
            }
        }

        /// <summary>
        /// 探索小队进入或离开
        /// </summary>
        /// <param name="isEnter"></param>
        void ExpTeamEnter(bool isEnter)
        {
            if (isEnter)
            {
                if(this.plot_Statue == Plot_Statue.未探明)
                {
                    this.ChangeType(Plot_Statue.可探索);
                }
            }
            else
            {
                if (this.plot_Statue == Plot_Statue.可探索)
                {
                    this.ChangeType(Plot_Statue.未探明);
                }
            }
        }

        /// <summary>
        /// 进入选择拓展开拓小队中，被选择的状态
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowSelectedColor(bool isShow)
        {
            if(isShow)
            {
                this.SR.color = this.selectedColor;
            }
            else
            {
                this.SR.color = this.curColor;
            }
        }

        public void OnMouseEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            //Debug.Log("鼠标选择");
        }

        public void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            this.clickSelectedSubject.OnNext(this);
            //Debug.Log("鼠标点击");
        }
    }
}
