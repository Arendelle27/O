using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINoviceGuideWindow : MonoBehaviour
{
    [SerializeField, LabelText("新手指引窗口Id"), Tooltip("新手指引窗口Id")]
    public int id;

    [SerializeField, LabelText("新手指引箭头"), Tooltip("放入新手指引箭头")]
    public Transform UINoviceGuideArrowTransform;

    [SerializeField, LabelText("新手指引描述文本"), Tooltip("放入新手指引描述文本")]
    public Text NoviceGuideDescriptionText;

    /// <summary>
    /// 设置新手指引信息
    /// </summary>
    /// <param name="noviceGuideDefine"></param>
    public void SetInfo(NoviceGuideDefine noviceGuideDefine)
    {
        this.gameObject.SetActive(false);
        this.id=noviceGuideDefine.Id;
        this.NoviceGuideDescriptionText.text = noviceGuideDefine.GuideDescription;
        this.transform.position = new Vector3(noviceGuideDefine.NoviceGuideWindowPosX, noviceGuideDefine.NoviceGuideWindowPosY, 0);

        if(noviceGuideDefine.IsArrow)
        {
            this.UINoviceGuideArrowTransform.gameObject.SetActive(true);
            this.UINoviceGuideArrowTransform.position = new Vector3(noviceGuideDefine.NoviceGuideArrowPosX, noviceGuideDefine.NoviceGuideArrowPosY, 0);
            this.UINoviceGuideArrowTransform.rotation = Quaternion.Euler(0, 0, noviceGuideDefine.NoviceGuideArrowRotZ);
        }
        else
        {
            this.UINoviceGuideArrowTransform.gameObject.SetActive(false);
        }
        this.gameObject.SetActive(true);
    }
}
