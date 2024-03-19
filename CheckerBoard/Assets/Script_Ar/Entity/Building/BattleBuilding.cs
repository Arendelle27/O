using ENTITY;
using MANAGER;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBuilding :Building
{
    public override void SetInfo(Plot plot, Building_Type type)
    {
        base.SetInfo(plot, type);
        this.TID=DataManager.BuildingScriptLists[2][(int)this.type - (int)Building_Type.战斗建筑 - 1].TID;

        this.resourcesCost = DataManager.BuildingScriptLists[2][this.TID].ResourcesCost;
        this.attack = DataManager.BuildingScriptLists[2][this.TID].Attack;
        this.hostilityToRobot = DataManager.BuildingScriptLists[2][this.TID].HostilityToRobot;
        this.hostilityToHuman = DataManager.BuildingScriptLists[2][this.TID].HostilityToHuman;

        this.SR.sprite = DataManager.BuildingScriptLists[2][this.TID].sprite;//设置建筑的图片
    }

    public void Battle()
    {

    }

    /// <summary>
    /// 建造消耗
    /// </summary>
    public override void SpendResource()
    {
        int[] cost = this.resourcesCost;
        ResourcesManager.Instance.ChangeBuildingResources(cost);
    }

    public override void AddHostility()
    {
        if(SettlementManager.Instance.humanSettlements.ContainsKey(this.pos))
        {
            int hostility = this.hostilityToHuman;
            SettlementManager.Instance.humanSettlements[this.pos].AddHotility(hostility);
        }
        else if(SettlementManager.Instance.robotSettlements.ContainsKey(this.pos))
        {
            int hostility = this.hostilityToRobot;
            SettlementManager.Instance.robotSettlements[this.pos].AddHotility(hostility);
        }
    }

}
