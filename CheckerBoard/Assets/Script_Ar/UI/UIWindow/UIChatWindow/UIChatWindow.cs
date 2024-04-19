using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChatWindow : UIWindow
{
    [SerializeField, LabelText("头像图片"), Tooltip("放入头像图片")]
    public Image HeadPortraitImage;
    [SerializeField, LabelText("名称文本"), Tooltip("放入名称文本")]
    public Text NameText;
    [SerializeField, LabelText("聊天内容文本"), Tooltip("放入聊天内容文本")]
    public Text ChatContentText;
    [SerializeField, LabelText("选项按键列表"), Tooltip("放入选项按键列表")]
    public List<UIOptionButton> Option1Button=new List<UIOptionButton>(3);
    //0是选项1，1是选项2，2是选项3

    public void SetInfo(ChatDefine chatDefine,int chatContentSort)
    {
        HeadPortraitImage.sprite = SpriteManager.headPortraitSprites[chatDefine.HeadPortrait];
        NameText.text = chatDefine.Name;

        foreach (var optionButton in Option1Button)
        {
            optionButton.gameObject.SetActive(false);
        }

        if (chatContentSort==-1)
        {
            ChatContentText.text = chatDefine.ChatContent;
            if (chatDefine.Option1 != null)
            {
                Option1Button[0].SetInfo(chatDefine.Option1);
            }
            if (chatDefine.Option2 != null)
            {
                Option1Button[1].SetInfo(chatDefine.Option2);
            }
            if (chatDefine.Option3 != null)
            {
                Option1Button[2].SetInfo(chatDefine.Option3);
            }
        }
        else
        {
            switch (chatContentSort)
            {
                case 0:
                    ChatContentText.text = chatDefine.Answer1;
                    break;
                case 1:
                    ChatContentText.text = chatDefine.Answer2;
                    break;
                case 2:
                    ChatContentText.text = chatDefine.Answer3;
                    break;
            }
        }
    }
}
