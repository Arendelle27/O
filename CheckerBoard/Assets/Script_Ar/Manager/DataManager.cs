using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MANAGER;
using Newtonsoft.Json;
using UnityEngine;

public static class DataManager
{

    //建筑脚本属性列表
    public static List<List<BuildingType>> BuildingScriptLists = new List<List<BuildingType>>();
    //板块属性列表
    public static Dictionary<int, PlotDefine> PlotDefines;
    //交易属性列表
    public static Dictionary<int, Dictionary<int, TransactionDefine>> TransactionDefines;
    //对抗属性列表
    public static Dictionary<int, Dictionary<int, ConfrontDefine>> ConfrontDefines;
    //小队升级属性列表
    public static Dictionary<int, TeamUpgradeDefine> TeamUpgradeDefines;
    //交易升级属性列表
    public static Dictionary<int, TransactionUpgradeDefine> TransactionUpgradeDefines;
    //行动点升级属性列表
    public static Dictionary<int, ExecutionUpgradeDefine> ExecutionUpgradeDefines;

    public static Dictionary<int, UpgradePointCostDefine> UpgradePointCostDefines;

    public static Dictionary<int, StageDecisionCostDefine> StageDecisionCostDefines;

    public static Dictionary<int, NoviceGuideDefine> NoviceGuideDefines;

    public static Dictionary<int, NPCDefine> NPCDefines;

    public static Dictionary<int,Dictionary<int, ChatDefine>> ChatDefines;

    public static Dictionary<int, ChatConditionDefine> ChatConditionDefines;

    public static Dictionary<int,QuestDefine> QuestDefines;

    public static Dictionary<int, CameraDefine> CameraDefines;

    public static Dictionary<int, CGDefine> CGDefines;

