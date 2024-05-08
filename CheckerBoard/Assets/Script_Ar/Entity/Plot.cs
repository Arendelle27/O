using MANAGER;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ArchiveManager;

namespace ENTITY
{
    public class Plot : Entity
    {
        [SerializeField, LabelText("状态遮罩"), Tooltip("显示板块状态")]
        public SpriteRenderer statueMask;

        [SerializeField, LabelText("格子定义"), ReadOnly]
        public PlotDefine plotDefine;

        [SerializeField, LabelText("板块状态"), ReadOnly]
        public Plot_Statue plot_Statue;

        [SerializeField, LabelText("板块类型"), ReadOnly]
        public int plotType;
        //0为资源类，1为事件类

        [SerializeField, LabelText("遮罩图像列表"), Tooltip("遮罩图像列表")]
        public List<Sprite> maskSprite;
        //0可探索状态遮罩，1可移动状态遮罩
        [SerializeField, LabelText("遮罩类型"), ReadOnly]
        int statueSort = -1;//-1为关闭遮罩，0为可探索，1为可移动

        [SerializeField, LabelText("位置"), ReadOnly]
        public Vector2Int pos;

        [SerializeField, LabelText("流浪者"), ReadOnly]
        public Wanderer wanderer;
        [SerializeField, LabelText("建筑"), ReadOnly]
        public Building building;
        [SerializeField, LabelText("事件地区"), ReadOnly]
        public EventArea eventArea;
        [SerializeField, LabelText("板块上的npc"), ReadOnly]
        public List<Npc> npcs=new List<Npc>();

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
        //资源类型  0，1，2，-1为无资源

        [SerializeField, LabelText("是否可以进入"), ReadOnly]
        public bool canEnter;

        #region 事件
        [SerializeField, LabelText("鼠标点击板块"), ReadOnly]
        public Subject<Plot> clickSelectedSubject = new Subject<Plot>();

        //[SerializeField, LabelText("鼠标进入板块"), ReadOnly]
        //public Subject<Vector2Int> enterSelectedSubject = new Subject<Vector2Int>();

        [SerializeField, LabelText("通过板块解锁板块"), ReadOnly]
        public Subject<int> unLoadByPlot=new Subject<int>();
        #endregion

        private void Start()
        {

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


            this.ObserveEveryValueChanged(_ => this.statueSort).Subscribe(_ =>
            {
                this.ChangeStatueMask(this.statueSort);
            });
        }

        private void OnEnable()
        {
            this.clickSelectedSubject = new Subject<Plot>();
            //this.enterSelectedSubject = new Subject<Vector2Int>();
            this.unLoadByPlot = new Subject<int>();
        }

        private void OnDisable()
        {
            this.clickSelectedSubject.OnCompleted();

            //this.enterSelectedSubject.OnCompleted();

            this.unLoadByPlot.OnCompleted();

        }

        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {
            this.wanderer = null;
            this.building = null;
            this.eventArea = null;
            this.HaveExploratoryTeam = false;
        }

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="plotDefine"></param>
        public void SetInfo(PlotDefine plotDefine)
        {
            this.Init();

            this.plotDefine= plotDefine;
            this.plotType = plotDefine.Type;
            //this.figure.text = plotDefine.ID.ToString();
            if(SpriteManager.plotSprites.ContainsKey(plotDefine.Name))
            {
                this.SR.sprite = SpriteManager.plotSprites[plotDefine.Name];
            }


            switch (this.plotType)
            {
                case 0://资源板块

                    this.buildingResources[0]= plotDefine.ResourceType;
                    this.buildingResources[1] = plotDefine.ResourceTotal;
                    break;
                case 1://事件板块
                    this.eventArea=EventAreaManager.Instance.GetEventArea(plotDefine.EventType, this);

                    break;
            }

            ChangeType(Plot_Statue.未探明);
            this.isFirstExplored = true;
            if(this.plotDefine.IsSpecialGeneration)
            {
                //特殊生成，根据是否解锁，判断是否可以进入
                this.canEnter = PlotManager.Instance.plotTypes[0].Contains(this.plotDefine.ID)|| PlotManager.Instance.plotTypes[1].Contains(this.plotDefine.ID);
            }
            else
            {
                this.canEnter = true;
            }


        }

