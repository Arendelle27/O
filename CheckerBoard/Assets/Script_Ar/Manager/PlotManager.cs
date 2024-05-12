using ENTITY;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UniRx;
using System;
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
        [SerializeField, LabelText("存在的已探索地块"), ReadOnly]
        public HashSet<Vector2Int> haveExploredPlots=new HashSet<Vector2Int>();

        [SerializeField, LabelText("存在的可探索地块"), ReadOnly]
        HashSet<Vector2Int> canExploredPlots = new HashSet<Vector2Int>();

        //储存当前存在的格子
        public Dictionary<Vector2Int, Plot> plots = new Dictionary<Vector2Int, Plot>();

        [SerializeField, LabelText("当前存在的非特殊生成地块"), ReadOnly]
        internal Dictionary<int, PlotDefine> plotTypeDesepical = new Dictionary<int, PlotDefine>();

        [SerializeField, LabelText("当前存在的地块种类"), ReadOnly]
        public List<HashSet<int>> plotTypes = new List<HashSet<int>>(2) {new HashSet<int>(), new HashSet<int>()};
        //存储格子类型,0为资源地块，1为事件地块

        [SerializeField, LabelText("当前格子权重总和"),ReadOnly]
        int weightTotal = 0;//格子权重总和

        [SerializeField, LabelText("解锁格子条件"), ReadOnly]
        internal List<Dictionary<int,int>> plotConditions = new List<Dictionary<int, int>>(3) { new Dictionary<int, int>(), new Dictionary<int, int>(), new Dictionary<int, int>() };
        //存储格子条件,0为板块,1为回合,2为道具

        [SerializeField, LabelText("解锁格子的道具"), ReadOnly]
        internal Dictionary<Prop_Type, bool> unloadProp=new Dictionary<Prop_Type, bool>();
        [SerializeField, LabelText("道具解锁事件"), ReadOnly]
        Dictionary<Prop_Type, IDisposable> unlockIDisposableProp = new Dictionary<Prop_Type, IDisposable> { };

        //回合数解锁格子
        IDisposable unlockByRound;
        #endregion

        [SerializeField, LabelText("地图模式"), ReadOnly]
        public Map_Mode map_Mode = Map_Mode.正常;

        ////是否右键选择移动格子中的流浪者
        //IDisposable select;
        ////是否取消移动格子中的流浪者
        //IDisposable cancel;

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
                        //case Map_Mode.正常:
                        //    if (this.select != null)
                        //    {
                        //        select.Dispose();
                        //    }

                        //    if (value.wanderer != null)
                        //    {
                        //        this.IsMoveWanaderer(value);
                        //    }
                        //    break;
                        case Map_Mode.选择目的地位置:
                            if (this.canMovePlots.Contains(value)&& value.wanderer == null)
                            {
                                this.CaculateMoveExecutionCost(value);
                            }
                            break;
                        case Map_Mode.拓展探索小队:
                            if(CapabilityManager.Instance.expendExploratoryAmount > 0)
                            {
                                WandererManager.Instance.ExtendExpTeam(value);//拓展探索小队
                            }
                            break;
                    }

                }
            }
        }

        [SerializeField, LabelText("移动消耗的行动力"), ReadOnly]
        public int moveExecutionCost;//移动消耗的行动力

        void Start()
        {

            this.ObserveEveryValueChanged(_ => this.haveExploredPlots.Count).Subscribe(_ =>
            {
                this.UpdateCanExplorePlot();
            });

            this.ObserveEveryValueChanged(_ => this.moveExecutionCost).Subscribe(_ =>
            {
                (UIMain.Instance.uiPanels[2] as UIMovePanel).curCxecutiontext.text = this.moveExecutionCost.ToString();
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
            for (int i = -3; i <= 3; i++)
                for (int j = -3; j <= 3; j++)
                {
                    this.GetPlotAndDefine(new Vector2Int(i, j));
                }
            yield return null;

            this.plots[new Vector2Int(0, 0)].canEnter = true;

            this.SelectedPlot = this.plots[new Vector2Int(0, 0)];
        }

        /// <summary>
        /// 读取存档
        /// </summary>
        public IEnumerator ReadArchive()
        {
            this.Init();
            ArchiveManager.PlotManagerData plotManagerData = ArchiveManager.archive.plotManagerData;
            yield return null;

            for(int i=0;i<this.plotTypes.Count;i++)
            {
                HashSet<int> hs=new HashSet<int>(plotManagerData.plotTypes[i].plotTypes);
                //foreach (var id in plotManagerData.plotTypes[i].plotTypes)
                //{
                //    hs.Add(id);
                //}
                this.plotTypes[i] = hs;
            }

            for(int i = 0; i < this.plotConditions.Count; i++)
            {
                Dictionary<int, int> dic = new Dictionary<int, int>();
                for(int j = 0; j < plotManagerData.plotConditions[i].ids.Count; j++)
                {
                    dic.Add(plotManagerData.plotConditions[i].ids[j], plotManagerData.plotConditions[i].conditions[j]);
                }
                this.plotConditions[i] = dic;


            }

            foreach(var plotTypeDespicalId in plotManagerData.plotTypeDesepicalIds)
            {
                this.plotTypeDesepical.Add(plotTypeDespicalId, DataManager.PlotDefines[plotTypeDespicalId]);
            }

            foreach (var prop in plotManagerData.unloadPropsData)
            {
                this.unloadProp.Add(prop.propName, prop.isUnloaded);
                this.unlockIDisposableProp.Add(prop. propName, null);
            }

            foreach (var plot in plotManagerData.plotsData)
            {
                Plot p = this.GetGrid(plot.pos);
                p.SetInfo(DataManager.PlotDefines[plot.plotDefineId]);
                p.ChangeType(plot.plotStatue);
                p.canEnter = plot.canEnter;
                p.buildingResources = plot.buildingResources;
                p.isFirstExplored = plot.isFirstExplored;

                this.UnlockPlotByPlot(p);//订阅解锁格子
            }

            yield return null;

            this.CalculateWeight();

            this.haveExploredPlots= new HashSet<Vector2Int>(plotManagerData.haveExploredPlots);

            this.UnlockByRound();

            foreach(var id in this.plotConditions[2].Keys)//道具解锁格子
            {
                this.unlockIDisposableProp[(Prop_Type)plotConditions[2][id]]= this.ObserveEveryValueChanged(_ => this.unloadProp[(Prop_Type)plotConditions[2][id]])
                    .Where(_ => this.unloadProp[(Prop_Type)plotConditions[2][id]])
                    .First()
                    .Subscribe(_ =>
                    {
                        //if (this.unloadProp[(Prop_Type)plotConditions[2][id]])
                        //{
                        this.plotTypes[DataManager.PlotDefines[id].Type].Add(id);
                        this.unlockIDisposableProp[(Prop_Type)plotConditions[2][id]].Dispose();
                        this.plotConditions[2].Remove(id);
                        Debug.Log("解锁通过道具解锁格子");
                        this.PlotUnlock(id);

                        //}
                        QuestManager.Instance.QuestEnd(2, plotConditions[2][id]);
                    });
                break;
            }


            //foreach (var plot in ArchiveManager.archive.plotData)
            //{
            //    this.plots[plot.pos].ReadArchive(plot);
            //}
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        public void GameOver()
        {
            if (this.unlockByRound != null)//取消订阅回合解锁格子
            {
                unlockByRound.Dispose();
            }
            for (int i = 0; i < this.unlockIDisposableProp.Count;)
            {
                var id = this.unlockIDisposableProp.ElementAt(i);
                if (id.Value != null)
                {
                    id.Value.Dispose();
                }
                this.unlockIDisposableProp.Remove(id.Key);
            }
            this.map_Mode = Map_Mode.正常;

            this.plotTypeDesepical.Clear();
            this.haveExploredPlots.Clear();
            this.canExploredPlots.Clear();
            this.plotTypes = new List<HashSet<int>>(3) { new HashSet<int>(), new HashSet<int>() };
            this.weightTotal = 0;
            this.plotConditions = new List<Dictionary<int, int>>(3) { new Dictionary<int, int>(), new Dictionary<int, int>(), new Dictionary<int, int>() };
            this.unloadProp = new Dictionary<Prop_Type, bool>();

            this.RemoveAllPlot();
        }

        /// <summary>
        /// 生成格子
        /// </summary>
        /// <param name="pos"></param>
        Plot GetPlotAndDefine(Vector2Int pos)
        {
            Plot plot = this.GetGrid(pos);
            if (pos == new Vector2Int(0, 0))
            {
                plot.SetInfo(DataManager.PlotDefines[7]);
            }
            else
            {
                plot.SetInfo(this.GetPlotDefine(pos));
            }
            this.UnlockPlotByPlot(plots[plot.pos]);//订阅解锁格子
            return plot;
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
            //plot.Discover(false);//关闭贴图

            this.plots.Add(pos, plot);

            //订阅点击格子事件
            plot.clickSelectedSubject.Subscribe(plot =>
            {
                this.SelectedPlot = plot;
                if (this.map_Mode == Map_Mode.正常)//地图模式为正常
                {
                    if(plot.wanderer == null)//格子上没有流浪者
                        return;

                    if (plot.plotDefine.CanBuild//板块可以建造
                        && plot.building == null//板块上没有建筑
                        )
                    {
                        UISelectedWindow uSW= UIManager.Instance.Show<UISelectedWindow>();
                        uSW.OpenWindow(0);

                    }
                    if (plot.building != null)//板块上有建筑
                    {
                        BuildingManager.Instance.selectedBuilding = plot.building;
                        UISelectedWindow uSW = UIManager.Instance.Show<UISelectedWindow>();
                        uSW.OpenWindow(1);
                    }
                    if(plot.plotDefine.EventType==Event_Area_Type.交易&&plot.wanderer!=null)//板块上有事件地区
                    {
                        //EventAreaManager.Instance.selectedEventArea = plot.eventArea;
                        UISelectedWindow uSW = UIManager.Instance.Show<UISelectedWindow>();
                        uSW.OpenWindow(2);
                    }
                    if(EventManager.Instance.curClashArea!=null&& plot.eventArea== EventManager.Instance.curClashArea)//板块上有冲突地区
                    {
                        UISelectedWindow uSW = UIManager.Instance.Show<UISelectedWindow>();
                        uSW.OpenWindow(3);
                    }
                    if(plot.plotType==0)
                    {
                        UISelectedWindow uSW = UIManager.Instance.Show<UISelectedWindow>();
                        uSW.OpenWindow(4);
                    }
                }
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
                        this.plotTypes[pD.Type].Add(pD.ID);
                        this.UpdateWeightTotalsAndDespeicalType(false);
                        break;
                    case Plot_Condition_Type.板块:
                        this.plotConditions[0].Add(pD.ID, pD.UnlockValue);
                        break;
                    case Plot_Condition_Type.回合:
                        this.plotConditions[1].Add(pD.ID, pD.UnlockValue);
                        break;
                    case Plot_Condition_Type.道具:
                        this.plotConditions[2].Add(pD.ID, pD.UnlockValue);
                        this.unloadProp.Add((Prop_Type)pD.UnlockValue, false);
                        this.unlockIDisposableProp.Add((Prop_Type)pD.UnlockValue, null);

                        this.unlockIDisposableProp[(Prop_Type)plotConditions[2][pD.ID]]=this.ObserveEveryValueChanged(_=>this.unloadProp[(Prop_Type)plotConditions[2][pD.ID]])
                            .Where(_=>this.unloadProp[(Prop_Type)plotConditions[2][pD.ID]])
                            .First()
                            .Subscribe(_ =>
                        {
                            if (this.unloadProp[(Prop_Type)plotConditions[2][pD.ID]])
                            {
                                this.plotTypes[pD.Type].Add(pD.ID);
                                this.unlockIDisposableProp[(Prop_Type)plotConditions[2][pD.ID]].Dispose();
                                this.plotConditions[2].Remove(pD.ID);
                                Debug.Log("解锁通过道具解锁格子");
                                this.PlotUnlock(pD.ID);
                            }
                        });
                        break;
                }
            }

            this.UnlockByRound();
        }

        /// <summary>
        /// 通过回合解锁格子
        /// </summary>
        void UnlockByRound()
        {
            unlockByRound = RoundManager.Instance.unlockPlotByRound.Subscribe(roundNumber =>
            {
                for (int i = 0; i < this.plotConditions[1].Count;)
                {
                    var id = this.plotConditions[1].ElementAt(i).Key;
                    if (this.plotConditions[1][id] <= roundNumber)
                    {
                        this.plotTypes[DataManager.PlotDefines[id].Type].Add(id);
                        this.plotConditions[1].Remove(id);
                        Debug.Log("解锁通过回合解锁格子");
                        this.PlotUnlock(id);
                    }
                    else
                    {
                        i++;
                    }
                }
                if (this.plotConditions[1].Count == 0)
                {
                    unlockByRound.Dispose();
                }
            });
        }

        /// <summary>
        /// 更新格子权重总和非特殊生成的格子类型,并根据需要重新生成非特殊生成的格子
        /// </summary>
        /// <param name="index"></param>
        void UpdateWeightTotalsAndDespeicalType(bool isRegenerateDefine=false)
        {
            Dictionary<int, PlotDefine> dic = new Dictionary<int, PlotDefine>();
            for (int n = 0; n < this.plotTypes.Count; n++)
            {
                foreach (var id in this.plotTypes[n])
                {
                    if (!this.plotTypeSepical.ContainsValue(id))
                    {
                        dic.Add(id, DataManager.PlotDefines[id]);
                    }
                }
            }
            this.plotTypeDesepical=dic;

            this.CalculateWeight();

            if(isRegenerateDefine)
            {
                //重新生成非特殊生成的格子
                this.RegenerateDespecialPlotDefine();
            }
        }


        /// <summary>
        /// 计算格子权重
        /// </summary>
        void CalculateWeight()
        {
            this.weightTotal = 0;
            //计算格子权重总和
            for (int i = 0; i < plotTypes.Count; i++)
            {
                foreach (var id in this.plotTypes[i])
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
        /// Npc进入或离开板块
        /// </summary>
        /// <param name="isAppear"></param>
        /// <param name="pos"></param>
        /// <param name="npc"></param>
        public Plot NPCAppear(Vector2Int pos)
        {
            Plot plot = null;
            if(this.plots.ContainsKey(pos))
            {
                plot = this.plots[pos];
            }
            else
            {
                plot=this.GetPlotAndDefine(pos);
            }
            return plot;
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
        public void ExpendPlot(Vector2Int pos)
        {
            foreach (var expTeam in WandererManager.Instance.exploratoryTeams.Keys)
            {
                List<int> xs = new List<int>() { 0 };
                List<int> ys = new List<int>() { 0 };
                if (expTeam.x >= 0)
                {
                    xs.AddRange(new List<int>{-1,1,2});
                }
                if (expTeam.x <= 0)
                {
                    xs.AddRange(new List<int> { -1, -2 ,1});
                }

                if (expTeam.y >= 0)
                {
                    ys.AddRange(new List<int> {-1, 1, 2 });
                }
                if (expTeam.y <= 0)
                {
                    ys.AddRange(new List<int> { -1, -2 ,1});
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
                            //Plot plot= this.GetGrid(v2);//生成格子
                            ////PlotDefine pD = this.GetPlotDefine(v2);
                            ////if (pD.IsSpecialGeneration)//是否是特殊生成
                            ////{
                            ////    if (this.plotType[0].Contains(pD.ID) || this.plotType[1].Contains(pD.ID))//是否已解锁
                            ////    {
                            ////        this.plots[v2].SetInfo(pD);
                            ////    }
                            ////    else
                            ////    {
                            ////        this.plots[v2].SetInfo(pD, false);
                            ////    }
                            ////}
                            ////else
                            ////{
                            ////    this.plots[v2].SetInfo(pD);
                            ////}
                            //this.plots[plot.pos].SetInfo(this.GetPlotDefine(plot.pos));
                            //this.UnlockPlotByPlot(plots[v2]);//订阅解锁格子
                            this.GetPlotAndDefine(v2);
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
            if (this.plotConditions[0].ContainsValue(plot.plotDefine.ID))
            {
                //订阅解锁格子事件
                plot.unLoadByPlot=new Subject<int>();
                plot.unLoadByPlot
                    .First()
                    .Subscribe(id =>
                {
                    //this.plotTypes[plot.plotDefine.Type].Add(plot.plotDefine.ID);
                    List<int> unlockPlotDefineIds = new List<int>();
                    for(int i = 0; i < this.plotConditions[0].Count; i++)
                    {
                        var item = this.plotConditions[0].ElementAt(i);
                        if (item.Value == id)
                        {
                            unlockPlotDefineIds.Add(item.Key);
                            this.plotTypes[plot.plotDefine.Type].Add(item.Key);
                            break;
                        }
                    }
                    foreach (var unlockPlotDefineId in unlockPlotDefineIds)
                    {
                        this.plotConditions[0].Remove(unlockPlotDefineId);

                        Debug.Log("解锁通过板块解锁格子");
                        this.PlotUnlock(unlockPlotDefineId);
                    }
                    plot.unLoadByPlot.Dispose();

                });
            }
        }

        /// <summary>
        /// 解锁板块
        /// </summary>
        /// <param name="plotDefineId"></param>
        void PlotUnlock(int plotDefineId)
        {
            if (this.plotTypeSepical.ContainsValue(plotDefineId))
            {
                //是特殊生成
                MessageManager.Instance.AddMessage(Message_Type.探索, string.Format("地图上的{0}可以进入了", DataManager.PlotDefines[plotDefineId].Name));
                foreach (var pos in this.plotTypeSepical.Keys)
                {
                    if(this.plotTypeSepical[pos] == plotDefineId) //是对应类型
                    {
                        Plot plot = null;
                        if (this.plots.ContainsKey(pos) )//板块存在
                        {
                            plot = this.plots[pos];
                        }
                        else
                        {
                            plot = this.GetPlotAndDefine(pos);
                        }
                        plot.canEnter = true;
                        if (plot.plotDefine.Name == "地下室（未解锁）")//如果是地下室
                        {
                            plot.SR.sprite = SpriteManager.plotSprites["地下室（解锁）"];
                        }
                    }

                }
            }
            else
            {
                MessageManager.Instance.AddMessage(Message_Type.探索, string.Format("地图上板块发生了巨大的变化，出现了{0}", DataManager.PlotDefines[plotDefineId].Name));
                //非特殊生成
                this.UpdateWeightTotalsAndDespeicalType(true);
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

        public void IsMoveWanderer()
        {
            //if (this.map_Mode == Map_Mode.正常)
            //{
                //选择移动点
            Debug.Log("选择移动点");

            if(ResourcesManager.Instance.execution<=0)
            {
                MessageManager.Instance.AddMessage(Message_Type.探索, "行动点不足");
                return;
            }
            this.EnterMoveWanderer(true);
            //}
        }

        //记录可移动到的板块
        List<Plot> canMovePlots = new List<Plot>();
        //移动目标板块
        Plot moveAimPlot=null;

        /// <summary>
        /// 计算可移动范围
        /// </summary>
        void CaculateCanMoveScope()
        {
            if(this.canMovePlots.Count!=0)
            {
                this.canMovePlots.Clear();
            }
            float r = ResourcesManager.Instance.execution * 3 + 0.5f;
            for(int i = -Mathf.CeilToInt(r); i <= Mathf.CeilToInt(r); i++)
            {
                for (int j = -Mathf.CeilToInt(r); j <= Mathf.CeilToInt(r); j++)
                {
                    if (i * i + j * j <= r * r)
                    {
                        Vector2Int v2 = new Vector2Int(i, j)+WandererManager.Instance.wanderer.plot.pos;
                        if(!this.plots.ContainsKey(v2))
                        {
                            continue;
                        }
                        if (this.plots[v2].plot_Statue!=Plot_Statue.未探明)
                        {
                            this.plots[v2].CanMoveIn(true);

                            this.canMovePlots.Add(this.plots[v2]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 计算消耗的行动力
        /// </summary>
        /// <param name="plot"></param>
        public void CaculateMoveExecutionCost(Plot plot)
        {
            this.moveAimPlot=plot;
            WandererManager.Instance.destinationSign.SetInfo(plot);//设置目的地标志
            float distance= Vector2Int.Distance(WandererManager.Instance.wanderer.plot.pos, plot.pos);
            for (int i = ResourcesManager.Instance.execution; i >0; i--)
            {
                if(distance>=(i-1)*3+0.5f)
                {
                    this.moveExecutionCost = i;
                    break;
                }
            }
        }

        /// <summary>
        /// 确认移动流浪者
        /// </summary>
        public bool MoveWanderer(bool isMove)
        {
            //this.EnterMoveWanderer(false);
            if (isMove)
            {
                if (this.moveAimPlot == null)//目的地为空
                {
                    return false;
                }
                if (!this.selectedPlot.canEnter)
                {
                    Debug.Log("目的地暂时进不去");
                    MessageManager.Instance.AddMessage(Message_Type.探索, "不知为何，目的地进不去");
                    return false;
                }

                ResourcesManager.Instance.ChangeExecution(-this.moveExecutionCost);//消耗行动点
                MainThreadDispatcher.StartUpdateMicroCoroutine(WandererManager.Instance.WandererMoveTo(this.moveAimPlot));//将流浪者移动到指定的板块
                MessageManager.Instance.AddMessage(Message_Type.探索, string.Format("消耗{0}行动力，移动到（{1}，{2}）", this.moveExecutionCost, this.moveAimPlot.pos.x, this.moveAimPlot.pos.y));

            }
            else
            {
                this.EnterMoveWanderer(false);
            }

            WandererManager.Instance.destinationSign.Hide();

            this.moveAimPlot = null;
            return true;
        }


        #endregion

        /// <summary>
        /// 流浪者进入给定板块
        /// </summary>
        /// <param name="enterPlot"></param>
        public void WanderEnter(Plot enterPlot)
        {
            enterPlot.wanderer = WandererManager.Instance.wanderer;//流浪者进入人类聚落

            this. ExpendPlot(enterPlot.pos);//拓展板块

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
            foreach (var expTeam in WandererManager.Instance.exploratoryTeams.Keys)
            {
                Vector2Int v2 = new Vector2Int(WandererPlot.pos.x + expTeam.x, WandererPlot.pos.y + expTeam.y);
                if (this.plots.ContainsKey(v2))
                {
                    if(isEnter)
                    {
                        this.AddOrRemoveExpTeamPlot(v2, isEnter);
                        Debug.Log(v2 + "探索小队跟随进入");
                    }
                    else
                    {
                        this.plots[v2].HaveExploratoryTeam = false;
                    }
                    WandererManager.Instance.exploratoryTeams[expTeam].transform.position = this.plots[v2].transform.position;
                }
            }
            //this.UpdateCanExplorePlot();
        }

        /// <summary>
        /// 增加或移除探索小队
        /// </summary>
        /// <param name="v2"></param>
        /// <param name="isEnter"></param>
        public void AddOrRemoveExpTeamPlot(Vector2Int v2,bool isEnter)
        {

                this.plots[v2].HaveExploratoryTeam = isEnter;
                if (isEnter && !this.haveExploredPlots.Contains(v2))
                {
                    this.haveExploredPlots.Add(v2);
                }
                else if (!isEnter && this.haveExploredPlots.Contains(v2))
                {
                    this.haveExploredPlots.Remove(v2);
                }
            
        }

        /// <summary>
        /// 更新可探索区域
        /// </summary>
        void UpdateCanExplorePlot()
        {
            //foreach (var canExploredPlot in this.canExploredPlots)
            //{
            //    if (!this.plots[canExploredPlot].HaveExploratoryTeam/*&& this.plots[canExploredPlot].plot_Statue!=Plot_Statue.已探索*/)
            //    {
            //        this.plots[canExploredPlot].ChangeType(Plot_Statue.未探明);
            //    }
            //}
            this.canExploredPlots.Clear();

            foreach (var exploredPlot in this.haveExploredPlots)
            {

                for (int x=-1;x<=1;x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector2Int pos = exploredPlot + new Vector2Int(x, y);
                        if(!this.plots.ContainsKey(pos))
                        {
                            continue;
                        }
                        if (!this.haveExploredPlots.Contains(pos))
                        {
                            this.plots[pos].ChangeType(Plot_Statue.可探索);
                            this.canExploredPlots.Add(pos);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 进入和退出移动流浪者模式
        /// </summary>
        /// <param name="isEnter"></param>
        public void EnterMoveWanderer(bool isEnter)
        {
            if(isEnter)
            {
                this.map_Mode = Map_Mode.选择目的地位置;

                this.moveExecutionCost = 0;
                UIMain.Instance.ChangeToGamePanel(2);//选择落点时打开移动UI界面
                this.CaculateCanMoveScope();
            }
            else
            {
                this.map_Mode = Map_Mode.正常;
                UIMain.Instance.ChangeToGamePanel(1);//选择完落点打开UI界面

                foreach (var plot in this.canMovePlots)
                {
                    plot.CanMoveIn(false);
                }

                this.canMovePlots.Clear();
            }
        }

        /// <summary>
        /// 进入和退出扩展模式
        /// </summary>
        /// <param name="isExtend"></param>
        public void EnterSelectExtendExpTeam(bool isExtend)
        {
            if(isExtend) 
            {
                this.map_Mode = Map_Mode.拓展探索小队;
                UIMain.Instance.ChangeToGamePanel(3);
                if (NoviceGuideManager.Instance.isGuideStage[5])//是否处于新手指引阶段
                {
                    NoviceGuideManager.Instance.uINoviceGuidePanel.masks[0].gameObject.SetActive(false);
                }
            }
            else
            {
                this.map_Mode = Map_Mode.正常;
                UIMain.Instance.ChangeToGamePanel(1);//恢复到游戏界面
                if (NoviceGuideManager.Instance.isGuideStage[5])//是否处于新手指引阶段
                {
                    NoviceGuideManager.Instance.NoviceGuideStage++;
                }
                //UIManager.Instance.Show<UIStrengthenCapabilityWindow>();
            }
        }
    }
}