    /// <summary>
    /// 读取建筑脚本列表
    /// </summary>
    public static IEnumerator Load()
    {
        Dictionary<int, Dictionary<int, BuildingDefine>> BuildingDefines;
        string json = File.ReadAllText(PathConfig.GetDataTxtPath("BuildingDefine.txt"));
        BuildingDefines = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, BuildingDefine>>>(json);


        for (int i = 0; i < 3; i++)
        {
            List<BuildingType> buildingList = (Resources.Load<BuildingTypeList>(PathConfig.GetScriptableList(BuildingDefines[i][0].Class.ToString()))).buildingTypeList;
            for (int j = 0; j < BuildingDefines[i].Keys.Count; j++)
            {
                buildingList[j].TID = BuildingDefines[i][j].TID;
                buildingList[j].Name = BuildingDefines[i][j].Name;
                buildingList[j].Class = BuildingDefines[i][j].Class;
                buildingList[j].Type = BuildingDefines[i][j].Type;
                buildingList[j].Resource = BuildingDefines[i][j].Resource;
                buildingList[j].Description = BuildingDefines[i][j].Description;
                buildingList[j].Condition = BuildingDefines[i][j].Condition;
                buildingList[j].NumericalValue = BuildingDefines[i][j].NumericalValue;
                buildingList[j].ResourcesCost[0] = BuildingDefines[i][j].Resource1Cost;
                buildingList[j].ResourcesCost[1] = BuildingDefines[i][j].Resource2Cost;
                buildingList[j].ResourcesCost[2] = BuildingDefines[i][j].Resource3Cost;
                buildingList[j].Attack = BuildingDefines[i][j].Attack;
                buildingList[j].HostilityToRobot = BuildingDefines[i][j].HostilityToRobot;
                buildingList[j].HostilityToHuman = BuildingDefines[i][j].HostilityToHuman;
                buildingList[j].sprite = Resources.Load<Sprite>(PathConfig.GetBuildingItemSpritePath(BuildingDefines[i][j].Type.ToString()));

                switch (i)
                {
                    case 0:
                        (buildingList[j] as GatheringBuildingType).GatherResourceRounds = BuildingDefines[i][j].GatherResourceRounds;
                        (buildingList[j] as GatheringBuildingType).GatherResourceType = BuildingDefines[i][j].GatherResourceType;
                        (buildingList[j] as GatheringBuildingType).GatherResourceAmount = BuildingDefines[i][j].GatherResourceAmount;
                        break;
                    case 1:
                        (buildingList[j] as ProductionBuildingType).Production = BuildingDefines[i][j].Production;
                        break;
                    case 2:

                        break;
                }
            }
            BuildingScriptLists.Add(buildingList);
        }
        yield return null;

        json = File.ReadAllText(PathConfig.GetDataTxtPath("PlotDefine.txt"));
        PlotDefines = JsonConvert.DeserializeObject<Dictionary<int, PlotDefine>>(json);
        yield return null;

        json = File.ReadAllText(PathConfig.GetDataTxtPath("TransactionDefine.txt"));
        TransactionDefines = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, TransactionDefine>>>(json);
        yield return null;

        json = File.ReadAllText(PathConfig.GetDataTxtPath("ConfrontDefine.txt"));
        ConfrontDefines = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, ConfrontDefine>>>(json);
        yield return null;

        json = File.ReadAllText(PathConfig.GetDataTxtPath("TeamUpgradeDefine.txt"));
        TeamUpgradeDefines = JsonConvert.DeserializeObject<Dictionary<int, TeamUpgradeDefine>>(json);
        yield return null;

        json = File.ReadAllText(PathConfig.GetDataTxtPath("TransactionUpgradeDefine.txt"));
        TransactionUpgradeDefines = JsonConvert.DeserializeObject<Dictionary<int, TransactionUpgradeDefine>>(json);
        yield return null;

        json = File.ReadAllText(PathConfig.GetDataTxtPath("ExecutionUpgradeDefine.txt"));
        ExecutionUpgradeDefines = JsonConvert.DeserializeObject<Dictionary<int, ExecutionUpgradeDefine>>(json);
        yield return null;

        json = File.ReadAllText(PathConfig.GetDataTxtPath("UpgradePointCostDefine.txt"));
        UpgradePointCostDefines = JsonConvert.DeserializeObject<Dictionary<int, UpgradePointCostDefine>>(json);
        yield return null;

        json= File.ReadAllText(PathConfig.GetDataTxtPath("StageDecisionCostDefine.txt"));
        StageDecisionCostDefines = JsonConvert.DeserializeObject<Dictionary<int, StageDecisionCostDefine>>(json);
        yield return null;

        json = File.ReadAllText(PathConfig.GetDataTxtPath("NoviceGuideDefine.txt"));
        NoviceGuideDefines = JsonConvert.DeserializeObject<Dictionary<int, NoviceGuideDefine>>(json);
        yield return null;

        json = File.ReadAllText(PathConfig.GetDataTxtPath("NPCDefine.txt"));
        NPCDefines = JsonConvert.DeserializeObject<Dictionary<int, NPCDefine>>(json);
        yield return null;

        json = File.ReadAllText(PathConfig.GetDataTxtPath("ChatDefine.txt"));
        ChatDefines = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, ChatDefine>>>(json);
        yield return null;

        json = File.ReadAllText(PathConfig.GetDataTxtPath("ChatConditionDefine.txt"));
        ChatConditionDefines = JsonConvert.DeserializeObject<Dictionary<int, ChatConditionDefine>>(json);
        yield return null;

        json = File.ReadAllText(PathConfig.GetDataTxtPath("QuestDefine.txt"));
        QuestDefines = JsonConvert.DeserializeObject<Dictionary<int, QuestDefine>>(json);
        yield return null;

        json = File.ReadAllText(PathConfig.GetDataTxtPath("CameraDefine.txt"));
        CameraDefines = JsonConvert.DeserializeObject<Dictionary<int, CameraDefine>>(json);
        yield return null;

        json = File.ReadAllText(PathConfig.GetDataTxtPath("CGDefine.txt"));
        CGDefines = JsonConvert.DeserializeObject<Dictionary<int, CGDefine>>(json);
        yield return null;

        SpriteManager.Load();//加载精灵
        yield return null;
    }

#if UNITY_EDITOR
    public static void LoadNoviceGuideDefine()
    {
        string json = File.ReadAllText(PathConfig.GetDataTxtPath("NoviceGuideDefine.txt"));
        NoviceGuideDefines = JsonConvert.DeserializeObject<Dictionary<int, NoviceGuideDefine>>(json);
    }

    public static void LoadCameraDefine()
    {
        string json = File.ReadAllText(PathConfig.GetDataTxtPath("CameraDefine.txt"));
        CameraDefines = JsonConvert.DeserializeObject<Dictionary<int, CameraDefine>>(json);
    }
    //********************************************************************************************************************
    public static void SaveNoviceGuidePos()
    {
        string json = JsonConvert.SerializeObject(NoviceGuideDefines, Formatting.Indented);
        File.WriteAllText(PathConfig.GetDataTxtPath("NoviceGuideDefine.txt"), json);
    }

    public static void SaveCameraDefine()
    {
        string json = JsonConvert.SerializeObject(CameraDefines, Formatting.Indented);
        File.WriteAllText(PathConfig.GetDataTxtPath("CameraDefine.txt"), json);
    }
#endif
}
