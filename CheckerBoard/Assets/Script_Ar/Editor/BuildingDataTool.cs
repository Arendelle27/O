using ENTITY;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildingDataTool : Editor
{
    [MenuItem("Tools/���ý�������")]
     static void SetBuildingData()
    {
        Dictionary<int, Dictionary<int, BuildingDefine>> BuildingDefines = null;
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
                        (buildingList[j] as ProductionBuildingType).Production = BuildingDefines[i][j].Production;
                        break;
                    case 1:
                        (buildingList[j] as GatheringBuildingType).GatherResourceRounds = BuildingDefines[i][j].GatherResourceRounds;
                        (buildingList[j] as GatheringBuildingType).GatherResourceType = BuildingDefines[i][j].GatherResourceType;
                        (buildingList[j] as GatheringBuildingType).GatherResourceAmount = BuildingDefines[i][j].GatherResourceAmount;
                        break;
                    case 2:

                        break;
                }
            }
        }
    }

}