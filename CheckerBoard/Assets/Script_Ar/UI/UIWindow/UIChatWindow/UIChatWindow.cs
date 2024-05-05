using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIChatWindow : UIWindow
{
    [SerializeField, LabelText("ͷ��ͼƬ"), Tooltip("����ͷ��ͼƬ")]
    public Image HeadPortraitImage;
    [SerializeField, LabelText("�����ı�"), Tooltip("���������ı�")]
    public Text NameText;
    [SerializeField, LabelText("���������ı�"), Tooltip("�������������ı�")]
    public Text ChatContentText;
    [SerializeField, LabelText("ѡ����б�"), Tooltip("����ѡ����б�")]
    public List<UIOptionButton> Option1Button=new List<UIOptionButton>(3);
    //0��ѡ��1��1��ѡ��2��2��ѡ��3

    public void SetInfo(ChatDefine chatDefine)
    {
        HeadPortraitImage.sprite = SpriteManager.headPortraitSprites[chatDefine.HeadPortrait];
        NameText.text = chatDefine.Name;

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
