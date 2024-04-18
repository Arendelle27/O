using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINoviceGuideWindow : MonoBehaviour
{
    [SerializeField, LabelText("����ָ������Id"), Tooltip("����ָ������Id")]
    public int id;

    [SerializeField, LabelText("����ָ����ͷ"), Tooltip("��������ָ����ͷ")]
    public Transform UINoviceGuideArrowTransform;

    [SerializeField, LabelText("����ָ�������ı�"), Tooltip("��������ָ�������ı�")]
    public Text NoviceGuideDescriptionText;

    /// <summary>
    /// ��������ָ����Ϣ
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
