using ENTITY;
using Managers;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIBUILDING;
using UniRx;
using UnityEngine;

namespace MANAGER
{
    public class BuildingManager : MonoSingleton<BuildingManager>
    {
        #region 每局游戏开始时初始化  
        //储存当前存在的建筑
        [SerializeField, LabelText("当前存在的采集建筑"), ReadOnly]
        public Dictionary<Vector2Int,GatheringBuilding> gatheringBuildings=new Dictionary<Vector2Int, GatheringBuilding>();

        [SerializeField, LabelText("当前存在的生产建筑"), ReadOnly]
        public Dictionary<Vector2Int, ProductionBuilding> productionBuildings = new Dictionary<Vector2Int, ProductionBuilding>();

        [SerializeField, LabelText("当前存在的战斗建筑"), ReadOnly]
        public Dictionary<Vector2Int, BattleBuilding> battleBuildings = new Dictionary<Vector2Int, BattleBuilding>();

        [SerializeField, LabelText("建筑的解锁条件"), ReadOnly]
        public Dictionary<Building_Condition_Type,Dictionary<Building_Type,int>> buildCondition = new Dictionary<Building_Condition_Type, Dictionary<Building_Type, int>>()
        {
            {Building_Condition_Type.资源1,new Dictionary<Building_Type, int>() },
            {Building_Condition_Type.资源2,new Dictionary<Building_Type, int>() },
            {Building_Condition_Type.资源3,new Dictionary<Building_Type, int>() },
            {Building_Condition_Type.蓝图,new Dictionary<Building_Type, int>() },
            {Building_Condition_Type.回合数,new Dictionary<Building_Type, int>() },
            {Building_Condition_Type.厉害的战斗机器,new Dictionary<Building_Type, int>() },
        };

        [SerializeField, LabelText("当前存在的建筑类型"), ReadOnly]
        public List<List<Building_Type>> buildingTypes = new List<List<Building_Type>>()
        {
            new List<Building_Type>(),
            new List<Building_Type>(),
            new List<Building_Type>(),
        };

        [SerializeField, LabelText("建筑蓝图"), ReadOnly]
        public Dictionary<int, bool> bluePrints=new Dictionary<int, bool>();

        [SerializeField, LabelText("数量解锁事件"), ReadOnly]
        Dictionary<Building_Condition_Type, IDisposable> unlockIDisposableAmount = new Dictionary<Building_Condition_Type, IDisposable> { };

        [SerializeField, LabelText("蓝图解锁事件"), ReadOnly]
        Dictionary<int, IDisposable> unlockIDisposableBlueprint = new Dictionary<int, IDisposable>{};
        #endregion

        [SerializeField, LabelText("被选中的建筑"), ReadOnly]
        public Building selectedBuilding;

        private void Start()
        {

            this.ObserveEveryValueChanged(_ => this.buildingTypes[0].Count).Subscribe(_ =>
            {
                (UIMain.Instance.uISelectedWindow.uISelectedWindows[0] as UIBuildingWindow)?.UpdateBuildingList(0);
            });

            this.ObserveEveryValueChanged(_ => this.buildingTypes[1].Count).Subscribe(_ =>
            {
                (UIMain.Instance.uISelectedWindow.uISelectedWindows[0] as UIBuildingWindow)?.UpdateBuildingList(1);
            });

            this.ObserveEveryValueChanged(_ => this.buildingTypes[2].Count).Subscribe(_ =>
            {
                (UIMain.Instance.uISelectedWindow.uISelectedWindows[0] as UIBuildingWindow)?.UpdateBuildingList(2);
            });
        }


        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {
            for(int i=0; i<this.unlockIDisposableAmount.Count;)
            {
                var id = this.unlockIDisposableAmount.ElementAt(i);
                if(id.Value!=null)
                {
                    id.Value.Dispose();
                }
                this.unlockIDisposableAmount.Remove(id.Key);
            }

            for (int i = 0; i < this.unlockIDisposableBlueprint.Count;)
            {
                var id = this.unlockIDisposableBlueprint.ElementAt(i);
                if (id.Value != null)
                {
                    id.Value.Dispose();
                }
                this.unlockIDisposableBlueprint.Remove(id.Key);
            }


            for (int i = 0; i < this.gatheringBuildings.Count;)
            {
                var item = this.gatheringBuildings.ElementAt(i);
                this.RemoveBuilding(0, item.Value);
            }

            for (int i = 0; i < this.productionBuildings.Count;)
            {
                var item = this.productionBuildings.ElementAt(i);
                this.RemoveBuilding(1, item.Value);
            }

            for (int i = 0; i < this.battleBuildings.Count;)
            {
                var item = this.battleBuildings.ElementAt(i);
                this.RemoveBuilding(2, item.Value);
            }

            //初始化建筑类型
            for (int i = 0; i < this.buildingTypes.Count; i++)
            {
                this.buildingTypes[i].Clear();
            }

            foreach(var conditionType in buildCondition)
            {
                conditionType.Value.Clear();
            }

            this.bluePrints.Clear();

            this.InitBuildingType();
        }

