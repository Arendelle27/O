using ENTITY;
using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatheringBuilding : Building
{
    [SerializeField, LabelText("采集资源类型"), ReadOnly]
    public int gatherResourceType;
    [SerializeField, LabelText("采集资源数量"), ReadOnly]
    public int gatherResourceAmount;
    [SerializeField, LabelText("采集资源所需回合数"), ReadOnly]
    public int gatherResourceRounds;

    [SerializeField, LabelText("已存在回合数"), ReadOnly]
    public int existRound = 0;
    public override void SetInfo(Plot plot, Building_Type type)
    {
        base.SetInfo(plot, type);
        this.TID = DataManager.BuildingScriptLists[0][(int)type - (int)Building_Type.自动采集建筑 - 1].TID;

        this.buildingname = DataManager.BuildingScriptLists[0][this.TID].Name;
        this.description = DataManager.BuildingScriptLists[0][this.TID].Description;
        this.resourcesCost = DataManager.BuildingScriptLists[0][this.TID].ResourcesCost;
        this.attack = DataManager.BuildingScriptLists[0][this.TID].Attack;
        this.hostilityToRobot = DataManager.BuildingScriptLists[0][this.TID].HostilityToRobot;
        this.hostilityToHuman = DataManager.BuildingScriptLists[0][this.TID].HostilityToHuman;
        this.gatherResourceType = (DataManager.BuildingScriptLists[0][this.TID] as GatheringBuildingType).GatherResourceType;
        this.gatherResourceAmount = (DataManager.BuildingScriptLists[0][this.TID] as GatheringBuildingType).GatherResourceAmount;
        this.gatherResourceRounds = (DataManager.BuildingScriptLists[0][this.TID] as GatheringBuildingType).GatherResourceRounds;

        this.SR.sprite = DataManager.BuildingScriptLists[0][this.TID].sprite;//设置建筑的图片
    }

    public void SetInfo(Plot plot, Building_Type type,int existRound)
    {
        this.SetInfo(plot, type);
        this.SetRound(existRound);
    }

    /// <summary>
    /// 设置已存在回合数
    /// </summary>
    /// <param name="existRound"></param>
    public void SetRound(int existRound)
    {
        this.existRound=existRound;
    }

    /// <summary>
    /// 采集资源
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
            Debug.LogFormat("采集资源类型{0}与地块资源类型{1}不匹配", this.gatherResourceType, PlotManager.Instance.plots[this.pos].plotDefine.ResourceType);
        }
        existRound++;

        return resource;
    }

    

}