        /// <summary>
        /// 资源被消耗完或者结局冲突之后格子变为荒原
        /// </summary>
        public void ChangeToNormalType()
        {
            this.plotDefine = DataManager.PlotDefines[7];
            this.plotType = this.plotDefine.Type;


            if (SpriteManager.plotSprites.ContainsKey(this.plotDefine.Name))
            {
                this.SR.sprite = SpriteManager.plotSprites[this.plotDefine.Name];
            }

            this.eventArea= null;
            this.buildingResources[0] = -1;
            this.canEnter = true;

            this.ChangeType(this.plot_Statue);
        }

        /// <summary>
        /// 改变为冲突类型
        /// </summary>
        /// <param name="clashType"></param>
        public void ChangeToClashType(int clashType)
        {
            switch (clashType)
            {
                case 0:
                    this.plotDefine = DataManager.PlotDefines[12];//人类冲突区
                    break;
                default:
                    this.plotDefine = DataManager.PlotDefines[8];//机器人冲突区
                    break;
            }
            this.plotType = this.plotDefine.Type;
            //this.figure.text = this.plotDefine.ID.ToString();
            //this.figure.color = Color.gray;

            if (SpriteManager.plotSprites.ContainsKey(this.plotDefine.Name))
            {
                this.SR.sprite = SpriteManager.plotSprites[this.plotDefine.Name];
            }

            this.eventArea = EventAreaManager.Instance.GetEventArea(this.plotDefine.EventType, this);
            this.canEnter = true;

            if(this.building!=null)
            {
                BuildingManager.Instance.RemoveBuilding(BuildingManager.BuildingTypeToIndex(this.building.type), this.building);
                this.building = null;
            }

            this.ChangeType(this.plot_Statue);
        }

        ///// <summary>
        ///// 读档
        ///// </summary>
        ///// <param name="plotData"></param>
        //public void ReadArchive(PlotData plotData)
        //{
        //    this.Init();
        //    this.ChangeType(plotData.plotType);
        //}

        #region 改变格子状态
        /// <summary>
        /// 发现或者隐藏
        /// </summary>
        /// <param name="isDiscover"></param>
        public void Discover(bool isDiscover)
        {
            this.SR.enabled = isDiscover;
        }

        /// <summary>
        /// 改变状态遮罩颜色
        /// </summary>
        void ChangeStatueMask(int maskSort)//-1为关闭遮罩，0为可探索，1为可移动
        {
            //Color color=new Color(curColor.r,curColor.g,curColor.b,ParameterConfig.diaphaneity);
            //this.statueMask.color= color;
            if(maskSort==-1)
            {
                this.statueMask.gameObject.SetActive(false);
            }
            else
            {
                this.statueMask.gameObject.SetActive(true);
                this.statueMask.sprite = this.maskSprite[maskSort];
            }
        }

        /// <summary>
        /// 随格子类型改变而改变
        /// </summary>
        /// <param name="plot_Type"></param>
        public void ChangeType(Plot_Statue plot_Type)
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
                //case Plot_Statue.已开发:
                //    ChangeToDeveloped();
                //    break;
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
            //this.curColor = Color.black;
            this.statueSort = -1;
            if(this.plotType==0)
            {
                this.Discover(false);
            }
            else
            {
                this.Discover(true);
            }
        }

        /// <summary>
        /// 改变为可探明状态
        /// </summary>
        void ChangeToCanDisCover()
        {
            this.plot_Statue = Plot_Statue.可探索;
            //this.curColor = Color.red;
            this.statueSort = 0;
            if (this.plotType == 0)
            {
                this.Discover(false);
            }
            else
            {
                this.Discover(true);
            }
        }

        /// <summary>
        /// 改变为已探明状态
        /// </summary>
        void ChangeToDiscovered()
        {
            this.plot_Statue = Plot_Statue.已探索;
            this.Discover(true);
            //this.curColor = Color.yellow;
            this.statueSort = -1;
        }

