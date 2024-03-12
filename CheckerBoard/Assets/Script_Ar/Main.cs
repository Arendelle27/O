using MANAGER;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoSingleton<Main>
{
    private void Awake()
    {
        //GameObjectPool.Instance.Init();
        
    }

    private void Start()
    {
        UIMain.Instance.ChangeToGamePanel(0);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
     public IEnumerator Init()
    {
        RoundManager.Instance.Init();
        yield return null;
        DataManager.Instance.Init();
        yield return null;
        PlotManager.Instance.Init();
        yield return null;
        SettlementManager.Instance.Init();
        yield return null;
        BuildingManager.Instance.Init();
        yield return null;
        WandererManager.Instance.Init();
        yield return null;
        
        UIMain.Instance.ChangeToGamePanel(1);
    }

    /// <summary>
    /// 一局游戏结束
    /// </summary>
    public void GameOver()
    {
        UIMain.Instance.ChangeToGamePanel(3);
        UIManager.Instance.Show<UIScoreWindow>();
    }
}
