using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPool :Singleton<GameObjectPool>
{
    //地块对象池
    public ObjectPool<GameObject> Plots { get; set; }
    //建筑对象池
    public ObjectPool<GameObject> Buildings { get; set; }
    //流浪者对象
    public GameObject Wanderer { get; set; }
    //目的地提示牌对象
    public GameObject DestinationSigns { get; set; }
    //人类聚落对象池
    public ObjectPool<GameObject> HumanSettlements { get; set; }
    //机械聚落对象池
    public ObjectPool<GameObject> RobotSettlements { get; set; }

    //UI建筑物品对象池
    public ObjectPool<GameObject> UIBuildingItems { get; set; }

    public GameObjectPool()
    {
        this.Plots = new ObjectPool<GameObject>(GetObject_Plot, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 10, 100);
        this.Buildings = new ObjectPool<GameObject>(GetObject_Building, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 10, 50);
        this.Wanderer = Resources.Load<GameObject>(PathConfig.GetEntityPrefabPath("Wanderer"));
        this.DestinationSigns = Resources.Load<GameObject>(PathConfig.GetEntityPrefabPath("DestinationSign"));
        this.HumanSettlements = new ObjectPool<GameObject>(GetObject_HumanSettlement, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 5, 10);
        this.RobotSettlements = new ObjectPool<GameObject>(GetObject_RobotSettlement, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 5, 10);

        this.UIBuildingItems = new ObjectPool<GameObject>(GetObject_UIBuildingItem, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 10, 20);

    }

    public void Init()
    {

    }

    #region 实体对象池对象获取
    /// <summary>
    /// 获得地块对象
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_Plot()
    {
        GameObject prefab = Resources.Load<GameObject>(PathConfig.GetEntityPrefabPath("Plot"));
        return prefab;
    }

    /// <summary>
    /// 获得建筑对象
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_Building()
    {
        GameObject prefab = Resources.Load<GameObject>(PathConfig.GetEntityPrefabPath("Building"));
        return prefab;
    }

    /// <summary>
    /// 获得人类聚落对象
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_HumanSettlement()
    {
        GameObject prefab = Resources.Load<GameObject>(PathConfig.GetEntityPrefabPath("HumanSettlement"));
        return prefab;
    }

    /// <summary>
    /// 获得机械聚落对象
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_RobotSettlement()
    {
        GameObject prefab = Resources.Load<GameObject>(PathConfig.GetEntityPrefabPath("RobotSettlement"));
        return prefab;
    }

    #endregion

    #region UI对象池对象获取
    /// <summary>
    /// 获得UI建筑物预制体
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_UIBuildingItem()
    {
        GameObject prefab = Resources.Load<GameObject>(PathConfig.GetUIPrefabPath("UIBuildingItem"));
        return prefab;
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
    }
}
