using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MANAGER;

public class UIOptionButton : MonoBehaviour
{
    [SerializeField, LabelText("选项Id"), Tooltip("放入选项Id")]
    public int id;

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
            ChatManager.Instance.UpdateChatWindow(this.id);
        }
        );
    }

    public void SetInfo(string description)
    {
        this.gameObject.SetActive(true);
        this.OptionText.text = description;
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);
    }
}
