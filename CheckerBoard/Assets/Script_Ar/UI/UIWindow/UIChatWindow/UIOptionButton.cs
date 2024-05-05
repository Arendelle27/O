using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MANAGER;

public class UIOptionButton : MonoBehaviour
{
    [SerializeField, LabelText("ѡ��Id"), ReadOnly]
    int subChatContentId;

    [SerializeField, LabelText("���촰��"), Tooltip("�������촰��")]
    public UIChatWindow chatWindow;

    [SerializeField, LabelText("ѡ���ı�"), Tooltip("����ѡ���ı�")]
    public Text OptionText;

    RectTransform rectTransform;

    private void Awake()
    {
        this.rectTransform = this.GetComponent<RectTransform>();

        this.GetComponent<Button>().OnClickAsObservable().Subscribe(_ =>
        {
            ChatManager.Instance.CurChatContentId=this.subChatContentId;
        }
        );
    }

    public void SetInfo(int chatContentId, string description)
    {
        this.gameObject.SetActive(true);
        this.subChatContentId= chatContentId;
        this.OptionText.text = description;
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);
    }
}
