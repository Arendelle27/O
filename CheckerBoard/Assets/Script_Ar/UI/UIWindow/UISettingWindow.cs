using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UISettingWindow: UIWindow
{
    [SerializeField, LabelText("����"), Tooltip("������Ϸ")]
    public Button SaveButton;

    [SerializeField, LabelText("������Ϸ"), Tooltip("������Ϸ")]
    public Button BackToGameButton;

    [SerializeField, LabelText("����������"), Tooltip("����������")]
    public Button BackButton;

    [SerializeField, LabelText("�˳���ť"), Tooltip("�˳���Ϸ")]
    public Button QuitButton;

    private void Start()
    {
        this.SaveButton.OnClickAsObservable().Subscribe(_ =>
        {
            ArchiveManager.SaveData();
        });

        this.BackToGameButton.OnClickAsObservable().Subscribe(_ =>
        {
            this.OnCloseClick();
        });

        this.BackButton.OnClickAsObservable().Subscribe(_ =>
        {
            UIMain.Instance.ChangeToGamePanel(0);
            this.OnCloseClick();
        });

        this.QuitButton.OnClickAsObservable().Subscribe(_ =>
        {
            Application.Quit();
        });
    }
}
