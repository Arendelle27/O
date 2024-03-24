using ENTITY;
using Managers;
using Sirenix.OdinInspector;
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
        //���浱ǰ���ڵĽ���
        [SerializeField, LabelText("��ǰ���ڵĲɼ�����"), ReadOnly]
        public Dictionary<Vector2Int,GatheringBuilding> gatheringBuildings=new Dictionary<Vector2Int, GatheringBuilding>();

        [SerializeField, LabelText("��ǰ���ڵ���������"), ReadOnly]
        public Dictionary<Vector2Int, ProductionBuilding> productionBuildings = new Dictionary<Vector2Int, ProductionBuilding>();

        [SerializeField, LabelText("��ǰ���ڵ�ս������"), ReadOnly]
        public Dictionary<Vector2Int, BattleBuilding> battleBuildings = new Dictionary<Vector2Int, BattleBuilding>();

        [SerializeField, LabelText("��ǰ���ڵĽ�������"), ReadOnly]
        public List<List<Building_Type>> buildingTypes = new List<List<Building_Type>>()
        {
            new List<Building_Type>(),
            new List<Building_Type>(),
            new List<Building_Type>(),
        };

        [SerializeField, LabelText("������ͼ"), ReadOnly]
        Dictionary<int, bool> bluePrints;

        [SerializeField, LabelText("����UI����"), ReadOnly]
        public UIBuildingWindow buildingWindow;

        private void Start()
        {
            this.buildingWindow = UIManager.Instance.Show<UIBuildingWindow>();

            this.ObserveEveryValueChanged(_ => this.buildingTypes[0].Count).Subscribe(_ =>
            {
                this.buildingWindow?.UpdateBuildingList(0);
            });

            this.ObserveEveryValueChanged(_ => this.buildingTypes[1].Count).Subscribe(_ =>
            {
                this.buildingWindow?.UpdateBuildingList(1);
            });

            this.ObserveEveryValueChanged(_ => this.buildingTypes[2].Count).Subscribe(_ =>
            {
                this.buildingWindow?.UpdateBuildingList(2);
            });
        }


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

            this.bluePrints = new Dictionary<int, bool>()
            {
                {1,false },
                {2,false },
            };

            //��ʼ����������
            for (int i = 0; i < this.buildingTypes.Count; i++)
            {
                this.buildingTypes[i].Clear();
            }

            this.GetBuildingType();
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
        /// h��ȡ��������
        /// </summary>
        void GetBuildingType()
        {
            foreach(var list in DataManager.BuildingScriptLists)
            {
                foreach(var building in list)
                {
                    for(int i = 0; i < this.buildingTypes.Count; i++)
                    {
                        if(buildingTypes[i].Contains(building.Type))
                        {
                            return;
                        }
                    }

                    bool isUnlock = false;
                    switch (building.Class)
                    {
                        case Building_Type.�Զ��ɼ�����:
                            switch (building.Condition)
                            {
                                case Building_Condition_Type.��Դ1:
                                    isUnlock=ResourcesManager.Instance.UnlockBuildingTypeByResource(0,building.NumericalValue);
                                    break;
                                case Building_Condition_Type.��Դ2:
                                    isUnlock=ResourcesManager.Instance.UnlockBuildingTypeByResource(1, building.NumericalValue);
                                    break;
                                case Building_Condition_Type.��Դ3:
                                    isUnlock=ResourcesManager.Instance.UnlockBuildingTypeByResource(2, building.NumericalValue);
                                    break;
                                case Building_Condition_Type.��ͼ:
                                    isUnlock = this.bluePrints[building.NumericalValue];
                                    break;
                            }
                            if(isUnlock)
                            {
                                this.buildingTypes[0].Add(building.Type);
                            }
                            break;
                        case Building_Type.��������:
                            switch (building.Condition)
                            {
                                case Building_Condition_Type.��:
                                    isUnlock = true;
                                    break;
                                case Building_Condition_Type.�غ���:
                                    isUnlock=RoundManager.Instance.UnlockBuildingTypeByResource(building.NumericalValue);
                                    break;
                            }
                            if(isUnlock)
                            {
                                this.buildingTypes[1].Add(building.Type);
                            }
                            break;
                        case Building_Type.ս������:
                            switch (building.Condition)
                            {
                                case Building_Condition_Type.��ͼ:
                                    isUnlock = this.bluePrints[building.NumericalValue];
                                    break;
                                case Building_Condition_Type.������ս������:
                                    if(this.battleBuildings.Count>building.NumericalValue)
                                    {
                                        isUnlock = true;
                                    }
                                    break;
                            }
                            if(isUnlock)
                            {
                                this.buildingTypes[2].Add(building.Type);
                            }
                            break;
                    }
                }
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
                ResourcesManager.Instance.ChangeExecution(-1);//����һ���ж���
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
            this.GetBuildingType();
        }
    }
}

