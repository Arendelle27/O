using MANAGER;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoSingleton<Main>
{
    private void Awake()
    {
        GameObjectPool.Instance.Init();
        
    }

    private void Start()
    {
        UIMain.Instance.ChangeToGamePanel(false);
    }

    public IEnumerator Init()
    {
        RoundManager.Instance.Init();
        yield return null;
        ResourceManager.Instance.Init();
        yield return null;
        PlotManager.Instance.Init();
        yield return null;
        BuildingManager.Instance.Init();
        yield return null;
        WandererManager.Instance.Init();
        yield return null;
        
        UIMain.Instance.ChangeToGamePanel(true);
    }

    /// <summary>
    /// 一局游戏结束
    /// </summary>
    public void GameOver()
    {
        UIManager.Instance.Show<UIScorePanel>();
    }
}
