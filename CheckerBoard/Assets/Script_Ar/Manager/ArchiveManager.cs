using ENTITY;
using MANAGER;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static ArchiveManager.PlotManagerData;

public static class ArchiveManager
{
    [Serializable]
    public class Archive
    {
        public PlotManagerData plotManagerData=new PlotManagerData();//地块管理器数据
        public WandererManagerData wandererManagerData=new WandererManagerData();
        public List<BuildingData> buildingData=new List<BuildingData>();
        //public List<SettlementData> settlementData=new List<SettlementData>();
        public int roundNumber;
        public int wealth;
        public List<int> buildingRes=new List<int>();
        public int execution;
        public Vector3 CameraPos;
    }

    [Serializable]
    public class PlotManagerData
    {
        [Serializable]
        public class UnloadPropData
        {
            public string propName;
            public bool isUnloaded;
        }

        [Serializable]
        public class PlotData
        {
            public Vector2Int pos;
            public int plotDefineId;
            public Plot_Statue plotType;
            public bool isFirstExplored;
            public List<int> buildingResources;
        }

        public List<UnloadPropData> unloadPropsData = new List<UnloadPropData>();//地块管理器数据
        public List<PlotData> plotsData = new List<PlotData>();
    }

    [Serializable]
    public class WandererManagerData
    {
        [Serializable]
        public class WandererData
        {
            public Vector2Int pos;
            public int level;
        }

        public WandererData wandererData=new WandererData();
        public List<Vector2Int> exploratoryTeams=new List<Vector2Int>();
    }

    [Serializable]
    public class BuildingData  
    {
        public Vector2Int pos;
        public Building_Type buildingType;
    }

    [Serializable]
    public class SettlementData
    {
        public Vector2Int pos;
        public bool isHumanSettlement;
        public int hotility;
    }

    //存档
    public static Archive archive;

    /// <summary>
    /// 保存游戏
    /// </summary>
    public static void SaveData()
    {
        Archive arc = new Archive();//建立存档
        #region 地块管理器数据
        foreach (var plot in PlotManager.Instance.unloadProp)
        {
            arc.plotManagerData.unloadPropsData. Add(new UnloadPropData
            {
                propName = plot.Key,
                isUnloaded = plot.Value
            });
        }

        foreach (var plot in PlotManager.Instance.plots.Values)
        {
            arc.plotManagerData.plotsData.Add(new PlotData
            {
                pos = plot.pos,
                plotDefineId = plot.plotDefine.ID,
                plotType = plot.plot_Statue,
                isFirstExplored = plot.isFirstExplored,
                buildingResources=plot.buildingResources
            });
        }
        #endregion

        #region 流浪者管理器数据
        arc.wandererManagerData.wandererData.pos= WandererManager.Instance.wanderer.plot.pos;
        arc.wandererManagerData.wandererData.level = WandererManager.Instance.wanderer.level;
        arc.wandererManagerData.exploratoryTeams = WandererManager.Instance.exploratoryTeams.Keys.ToList<Vector2Int>();
        #endregion

        foreach (var building in BuildingManager.Instance.gatheringBuildings.Values)
        {
            arc.buildingData.Add(new BuildingData
            {
                pos = building.pos,
                buildingType = building.type
            });
        }

        foreach (var building in BuildingManager.Instance.productionBuildings.Values)
        {
            arc.buildingData.Add(new BuildingData
            {
                pos = building.pos,
                buildingType = building.type
            });
        }

        foreach (var building in BuildingManager.Instance.battleBuildings.Values)
        {
            arc.buildingData.Add(new BuildingData
            {
                pos = building.pos,
                buildingType = building.type
            });
        }

        //foreach(var settlement in EventAreaManager.Instance.humanSettlements.Values)
        //{
        //    arc.settlementData.Add(new SettlementData
        //    {
        //        pos=settlement.pos,
        //        isHumanSettlement=true,
        //        hotility=settlement.hotility
        //    });
        //}
        //foreach (var settlement in EventAreaManager.Instance.robotSettlements.Values)
        //{
        //    arc.settlementData.Add(new SettlementData
        //    {
        //        pos = settlement.pos,
        //        isHumanSettlement = false,
        //        hotility = settlement.hotility
        //    });
        //}

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
