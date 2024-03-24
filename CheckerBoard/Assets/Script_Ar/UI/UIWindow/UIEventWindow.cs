using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEventWindow : UIWindow
{
    [SerializeField, LabelText("CG图片"), Tooltip("事件图片")]
    public Image CG;

    [SerializeField, LabelText("事件名称"), Tooltip("事件名称")]
    public Text EventName;
    [SerializeField, LabelText("事件描述"), Tooltip("事件描述")]
    public Text EventDescription;

    [SerializeField, LabelText("肯定按键"), Tooltip("选择好")]
    public Button yesButton;
    [SerializeField, LabelText("肯定文本"), Tooltip("选择好的文本")]
    public Text yesText;
    [SerializeField, LabelText("否定按键"), Tooltip("选择坏")]
    public Button noButton;
    [SerializeField, LabelText("否定文本"), Tooltip("选择坏的文本")]
    public Text noText;

    private void Start()
    {
        this.yesButton.onClick.AddListener(this.OnYesClick);
        this.noButton.onClick.AddListener(this.OnNoClick);
    }
}
