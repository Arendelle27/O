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
        [SerializeField, LabelText("当前存在的建筑"), ReadOnly]
        public Dictionary<int,Building> buildings=new Dictionary<int, Building>();
        //当前建筑的Id+1
        int ids;

        public List<Building_Type> GatheringTypes = new List<Building_Type>() {
            //Building_Type.自动采集建筑_1,
            //Building_Type.自动采集建筑_2,
        };

        public List<Building_Type> ProductionTypes = new List<Building_Type>() {
            //Building_Type.生产建筑_1,
            //Building_Type.生产建筑_2
        };

        public void Init()
        {
            for (int i = 0; i < buildings.Count;)
            {
                var item = buildings.ElementAt(i);
                this.RemoveBuilding(item.Value);
            }
            this.ids = 0;

        }

        /// <summary>
        /// 在给定的板块上建造给定的建筑建筑
        /// </summary>
        /// <param name="type"></param>
        /// <param name="plot"></param>
        public void GetBuilding(Building_Type type,Plot plot)
        {
            if(DataManager.Instance.CanBuild(type))
            {
                DataManager.Instance.ChangeBuildingResources(new int[3] {-1,-1,-1});
                DataManager.Instance.ChangeExecution(-1);

                GameObject go = GameObjectPool.Instance.Buildings.Get();
                go.transform.parent = this.transform;
                go.transform.position = plot.transform.position - new Vector3(0, 0, 0.3f);

                Building building = go.GetComponent<Building>();
                building.SetInfo(this.ids, type);
                this.buildings.Add(this.ids, building);//添加到建筑列表

                plot.building = building;

                plot.settlement?.AddHotility(false);//建造建筑增加敌意值

                this.ids++;
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
            this.buildings.Remove(building.id);

        }

        /// <summary>
        /// 回合结束，进行结算
        /// </summary>
        public void RoundOver()
        {

        }
    }
}

