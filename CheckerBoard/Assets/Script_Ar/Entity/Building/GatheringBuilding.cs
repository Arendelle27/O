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
    int existReound = 0;
    public override void SetInfo(Plot plot, Building_Type type)
    {
        base.SetInfo(plot, type);
        this.TID = DataManager.BuildingScriptLists[0][(int)type - (int)Building_Type.自动采集建筑 - 1].TID;

        this.resourcesCost = DataManager.BuildingScriptLists[0][this.TID].ResourcesCost;
        this.attack = DataManager.BuildingScriptLists[0][this.TID].Attack;
        this.hostilityToRobot = DataManager.BuildingScriptLists[0][this.TID].HostilityToRobot;
        this.hostilityToHuman = DataManager.BuildingScriptLists[0][this.TID].HostilityToHuman;
        this.gatherResourceType = (DataManager.BuildingScriptLists[0][this.TID] as GatheringBuildingType).GatherResourceType;
        this.gatherResourceAmount = (DataManager.BuildingScriptLists[0][this.TID] as GatheringBuildingType).GatherResourceAmount;
        this.gatherResourceRounds = (DataManager.BuildingScriptLists[0][this.TID] as GatheringBuildingType).GatherResourceRounds;

        this.SR.sprite = DataManager.BuildingScriptLists[0][this.TID].sprite;//设置建筑的图片
    }

    /// <summary>
    /// 采集资源
    /// </summary>
    public List<int> Gather()
    {
        List<int> resource = new List<int>() { 0, 0 };
        if (this.existReound != 0 && this.existReound%(gatherResourceRounds) == 0)
        {
            resource[0] = this.gatherResourceType;
            resource[1] = this.gatherResourceAmount;
        }
        return resource;
    }

    /// <summary>
    /// 建造消耗
    /// </summary>
    public override void SpendResource()
    {
        int[] cost= DataManager.BuildingScriptLists[0][(int)this.type - (int)Building_Type.自动采集建筑 - 1].ResourcesCost;
        ResourcesManager.Instance.ChangeBuildingResources(cost);
    }

    public override void AddHostility()
    {
        if (SettlementManager.Instance.humanSettlements.ContainsKey(this.pos))
        {
            int hostility = DataManager.BuildingScriptLists[0][(int)this.type - (int)Building_Type.自动采集建筑 - 1].HostilityToHuman;
            SettlementManager.Instance.humanSettlements[this.pos].AddHotility(hostility);
        }
        else if (SettlementManager.Instance.robotSettlements.ContainsKey(this.pos))
        {
            int hostility = DataManager.BuildingScriptLists[0][(int)this.type - (int)Building_Type.自动采集建筑 - 1].HostilityToRobot;
            SettlementManager.Instance.robotSettlements[this.pos].AddHotility(hostility);
        }
    }
}
