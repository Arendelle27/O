using MANAGER;
using Managers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Main : MonoSingleton<Main>
{
    [SerializeField, LabelText("主要相机"), Tooltip("放入主要相机")]
    public MainCamera mainCamera;
    private void Awake()
    {
        MainThreadDispatcher.StartUpdateMicroCoroutine(this.OnAwake());

    }

    IEnumerator OnAwake()
    {
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Log.Init("Unity");
        yield return null;
        ArchiveManager.LoadData();//加载存档
        yield return null;
        MainThreadDispatcher.StartUpdateMicroCoroutine(DataManager.Load());//读取建筑脚本列表
        yield return null;
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
        this.mainCamera?.Restart();
        yield return null;
        MainThreadDispatcher.StartUpdateMicroCoroutine(PlotManager.Instance.Restart());
        yield return null;
        RoundManager.Instance.Restart();
        yield return null;
        ResourcesManager.Instance.Restart();
        yield return null;
        EventAreaManager.Instance.Restart();
        yield return null;
        BuildingManager.Instance.Restart();
        yield return null;
        WandererManager.Instance.Restart();
        yield return null;

        this.Init();
    }

    public IEnumerator ReadArchive()
    {
        this.mainCamera?.ReadArchive();
        yield return null;
        RoundManager.Instance.ReadArchive();
        yield return null;
        ResourcesManager.Instance.ReadArchive();
        yield return null;
        PlotManager.Instance.ReadArchive();
        yield return null;
        EventAreaManager.Instance.ReadArchive();
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
        this.mainCamera?.StopControl();
        UIMain.Instance.ChangeToGamePanel(3);
        UIManager.Instance.Show<UIScoreWindow>();
    }
}
