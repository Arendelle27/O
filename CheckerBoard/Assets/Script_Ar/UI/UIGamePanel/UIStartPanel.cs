using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MANAGER;
using Managers;

public class UIStartPanel : UIPanel
{
    [SerializeField, LabelText("��ʼ����Ϸ"), Tooltip("���¿�ʼ��Ϸ")]
    public Button restartGame;

    [SerializeField, LabelText("������Ϸ"), Tooltip("�����Ѿ��������Ϸ")]
    public Button continueGame;

    [SerializeField, LabelText("����"), Tooltip("������Ϸ����")]
    public Button setting;

    [SerializeField, LabelText("�˳���Ϸ"), Tooltip("�˳���Ϸ")]
    public Button exitGame;

    [SerializeField, LabelText("�Ƿ�������ֽ̳�"), Tooltip("�����Ƿ�������ֽ̳̿���")]
    public Toggle isNovicGuideToggle;

    void Start()
    {
        this.restartGame.OnClickAsObservable().Subscribe(_ =>
        {
            //���¿�ʼ��Ϸ
            MainThreadDispatcher.StartUpdateMicroCoroutine(Main.Instance.Restart());
        });

        this.continueGame.OnClickAsObservable().Subscribe(_ =>
        {
            //�����ϴα������Ϸ����
            if(ArchiveManager.archive!=null)
            {
                MainThreadDispatcher.StartUpdateMicroCoroutine(Main.Instance.ReadArchive());
            }
            else
            {
                Debug.Log("û�д浵");
            }

        });

        this.setting.OnClickAsObservable().Subscribe(_ =>
        {
            UIManager.Instance.Show<UISettingWindow>();
            //�����ý���

        });

        this.exitGame.OnClickAsObservable().Subscribe(_ =>
        {
            //�˳���Ϸ
            Application.Quit();
        });
    }

}
