using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using UnityEngine;

public class UIMain : MonoSingleton<UIMain>
{
    [SerializeField, LabelText("游戏开始的面板"), Tooltip("放入游戏开始的UI界面")]
    public UIStartPanel startPanel;

    [SerializeField, LabelText("游戏中的UI界面"), Tooltip("放入游戏中的UI界面")]
    public UIGamePanel gamePanel;

    /// <summary>
    /// 切换开始面板和游戏面板
    /// </summary>
    /// <param name="ischange"></param>
    public void ChangeToGamePanel(bool ischange)
    {
        this.startPanel.gameObject.SetActive(!ischange);
        this.gamePanel.gameObject.SetActive(ischange);
    }

}