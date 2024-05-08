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
    [SerializeField, LabelText("存活天数"), Tooltip("重新开始游戏")]
    public Text roundnumber;

    [SerializeField, LabelText("重新开始游戏"), Tooltip("重新开始游戏按键")]
    public Button RestartButton;
    [SerializeField, LabelText("退出游戏"), Tooltip("退出游戏按键")]
    public Button ExitButton;

    private void Start()
    {
        this.RestartButton.OnClickAsObservable().Subscribe(_ =>
        {
            //重新开始游戏
            Main.Instance.ReStart();
            this.OnCloseClick();
        });

        this.ExitButton.OnClickAsObservable().Subscribe(_ =>
        {
            //退出游戏
            UIMain.Instance.ChangeToGamePanel(0);
            this.OnCloseClick();
        });
    }

    private void OnEnable()
    {
        this.UpdateUI();
    }

    /// <summary>
    /// 更新UI
    /// </summary>
    void UpdateUI()
    {
        this.roundnumber.text = RoundManager.Instance.roundNumber.ToString();
    }
}
