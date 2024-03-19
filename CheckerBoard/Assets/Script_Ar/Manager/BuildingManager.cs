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
        [SerializeField, LabelText("��ǰ���ڵĲɼ�����"), ReadOnly]
        public Dictionary<Vector2Int,GatheringBuilding> gatheringBuildings=new Dictionary<Vector2Int, GatheringBuilding>();

        [SerializeField, LabelText("��ǰ���ڵ���������"), ReadOnly]
        public Dictionary<Vector2Int, ProductionBuilding> productionBuildings = new Dictionary<Vector2Int, ProductionBuilding>();

        [SerializeField, LabelText("��ǰ���ڵ�ս������"), ReadOnly]
        public Dictionary<Vector2Int, BattleBuilding> battleBuildings = new Dictionary<Vector2Int, BattleBuilding>();

        [SerializeField, LabelText("�ɼ���������"),Tooltip("�ɼ������������")]
        public List<Building_Type> GatheringTypes = new List<Building_Type>() {};

        [SerializeField, LabelText("������������"), Tooltip("���������������")]
        public List<Building_Type> ProductionTypes = new List<Building_Type>() {};

        [SerializeField, LabelText("ս����������"), Tooltip("���������������")]
        public List<Building_Type> BattleTypes = new List<Building_Type>()  {};


        /// <summary>
        /// ��ʼ��
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
        /// �ؿ�
        /// </summary>
        public void Restart()
        {
            this.Init();
        }

        /// <summary>
        /// ����
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
        /// �Ӷ�����л�ȡ����
        /// </summary>
        /// <param name="type"></param>
        /// <param name="plot"></param>
        void GetBuilding(Building_Type type, Plot plot)
        {
            int sort = (int)type;
            GameObject gO;
            if(sort>(int)Building_Type.�Զ��ɼ�����&&sort<(int)Building_Type.��������)
            {
                gO = GameObjectPool.Instance.GatheringBuildings.Get();
                GatheringBuilding building = gO.GetComponent<GatheringBuilding>();
                this.gatheringBuildings.Add(plot.pos, building);
                building.SetInfo(plot, type);
                plot.building = building;
            }
            else if(sort > (int)Building_Type.�������� && sort < (int)Building_Type.ս������)
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
        /// �ڸ����İ���Ͻ�������Ľ�������
        /// </summary>
        /// <param name="type"></param>
        /// <param name="plot"></param>
        public void Build(Building_Type type,Plot plot)
        {
            if(plot.building!=null)
            {
                Debug.Log("�Ѿ��н�����");
                return;
            }
            if(ResourcesManager.Instance.CanBuild(type))
            {
                this.GetBuilding(type, plot);
                //������Դ
                //ResourcesManager.Instance.ChangeBuildingResources(new int[3] {-1,-1,-1});
                ResourcesManager.Instance.ChangeExecution(-1);
                //���콨�����ӵ���ֵ

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
        /// ��������������Դ�ܺ�
        /// </summary>
        void GatherBuildingResources()
        {
            int[] buildingResourcesGathering = new int[3] { 0, 0, 0 };
            foreach (var item in this.gatheringBuildings)
            {
                List<int> resource = item.Value.Gather();
                buildingResourcesGathering[resource[0]] += resource[1];
            }
            ResourcesManager.Instance.ChangeBuildingResources(buildingResourcesGathering);
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
        /// �غϽ��������н���
        /// </summary>
        public void RoundOver()
        {
            this.GatherBuildingResources();
            this.ProduceWealth();
        }
    }
}

