using ENTITY;
using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : Building
{
    [SerializeField, LabelText("每回合产出货币数量"), ReadOnly]
    public int production;
    public override void SetInfo(Plot plot, Building_Type type)
    {
        base.SetInfo(plot, type);
        this.TID = DataManager.BuildingScriptLists[1][(int)this.type - (int)Building_Type.生产建筑 - 1].TID;

        this.resourcesCost = DataManager.BuildingScriptLists[1][this.TID].ResourcesCost;
        this.attack = DataManager.BuildingScriptLists[1][this.TID].Attack;
        this.hostilityToRobot = DataManager.BuildingScriptLists[1][this.TID].HostilityToRobot;
        this.hostilityToHuman = DataManager.BuildingScriptLists[1][this.TID].HostilityToHuman;
        
        this.production = (DataManager.BuildingScriptLists[1][this.TID] as ProductionBuildingType).Production;

        this.SR.sprite = DataManager.BuildingScriptLists[1][(int)type - (int)Building_Type.生产建筑 - 1].sprite;//设置建筑的图片
    }
    /// <summary>
    /// 生产
    /// </summary>
    public int Produce()
    {
        if (PlotManager.Instance.plots[this.pos].HaveExploratoryTeam)
        {
            return this.production;
        }
        return 0;
    }

}
