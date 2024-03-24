using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEventWindow : UIWindow
{
    [SerializeField, LabelText("CGͼƬ"), Tooltip("�¼�ͼƬ")]
    public Image CG;

    [SerializeField, LabelText("�¼�����"), Tooltip("�¼�����")]
    public Text EventName;
    [SerializeField, LabelText("�¼�����"), Tooltip("�¼�����")]
    public Text EventDescription;

    [SerializeField, LabelText("�϶�����"), Tooltip("ѡ���")]
    public Button yesButton;
    [SerializeField, LabelText("�϶��ı�"), Tooltip("ѡ��õ��ı�")]
    public Text yesText;
    [SerializeField, LabelText("�񶨰���"), Tooltip("ѡ��")]
    public Button noButton;
    [SerializeField, LabelText("���ı�"), Tooltip("ѡ�񻵵��ı�")]
    public Text noText;

    private void Start()
    {
        this.yesButton.onClick.AddListener(this.OnYesClick);
        this.noButton.onClick.AddListener(this.OnNoClick);
    }
}
