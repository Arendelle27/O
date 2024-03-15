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
        ArchiveManager.LoadData();//加载存档
        
    }

    private void Start()
    {
        UIMain.Instance.ChangeToGamePanel(0);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
     void Init()
    {
        
        UIMain.Instance.ChangeToGamePanel(1);
    }

    public IEnumerator Restart()
    {
        RoundManager.Instance.Restart();
        yield return null;
        DataManager.Instance.Restart();
        yield return null;
        PlotManager.Instance.Restart();
        yield return null;
        SettlementManager.Instance.Restart();
        yield return null;
        BuildingManager.Instance.Restart();
        yield return null;
        WandererManager.Instance.Restart();
        yield return null;

        this.Init();
    }

    public IEnumerator ReadArchive()
    {
        RoundManager.Instance.ReadArchive();
        yield return null;
        DataManager.Instance.ReadArchive();
        yield return null;
        PlotManager.Instance.ReadArchive();
        yield return null;
        SettlementManager.Instance.ReadArchive();
        yield return null;
        BuildingManager.Instance.ReadArchive();
        yield return null;
        WandererManager.Instance.ReadArchive();
        yield return null;

        this.Init();
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
