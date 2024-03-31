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
using System.Linq;

namespace MANAGER
{
    public class PlotManager : MonoSingleton<PlotManager>
    {

        [SerializeField, LabelText("地图"), Tooltip("瓦片地图")]
        public Tilemap map;

        [SerializeField, LabelText("地图范围"), Tooltip("0为左边界，1为右边界，2为下边界，3为上边界")]
        public List<int> mapScope = new List<int>(4) {-10,10,-10,10 };
        //地图范围,0为左边界，1为右边界，2为下边界，3为上边界

        [SerializeField, LabelText("所有的特殊生成的地块"), ReadOnly]
        public Dictionary<Vector2Int,int> plotTypeSepical;//key为位置，value为格子类型

        #region 每局游戏开始时初始化的数据
        //储存当前存在的格子
        public Dictionary<Vector2Int, Plot> plots = new Dictionary<Vector2Int, Plot>();

        [SerializeField, LabelText("当前存在的非特殊生成地块"), ReadOnly]
        internal Dictionary<int, PlotDefine> plotTypeDesepical = new Dictionary<int, PlotDefine>();

        [SerializeField, LabelText("当前存在的地块种类"), ReadOnly]
        public List<HashSet<int>> plotType = new List<HashSet<int>>(2) {new HashSet<int>(), new HashSet<int>()};
        //存储格子类型,0为资源地块，1为事件地块

        [SerializeField, LabelText("当前格子权重总和"),ReadOnly]
        int weightTotal = 0;//格子权重总和

        [SerializeField, LabelText("解锁格子条件"), ReadOnly]
        internal List<Dictionary<int,string>> plotCondition = new List<Dictionary<int, string>>(3) {new Dictionary<int, string>(),new Dictionary<int, string>(),new Dictionary<int, string>() };
        //存储格子条件,0为板块,1为回合,2为道具

        [SerializeField, LabelText("解锁格子的道具"), ReadOnly]
        internal Dictionary<string, bool> unloadProp;

        //回合数解锁格子
        IDisposable unlockByRound;
        #endregion

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
                            if (value.HaveExploratoryTeam&& value.wanderer == null)
                            {
                                this.MoveWanderer(value);
                            }
                            break;
                        case Map_Mode.拓展探索小队:
                            if(ResourcesManager.Instance.levelPromptionAmount>0)
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
            this.ObserveEveryValueChanged(_ => this.plotType[0].Count).Subscribe(_ =>
            {
                this.UpdateWeightTotalsAndDespeicalType();


            });

            this.ObserveEveryValueChanged(_ => this.plotType[1].Count).Subscribe(_ =>
            {
                this.UpdateWeightTotalsAndDespeicalType();
            });
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            if (this.plotTypeSepical == null)
            {
                this.plotTypeSepical = new Dictionary<Vector2Int, int>();
                foreach (var pD in DataManager.PlotDefines.Values)
                {
                    if (pD.IsSpecialGeneration)
                    {
                        for (int i = pD.HorizontalMin; i <= pD.HorizontalMax; i++)
                        {
                            for (int j = pD.VerticalMin; j <= pD.VerticalMax; j++)
                            {
                                Vector2Int v2 = new Vector2Int(i, j);
                                if (this.plotTypeSepical.ContainsKey(v2))
                                {
                                    this.plotTypeSepical[v2] = pD.ID;
                                }
                                else
                                {
                                    this.plotTypeSepical.Add(new Vector2Int(i, j), pD.ID);
                                }
                            }
                        }
                    }
                }
            }//初始化特殊生成的格子类型

            if(this.unlockByRound!=null)//取消订阅回合解锁格子
            {
                unlockByRound.Dispose();
            }

            this.RemoveAllPlot();

            this.plotType = new List<HashSet<int>>(3) { new HashSet<int>(), new HashSet<int>() };
            this.weightTotal = 0;
            this.plotCondition = new List<Dictionary<int, string>>(3) { new Dictionary<int, string>(), new Dictionary<int, string>(), new Dictionary<int, string>() };
            this.unloadProp = new Dictionary<string, bool>();

            this.map_Mode = Map_Mode.正常;
        }

