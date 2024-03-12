using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPool :MonoSingleton<GameObjectPool>
{
    //�ؿ�����
    public ObjectPool<GameObject> Plots { get; set; }
    //���������
    public ObjectPool<GameObject> Buildings { get; set; }
    //�����߶���
    public GameObject Wanderer { get; set; }
    ////Ŀ�ĵ���ʾ�ƶ���
    //public GameObject DestinationSigns { get; set; }
    //�����������
    public ObjectPool<GameObject> HumanSettlements { get; set; }
    //��е��������
    public ObjectPool<GameObject> RobotSettlements { get; set; }

    //UI������Ʒ�����
    public ObjectPool<GameObject> UIBuildingItems { get; set; }

    public void Awake()
    {
        this.Plots = new ObjectPool<GameObject>(GetObject_Plot, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 10, 100);
        this.Buildings = new ObjectPool<GameObject>(GetObject_Building, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 10, 50);
        this.Wanderer = Resources.Load<GameObject>(PathConfig.GetEntityPrefabPath("Wanderer"));
        //this.DestinationSigns = Resources.Load<GameObject>(PathConfig.GetEntityPrefabPath("DestinationSign"));
        this.HumanSettlements = new ObjectPool<GameObject>(GetObject_HumanSettlement, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 5, 10);
        this.RobotSettlements = new ObjectPool<GameObject>(GetObject_RobotSettlement, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 5, 10);

        this.UIBuildingItems = new ObjectPool<GameObject>(GetObject_UIBuildingItem, ActionOnGet, ActionOnReturn, ActionOnDestory, true, 10, 20);

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

    /// <summary>
    /// ��ý�������
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_Building()
    {
        return this.GetEntityObject("Building");
    }

    /// <summary>
    /// �������������
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_HumanSettlement()
    {
        return this.GetEntityObject("HumanSettlement");
    }

    /// <summary>
    /// ��û�е�������
    /// </summary>
    /// <returns></returns>
    GameObject GetObject_RobotSettlement()
    {
        return this.GetEntityObject("RobotSettlement");
    }

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
