using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIScoreWindow : UIWindow
{
    [SerializeField, LabelText("�������"), Tooltip("���¿�ʼ��Ϸ")]
    public Text roundnumber;


    public Button RestartButton;

    public Button ExitButton;

    private void Start()
    {
        this.RestartButton.OnClickAsObservable().Subscribe(_ =>
        {
            //���¿�ʼ��Ϸ
            MainThreadDispatcher.StartUpdateMicroCoroutine(Main.Instance.Restart());
            this.OnCloseClick();
        });

        this.ExitButton.OnClickAsObservable().Subscribe(_ =>
        {
            //�˳���Ϸ
            UIMain.Instance.ChangeToGamePanel(0);
            this.OnCloseClick();
        });
    }

    private void OnEnable()
    {
        this.UpdateUI();
    }

    /// <summary>
    /// ����UI
    /// </summary>
    void UpdateUI()
    {
        this.roundnumber.text = RoundManager.Instance.roundNumber.ToString();
    }
}