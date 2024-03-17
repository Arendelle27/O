using ENTITY;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildingDataTool : Editor
{
    [MenuItem("Tools/Set Building Data")]
     static void SetBuildingData()
    {
        //获得建筑列表
        //BuildingTypeList bTL= Resources.Load<BuildingTypeList>(PathConfig.GetScriptableList("BuildingList"));
        List<BuildingType> bTL = (Resources.Load<BuildingTypeList>(PathConfig.GetScriptableList("BuildingList"))).buildingTypeList;
        for (int i = 0;i<bTL.Count;i++)
        {
            //if(i<3)
            //{
            //    bTL[i].type = (Building_Type)i;
            //    Sprite sp = Resources.Load<Sprite>(PathConfig.GetBuildingItemSpritePath(((Building_Type)i).ToString()));
            //    bTL[i].sprite = sp;
            //}
            //else if(i>=3&&i<5)
            //{

            //}
            //else if(i>=5&&i<6)
            //{

            //}
            bTL[i].type = (Building_Type)i;
            Sprite sp = Resources.Load<Sprite>(PathConfig.GetBuildingItemSpritePath(((Building_Type)i).ToString()));
            bTL[i].sprite = sp;
        }
    }
}
