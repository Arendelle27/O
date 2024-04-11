using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPool :MonoSingleton<GameObjectPool>
{
    //�ؿ�����
    public ObjectPool<GameObject> Plots { get; set; }
    ////���������
    //public ObjectPool<GameObject> Buildings { get; set; }
    //�����߶���
    public GameObject Wanderer { get; set; }
    //Ŀ�ĵ���ʾ�ƶ���
    public GameObject DestinationSign { get; set; }
    //̽��С�Ӷ���
    public ObjectPool<GameObject> ExploratoryTeams { get; set; }
    ////�����������
    //public ObjectPool<GameObject> HumanSettlements { get; set; }
    ////��е��������
    //public ObjectPool<GameObject> RobotSettlements { get; set; }

    //�ɼ����������
    public ObjectPool<GameObject> GatheringBuildings { get; set; }

    //�������������
    public ObjectPool<GameObject> ProductionBuildings { get; set; }

    //ս�����������
    public ObjectPool<GameObject> BattleBuildings { get; set; }

    //UI������Ʒ�����
    public ObjectPool<GameObject> UIBuildingItems { get; set; }
    //UI��Ʒ��Ʒ�����
    public ObjectPool<GameObject> UICommodityItems { get; set; }
    //UI��Ʒ�����
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
    /// ���ʵ�����
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
    /// ���UI����
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    GameObject GetUIObject(string name)
    {
        GameObject prefab = Instantiate(Resources.Load<GameObject>(PathConfig.GetUIPrefabPath(name)),this.transform);
        prefab.SetActive(false);
        return prefab;
    }

    #region ʵ�����ض����ȡ
    /// <summary>
    /// ��õؿ����
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
    /// ��ý�������
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
    ///// �������������
    ///// </summary>
    ///// <returns></returns>
    //GameObject GetObject_HumanSettlement()
    //{
    //    return this.GetSettlementObject("HumanSettlement");
    //}

    ///// <summary>
    ///// ��û�е�������
    ///// </summary>
    ///// <returns></returns>
    //GameObject GetObject_RobotSettlement()
    //{
    //    return this.GetSettlementObject("RobotSettlement");
    //}

    #endregion

    #region UI����ض����ȡ
    /// <summary>
    /// ���UI������Ԥ����
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_UIBuildingItem()
    {
        return this.GetUIObject("UIBuildingItem");
    }

    /// <summary>
    /// ���UI��Ʒ��ƷԤ����
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_UICommodityItem()
    {
        return this.GetUIObject("UICommodityItem");
    }
    /// <summary>
    /// ���UI�������ƷԤ����
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_UIGoodItem()
    {
        return this.GetUIObject("UIGoodItem");
    }
    /// <summary>
    /// ���UI��������Ԥ����
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
