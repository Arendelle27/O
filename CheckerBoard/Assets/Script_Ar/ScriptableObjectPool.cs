using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScriptableObjectPool
{ 
    //�����ű������б�
    public static List<BuildingType> buildingScriptList=new List<BuildingType>();

    /// <summary>
    /// ��ȡ�����ű��б�
    /// </summary>
    public static void ReadBuildingScriptList()
    {
        buildingScriptList = (Resources.Load<BuildingTypeList>(PathConfig.GetScriptableList("BuildingList"))).buildingTypeList;
    }
}
