using MANAGER;
using Managers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Main : MonoSingleton<Main>
{
    [SerializeField, LabelText("��Ҫ���"), Tooltip("������Ҫ���")]
    public MainCamera mainCamera;
    private void Awake()
    {
        MainThreadDispatcher.StartUpdateMicroCoroutine(this.OnAwake());

    }

    IEnumerator OnAwake()
    {
        ArchiveManager.LoadData();//���ش浵
        yield return null;
        MainThreadDispatcher.StartUpdateMicroCoroutine(DataManager.Load());//��ȡ�����ű��б�
        yield return null;
    }

    private void Start()
    {
        UIMain.Instance.ChangeToGamePanel(0);
    }

    /// <summary>
    /// ��ʼ��
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
        RoundManager.Instance.Restart();
        yield return null;
        ResourcesManager.Instance.Restart();
        yield return null;
        MainThreadDispatcher.StartUpdateMicroCoroutine(PlotManager.Instance.Restart());
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
        this.mainCamera?.ReadArchive();
        yield return null;
        RoundManager.Instance.ReadArchive();
        yield return null;
        ResourcesManager.Instance.ReadArchive();
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
    /// һ����Ϸ����
    /// </summary>
    public void GameOver()
    {
        this.mainCamera?.StopControl();
        UIMain.Instance.ChangeToGamePanel(3);
        UIManager.Instance.Show<UIScoreWindow>();
    }
}
