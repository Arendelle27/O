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
        SoundManager.Instance.RoundStart(0);
        MainThreadDispatcher.StartUpdateMicroCoroutine(this.Load());
        UIMain.Instance.ChangeToGamePanel(0);
        CGManager.Instance.ReStart();
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    /// <returns></returns>
     void Init()
    {
        UIMain.Instance.ChangeToGamePanel(1);
        SoundManager.Instance.RoundStart(0);
    }

    Coroutine reStartCor;

    public void ReStart()
    {
        if(this.reStartCor!=null)
        {
            StopCoroutine(this.reStartCor);
        }
        this.reStartCor = StartCoroutine(this.RestartCor());
    }

    public IEnumerator RestartCor()
    {
        this.mainCamera?.Restart();
        //CGManager.Instance.PlayCG(0);//���ſ�������
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
        NpcManager.Instance.Restart();
        yield return null;
        WandererManager.Instance.Restart();
        yield return null;
        QuestManager.Instance.Restart();
        yield return null;
        MessageManager.Instance.ReStart();
        yield return new WaitForSeconds(1f);
        this.Init();
        ChatManager.Instance.Restart();
        yield return null;

        //MessageManager.Instance.AddMessage(Message_Type.ָ��, "��Ϸ��ʼ��!");


        //if ((UIMain.Instance.uiPanels[0] as UIStartPanel).isNovicGuideToggle.isOn)//�ж��Ƿ�������ֽ̳�
        //{
        //    ChatManager.Instance.CurChatId = 0;
        //}
        //else
        //{
        //    QuestManager.Instance.GetQuest(-1);//���ܵ�һ������
        //}

    }

    Coroutine readArchiveCor;

    public void ReadArchive()
    {
        if (this.readArchiveCor != null)
        {
            StopCoroutine(this.readArchiveCor);
        }
        this.readArchiveCor = StartCoroutine(this.ReadArchiveCor());
    }

    public IEnumerator ReadArchiveCor()
    {

        this.mainCamera?.ReadArchive();
        yield return null;
        RoundManager.Instance.ReadArchive();
        yield return null;
        CapabilityManager.Instance.ReadArchive();
        yield return null;
        ResourcesManager.Instance.ReadArchive();
        yield return null;
        MainThreadDispatcher.StartUpdateMicroCoroutine(PlotManager.Instance.ReadArchive());
        yield return null;
        EventAreaManager.Instance.ReadArchive();
        yield return null;
        MainThreadDispatcher.StartUpdateMicroCoroutine(BuildingManager.Instance.ReadArchive());
        yield return null;
        MessageManager.Instance.ReadArchive();
        yield return null;
        EventManager.Instance.ReadArchive();
        yield return null;
        NpcManager.Instance.ReadArchive();
        yield return null;
        WandererManager.Instance.ReadArchive();
        yield return null;
        QuestManager.Instance.ReadArchive();
        yield return new WaitForSeconds(1f);
        this.Init();
        ChatManager.Instance.ReadArchive();
        yield return null;

    }


    /// <summary>
    /// һ����Ϸ����
    /// </summary>
    public IEnumerator GameOver(int endId)
    {
        this.mainCamera?.StopControl();
        //UIManager.Instance.Show<UIScoreWindow>();

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
        yield return null;
        QuestManager.Instance.GameOver();
        yield return null;
        NpcManager.Instance.GameOver();
        yield return null;
        ChatManager.Instance.GameOver();
        (UIMain.Instance.ChangeToGamePanel(4) as UIEndPanel).SetInfo(endId);

        SoundManager.Instance.RoundStart(100);
    }
}
