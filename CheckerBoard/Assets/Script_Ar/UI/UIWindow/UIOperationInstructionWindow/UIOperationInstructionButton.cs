using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOperationInstructionButton : TabButton
{
    [SerializeField, LabelText("��ѡ��ͼ��"), Tooltip("���뱻ѡ��ͼ��")]
    public GameObject activeOIImage;

    [SerializeField, LabelText("δ��ѡ��ͼ��"), Tooltip("����δ��ѡ��ͼ��")]
    public GameObject normalOIImage;

    public override void Select(bool select)
    {
        if (!this.gameObject.activeSelf)
            return;
        this.activeOIImage.SetActive(select);
        this.normalOIImage.SetActive(!select);

    }
}
