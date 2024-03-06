using ENTITY;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
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
            Building_Type.自动采集建筑_1,
            Building_Type.自动采集建筑_2,
        };

        public List<Building_Type> ProductionTypes = new List<Building_Type>() {
            Building_Type.生产建筑_1,
            Building_Type.生产建筑_2,
        };

        public void Init()
        {
            foreach(var building in this.buildings)
            {
                this.RemoveBuilding(building.Key);
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
            if(ResourceManager.Instance.CanBuild(type))
            {
                ResourceManager.Instance.ChangeBuildingResources(new int[3] {-1,-1,-1});
                ResourceManager.Instance.ChangeExecution(-1);

                GameObject go = Instantiate(GameObjectPool.Instance.Buildings.Get(), this.transform);
                go.transform.position = plot.transform.position - new Vector3(0, 0, 0.3f);

                Building building = go.GetComponent<Building>();
                building.SetInfo(this.ids, type);


                plot.building = building;

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
        public void RemoveBuilding(int removeId)
        {
            GameObjectPool.Instance.Buildings.Release(this.buildings[removeId].gameObject);
            this.buildings.Remove(removeId);

        }

        /// <summary>
        /// 回合结束，进行结算
        /// </summary>
        public void RoundOver()
        {

        }
    }
}

