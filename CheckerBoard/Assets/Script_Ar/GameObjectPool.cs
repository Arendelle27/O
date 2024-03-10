using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPool :Singleton<GameObjectPool>
{
    //�ؿ�����
    public ObjectPool<GameObject> Plots { get; set; }
    //���������
    public ObjectPool<GameObject> Buildings { get; set; }
    //�����߶���
    public GameObject Wanderer { get; set; }
    //Ŀ�ĵ���ʾ�ƶ���
    public GameObject DestinationSigns { get; set; }
    //�����������
    public ObjectPool<GameObject> HumanSettlements { get; set; }
    //��е��������
    public ObjectPool<GameObject> RobotSettlements { get; set; }

    //UI������Ʒ�����
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

    #region ʵ�����ض����ȡ
    /// <summary>
    /// ��õؿ����
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_Plot()
    {
        GameObject prefab = Resources.Load<GameObject>(PathConfig.GetEntityPrefabPath("Plot"));
        return prefab;
    }

    /// <summary>
    /// ��ý�������
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_Building()
    {
        GameObject prefab = Resources.Load<GameObject>(PathConfig.GetEntityPrefabPath("Building"));
        return prefab;
    }

    /// <summary>
    /// �������������
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_HumanSettlement()
    {
        GameObject prefab = Resources.Load<GameObject>(PathConfig.GetEntityPrefabPath("HumanSettlement"));
        return prefab;
    }

    /// <summary>
    /// ��û�е�������
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_RobotSettlement()
    {
        GameObject prefab = Resources.Load<GameObject>(PathConfig.GetEntityPrefabPath("RobotSettlement"));
        return prefab;
    }

    #endregion

    #region UI����ض����ȡ
    /// <summary>
    /// ���UI������Ԥ����
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
