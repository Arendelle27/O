using ENTITY;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MANAGER
{
    public class BuildingManager : MonoSingleton<BuildingManager>
    {
        //储存当前存在的建筑
        [SerializeField, LabelText("当前存在的采集建筑"), ReadOnly]
        public Dictionary<Vector2Int,GatheringBuilding> gatheringBuildings=new Dictionary<Vector2Int, GatheringBuilding>();

        [SerializeField, LabelText("当前存在的生产建筑"), ReadOnly]
        public Dictionary<Vector2Int, ProductionBuilding> productionBuildings = new Dictionary<Vector2Int, ProductionBuilding>();

        [SerializeField, LabelText("当前存在的战斗建筑"), ReadOnly]
        public Dictionary<Vector2Int, BattleBuilding> battleBuildings = new Dictionary<Vector2Int, BattleBuilding>();

        [SerializeField, LabelText("采集建筑类型"),Tooltip("采集建筑物的类型")]
        public List<Building_Type> GatheringTypes = new List<Building_Type>() {
            //Building_Type.自动采集建筑_1,
            //Building_Type.自动采集建筑_2,
        };

        [SerializeField, LabelText("生产建筑类型"), Tooltip("生产建筑物的类型")]
        public List<Building_Type> ProductionTypes = new List<Building_Type>() {
            //Building_Type.生产建筑_1,
            //Building_Type.生产建筑_2
        };

        [SerializeField, LabelText("战斗建筑类型"), Tooltip("生产建筑物的类型")]
        public List<Building_Type> BattleTypes = new List<Building_Type>()
        {
            //Building_Type.战斗建筑_1,
        };

        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {
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
        /// 从对象池中获取建筑
        /// </summary>
        /// <param name="type"></param>
        /// <param name="plot"></param>
        void GetBuilding(Building_Type type, Plot plot)
        {
            int sort = (int)type;
            GameObject gO;
            if(sort<3)
            {
                gO = GameObjectPool.Instance.GatheringBuildings.Get();
                GatheringBuilding building = gO.GetComponent<GatheringBuilding>();
                this.gatheringBuildings.Add(plot.pos, building);
                building.SetInfo(plot, type);
                plot.building = building;
            }
            else if(sort<5)
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
            gO.transform.parent = this.transform;
        }

        /// <summary>
        /// 在给定的板块上建造给定的建筑建筑
        /// </summary>
        /// <param name="type"></param>
        /// <param name="plot"></param>
        public void Build(Building_Type type,Plot plot)
        {
            if(DataManager.Instance.CanBuild(type))
            {
                this.GetBuilding(type, plot);
                //消耗资源
                DataManager.Instance.ChangeBuildingResources(new int[3] {-1,-1,-1});
                DataManager.Instance.ChangeExecution(-1);
                //建造建筑增加敌意值
                plot.settlement?.AddHotility(false);

            }
            else
            {
                Debug.Log("资源不足");
            }
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
        }

        /// <summary>
        /// 回合结束，进行结算
        /// </summary>
        public void RoundOver()
        {

        }
    }
}

