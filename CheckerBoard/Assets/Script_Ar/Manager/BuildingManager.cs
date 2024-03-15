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
        //���浱ǰ���ڵĽ���
        [SerializeField, LabelText("��ǰ���ڵĽ���"), ReadOnly]
        public Dictionary<Vector2Int,Building> buildings=new Dictionary<Vector2Int, Building>();

        [SerializeField, LabelText("�ɼ���������"),Tooltip("�ɼ������������")]
        public List<Building_Type> GatheringTypes = new List<Building_Type>() {
            //Building_Type.�Զ��ɼ�����_1,
            //Building_Type.�Զ��ɼ�����_2,
        };

        [SerializeField, LabelText("������������"), Tooltip("���������������")]
        public List<Building_Type> ProductionTypes = new List<Building_Type>() {
            //Building_Type.��������_1,
            //Building_Type.��������_2
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
        /// �ؿ�
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
        /// �Ӷ�����л�ȡ����
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
        /// �ڸ����İ���Ͻ�������Ľ�������
        /// </summary>
        /// <param name="type"></param>
        /// <param name="plot"></param>
        public void Build(Building_Type type,Plot plot)
        {
            if(DataManager.Instance.CanBuild(type))
            {
                this.GetBuilding(type, plot);
                //������Դ
                DataManager.Instance.ChangeBuildingResources(new int[3] {-1,-1,-1});
                DataManager.Instance.ChangeExecution(-1);
                //���콨�����ӵ���ֵ
                plot.settlement?.AddHotility(false);

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
            this.buildings.Remove(building.pos);

        }

        /// <summary>
        /// �غϽ��������н���
        /// </summary>
        public void RoundOver()
        {

        }
    }
}

