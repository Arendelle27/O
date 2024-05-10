using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PathConfig 
{
    //ʵ��Ԥ����λ��
    const string Entity_Prefab_Path = "Prefabs/{0}";

    const string Building_Prefab_Path = "Prefabs/Building/{0}";

    const string Settlement_Prefab_Path = "Prefabs/Settlement/{0}";

    //UIԤ����λ��
    const string UI_Prefab_Path = "Prefabs/UI/{0}";

    //ͼ��λ��
    const string BuildingResource_Sprite_Paths = "UI/BuildingResource/{0}";

    const string Prop_Sprite_Paths = "UI/Prop/{0}";

    const string BuildingItem_Sprite_Paths = "UI/Building/{0}";

    const string Plot_Sprite_Paths = "UI/Plot/{0}";

    const string HeadPortrait_Sprite_Paths = "UI/HeadPortrait/{0}";

    const string NpcNormal_Sprite_Paths = "UI/NpcNormal/{0}";

    const string NpcChat_Sprite_Paths = "UI/NpcChat/{0}";



    //������Чλ��
    const string Music_Path = "Music/{0}";
    const string Sound_Path = "Voice/{0}";

    //ScriptableObjectλ��
    const string Scriptable_List_Path = "ScriptableObject/List/{0}";

    //�浵����λ��
    const string Data_Txt_Path = "Data/{0}";

    //������������
    public static List<string> Building_Names = new List<string> { "�Զ��ɼ�����", "��������", "ս������" };

    //��ȡʵ��Ԥ����λ��
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

    //��ȡUIԤ����λ��
    public static string GetUIPrefabPath(string name)
    {

        return string.Format(UI_Prefab_Path, name);
    }
    //UIBuildingItem

    public static string GetBuildingResourceSpritePath(string name)
    {
        return string.Format(BuildingResource_Sprite_Paths, name);
    }

    public static string GetPropSpritePath(string name)
    {
        return string.Format(Prop_Sprite_Paths, name);
    }

    public static string GetHeadPortraitSpritePath(string name)
    {
        return string.Format(HeadPortrait_Sprite_Paths, name);
    }

    public static string GetBuildingItemSpritePath(string name)
    {
        return string.Format(BuildingItem_Sprite_Paths, name);
    }
    //BuildingItemSprite

    public static string GetPlotSpritePath(string name)
    {
        return string.Format(Plot_Sprite_Paths, name);
    }
    //PlotSprite

    public static string GetNpcNormalSpritePath(string name)
    {
        return string.Format(NpcNormal_Sprite_Paths, name);
    }
    public static string GetNpcChatSpritePath(string name)
    {
        return string.Format(NpcChat_Sprite_Paths, name);
    }
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

    public static string GetDataTxtPath(string name)
    {
        return string.Format(Data_Txt_Path, name);
    }
}
