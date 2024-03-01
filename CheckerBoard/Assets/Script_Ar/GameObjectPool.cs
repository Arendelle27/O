using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPool :Singleton<GameObjectPool>
{
    //地块对象池
    public ObjectPool<GameObject> Plots { get; set; }
    //建筑对象池
    public ObjectPool<GameObject> Buildings { get; set; }

    //流浪者对象池
    public ObjectPool<GameObject> Wanderers { get; set; }

    public GameObjectPool()
    {
        this.Plots = new ObjectPool<GameObject>(GetObject_Plot, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 10, 100);
        this.Buildings = new ObjectPool<GameObject>(GetObject_Building, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 10, 50);
        this.Wanderers = new ObjectPool<GameObject>(GetObject_Wanderer, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 1, 1);
    }

    public void Init()
    {

    }

    /// <summary>
    /// 获得地块对象
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_Plot()
    {
        GameObject prefab = Resources.Load<GameObject>(PathConfig.Plot_Prefab_Path);
        return prefab;
    }

    /// <summary>
    /// 获得建筑对象
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_Building()
    {
        GameObject prefab = Resources.Load<GameObject>(PathConfig.Building_Prefab_Path);
        return prefab;
    }

    /// <summary>
    /// 获得流浪者对象
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_Wanderer()
    {
        GameObject prefab = Resources.Load<GameObject>(PathConfig.Wanderer_Prefab_Path);
        return prefab;
    }

    GameObject GetObject_UIBuildingItems()
    {
        GameObject prefab = Resources.Load<GameObject>(PathConfig.UI_BuildingItem_Prefab_Path);
        return prefab;
    }

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
