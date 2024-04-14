using ENTITY;
using MANAGER;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class ArchiveManager
{
    [Serializable]
    public class Archive
    {
        public PlotManagerData plotManagerData;//�ؿ����������
        public WandererManagerData wandererManagerData;
        public BuildingManagerData buildingManagerData;
        public EventAreaManagerData eventAreaManagerData;
        public int roundNumber;
        public int wealth;
        public List<int> buildingRes;//������Դ
        public int execution;
        public Vector3 CameraPos;
    }

    [Serializable]
    public class PlotManagerData
    {
        [Serializable]
        public class PlotData
        {
            public Vector2Int pos;
            public int plotDefineId;
            public Plot_Statue plotStatue;
            public bool isFirstExplored;
            public List<int> buildingResources;
            public bool canEnter;
        }

        [Serializable]
        public class PlotTypeData
        {
            public List<int> plotTypes;
        }

        [Serializable]
        public class PlotConditions
        {
            public List<int> ids;
            public List<string> conditions;
        }

        [Serializable]
        public class UnloadPropData
        {
            public string propName;
            public bool isUnloaded;
        }

        public List<Vector2Int> haveExploredPlots = new List<Vector2Int>();//��̽���ؿ�
        public List<PlotData> plotsData = new List<PlotData>();//�ؿ�����
        public List<int> plotTypeDesepicalIds=new List<int>();
        public List<PlotTypeData> plotTypes = new List<PlotTypeData>(2) { new PlotTypeData(), new PlotTypeData() };//�ؿ�����
        public List<PlotConditions> plotConditions = new List<PlotConditions>(3){ new PlotConditions(), new PlotConditions(), new PlotConditions()};
        public List<UnloadPropData> unloadPropsData = new List<UnloadPropData>();//�ؿ����������
    }

    [Serializable]
    public class WandererManagerData
    {
        [Serializable]
        public class WandererData
        {
            public Vector2Int pos;
        }

        public WandererData wandererData=new WandererData();
        public List<Vector2Int> exploratoryTeams=new List<Vector2Int>();
    }

    [Serializable]
    public class BuildingManagerData
    {
        [Serializable]
        public class BuildingData
        {
            public Vector2Int pos;
            public Building_Type buildingType;

            public int existRound = 0;
        }

        [Serializable]
        public class BuildingContitionData
        {
            public Building_Type buildingType;
            public int amount;
        }
        [Serializable]
        public class BuildingContitionsData
        {
            public List<BuildingContitionData> buildingContitions = new List<BuildingContitionData>();
        }
        [Serializable]
        public class BuildingTypesData
        {
            public List<Building_Type> buildingTypes = new List<Building_Type>();
        }
        [Serializable]
        public class BluePrintsData
        {
            public int bluePrintId;
            public bool isUnlocked;
        }

        public List<BuildingData> buildingsData = new List<BuildingData>();
        public int totalAttack = 0;
        public List<BuildingContitionsData> buildingConditions = new List<BuildingContitionsData>(6){ new BuildingContitionsData(), new BuildingContitionsData(), new BuildingContitionsData(),new BuildingContitionsData(),new BuildingContitionsData(),new BuildingContitionsData()};
        public List<BuildingTypesData> buildingTypes = new List<BuildingTypesData>(3) { new BuildingTypesData(), new BuildingTypesData(), new BuildingTypesData() };
        public List<BluePrintsData> bluePrintsData = new List<BluePrintsData>();
    }

    [Serializable]
    public class EventAreaManagerData
    {
        [Serializable]
        public class PurchaseObjectStatueData
        {
            public int purchaseObjectId;
            public List<int> PurchaseObjectStatue =new List<int>(2){0,0};
        }
        [Serializable]
        public class SellObjectStatueData
        {
            public int sellObjectId;
            public int sellObjectStatue;
        }

        public List<PurchaseObjectStatueData> purchaseObjectStatueDatas = new List<PurchaseObjectStatueData>(2) {new PurchaseObjectStatueData(),new PurchaseObjectStatueData()};
        public List<SellObjectStatueData> sellObjectsStatueDatas = new List<SellObjectStatueData>(2) {new SellObjectStatueData(),new SellObjectStatueData()};
        public List<float> hotility=new List<float>(2) { 0f,0f};
    }

    //�浵
    public static Archive archive;

    /// <summary>
    /// ������Ϸ
    /// </summary>
    public static void SaveData()
    {
        Archive arc = new Archive();//�����浵
        #region �ؿ����������
        PlotManagerData plotManagerData = new PlotManagerData();
        //�ؿ�����
        foreach (var plot in PlotManager.Instance.plots.Values)
        {
            plotManagerData.plotsData.Add(new PlotManagerData.PlotData
            {
                pos = plot.pos,
                plotDefineId = plot.plotDefine.ID,
                plotStatue = plot.plot_Statue,
                isFirstExplored = plot.isFirstExplored,
                buildingResources = plot.buildingResources,
                canEnter = plot.canEnter
            }); ;
        }
        //��̽���ؿ�
        plotManagerData.haveExploredPlots = PlotManager.Instance.haveExploredPlots.ToList();
        //������ؿ�����Id
        plotManagerData.plotTypeDesepicalIds = PlotManager.Instance.plotTypeDesepical.Keys.ToList();
        //�ؿ�����
        for (int i=0;i<plotManagerData.plotTypes.Count;i++)
        {
            plotManagerData.plotTypes[i].plotTypes = PlotManager.Instance.plotTypes[i].ToList();
        }
        //�ؿ��������
        for(int i=0; i<plotManagerData.plotConditions.Count;i++)
        {
            plotManagerData.plotConditions[i].ids = PlotManager.Instance.plotConditions[i].Keys.ToList();
            plotManagerData.plotConditions[i].conditions = PlotManager.Instance.plotConditions[i].Values.ToList();
        }
        //��������
        foreach (var plot in PlotManager.Instance.unloadProp)
        {
            plotManagerData.unloadPropsData.Add(new PlotManagerData.UnloadPropData
            {
                propName = plot.Key,
                isUnloaded = plot.Value
            });
        }

        arc.plotManagerData = plotManagerData;
        #endregion

        #region �����߹���������
        WandererManagerData wandererManagerData = new WandererManagerData();

        //����������
        wandererManagerData.wandererData.pos = WandererManager.Instance.wanderer.plot.pos;
        //̽��С��
        wandererManagerData.exploratoryTeams = WandererManager.Instance.exploratoryTeams.Keys.ToList<Vector2Int>();

        arc.wandererManagerData = wandererManagerData;
        #endregion

        #region ��������������
        BuildingManagerData buildingManagerData = new BuildingManagerData();

        foreach (var building in BuildingManager.Instance.gatheringBuildings.Values)
        {
            buildingManagerData.buildingsData.Add(new BuildingManagerData.BuildingData
            {
                pos = building.pos,
                buildingType = building.type,
                existRound = building.existRound
            });
        }

        foreach (var building in BuildingManager.Instance.productionBuildings.Values)
        {
            buildingManagerData.buildingsData.Add(new BuildingManagerData.BuildingData
            {
                pos = building.pos,
                buildingType = building.type
            });
        }

        foreach (var building in BuildingManager.Instance.battleBuildings.Values)
        {
            buildingManagerData.buildingsData.Add(new BuildingManagerData.BuildingData
            {
                pos = building.pos,
                buildingType = building.type
            });
        }

        buildingManagerData.totalAttack = BuildingManager.Instance.totalAttack;

        for (int i = 0; i < buildingManagerData.buildingConditions.Count; i++)
        {
            foreach (var building in BuildingManager.Instance.buildingConditions[(Building_Condition_Type)i])
            {
                buildingManagerData.buildingConditions[i].buildingContitions.Add(new BuildingManagerData.BuildingContitionData
                {
                    buildingType = building.Key,
                    amount = building.Value
                });
            }
        }

        for (int i = 0; i < buildingManagerData.buildingTypes.Count; i++)
        {
            buildingManagerData.buildingTypes[i].buildingTypes = BuildingManager.Instance.buildingTypes[i].ToList();
        }

        foreach (var bluePrint in BuildingManager.Instance.bluePrints)
        {
            buildingManagerData.bluePrintsData.Add(new BuildingManagerData.BluePrintsData
            {
                bluePrintId = bluePrint.Key,
                isUnlocked = bluePrint.Value
            });
        }

        arc.buildingManagerData = buildingManagerData;
        #endregion

        #region �¼�������������
        EventAreaManagerData eventAreaManagerData = new EventAreaManagerData();
        //������Ʒ״̬
        for (int i = 0; i < eventAreaManagerData.purchaseObjectStatueDatas.Count; i++)
        {
            foreach (var purchaseObject in EventAreaManager.Instance.purchaseObjectsStatue[i])
            {
                eventAreaManagerData.purchaseObjectStatueDatas[i].purchaseObjectId = purchaseObject.Key;
                eventAreaManagerData.purchaseObjectStatueDatas[i].PurchaseObjectStatue = purchaseObject.Value.ToList();
            }
        }
        //������Ʒ״̬
        for (int i = 0; i < eventAreaManagerData.sellObjectsStatueDatas.Count; i++)
        {
            foreach (var sellObject in EventAreaManager.Instance.sellObjectsStatue[i])
            {
                eventAreaManagerData.sellObjectsStatueDatas[i].sellObjectId = sellObject.Key;
                eventAreaManagerData.sellObjectsStatueDatas[i].sellObjectStatue = sellObject.Value;
            }
        }
        //�жԶ�
        eventAreaManagerData.hotility = EventAreaManager.Instance.hotility.ToList();

        arc.eventAreaManagerData = eventAreaManagerData;
        #endregion


        arc.roundNumber = RoundManager.Instance.roundNumber;

        arc.wealth = ResourcesManager.Instance.wealth;

        arc.buildingRes = ResourcesManager.Instance.buildingResources;

        arc.execution = ResourcesManager.Instance.execution;

        arc.CameraPos = Camera.main.transform.position;

        string json = JsonUtility.ToJson(arc);
        ArchiveTool.SaveByJson("Archive.json", json);

        archive = arc;

    }

    public static void LoadData()
    {
        string a = ArchiveTool.LoadByJson("Archive.json");
        Archive arc = JsonUtility.FromJson<Archive>(a);

        archive = arc;

    }
}