        /// <summary>
        /// 重开
        /// </summary>
        public IEnumerator Restart()
        {
            this.Init();
            yield return null;
            this.InitPlotType();
            yield return null;
            for (int i = -5; i <= 5; i++)
                for (int j = -3; j <= 3; j++)
                {
                    this.GetPlotAndDefine(new Vector2Int(i, j));
                }
            yield return null;

            this.SelectedPlot = this.plots[new Vector2Int(0, 0)];
        }

        /// <summary>
        /// 读取存档
        /// </summary>
        public void ReadArchive()
        {
            //foreach (var plot in ArchiveManager.archive.plotData)
            //{
            //    this.plots[plot.pos].ReadArchive(plot);
            //}
        }

        /// <summary>
        /// 生成格子
        /// </summary>
        /// <param name="pos"></param>
        void GetPlotAndDefine(Vector2Int pos)
        {
            Plot plot = this.GetGrid(pos);
            if(pos==new Vector2Int(0,0))
            {
                plot.SetInfo(DataManager.PlotDefines[7]);
            }
            else
            {
                plot.SetInfo(this.GetPlotDefine(pos));
            }
            this.UnlockPlotByPlot(plots[plot.pos]);//订阅解锁格子
        }

        /// <summary>
        /// 在给定的位置产生格子
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        Plot GetGrid(Vector2Int pos)
        {
            GameObject gO = GameObjectPool.Instance.Plots.Get();
            gO.transform.parent = this.transform;
            Vector3Int v3= new Vector3Int(pos.x, pos.y, 0);
            gO.transform.position = this.map.GetCellCenterWorld(v3);

            Plot plot = gO.GetComponent<Plot>();
            plot.pos = pos;

            this.plots.Add(pos, plot);

            //订阅点击格子事件
            plot.clickSelectedSubject.Subscribe(p =>
            {
                if(this.map_Mode == Map_Mode.正常)//地图模式为正常
                {
                    if(p.wanderer == null)//格子上没有流浪者
                        return;

                    if (p.plotDefine.CanBuild//板块可以建造
                        && p.building == null//板块上没有建筑
                        )
                    {
                        UISelectedWindow uSW= UIManager.Instance.Show<UISelectedWindow>();
                        uSW.OpenWindow(0);

                    }
                    if (p.building != null)//板块上有建筑
                    {
                        BuildingManager.Instance.selectedBuilding = p.building;
                        UISelectedWindow uSW = UIManager.Instance.Show<UISelectedWindow>();
                        uSW.OpenWindow(1);
                    }
                    if(p.eventArea!=null)//板块上有事件地区
                    {
                        EventAreaManager.Instance.selectedEventArea = p.eventArea;
                        UISelectedWindow uSW = UIManager.Instance.Show<UISelectedWindow>();
                        uSW.OpenWindow(2);
                    }
                }

                this.SelectedPlot = p;
            });

            return plot;
        }

