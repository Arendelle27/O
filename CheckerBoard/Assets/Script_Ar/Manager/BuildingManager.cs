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
        //���浱ǰ���ڵĽ���
        [SerializeField, LabelText("��ǰ���ڵĽ���"), ReadOnly]
        public Dictionary<int,Building> buildings=new Dictionary<int, Building>();
        //��ǰ������Id+1
        int ids;

        public List<Building_Type> GatheringTypes = new List<Building_Type>() {
            //Building_Type.�Զ��ɼ�����_1,
            //Building_Type.�Զ��ɼ�����_2,
        };

        public List<Building_Type> ProductionTypes = new List<Building_Type>() {
            //Building_Type.��������_1,
            //Building_Type.��������_2
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
        /// �ڸ����İ���Ͻ�������Ľ�������
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
                this.buildings.Add(this.ids, building);//��ӵ������б�

                plot.building = building;

                plot.settlement?.AddHotility(false);//���콨�����ӵ���ֵ

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
        public void RemoveBuilding(Building building)
        {
            GameObjectPool.Instance.Buildings.Release(building.gameObject);
            this.buildings.Remove(building.id);

        }

        /// <summary>
        /// �غϽ��������н���
        /// </summary>
        public void RoundOver()
        {

        }
    }
}