        /// <summary>
        /// 重开
        /// </summary>
        public void Restart()
        {
            this.Init();
        }

        /// <summary>
        /// 读档
        /// </summary>
        public void ReadArchive()
        {
            this.Init();
            foreach(var buildingData in ArchiveManager.archive.buildingData)
            {
                this.GetBuilding(buildingData.buildingType, PlotManager.Instance.plots[buildingData.pos]);
            }
        }

        /// <summary>
        /// 初始化建筑类型，根据建筑的解锁条件，订阅解锁事件
        /// </summary>
        void InitBuildingType()
        {

            foreach(var list in DataManager.BuildingScriptLists)
            {
                foreach(var building in list)
                {
                    switch (building.Condition)
                    {
                        case Building_Condition_Type.无://无条件,立刻解锁
                            switch (building.Class)
                            {
                                case Building_Type.自动采集建筑:
                                    this.buildingTypes[0].Add(building.Type);
                                    break;
                                case Building_Type.生产建筑:
                                    this.buildingTypes[1].Add(building.Type);
                                    break;
                                case Building_Type.战斗建筑:
                                    this.buildingTypes[2].Add(building.Type);
                                    break;
                            }
                            break;
                        case Building_Condition_Type.资源1:
                            this.buildCondition[Building_Condition_Type.资源1].Add(building.Type, building.NumericalValue);
                            if(!this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.资源1))
                                this.unlockIDisposableAmount.Add(Building_Condition_Type.资源1, null);
                            break;
                        case Building_Condition_Type.资源2:
                            this.buildCondition[Building_Condition_Type.资源2].Add(building.Type, building.NumericalValue);
                            if (!this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.资源2))
                                this.unlockIDisposableAmount.Add(Building_Condition_Type.资源2, null);
                            break;
                        case Building_Condition_Type.资源3:
                            this.buildCondition[Building_Condition_Type.资源3].Add(building.Type, building.NumericalValue);
                            if (!this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.资源3))
                                this.unlockIDisposableAmount.Add(Building_Condition_Type.资源3, null);
                            break;
                        case Building_Condition_Type.蓝图:
                            this.buildCondition[Building_Condition_Type.蓝图].Add(building.Type, building.NumericalValue);
                            this.bluePrints.Add(building.NumericalValue, false);//初始化蓝图
                            this.unlockIDisposableBlueprint.Add(building.NumericalValue, null);
                            break;
                        case Building_Condition_Type.回合数:
                            this.buildCondition[Building_Condition_Type.回合数].Add(building.Type, building.NumericalValue);
                            if (!this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.回合数))
                                this.unlockIDisposableAmount.Add(Building_Condition_Type.回合数, null);
                            break;
                        case Building_Condition_Type.厉害的战斗机器:
                            this.buildCondition[Building_Condition_Type.厉害的战斗机器].Add(building.Type, building.NumericalValue);
                                if (!this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.厉害的战斗机器))
                            this.unlockIDisposableAmount.Add(Building_Condition_Type.厉害的战斗机器, null);
                            break;
                    }
                }
            }
            if (this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.资源1))
                this.unlockIDisposableAmount[Building_Condition_Type.资源1] = ResourcesManager.Instance.unlockByResouces[0]
                .Subscribe(resource1 =>
                {
                    for (int i = 0; i < this.buildCondition[Building_Condition_Type.资源1].Count;)
                    {
                        var type = this.buildCondition[Building_Condition_Type.资源1].ElementAt(i).Key;
                        if (resource1 >= this.buildCondition[Building_Condition_Type.资源1][type])
                        {
                            int sort;
                            if ((int)type > (int)Building_Type.自动采集建筑 && (int)type < (int)Building_Type.生产建筑)
                            {
                                sort = 0;
                            }
                            else if ((int)type > (int)Building_Type.生产建筑 && (int)type < (int)Building_Type.战斗建筑)
                            {
                                sort = 1;
                            }
                            else
                            {
                                sort = 2;
                            }
                            this.buildingTypes[sort].Add(type);
                            this.buildCondition[Building_Condition_Type.资源1].Remove(type);
                            Debug.Log("解锁通过资源1解锁建筑");
                        }
                        else
                        {
                            i++;
                        }
                    }
                    if (this.buildCondition[Building_Condition_Type.资源1].Count == 0)
                    {
                        unlockIDisposableAmount[Building_Condition_Type.资源1].Dispose();
                    }
                });


            if (this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.资源2))
                this.unlockIDisposableAmount[Building_Condition_Type.资源2] = ResourcesManager.Instance.unlockByResouces[1]
                    .Subscribe(resource2 =>
                    {
                        for (int i = 0; i < this.buildCondition[Building_Condition_Type.资源2].Count;)
                        {
                            var type = this.buildCondition[Building_Condition_Type.资源2].ElementAt(i).Key;
                            if (resource2 >= this.buildCondition[Building_Condition_Type.资源2][type])
                            {
                                int sort;
                                if ((int)type > (int)Building_Type.自动采集建筑 && (int)type < (int)Building_Type.生产建筑)
                                {
                                    sort = 0;
                                }
                                else if ((int)type > (int)Building_Type.生产建筑 && (int)type < (int)Building_Type.战斗建筑)
                                {
                                    sort = 1;
                                }
                                else
                                {
                                    sort = 2;
                                }
                                this.buildingTypes[sort].Add(type);
                                this.buildCondition[Building_Condition_Type.资源2].Remove(type);
                                Debug.Log("解锁通过资源2解锁建筑");
                            }
                            else
                            {
                                i++;
                            }
                        }
                        if (this.buildCondition[Building_Condition_Type.资源2].Count == 0)
                        {
                            unlockIDisposableAmount[Building_Condition_Type.资源2].Dispose();
                        }
                    });

            if (this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.资源3))
                this.unlockIDisposableAmount[Building_Condition_Type.资源3] = ResourcesManager.Instance.unlockByResouces[2]
                    .Subscribe(resource3 =>
                    {
                        for (int i = 0; i < this.buildCondition[Building_Condition_Type.资源3].Count;)
                        {
                            var type = this.buildCondition[Building_Condition_Type.资源3].ElementAt(i).Key;
                            if (resource3 >= this.buildCondition[Building_Condition_Type.资源3][type])
                            {
                                int sort;
                                if ((int)type > (int)Building_Type.自动采集建筑 && (int)type < (int)Building_Type.生产建筑)
                                {
                                    sort = 0;
                                }
                                else if ((int)type > (int)Building_Type.生产建筑 && (int)type < (int)Building_Type.战斗建筑)
                                {
                                    sort = 1;
                                }
                                else
                                {
                                    sort = 2;
                                }
                                this.buildingTypes[sort].Add(type);
                                this.buildCondition[Building_Condition_Type.资源3].Remove(type);
                                Debug.Log("解锁通过资源3解锁建筑");
                            }
                            else
                            {
                                i++;
                            }
                        }
                        if (this.buildCondition[Building_Condition_Type.资源3].Count == 0)
                        {
                            unlockIDisposableAmount[Building_Condition_Type.资源3].Dispose();
                        }
                    });

            if (this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.回合数))
                this.unlockIDisposableAmount[Building_Condition_Type.回合数] = RoundManager.Instance.unlockPlotByRound
                    .Subscribe(roundNumber =>
                    {
                        for (int i = 0; i < this.buildCondition[Building_Condition_Type.回合数].Count;)
                        {
                            var type = this.buildCondition[Building_Condition_Type.回合数].ElementAt(i).Key;
                            if (roundNumber >= this.buildCondition[Building_Condition_Type.回合数][type])
                            {
                                int sort;
                                if ((int)type > (int)Building_Type.自动采集建筑 && (int)type < (int)Building_Type.生产建筑)
                                {
                                    sort = 0;
                                }
                                else if ((int)type > (int)Building_Type.生产建筑 && (int)type < (int)Building_Type.战斗建筑)
                                {
                                    sort = 1;
                                }
                                else
                                {
                                    sort = 2;
                                }
                                this.buildingTypes[sort].Add(type);
                                this.buildCondition[Building_Condition_Type.回合数].Remove(type);
                                Debug.Log("解锁通过回合数解锁建筑");
                            }
                            else
                            {
                                i++;
                            }
                        }
                        if (this.buildCondition[Building_Condition_Type.回合数].Count == 0)
                        {
                            unlockIDisposableAmount[Building_Condition_Type.回合数].Dispose();
                        }
                    });

            for (int i = 0; i < this.bluePrints.Count; i++)
            {
                var id = this.bluePrints.ElementAt(i).Key;
                this.unlockIDisposableBlueprint[id] = this.ObserveEveryValueChanged(_ => this.bluePrints[id])
                    .Subscribe(bluePrint =>
                    {
                        if (bluePrint)
                        {
                            foreach (var type in this.buildCondition[Building_Condition_Type.蓝图].Keys)
                            {
                                if (this.buildCondition[Building_Condition_Type.蓝图][type] == id)
                                {
                                    int sort;
                                    if ((int)type > (int)Building_Type.自动采集建筑 && (int)type < (int)Building_Type.生产建筑)
                                    {
                                        sort = 0;
                                    }
                                    else if ((int)type > (int)Building_Type.生产建筑 && (int)type < (int)Building_Type.战斗建筑)
                                    {
                                        sort = 1;
                                    }
                                    else
                                    {
                                        sort = 2;
                                    }
                                    this.buildingTypes[sort].Add(type);
                                    this.buildCondition[Building_Condition_Type.蓝图].Remove(type);
                                    Debug.LogFormat("解锁通过蓝图{0}解锁建筑", id);
                                    break;
                                }
                            }
                        }
                        if (this.buildCondition[Building_Condition_Type.蓝图].Count == 0)
                        {
                            unlockIDisposableBlueprint[id].Dispose();
                        }
                    });
            }

            if (this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.厉害的战斗机器))
                this.unlockIDisposableAmount[Building_Condition_Type.厉害的战斗机器] = this.ObserveEveryValueChanged(_ => this.battleBuildings.Count)
                    .Subscribe(_ =>
                    {
                        for (int i = 0; i < this.buildCondition[Building_Condition_Type.厉害的战斗机器].Count;)
                        {
                            var type = this.buildCondition[Building_Condition_Type.厉害的战斗机器].ElementAt(i).Key;
                            if (this.battleBuildings.Count >= this.buildCondition[Building_Condition_Type.厉害的战斗机器][type])
                            {
                                int sort;
                                if ((int)type > (int)Building_Type.自动采集建筑 && (int)type < (int)Building_Type.生产建筑)
                                {
                                    sort = 0;
                                }
                                else if ((int)type > (int)Building_Type.生产建筑 && (int)type < (int)Building_Type.战斗建筑)
                                {
                                    sort = 1;
                                }
                                else
                                {
                                    sort = 2;
                                }
                                this.buildingTypes[sort].Add(type);
                                this.buildCondition[Building_Condition_Type.厉害的战斗机器].Remove(type);
                                Debug.Log("解锁通过厉害的战斗机器解锁建筑");
                            }
                            else
                            {
                                i++;
                            }
                        }
                        if (this.buildCondition[Building_Condition_Type.厉害的战斗机器].Count == 0)
                        {
                            unlockIDisposableAmount[Building_Condition_Type.厉害的战斗机器].Dispose();
                        }
                    });
        }

        /// <summary>
        /// 从对象池中获取建筑
        /// </summary>
        /// <param name="type"></param>
        /// <param name="plot"></param>
        void GetBuilding(Building_Type type, Plot plot)
        {
            int sort = (int)type;
            GameObject gO;
            if(sort>(int)Building_Type.自动采集建筑&&sort<(int)Building_Type.生产建筑)
            {
                gO = GameObjectPool.Instance.GatheringBuildings.Get();
                GatheringBuilding building = gO.GetComponent<GatheringBuilding>();
                this.gatheringBuildings.Add(plot.pos, building);
                building.SetInfo(plot, type);
                plot.building = building;
            }
            else if(sort > (int)Building_Type.生产建筑 && sort < (int)Building_Type.战斗建筑)
            {
                gO = GameObjectPool.Instance.ProductionBuildings.Get();
                ProductionBuilding building = gO.GetComponent<ProductionBuilding>();
                this.productionBuildings.Add(plot.pos, building);
                building.SetInfo(plot, type);
                plot.building = building;
            }
            else
            {
                gO = GameObjectPool.Instance.BattleBuildings.Get();
                BattleBuilding building = gO.GetComponent<BattleBuilding>();
                this.battleBuildings.Add(plot.pos, building);
                building.SetInfo(plot, type);
                plot.building = building;
            }
            gO.transform.SetParent(this.transform);
        }

        /// <summary>
        /// 在给定的板块上建造给定的建筑建筑
        /// </summary>
        /// <param name="type"></param>
        /// <param name="plot"></param>
        public bool Build(Building_Type type,Plot plot)
        {
            if(ResourcesManager.Instance.CanBuild(type))
            {
                if(plot.building==null)
                {
                    this.GetBuilding(type, plot);
                    return true;
                }
                else
                {
                    Debug.Log("该板块已有建筑");
                }
            }
            return false;
        }


        /// <summary>
        /// 删除建筑
        /// </summary>
        /// <param name="removeId"></param>
        public void RemoveBuilding(int sort, Building building)
        {
            switch(sort)
            {
                case 0:
                    GameObjectPool.Instance.GatheringBuildings.Release(building.gameObject);
                    this.gatheringBuildings.Remove(building.pos);
                    break;
                case 1:
                    GameObjectPool.Instance.ProductionBuildings.Release(building.gameObject);
                    this.productionBuildings.Remove(building.pos);
                    break;
                case 2:
                    GameObjectPool.Instance.BattleBuildings.Release(building.gameObject);
                    this.battleBuildings.Remove(building.pos);
                    break;
            }
            PlotManager.Instance.plots[building.pos].building = null;
        }

        /// <summary>
        /// 生产建筑生产资源总和
        /// </summary>
        void GatherBuildingResources()
        {
            int[] buildingResourcesGathering = new int[3] { 0, 0, 0 };
            foreach (var item in this.gatheringBuildings)
            {
                List<int> resource = item.Value.Gather();
                buildingResourcesGathering[resource[0]] += resource[1];
            }
            ResourcesManager.Instance.ChangeBuildingResources(buildingResourcesGathering,true);
        }

        void ProduceWealth()
        {
            int wealthProduction = 0;
            foreach (var item in this.productionBuildings)
            {
                int wealth = item.Value.Produce();
                wealthProduction += wealth;
            }
            ResourcesManager.Instance.ChangeWealth(wealthProduction);
        }


        /// <summary>
        /// 回合结束，进行结算
        /// </summary>
        public void RoundOver()
        {
            this.GatherBuildingResources();
            this.ProduceWealth();
            //this.GetBuildingType();
        }

        /// <summary>
        /// 根据建筑类型获取建筑
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static BuildingType GetBuildingByType(Building_Type type)
        {
            BuildingType buildingType;
            int sort = (int)type;
            if (sort > (int)Building_Type.自动采集建筑 && sort < (int)Building_Type.生产建筑)
            {
                buildingType = DataManager.BuildingScriptLists[0][(int)type - (int)Building_Type.自动采集建筑 - 1];
            }
            else if (sort > (int)Building_Type.生产建筑 && sort < (int)Building_Type.战斗建筑)
            {
                buildingType = DataManager.BuildingScriptLists[1][(int)type - (int)Building_Type.生产建筑 - 1];
            }
            else
            {
                buildingType = DataManager.BuildingScriptLists[2][(int)type - (int)Building_Type.战斗建筑 - 1];
            }
            return buildingType;
        }
    }
}