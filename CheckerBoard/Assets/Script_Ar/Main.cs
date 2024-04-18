using MANAGER;
using Managers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;

public class Main : MonoSingleton<Main>
{
    [SerializeField, LabelText("��Ҫ���"), Tooltip("������Ҫ���")]
    public MainCamera mainCamera;
    private void Awake()
    {
        //MainThreadDispatcher.StartUpdateMicroCoroutine(this.OnAwake());

    }

    IEnumerator Load()
    {
        UnityLogger.Init();


        MainThreadDispatcher.StartUpdateMicroCoroutine(DataManager.Load());//��ȡ�����ű��б�
        yield return null;
        ArchiveManager.LoadData();//���ش浵
        yield return null;
    }

    private void Start()
    {
        MainThreadDispatcher.StartUpdateMicroCoroutine(this.Load());
        UIMain.Instance.ChangeToGamePanel(0);
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    /// <returns></returns>
     void Init()
    {
        UIMain.Instance.ChangeToGamePanel(1);

        MessageManager.Instance.AddMessage(Message_Type.ָ��, "��Ϸ��ʼ��!");
    }

    public IEnumerator Restart()
    {
        this.mainCamera?.Restart();
        yield return null;
        MainThreadDispatcher.StartUpdateMicroCoroutine(PlotManager.Instance.Restart());
        yield return null;
        CapabilityManager.Instance.Restart();
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
        MessageManager.Instance.ReStart();
        yield return null;
        this.Init();
        NoviceGuideManager.Instance.OnStart();


    }

    public IEnumerator ReadArchive()
    {
        this.mainCamera?.ReadArchive();
        yield return null;
        RoundManager.Instance.ReadArchive();
        yield return null;
        ResourcesManager.Instance.ReadArchive();
        yield return null;
        MainThreadDispatcher.StartUpdateMicroCoroutine(PlotManager.Instance.ReadArchive());
        yield return null;
        EventAreaManager.Instance.ReadArchive();
        yield return null;
        MainThreadDispatcher.StartUpdateMicroCoroutine(BuildingManager.Instance.ReadArchive());
        yield return null;
        WandererManager.Instance.ReadArchive();
        yield return null;
        MessageManager.Instance.ReadArchive();
        yield return null;

        this.Init();
    }


    /// <summary>
    /// һ����Ϸ����
    /// </summary>
    public IEnumerator GameOver()
    {
        this.mainCamera?.StopControl();
        UIMain.Instance.ChangeToGamePanel(4);
        UIManager.Instance.Show<UIScoreWindow>();

        BuildingManager.Instance.GameOver();
        yield return null; 
        WandererManager.Instance.GameOver();
        yield return null;
        ResourcesManager.Instance.GameOver();
        yield return null;
        EventAreaManager.Instance.GameOver();
        yield return null;
        RoundManager.Instance.GameOver();
        yield return null;
        PlotManager.Instance.GameOver();
        yield return null;
        MessageManager.Instance.GameOver();

    }
}
