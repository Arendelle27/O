using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using UnityEngine;

public class UIMain : MonoSingleton<UIMain>
{
    [SerializeField, LabelText("��Ϸ��ʼ�����"), Tooltip("������Ϸ��ʼ��UI����")]
    public UIStartPanel startPanel;

    [SerializeField, LabelText("��Ϸ�е�UI����"), Tooltip("������Ϸ�е�UI����")]
    public UIGamePanel gamePanel;

    /// <summary>
    /// �л���ʼ������Ϸ���
    /// </summary>
    /// <param name="ischange"></param>
    public void ChangeToGamePanel(bool ischange)
    {
        this.startPanel.gameObject.SetActive(!ischange);
        this.gamePanel.gameObject.SetActive(ischange);
    }

}