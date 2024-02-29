using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathConfig 
{
    //地块预制体位置
    public const string Plot_Prefab_Path = "Prefabs/Plot";
    //建筑预制体位置
    public const string Building_Prefab_Path = "Prefabs/Building";
    //流浪者预制体位置
    public const string Wanderer_Prefab_Path = "Prefabs/Wanderer";

    #region UI图像
    public static List<string> UI_Building_Sprite_Paths = new List<string>()
    {
        "UI/Building/Gathering_1",
        "UI/Building/Gathering_2",
        "UI/Building/Production_1",
        "UI/Building/Production_2"
    };

    //public const string UI_Building_Gathering1_Path = "UI/Building/Gathering_1";
    //public const string UI_Building_Gathering2_Path = "UI/Building/Gathering_2";

    //public const string UI_Building_Production1_Path = "UI/Building/Production_1";
    //public const string UI_Building_Production2_Path = "UI/Building/Production_2";
    #endregion

    //建筑物UI预制体位置
    public const string UI_BuildingItem_Prefab_Path = "Prefabs/UI/UIBuildingItem";
}
