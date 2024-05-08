using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using UnityEngine;

public class UIMain : MonoSingleton<UIMain>
{
    [SerializeField, LabelText("游戏UI界面"), Tooltip("游戏的UI界面")]
    public List<UIPanel> uiPanels = new List<UIPanel>();
    //0.游戏开始的面板
    //1.游戏中的UI界面
    //2.移动流浪者的UI界面
    //3.选择拓展探索小队的UI界面
    //4.结束面板

    [SerializeField, LabelText("自动关闭的游戏窗口界面"), ReadOnly]
    public UISelectedWindow uISelectedWindow;

    //[SerializeField, LabelText("新手指引面板"), ReadOnly]
    //public UINoviceGuidePanel uINoviceGuidePanel;

    [SerializeField, LabelText("当前面板索引"),ReadOnly]
    public int curPanelIndex = -1;

    private void Awake()
    {
        this.uISelectedWindow = UIManager.Instance.Show<UISelectedWindow>();
    }

    /// <summary>
    /// 切换开始面板和游戏面板
    /// </summary>
    /// <param name="ischange"></param>
    public UIPanel ChangeToGamePanel(int index)
    {
        for (int i = 0; i < uiPanels.Count; i++)
        {
            if(i==index)
            {
                this.curPanelIndex = i;
                if (!uiPanels[i].gameObject.activeSelf)
                {
                    uiPanels[i].gameObject.SetActive(true);
                }
            }
            else
            {
                uiPanels[i].gameObject.SetActive(false);
            }
        }
        return uiPanels[index];
    }

}