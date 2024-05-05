using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIConfrontEventWindow : UIWindow
{
    [SerializeField, LabelText("对抗事件等级显示"), Tooltip("放入对抗事件等级显示文本")]
    public Text confrontLevelText;

    [SerializeField, LabelText("对抗事件描述"), Tooltip("放入对抗事件描述显示文本")]
    public Text confrontDescribeText;

    public List<UIConfrontEventPanel> uIConfrontEventPanels=new List<UIConfrontEventPanel>(3) { };

    public List<Button> copeWayButton = new List<Button>(3) { };

    private void Start()
    {
        this.copeWayButton[0].OnClickAsObservable().Subscribe(_ =>
        {
            SetPanel(0);
        });

        this.copeWayButton[1].OnClickAsObservable().Subscribe(_ =>
        {
            SetPanel(1);
        });

        this.copeWayButton[2].OnClickAsObservable().Subscribe(_ =>
        {
            SetPanel(2);
        });
    }

    private void OnEnable()
    {
        SetInfo();
        SetPanel(0);
    }

    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="confrontDefine"></param>
    public void SetInfo()
    {
        this.confrontLevelText.text = EventManager.Instance.curConfrontEvent.Level.ToString();
        this.confrontDescribeText.text = EventManager.Instance.curConfrontEvent.Description;
    }

    /// <summary>
    /// 打开对应面板
    /// </summary>
    /// <param name="index"></param>
    public void SetPanel(int index)
    {
        for (int i = 0; i < this.uIConfrontEventPanels.Count; i++)
        {
            this.uIConfrontEventPanels[i].gameObject.SetActive(i == index);
        }
    }
}
