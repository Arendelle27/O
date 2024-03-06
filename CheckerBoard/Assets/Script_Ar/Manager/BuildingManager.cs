using ENTITY;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MANAGER
{
    public class BuildingManager : MonoSingleton<BuildingManager>
    {
        //���浱ǰ���ڵĽ���
        [SerializeField, LabelText("��ǰ���ڵĽ���"), ReadOnly]
        public Dictionary<int,Building> buildings=new Dictionary<int, Building>();
        //��ǰ������Id+1
        int ids;

        public List<Building_Type> GatheringTypes = new List<Building_Type>() {
            Building_Type.�Զ��ɼ�����_1,
            Building_Type.�Զ��ɼ�����_2,
        };

        public List<Building_Type> ProductionTypes = new List<Building_Type>() {
            Building_Type.��������_1,
            Building_Type.��������_2,
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
        /// �ڸ����İ���Ͻ�������Ľ�������
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
                Debug.Log("��Դ����");
            }
        }


        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="removeId"></param>
        public void RemoveBuilding(int removeId)
        {
            GameObjectPool.Instance.Buildings.Release(this.buildings[removeId].gameObject);
            this.buildings.Remove(removeId);

        }

        /// <summary>
        /// �غϽ��������н���
        /// </summary>
        public void RoundOver()
        {

        }
    }
}

