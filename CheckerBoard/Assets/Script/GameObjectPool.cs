using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPool :Singleton<GameObjectPool>
{
    //�ؿ�����
    public ObjectPool<GameObject> Plots { get; set; }
    //���������
    public ObjectPool<GameObject> Buildings { get; set; }

    //�����߶����
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
    /// ��õؿ����
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_Plot()
    {
        GameObject prefab = Resources.Load<GameObject>(PathConfig.Plot_Prefab_Path);
        return prefab;
    }

    /// <summary>
    /// ��ý�������
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_Building()
    {
        GameObject prefab = Resources.Load<GameObject>(PathConfig.Building_Prefab_Path);
        return prefab;
    }

    /// <summary>
    /// ��������߶���
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
