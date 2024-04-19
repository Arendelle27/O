using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINoviceGuidePanel : UIWindow
{
    [SerializeField, LabelText("新手指引窗口"), Tooltip("放入新手指引窗口")]
    public UINoviceGuideWindow UINoviceGuideWindow;

    [SerializeField, LabelText("新手指引遮罩"), Tooltip("放入新手指引遮罩")]
    public List<UINoviceGuideMask> masks = new List<UINoviceGuideMask>(2);
    //(1):0为设置，1为建筑，2为移动，3为回合结束，4为升级
    //(2):0为确认移动

    [SerializeField, LabelText("需要自适应的UI"), Tooltip("放入需要自适应的UI")]
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
            case PlayCondition_Type.任意位置:
                this.masks[0].gameObject.SetActive(true);
                break;
            case PlayCondition_Type.移动:
                this.masks[0].gameObject.SetActive(true);
                this.masks[0].uIMasks[2].gameObject.SetActive(false);
                break;
            case PlayCondition_Type.移动地点:
                this.masks[1].gameObject.SetActive(true);
                break;
            case PlayCondition_Type.建造:
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