        ///// <summary>
        ///// 改变为已开发状态
        ///// </summary>
        //void ChangeToDeveloped()
        //{
        //    this.plot_Statue = Plot_Statue.已开发;
        //    //this.curColor = Color.green;
        //    this.statueSort = -1;
        //}
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

                if (this.plotDefine.CanBuild//板块可以建造
                    && this.building == null//板块上没有建筑
                    )
                {
                    (UIMain.Instance.uiPanels[1] as UIGamePanel).buildButton.gameObject.SetActive(true);//显示建造按钮
                }
                else
                {
                    (UIMain.Instance.uiPanels[1] as UIGamePanel).buildButton.gameObject.SetActive(false);//关闭建造按钮
                }

                NpcManager.Instance.NPCAppearUnlock(1, this.plotDefine.ID);//添加对话
                ChatManager.Instance.ChatConditionUnlock(2, this.plotDefine.ID);

                foreach (var npc in this.npcs)
                {
                    npc.ChatWithWander();
                }

                if (isFirstExplored)
                {
                    this.unLoadByPlot.OnNext(this.plotDefine.ID);//通过格子解锁格子
                    if (this.plotDefine.Type == 0)//资源板块
                    {
                        if (this.buildingResources[0] == -1)
                        {
                            return;
                        }
                        int[] res = new int[3] { 0, 0, 0 };
                        res[this.buildingResources[0]] = this.ReduceResource( this.plotDefine.ResourceFristtime);
                        ResourcesManager.Instance.ChangeBuildingResources(res, true);

                    }

                    this.isFirstExplored = false;
                }

                //EventManager.Instance.curClashArea = null;//清空记录的冲突区域

                //触发角色进入事件
                this.eventArea?.WandererEnter();
            }
        }

        /// <summary>
        /// 减少资源
        /// </summary>
        /// <param name="amountNeeded"></param>
        /// <returns></returns>
        public int ReduceResource(int amountNeeded)
        {
            int amountResult= 0;
            if (this.buildingResources[1] > amountNeeded)//资源足够
            {
                amountResult = amountNeeded;
                this.buildingResources[1] -= amountNeeded;
            }
            else if (this.buildingResources[1] > 0)//资源不足
            {
                amountResult = this.buildingResources[1];
                this.buildingResources[1] = 0;
                this.ChangeToNormalType();//变为荒原
            }
            return amountResult;
        }

        ///// <summary>
        ///// 建成或者拆除建筑
        ///// </summary>
        ///// <param name="isbuild"></param>
        //void BuildConstruction(bool isbuild)
        //{
        //    if(isbuild)
        //    {
        //        this.ChangeType(Plot_Statue.已开发);
        //    }
        //    else
        //    {
        //        if(this.plot_Statue==Plot_Statue.已开发)
        //        {
        //            this.ChangeType(Plot_Statue.已探索);
        //        }
        //    }
        //}

        /// <summary>
        /// 探索小队进入或离开
        /// </summary>
        /// <param name="isEnter"></param>
        void ExpTeamEnter(bool isEnter)
        {
            if (isEnter)
            {
                if (this.plot_Statue == Plot_Statue.未探明|| this.plot_Statue == Plot_Statue.可探索)
                {
                    this.ChangeType(Plot_Statue.已探索);
                }
            }
        }

        /// <summary>
        /// 进入选择拓展开拓小队中，被选择的状态
        /// </summary>
        /// <param name="canMove"></param>
        public void CanMoveIn(bool canMove)
        {
            if (canMove)
            {
                this.ChangeStatueMask(1);//可移动状态
            }
            else
            {
                this.ChangeStatueMask(this.statueSort);
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

        /// <summary>
        /// 队伍探索
        /// </summary>
        public void TeamExp()
        {
            if ( this.plot_Statue == Plot_Statue.已探索 && this.plotDefine.Type == 0)
            {
                if(this.buildingResources[0]==-1)
                {
                    return;
                }
                int[] res = new int[3] { 0, 0, 0 };
                res[this.buildingResources[0]] =this.ReduceResource(this.plotDefine.ResourceByRound);
                ResourcesManager.Instance.ChangeBuildingResources(res, true);
            }
        }
    }
}
