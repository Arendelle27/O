using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINoviceGuidePanel : UIWindow
{
    [SerializeField, LabelText("����ָ������"), Tooltip("��������ָ������")]
    public UINoviceGuideWindow UINoviceGuideWindow;

    [SerializeField, LabelText("����ָ������"), Tooltip("��������ָ������")]
    public List<UINoviceGuideMask> masks = new List<UINoviceGuideMask>(2);
    //(1):0Ϊ���ã�1Ϊ������2Ϊ�ƶ���3Ϊ�غϽ�����4Ϊ����
    //(2):0Ϊȷ���ƶ�

    [SerializeField, LabelText("��Ҫ����Ӧ��UI"), Tooltip("������Ҫ����Ӧ��UI")]
    public List<RectTransform> rectTransforms = new List<RectTransform>();

    public void SetInfo(NoviceGuideDefine noviceGuideDefine)
    {
        this.UINoviceGuideWindow.SetInfo(noviceGuideDefine);
        foreach (var mask in this.masks)
        {
            if(mask.gameObject.activeSelf)
            {
                foreach (var uIMask in mask.uIMasks)
                {
                    if(!uIMask.gameObject.activeSelf)
                    {
                        uIMask.gameObject.SetActive(true);
                    }
                }
                mask.gameObject.SetActive(false);
            }
        }
        switch (noviceGuideDefine.PlayCondition)
        {
            case PlayCondition_Type.����λ��:
                this.masks[0].gameObject.SetActive(true);
                break;
            case PlayCondition_Type.�ƶ�:
                this.masks[0].gameObject.SetActive(true);
                this.masks[0].uIMasks[2].gameObject.SetActive(false);
                break;
            case PlayCondition_Type.�ƶ��ص�:
                this.masks[1].gameObject.SetActive(true);
                break;
            case PlayCondition_Type.����:
                this.masks[0].gameObject.SetActive(true);
                this.masks[0].uIMasks[1].gameObject.SetActive(false);
                break;
        }

        foreach (var rectTransform in rectTransforms)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }

}
