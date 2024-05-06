using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIChatWindow : UIWindow
{
    [SerializeField, LabelText("立绘显示"), Tooltip("放入立绘显示")]
    public Image HeadPortraitImage;
    [SerializeField, LabelText("名称文本"), Tooltip("放入名称文本")]
    public Text NameText;
    [SerializeField, LabelText("名称文本物体"), Tooltip("放入名称文本物体")]
    public Transform nameTransform;
    [SerializeField, LabelText("聊天内容文本"), Tooltip("放入聊天内容文本")]
    public Text ChatContentText;
    [SerializeField, LabelText("选项按键列表"), Tooltip("放入选项按键列表")]
    public List<UIOptionButton> Option1Button=new List<UIOptionButton>(3);
    //0是选项1，1是选项2，2是选项3

    public void SetInfo(ChatDefine chatDefine)
    {
        if(chatDefine.HeadPortrait == HeadPortrait_Type.无)
        {
            HeadPortraitImage.gameObject.SetActive(false);
        }
        else
        {
            HeadPortraitImage.sprite = SpriteManager.headPortraitSprites[chatDefine.HeadPortrait];
            HeadPortraitImage.gameObject.SetActive(true);
        }

        if(chatDefine.Name == null)
        {
            nameTransform.gameObject.SetActive(false);
        }
        else
        {
            NameText.text = chatDefine.Name;
            nameTransform.gameObject.SetActive(true);
        }

        ChatContentText.text = chatDefine.ChatContent;

        foreach (var optionButton in Option1Button)
        {
            optionButton.gameObject.SetActive(false);
        }

        switch (chatDefine.IsOption)
        {
            case 1:
                if (chatDefine.Option1 != null)
                {
                    Option1Button[0].SetInfo(chatDefine.AnswerId1,chatDefine.Option1);
                }
                if (chatDefine.Option2 != null)
                {
                    Option1Button[1].SetInfo(chatDefine.AnswerId2,chatDefine.Option2);
                }
                if (chatDefine.Option3 != null)
                {
                    Option1Button[2].SetInfo(chatDefine.AnswerId3,chatDefine.Option3);
                }
                break;
            default:

                break;

        }

    }
}
