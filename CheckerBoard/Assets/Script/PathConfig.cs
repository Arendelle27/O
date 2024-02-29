using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathConfig 
{
    //�ؿ�Ԥ����λ��
    public const string Plot_Prefab_Path = "Prefabs/Plot";
    //����Ԥ����λ��
    public const string Building_Prefab_Path = "Prefabs/Building";
    //������Ԥ����λ��
    public const string Wanderer_Prefab_Path = "Prefabs/Wanderer";

    #region UIͼ��
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

    //������UIԤ����λ��
    public const string UI_BuildingItem_Prefab_Path = "Prefabs/UI/UIBuildingItem";
}
