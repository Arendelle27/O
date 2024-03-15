using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UISettingWindow: UIWindow
{
    [SerializeField, LabelText("保存"), Tooltip("保存游戏")]
    public Button SaveButton;

    [SerializeField, LabelText("返回游戏"), Tooltip("返回游戏")]
    public Button BackToGameButton;

    [SerializeField, LabelText("返回主界面"), Tooltip("返回主界面")]
    public Button BackButton;

    [SerializeField, LabelText("退出按钮"), Tooltip("退出游戏")]
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
