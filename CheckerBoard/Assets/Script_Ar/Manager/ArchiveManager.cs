using ENTITY;
using MANAGER;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TreeEditor;
using UnityEngine;

public static class ArchiveManager
{
    [Serializable]
    public class Archive
    {
        public List<PlotData> plotData=new List<PlotData>();
        public WandererData wandererData;
        public List<BuildingData> buildingData=new List<BuildingData>();
        public List<SettlementData> settlementData=new List<SettlementData>();
        public int roundNumber;
        public int wealth;
        public List<int> buildingRes=new List<int>();
        public int execution;

    }

    [Serializable]
    public class PlotData
    {
        public Vector2Int pos;
        public Plot_Type plotType;
    }

    [Serializable]
    public class WandererData  
    {
        public Vector2Int pos;
        public int level;
        public List<Vector2Int> exploratoryTeams;
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

    public static Archive archive;

    /// <summary>
    /// 保存游戏
    /// </summary>
    public static void SaveData()
    {
        Archive arc = new Archive();//建立存档
        foreach (var plot in PlotManager.Instance.plots.Values)
        {
            arc.plotData.Add(new PlotData
            {
                pos = plot.pos,
                plotType = plot.plot_Type
            });
        }

        var wanderer = WandererManager.Instance.wanderer;
        arc.wandererData = new WandererData()
        {
            pos = wanderer.plot.pos,
            level = wanderer.level,
            exploratoryTeams=WandererManager.Instance.exploratoryTeams
        };

        foreach (var building in BuildingManager.Instance.buildings.Values)
        {
            arc.buildingData.Add(new BuildingData
            {
                pos = building.pos,
                buildingType = building.type
            });
        }

        foreach(var settlement in SettlementManager.Instance.humanSettlements.Values)
        {
            arc.settlementData.Add(new SettlementData
            {
                pos=settlement.pos,
                isHumanSettlement=true,
                hotility=settlement.hotility
            });
        }
        foreach (var settlement in SettlementManager.Instance.robotSettlements.Values)
        {
            arc.settlementData.Add(new SettlementData
            {
                pos = settlement.pos,
                isHumanSettlement = false,
                hotility = settlement.hotility
            });
        }

        arc.roundNumber = RoundManager.Instance.roundNumber;

        arc.wealth = DataManager.Instance.wealth;

        arc.buildingRes = DataManager.Instance.buildingResources;

        arc.execution = DataManager.Instance.execution;

        //string ar=arc.ToString();

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
