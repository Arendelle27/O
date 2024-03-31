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
        #region ÿ����Ϸ��ʼʱ��ʼ��  
        //���浱ǰ���ڵĽ���
        [SerializeField, LabelText("��ǰ���ڵĲɼ�����"), ReadOnly]
        public Dictionary<Vector2Int,GatheringBuilding> gatheringBuildings=new Dictionary<Vector2Int, GatheringBuilding>();

        [SerializeField, LabelText("��ǰ���ڵ���������"), ReadOnly]
        public Dictionary<Vector2Int, ProductionBuilding> productionBuildings = new Dictionary<Vector2Int, ProductionBuilding>();

        [SerializeField, LabelText("��ǰ���ڵ�ս������"), ReadOnly]
        public Dictionary<Vector2Int, BattleBuilding> battleBuildings = new Dictionary<Vector2Int, BattleBuilding>();

        [SerializeField, LabelText("�����Ľ�������"), ReadOnly]
        public Dictionary<Building_Condition_Type,Dictionary<Building_Type,int>> buildCondition = new Dictionary<Building_Condition_Type, Dictionary<Building_Type, int>>()
        {
            {Building_Condition_Type.��Դ1,new Dictionary<Building_Type, int>() },
            {Building_Condition_Type.��Դ2,new Dictionary<Building_Type, int>() },
            {Building_Condition_Type.��Դ3,new Dictionary<Building_Type, int>() },
            {Building_Condition_Type.��ͼ,new Dictionary<Building_Type, int>() },
            {Building_Condition_Type.�غ���,new Dictionary<Building_Type, int>() },
            {Building_Condition_Type.������ս������,new Dictionary<Building_Type, int>() },
        };

        [SerializeField, LabelText("��ǰ���ڵĽ�������"), ReadOnly]
        public List<List<Building_Type>> buildingTypes = new List<List<Building_Type>>()
        {
            new List<Building_Type>(),
            new List<Building_Type>(),
            new List<Building_Type>(),
        };

        [SerializeField, LabelText("������ͼ"), ReadOnly]
        public Dictionary<int, bool> bluePrints=new Dictionary<int, bool>();

        [SerializeField, LabelText("���������¼�"), ReadOnly]
        Dictionary<Building_Condition_Type, IDisposable> unlockIDisposableAmount = new Dictionary<Building_Condition_Type, IDisposable> { };

        [SerializeField, LabelText("��ͼ�����¼�"), ReadOnly]
        Dictionary<int, IDisposable> unlockIDisposableBlueprint = new Dictionary<int, IDisposable>{};
        #endregion

        [SerializeField, LabelText("��ѡ�еĽ���"), ReadOnly]
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
        /// ��ʼ��
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

            //��ʼ����������
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
        /// ��ʼ���������ͣ����ݽ����Ľ������������Ľ����¼�
        /// </summary>
        void InitBuildingType()
        {

            foreach(var list in DataManager.BuildingScriptLists)
            {
                foreach(var building in list)
                {
                    switch (building.Condition)
                    {
                        case Building_Condition_Type.��://������,���̽���
                            switch (building.Class)
                            {
                                case Building_Type.�Զ��ɼ�����:
                                    this.buildingTypes[0].Add(building.Type);
                                    break;
                                case Building_Type.��������:
                                    this.buildingTypes[1].Add(building.Type);
                                    break;
                                case Building_Type.ս������:
                                    this.buildingTypes[2].Add(building.Type);
                                    break;
                            }
                            break;
                        case Building_Condition_Type.��Դ1:
                            this.buildCondition[Building_Condition_Type.��Դ1].Add(building.Type, building.NumericalValue);
                            if(!this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.��Դ1))
                                this.unlockIDisposableAmount.Add(Building_Condition_Type.��Դ1, null);
                            break;
                        case Building_Condition_Type.��Դ2:
                            this.buildCondition[Building_Condition_Type.��Դ2].Add(building.Type, building.NumericalValue);
                            if (!this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.��Դ2))
                                this.unlockIDisposableAmount.Add(Building_Condition_Type.��Դ2, null);
                            break;
                        case Building_Condition_Type.��Դ3:
                            this.buildCondition[Building_Condition_Type.��Դ3].Add(building.Type, building.NumericalValue);
                            if (!this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.��Դ3))
                                this.unlockIDisposableAmount.Add(Building_Condition_Type.��Դ3, null);
                            break;
                        case Building_Condition_Type.��ͼ:
                            this.buildCondition[Building_Condition_Type.��ͼ].Add(building.Type, building.NumericalValue);
                            this.bluePrints.Add(building.NumericalValue, false);//��ʼ����ͼ
                            this.unlockIDisposableBlueprint.Add(building.NumericalValue, null);
                            break;
                        case Building_Condition_Type.�غ���:
                            this.buildCondition[Building_Condition_Type.�غ���].Add(building.Type, building.NumericalValue);
                            if (!this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.�غ���))
                                this.unlockIDisposableAmount.Add(Building_Condition_Type.�غ���, null);
                            break;
                        case Building_Condition_Type.������ս������:
                            this.buildCondition[Building_Condition_Type.������ս������].Add(building.Type, building.NumericalValue);
                                if (!this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.������ս������))
                            this.unlockIDisposableAmount.Add(Building_Condition_Type.������ս������, null);
                            break;
                    }
                }
            }
            if (this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.��Դ1))
                this.unlockIDisposableAmount[Building_Condition_Type.��Դ1] = ResourcesManager.Instance.unlockByResouces[0]
                .Subscribe(resource1 =>
                {
                    for (int i = 0; i < this.buildCondition[Building_Condition_Type.��Դ1].Count;)
                    {
                        var type = this.buildCondition[Building_Condition_Type.��Դ1].ElementAt(i).Key;
                        if (resource1 >= this.buildCondition[Building_Condition_Type.��Դ1][type])
                        {
                            int sort;
                            if ((int)type > (int)Building_Type.�Զ��ɼ����� && (int)type < (int)Building_Type.��������)
                            {
                                sort = 0;
                            }
                            else if ((int)type > (int)Building_Type.�������� && (int)type < (int)Building_Type.ս������)
                            {
                                sort = 1;
                            }
                            else
                            {
                                sort = 2;
                            }
                            this.buildingTypes[sort].Add(type);
                            this.buildCondition[Building_Condition_Type.��Դ1].Remove(type);
                            Debug.Log("����ͨ����Դ1��������");
                        }
                        else
                        {
                            i++;
                        }
                    }
                    if (this.buildCondition[Building_Condition_Type.��Դ1].Count == 0)
                    {
                        unlockIDisposableAmount[Building_Condition_Type.��Դ1].Dispose();
                    }
                });


            if (this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.��Դ2))
                this.unlockIDisposableAmount[Building_Condition_Type.��Դ2] = ResourcesManager.Instance.unlockByResouces[1]
                    .Subscribe(resource2 =>
                    {
                        for (int i = 0; i < this.buildCondition[Building_Condition_Type.��Դ2].Count;)
                        {
                            var type = this.buildCondition[Building_Condition_Type.��Դ2].ElementAt(i).Key;
                            if (resource2 >= this.buildCondition[Building_Condition_Type.��Դ2][type])
                            {
                                int sort;
                                if ((int)type > (int)Building_Type.�Զ��ɼ����� && (int)type < (int)Building_Type.��������)
                                {
                                    sort = 0;
                                }
                                else if ((int)type > (int)Building_Type.�������� && (int)type < (int)Building_Type.ս������)
                                {
                                    sort = 1;
                                }
                                else
                                {
                                    sort = 2;
                                }
                                this.buildingTypes[sort].Add(type);
                                this.buildCondition[Building_Condition_Type.��Դ2].Remove(type);
                                Debug.Log("����ͨ����Դ2��������");
                            }
                            else
                            {
                                i++;
                            }
                        }
                        if (this.buildCondition[Building_Condition_Type.��Դ2].Count == 0)
                        {
                            unlockIDisposableAmount[Building_Condition_Type.��Դ2].Dispose();
                        }
                    });

            if (this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.��Դ3))
                this.unlockIDisposableAmount[Building_Condition_Type.��Դ3] = ResourcesManager.Instance.unlockByResouces[2]
                    .Subscribe(resource3 =>
                    {
                        for (int i = 0; i < this.buildCondition[Building_Condition_Type.��Դ3].Count;)
                        {
                            var type = this.buildCondition[Building_Condition_Type.��Դ3].ElementAt(i).Key;
                            if (resource3 >= this.buildCondition[Building_Condition_Type.��Դ3][type])
                            {
                                int sort;
                                if ((int)type > (int)Building_Type.�Զ��ɼ����� && (int)type < (int)Building_Type.��������)
                                {
                                    sort = 0;
                                }
                                else if ((int)type > (int)Building_Type.�������� && (int)type < (int)Building_Type.ս������)
                                {
                                    sort = 1;
                                }
                                else
                                {
                                    sort = 2;
                                }
                                this.buildingTypes[sort].Add(type);
                                this.buildCondition[Building_Condition_Type.��Դ3].Remove(type);
                                Debug.Log("����ͨ����Դ3��������");
                            }
                            else
                            {
                                i++;
                            }
                        }
                        if (this.buildCondition[Building_Condition_Type.��Դ3].Count == 0)
                        {
                            unlockIDisposableAmount[Building_Condition_Type.��Դ3].Dispose();
                        }
                    });

            if (this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.�غ���))
                this.unlockIDisposableAmount[Building_Condition_Type.�غ���] = RoundManager.Instance.unlockPlotByRound
                    .Subscribe(roundNumber =>
                    {
                        for (int i = 0; i < this.buildCondition[Building_Condition_Type.�غ���].Count;)
                        {
                            var type = this.buildCondition[Building_Condition_Type.�غ���].ElementAt(i).Key;
                            if (roundNumber >= this.buildCondition[Building_Condition_Type.�غ���][type])
                            {
                                int sort;
                                if ((int)type > (int)Building_Type.�Զ��ɼ����� && (int)type < (int)Building_Type.��������)
                                {
                                    sort = 0;
                                }
                                else if ((int)type > (int)Building_Type.�������� && (int)type < (int)Building_Type.ս������)
                                {
                                    sort = 1;
                                }
                                else
                                {
                                    sort = 2;
                                }
                                this.buildingTypes[sort].Add(type);
                                this.buildCondition[Building_Condition_Type.�غ���].Remove(type);
                                Debug.Log("����ͨ���غ�����������");
                            }
                            else
                            {
                                i++;
                            }
                        }
                        if (this.buildCondition[Building_Condition_Type.�غ���].Count == 0)
                        {
                            unlockIDisposableAmount[Building_Condition_Type.�غ���].Dispose();
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
                            foreach (var type in this.buildCondition[Building_Condition_Type.��ͼ].Keys)
                            {
                                if (this.buildCondition[Building_Condition_Type.��ͼ][type] == id)
                                {
                                    int sort;
                                    if ((int)type > (int)Building_Type.�Զ��ɼ����� && (int)type < (int)Building_Type.��������)
                                    {
                                        sort = 0;
                                    }
                                    else if ((int)type > (int)Building_Type.�������� && (int)type < (int)Building_Type.ս������)
                                    {
                                        sort = 1;
                                    }
                                    else
                                    {
                                        sort = 2;
                                    }
                                    this.buildingTypes[sort].Add(type);
                                    this.buildCondition[Building_Condition_Type.��ͼ].Remove(type);
                                    Debug.LogFormat("����ͨ����ͼ{0}��������", id);
                                    break;
                                }
                            }
                        }
                        if (this.buildCondition[Building_Condition_Type.��ͼ].Count == 0)
                        {
                            unlockIDisposableBlueprint[id].Dispose();
                        }
                    });
            }

            if (this.unlockIDisposableAmount.ContainsKey(Building_Condition_Type.������ս������))
                this.unlockIDisposableAmount[Building_Condition_Type.������ս������] = this.ObserveEveryValueChanged(_ => this.battleBuildings.Count)
                    .Subscribe(_ =>
                    {
                        for (int i = 0; i < this.buildCondition[Building_Condition_Type.������ս������].Count;)
                        {
                            var type = this.buildCondition[Building_Condition_Type.������ս������].ElementAt(i).Key;
                            if (this.battleBuildings.Count >= this.buildCondition[Building_Condition_Type.������ս������][type])
                            {
                                int sort;
                                if ((int)type > (int)Building_Type.�Զ��ɼ����� && (int)type < (int)Building_Type.��������)
                                {
                                    sort = 0;
                                }
                                else if ((int)type > (int)Building_Type.�������� && (int)type < (int)Building_Type.ս������)
                                {
                                    sort = 1;
                                }
                                else
                                {
                                    sort = 2;
                                }
                                this.buildingTypes[sort].Add(type);
                                this.buildCondition[Building_Condition_Type.������ս������].Remove(type);
                                Debug.Log("����ͨ��������ս��������������");
                            }
                            else
                            {
                                i++;
                            }
                        }
                        if (this.buildCondition[Building_Condition_Type.������ս������].Count == 0)
                        {
                            unlockIDisposableAmount[Building_Condition_Type.������ս������].Dispose();
                        }
                    });
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
            gO.transform.SetParent(this.transform);
        }

        /// <summary>
        /// �ڸ����İ���Ͻ�������Ľ�������
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
                    Debug.Log("�ð�����н���");
                }
            }
            return false;
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
            PlotManager.Instance.plots[building.pos].building = null;
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
        /// �غϽ��������н���
        /// </summary>
        public void RoundOver()
        {
            this.GatherBuildingResources();
            this.ProduceWealth();
            //this.GetBuildingType();
        }

        /// <summary>
        /// ���ݽ������ͻ�ȡ����
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static BuildingType GetBuildingByType(Building_Type type)
        {
            BuildingType buildingType;
            int sort = (int)type;
            if (sort > (int)Building_Type.�Զ��ɼ����� && sort < (int)Building_Type.��������)
            {
                buildingType = DataManager.BuildingScriptLists[0][(int)type - (int)Building_Type.�Զ��ɼ����� - 1];
            }
            else if (sort > (int)Building_Type.�������� && sort < (int)Building_Type.ս������)
            {
                buildingType = DataManager.BuildingScriptLists[1][(int)type - (int)Building_Type.�������� - 1];
            }
            else
            {
                buildingType = DataManager.BuildingScriptLists[2][(int)type - (int)Building_Type.ս������ - 1];
            }
            return buildingType;
        }
    }
}