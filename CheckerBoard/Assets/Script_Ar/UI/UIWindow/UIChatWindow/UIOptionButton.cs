using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MANAGER;

public class UIOptionButton : MonoBehaviour
{
    [SerializeField, LabelText("选项Id"), ReadOnly]
    int subChatContentId;

    [SerializeField, LabelText("聊天窗口"), Tooltip("放入聊天窗口")]
    public UIChatWindow chatWindow;

    [SerializeField, LabelText("选项文本"), Tooltip("放入选项文本")]
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
