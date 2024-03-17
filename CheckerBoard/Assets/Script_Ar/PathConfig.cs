using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathConfig 
{
    //实体预制体位置
    const string Entity_Prefab_Path = "Prefabs/{0}";

    const string Building_Prefab_Path = "Prefabs/Building/{0}";

    const string Settlement_Prefab_Path = "Prefabs/Settlement/{0}";

    //UI预制体位置
    const string UI_Prefab_Path = "Prefabs/UI/{0}";

    const string BuildingItem_Sprite_Paths = "UI/Building/{0}";

    const string Music_Path = "Music/{0}";
    const string Sound_Path = "Voice/{0}";


    const string Scriptable_List_Path = "ScriptableObject/List/{0}";

    //获取实体预制体位置
    public static string GetEntityPrefabPath(string name)
    {
        
        return string.Format(Entity_Prefab_Path, name);
    }
    //Plot,Buildings,Wander,DestinationSign
    //HumanSettlement,RobotSettlement

    public static string GetBuildingPrefabPath(string name)
    {
        return string.Format(Building_Prefab_Path, name);
    }

    public static string GetSettlementPrefabPath(string name)
    {
        return string.Format(Settlement_Prefab_Path, name);
    }

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

    public static string GetMusicPath(string name)
    {
        return string.Format(Music_Path, name);
    }

    public static string GetVoicePath(string name)
    {
        return string.Format(Sound_Path, name);
    }

    public static string GetScriptableList(string name)
    {
        return string.Format(Scriptable_List_Path, name);
    }
}