        /// <summary>
        /// 初始化格子类型
        /// </summary>
        void InitPlotType()
        {
            foreach (var pD in DataManager.PlotDefines.Values)
            {
                switch (pD.Condition)
                {
                    case Plot_Condition_Type.无:
                        this.plotType[pD.Type].Add(pD.ID);
                        break;
                    case Plot_Condition_Type.板块:
                        this.plotCondition[0].Add(pD.ID, pD.UnlockValue);
                        break;
                    case Plot_Condition_Type.回合:
                        this.plotCondition[1].Add(pD.ID, pD.UnlockValue);
                        break;
                    case Plot_Condition_Type.道具:
                        this.plotCondition[2].Add(pD.ID, pD.UnlockValue);
                        this.unloadProp.Add(pD.UnlockValue, false);

                        this.ObserveEveryValueChanged(_=>this.unloadProp[plotCondition[2][pD.ID]])
                            .First()
                            .Subscribe(_ =>
                        {
                            if (this.unloadProp[plotCondition[2][pD.ID]])
                            {
                                this.plotType[pD.Type].Add(pD.ID);
                                this.plotCondition[2].Remove(pD.ID);
                                Debug.Log("解锁通过道具解锁格子");
                                if (this.plotTypeSepical.ContainsValue(pD.ID))
                                {
                                    //是特殊生成
                                    foreach (var pos in this.plotTypeSepical.Keys)
                                    {
                                        if (this.plots.ContainsKey(pos) && this.plots[pos].plotDefine.ID == pD.ID)//板块存在并且是对应类型
                                        {
                                            this.plots[pos].canEnter = true;
                                        }
                                    }
                                }
                                else
                                {
                                    //非特殊生成
                                    this.RegenerateDespecialPlotDefine();
                                }
                            }
                        });
                        break;
                }
            }

            unlockByRound=RoundManager.Instance.unlockPlotByRound
                .Subscribe(roundNumber =>
                {
                    for(int i = 0; i < this.plotCondition[1].Count;)
                    {
                        var id = this.plotCondition[1].ElementAt(i).Key;
                        if (int.Parse(this.plotCondition[1][id]) == roundNumber)
                        {
                            this.plotType[DataManager.PlotDefines[id].Type].Add(id);
                            this.plotCondition[1].Remove(id);
                            Debug.Log("解锁通过回合解锁格子");
                            if(this.plotTypeSepical.ContainsValue(id))
                            {
                                //是特殊生成
                                foreach (var pos in this.plotTypeSepical.Keys)
                                {
                                    if (this.plots.ContainsKey(pos) && this.plots[pos].plotDefine.ID==id)//板块存在并且是对应类型
                                    {
                                        this.plots[pos].canEnter = true;
                                    }
                                }
                            }
                            else
                            {
                                //非特殊生成
                                this.RegenerateDespecialPlotDefine();
                            }
                        }
                        else
                        {
                            i++;
                        }
                    }
                    if (this.plotCondition[1].Count==0)
                    {
                        unlockByRound.Dispose();
                    }
                });


        }

        /// <summary>
        /// 更新格子权重总和
        /// </summary>
        /// <param name="index"></param>
        void UpdateWeightTotalsAndDespeicalType()
        {
            Dictionary<int, PlotDefine> dic = new Dictionary<int, PlotDefine>();
            for (int n = 0; n < this.plotType.Count; n++)
            {
                foreach (var id in this.plotType[n])
                {
                    if (!this.plotTypeSepical.ContainsValue(id))
                    {
                        dic.Add(id, DataManager.PlotDefines[id]);
                    }
                }
            }
            this.plotTypeDesepical=dic;

            this.weightTotal = 0;
            //计算格子权重总和
            for(int i = 0;i<plotType.Count;i++)
            {
                foreach (var id in this.plotType[i])
                {
                    this.weightTotal += DataManager.PlotDefines[id].Weights;
                }
            }
        }

        /// <summary>
        /// 设置格子类型
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        PlotDefine GetPlotDefine(Vector2Int pos)
        {
            if(this.plotTypeSepical.ContainsKey(pos))
            {
                return DataManager.PlotDefines[this.plotTypeSepical[pos]];
            }

            int r = UnityEngine.Random.Range(0, this.weightTotal);
            int left = 0;
            int right = 0;

            for (int i = 0; i < plotTypeDesepical.Count; i++)
            {
                var item = plotTypeDesepical.ElementAt(i);
                right += item.Value.Weights;
                if (r >= left && r <= right)
                {
                    return item.Value;
                }
                left = right;
            }

            return null;
        }


        /// <summary>
        /// 重新生成非未探索板块定义
        /// </summary>
        /// <param name="sort"></param>
        void RegenerateDespecialPlotDefine()//0代表资源地块，1代表事件地块
        {
            foreach (var plot in this.plots.Values)
            {
                if (!plot.plotDefine.IsSpecialGeneration && plot.plot_Statue==Plot_Statue.未探明)//非特殊生成,状态为未探明,且为指定类型
                {
                    int r = UnityEngine.Random.Range(0, this.weightTotal);
                    int left = 0;
                    int right = 0;

                    for (int i = 0; i < plotTypeDesepical.Count; i++)
                    {
                        var item = plotTypeDesepical.ElementAt(i).Value;
                        if (item.IsSpecialGeneration)
                        {
                            continue;
                        }
                        right += item.Weights;
                        if (r >= left && r <= right)
                        {
                            plot.SetInfo(item);
                            break;
                        }
                        left = right;
                    }
                }
            }
        }


