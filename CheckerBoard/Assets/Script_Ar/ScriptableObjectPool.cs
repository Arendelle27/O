using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScriptableObjectPool
{ 
    //建筑脚本属性列表
    public static List<BuildingType> buildingScriptList=new List<BuildingType>();

    /// <summary>
    /// 读取建筑脚本列表
    /// </summary>
    public static void ReadBuildingScriptList()
    {
        buildingScriptList = (Resources.Load<BuildingTypeList>(PathConfig.GetScriptableList("BuildingList"))).buildingTypeList;
    }
}
