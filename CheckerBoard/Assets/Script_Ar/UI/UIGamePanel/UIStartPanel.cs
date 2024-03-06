using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MANAGER;

public class UIStartPanel : MonoBehaviour
{
    [SerializeField, LabelText("��ʼ����Ϸ"), Tooltip("���¿�ʼ��Ϸ")]
    public Button restartGame;

    [SerializeField, LabelText("������Ϸ"), Tooltip("�����Ѿ��������Ϸ")]
    public Button continueGame;

    [SerializeField, LabelText("����"), Tooltip("������Ϸ����")]
    public Button setting;

    [SerializeField, LabelText("�˳���Ϸ"), Tooltip("�˳���Ϸ")]
    public Button exitGame;


    void Start()
    {
        this.restartGame.OnClickAsObservable().Subscribe(_ =>
        {
            //���¿�ʼ��Ϸ
            MainThreadDispatcher.StartUpdateMicroCoroutine(Main.Instance.Init());
        });

        this.continueGame.OnClickAsObservable().Subscribe(_ =>
        {
            //�����ϴα������Ϸ����

        });

        this.setting.OnClickAsObservable().Subscribe(_ =>
        {
            //�����ý���

        });

        this.exitGame.OnClickAsObservable().Subscribe(_ =>
        {
            //�˳���Ϸ
            Application.Quit();
        });
    }

}
