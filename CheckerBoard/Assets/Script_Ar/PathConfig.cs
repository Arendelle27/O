using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathConfig 
{
    //实体预制体位置
    const string Entity_Prefab_Path = "Prefabs/{0}";
    //UI预制体位置
    public const string UI_Prefab_Path = "Prefabs/UI/{0}";

    public const string BuildingItem_Sprite_Paths = "UI/Building/{0}";

    //获取实体预制体位置
    public static string GetEntityPrefabPath(string name)
    {
        
        return string.Format(Entity_Prefab_Path, name);
    }
    //Plot,Buildings,Wander,DestinationSign
    //HumanSettlement,RobotSettlement

    //获取UI预制体位置
    public static string GetUIPrefabPath(string name)
    {

        return string.Format(UI_Prefab_Path, name);
    }
    //UIBuildingItem

    public static string GetBuildingItemSpritePath(string name)
    {
        return string.Format(BuildingItem_Sprite_Paths, name);
    }
    //BuildingItemSprite
}
