using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOperationInstructionButton : TabButton
{
    [SerializeField, LabelText("被选中图像"), Tooltip("放入被选中图像")]
    public GameObject activeOIImage;

    [SerializeField, LabelText("未被选中图像"), Tooltip("放入未被选中图像")]
    public GameObject normalOIImage;

    public override void Select(bool select)
    {
        if (!this.gameObject.activeSelf)
            return;
        this.activeOIImage.SetActive(select);
        this.normalOIImage.SetActive(!select);

    }
}
