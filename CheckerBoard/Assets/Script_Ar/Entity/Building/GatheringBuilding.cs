using ENTITY;
using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatheringBuilding : Building
{
    [SerializeField, LabelText("�ɼ���Դ����"), ReadOnly]
    public int gatherResourceType;
    [SerializeField, LabelText("�ɼ���Դ����"), ReadOnly]
    public int gatherResourceAmount;
    [SerializeField, LabelText("�ɼ���Դ����غ���"), ReadOnly]
    public int gatherResourceRounds;

    [SerializeField, LabelText("�Ѵ��ڻغ���"), ReadOnly]
    public int existRound = 0;
    public override void SetInfo(Plot plot, Building_Type type)
    {
        base.SetInfo(plot, type);
        this.TID = DataManager.BuildingScriptLists[0][(int)type - (int)Building_Type.�Զ��ɼ����� - 1].TID;

        this.buildingname = DataManager.BuildingScriptLists[0][this.TID].Name;
        this.description = DataManager.BuildingScriptLists[0][this.TID].Description;
        this.resourcesCost = DataManager.BuildingScriptLists[0][this.TID].ResourcesCost;
        this.attack = DataManager.BuildingScriptLists[0][this.TID].Attack;
        this.hostilityToRobot = DataManager.BuildingScriptLists[0][this.TID].HostilityToRobot;
        this.hostilityToHuman = DataManager.BuildingScriptLists[0][this.TID].HostilityToHuman;
        this.gatherResourceType = (DataManager.BuildingScriptLists[0][this.TID] as GatheringBuildingType).GatherResourceType;
        this.gatherResourceAmount = (DataManager.BuildingScriptLists[0][this.TID] as GatheringBuildingType).GatherResourceAmount;
        this.gatherResourceRounds = (DataManager.BuildingScriptLists[0][this.TID] as GatheringBuildingType).GatherResourceRounds;

        this.SR.sprite = DataManager.BuildingScriptLists[0][this.TID].sprite;//���ý�����ͼƬ
    }

    public void SetInfo(Plot plot, Building_Type type,int existRound)
    {
        this.SetInfo(plot, type);
        this.SetRound(existRound);
    }

    /// <summary>
    /// �����Ѵ��ڻغ���
    /// </summary>
    /// <param name="existRound"></param>
    public void SetRound(int existRound)
    {
        this.existRound=existRound;
    }

    /// <summary>
    /// �ɼ���Դ
    /// </summary>
    public List<int> Gather()
    {
        List<int> resource = new List<int>() { 0, 0 };
        if(this.existRound % (gatherResourceRounds) != 0)
        {
            return resource;
        }

        if (this.gatherResourceType == PlotManager.Instance.plots[this.pos].plotDefine.ResourceType)
        {
            resource[0]=this.gatherResourceType;
            resource[1]= PlotManager.Instance.plots[this.pos].ReduceResource(this.gatherResourceAmount);
        }
        else
        {
            Debug.LogFormat("�ɼ���Դ����{0}��ؿ���Դ����{1}��ƥ��", this.gatherResourceType, PlotManager.Instance.plots[this.pos].plotDefine.ResourceType);
        }
        existRound++;

        return resource;
    }

    

}
