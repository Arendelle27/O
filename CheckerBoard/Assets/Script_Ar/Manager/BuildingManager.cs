using ENTITY;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ArchiveManager;

namespace MANAGER
{
    public class BuildingManager : MonoSingleton<BuildingManager>
    {
        //储存当前存在的建筑
        [SerializeField, LabelText("当前存在的建筑"), ReadOnly]
        public Dictionary<Vector2Int,Building> buildings=new Dictionary<Vector2Int, Building>();

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

        void Init()
        {
            for (int i = 0; i < buildings.Count;)
            {
                var item = buildings.ElementAt(i);
                this.RemoveBuilding(item.Value);
            }

        }

        /// <summary>
        /// 重开
        /// </summary>
        public void Restart()
        {
            this.Init();
        }

        public void ReadArchive()
        {
            this.Init();
            foreach(var buildingData in ArchiveManager.archive.buildingData)
            {
                this.GetBuilding(buildingData.buildingType, PlotManager.Instance.plots[buildingData.pos]);
                //GameObject gO = GameObjectPool.Instance.Buildings.Get();
                //gO.transform.parent = this.transform;
                //gO.transform.position = PlotManager.Instance.plots[buildingData.pos].transform.position - new Vector3(0, 0, 0.3f);
                //Building building = gO.GetComponent<Building>();
                //building.SetInfo(buildingData.pos, buildingData.buildingType);
                //this.buildings.Add(buildingData.pos, building);
                //PlotManager.Instance.plots[buildingData.pos].building = building;
            }
        }

        /// <summary>
        /// 从对象池中获取建筑
        /// </summary>
        /// <param name="type"></param>
        /// <param name="plot"></param>
        void GetBuilding(Building_Type type, Plot plot)
        {
            GameObject gO = GameObjectPool.Instance.Buildings.Get();
            gO.transform.parent = this.transform;
            gO.transform.position = plot.transform.position - new Vector3(0, 0, 0.3f);
            Building building = gO.GetComponent<Building>();
            building.SetInfo(plot.pos, type);
            this.buildings.Add(plot.pos, building);
            plot.building = building;
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
        public void RemoveBuilding(Building building)
        {
            GameObjectPool.Instance.Buildings.Release(building.gameObject);
            this.buildings.Remove(building.pos);

        }

        /// <summary>
        /// 回合结束，进行结算
        /// </summary>
        public void RoundOver()
        {

        }
    }
}

