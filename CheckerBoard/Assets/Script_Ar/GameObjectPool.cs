using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPool :MonoSingleton<GameObjectPool>
{
    //地块对象池
    public ObjectPool<GameObject> Plots { get; set; }
    ////建筑对象池
    //public ObjectPool<GameObject> Buildings { get; set; }
    //流浪者对象
    public GameObject Wanderer { get; set; }
    //目的地提示牌对象
    public GameObject DestinationSign { get; set; }
    //探索小队对象
    public ObjectPool<GameObject> ExploratoryTeams { get; set; }
    ////人类聚落对象池
    //public ObjectPool<GameObject> HumanSettlements { get; set; }
    ////机械聚落对象池
    //public ObjectPool<GameObject> RobotSettlements { get; set; }

    //采集建筑对象池
    public ObjectPool<GameObject> GatheringBuildings { get; set; }

    //生产建筑对象池
    public ObjectPool<GameObject> ProductionBuildings { get; set; }

    //战斗建筑对象池
    public ObjectPool<GameObject> BattleBuildings { get; set; }

    //UI建筑物品对象池
    public ObjectPool<GameObject> UIBuildingItems { get; set; }
    //UI商品物品对象池
    public ObjectPool<GameObject> UICommodityItems { get; set; }
    //UI物品对象池
    public ObjectPool<GameObject> UIGoodItems { get; set; }

    public ObjectPool<GameObject> UIStrengthenCapabilityItems { get; set; }


    public void Awake()
    {
        this.Plots = new ObjectPool<GameObject>(GetObject_Plot, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 80, 400);
        this.GatheringBuildings = new ObjectPool<GameObject>(GetObject_GatheringBuilding, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 10, 20);
        this.ProductionBuildings = new ObjectPool<GameObject>(GetObject_ProductionBuilding, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 10, 20);
        this.BattleBuildings = new ObjectPool<GameObject>(GetObject_BattleBuilding, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 10, 20);

        this.Wanderer = Instantiate(Resources.Load<GameObject>(PathConfig.GetEntityPrefabPath("Wanderer")));
        this.Wanderer.SetActive(false);

        this.DestinationSign = Instantiate(Resources.Load<GameObject>(PathConfig.GetEntityPrefabPath("DestinationSign")));
        this.DestinationSign.SetActive(false);
        //this.DestinationSigns = Resources.Load<GameObject>(PathConfig.GetEntityPrefabPath("DestinationSign"));
        this.ExploratoryTeams = new ObjectPool<GameObject>(GetExploratoryTeam, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 5, 10);
        //this.RobotSettlements = new ObjectPool<GameObject>(GetObject_RobotSettlement, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 5, 10);

        this.UIBuildingItems = new ObjectPool<GameObject>(GetObject_UIBuildingItem, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 10, 20);
        this.UICommodityItems = new ObjectPool<GameObject>(GetObject_UICommodityItem, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 3, 5);
        this.UIGoodItems = new ObjectPool<GameObject>(GetObject_UIGoodItem, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 3, 5);
        this.UIStrengthenCapabilityItems = new ObjectPool<GameObject>(GetObject_UIStrengthenCapabilityItem, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 3, 5);
    }


    /// <summary>
    /// 获得实体对象
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    GameObject GetEntityObject(string name)
    {
        GameObject prefab = Instantiate( Resources.Load<GameObject>(PathConfig.GetEntityPrefabPath(name)),this.transform);
        prefab.SetActive(false);
        return prefab;
    }

    GameObject GetBuildingObject(string name)
    {
        GameObject prefab = Instantiate(Resources.Load<GameObject>(PathConfig.GetBuildingPrefabPath(name)), this.transform);
        prefab.SetActive(false);
        return prefab;
    }

    //GameObject GetSettlementObject(string name)
    //{
    //    GameObject prefab = Instantiate(Resources.Load<GameObject>(PathConfig.GetSettlementPrefabPath(name)), this.transform);
    //    prefab.SetActive(false);
    //    return prefab;
    //}

    /// <summary>
    /// 获得UI对象
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    GameObject GetUIObject(string name)
    {
        GameObject prefab = Instantiate(Resources.Load<GameObject>(PathConfig.GetUIPrefabPath(name)),this.transform);
        prefab.SetActive(false);
        return prefab;
    }

    #region 实体对象池对象获取
    /// <summary>
    /// 获得地块对象
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_Plot()
    {
        return this.GetEntityObject("Plot");
    }

    GameObject GetExploratoryTeam()
    {
        return this.GetEntityObject("ExploratoryTeam");
    }

    /// <summary>
    /// 获得建筑对象
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_GatheringBuilding()
    {
        return this.GetBuildingObject("GatheringBuilding");
    }

    GameObject GetObject_ProductionBuilding()
    {
        return this.GetBuildingObject("ProductionBuilding");
    }

    GameObject GetObject_BattleBuilding()
    {
        return this.GetBuildingObject("BattleBuilding");
    }

    ///// <summary>
    ///// 获得人类聚落对象
    ///// </summary>
    ///// <returns></returns>
    //GameObject GetObject_HumanSettlement()
    //{
    //    return this.GetSettlementObject("HumanSettlement");
    //}

    ///// <summary>
    ///// 获得机械聚落对象
    ///// </summary>
    ///// <returns></returns>
    //GameObject GetObject_RobotSettlement()
    //{
    //    return this.GetSettlementObject("RobotSettlement");
    //}

    #endregion

    #region UI对象池对象获取
    /// <summary>
    /// 获得UI建筑物预制体
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_UIBuildingItem()
    {
        return this.GetUIObject("UIBuildingItem");
    }

    /// <summary>
    /// 获得UI商品物品预制体
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_UICommodityItem()
    {
        return this.GetUIObject("UICommodityItem");
    }
    /// <summary>
    /// 获得UI物出售物品预制体
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_UIGoodItem()
    {
        return this.GetUIObject("UIGoodItem");
    }
    /// <summary>
    /// 获得UI能力增长预制体
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_UIStrengthenCapabilityItem()
    {
        return this.GetUIObject("UIStrengthenCapabilityItem");
    }

    #endregion


    void ActionOnGet(GameObject g)
    {
        g.SetActive(true);
    }

    void ActionOnReturn(GameObject g)
    {
        g.SetActive(false);
    }

    void ActionOnDestory(GameObject g)
    {
        Destroy(g);
    }
}