        /// <summary>
        /// 拓展板块
        /// </summary>
        /// <param name="pos"></param>
        void ExpendPlot(Vector2Int pos)
        {
            foreach (var expTeam in WandererManager.Instance.exploratoryTeams)
            {
                List<int> xs = new List<int>() { 0 };
                List<int> ys = new List<int>() { 0 };
                if (expTeam.x >= 0)
                {
                    xs.AddRange(new List<int>{1,2});
                }
                else if (expTeam.x <= 0)
                {
                    xs.AddRange(new List<int> { -1, -2 });
                }

                if (expTeam.y >= 0)
                {
                    ys.AddRange(new List<int> { 1, 2 });
                }
                else if (expTeam.y <= 0)
                {
                    ys.AddRange(new List<int> { -1, -2 });
                }

                foreach (var x in xs)
                {
                    foreach (var y in ys)
                    {
                        int posX = pos.x + expTeam.x + x;
                        int posY = pos.y + expTeam.y + y;
                        if(posX < this.mapScope[0] || posX > this.mapScope[1] || posY < this.mapScope[2] || posY > this.mapScope[3])
                        {
                            continue;
                        }
                        Vector2Int v2 = new Vector2Int(posX,posY);
                        if (v2 == expTeam)
                        {
                            continue;
                        }
                        if (!this.plots.ContainsKey(v2))
                        {
                            Plot plot= this.GetGrid(v2);//生成格子
                            //PlotDefine pD = this.GetPlotDefine(v2);
                            //if (pD.IsSpecialGeneration)//是否是特殊生成
                            //{
                            //    if (this.plotType[0].Contains(pD.ID) || this.plotType[1].Contains(pD.ID))//是否已解锁
                            //    {
                            //        this.plots[v2].SetInfo(pD);
                            //    }
                            //    else
                            //    {
                            //        this.plots[v2].SetInfo(pD, false);
                            //    }
                            //}
                            //else
                            //{
                            //    this.plots[v2].SetInfo(pD);
                            //}
                            this.plots[plot.pos].SetInfo(this.GetPlotDefine(plot.pos));
                            this.UnlockPlotByPlot(plots[v2]);//订阅解锁格子
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据板块条件解锁格子
        /// </summary>
        /// <param name="plot"></param>
        void UnlockPlotByPlot(Plot plot)
        {
            if (this.plotCondition[0].ContainsKey(plot.plotDefine.ID))
            {
                //订阅解锁格子事件
                this.plots[plot.pos].unLoadByPlot
                    .First()
                    .Subscribe(id =>
                {
                    this.plotType[plot.plotDefine.Type].Add(plot.plotDefine.ID);
                    this.plotCondition[0].Remove(plot.plotDefine.ID);
                    Debug.Log("解锁通过板块解锁格子");
                    if (this.plotTypeSepical.ContainsValue(id))
                    {
                        //是特殊生成
                        foreach (var pos in this.plotTypeSepical.Keys)
                        {
                            if (this.plots.ContainsKey(pos) && this.plots[pos].plotDefine.ID == id)//板块存在并且是对应类型
                            {
                                this.plots[pos].canEnter = true;
                            }
                        }
                    }
                    else
                    {
                        //非特殊生成
                        this.RegenerateDespecialPlotDefine();
                    }
                });
            }
        }

        /// <summary>
        /// 清除所有板块
        /// </summary>
        void RemoveAllPlot()
        {
            for(int i=0;i<this.plots.Count;)
            {
                var plot = this.plots.ElementAt(i).Value;
                GameObjectPool.Instance.Plots.Release(plot.gameObject);
                this.plots.Remove(plot.pos);
            }
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
            UIMain.Instance.ChangeToGamePanel(1);//选择完落点打开UI界面

            if (this.cancel != null)
            {
                this.cancel.Dispose();
            }

            if (!ResourcesManager.Instance.CanMove(1))//判断是否有行动点
            {
                Debug.Log("行动点不足");
                return;
            }

            if (!des.canEnter)//判断是否有行动点
            {
                Debug.Log("目的地暂时进不去");
                return;
            }

            MainThreadDispatcher.StartUpdateMicroCoroutine(WandererManager.Instance.WandererMoveTo(des));//将流浪者移动到指定的板块
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

            this.ExpTeamEnterOrLeave(enterPlot, true);//探索小队伴随着流浪者进入

            this. ExpendPlot(enterPlot.pos);//拓展板块
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
