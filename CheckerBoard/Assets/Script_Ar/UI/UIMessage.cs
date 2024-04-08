using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIMessage : MonoBehaviour
{
    [SerializeField, LabelText("信息文本"), Tooltip("显示信息文本")]
    public Text messageText;
    [SerializeField, LabelText("信息内容滑块"), Tooltip("信息内容滑块")]
    public Scrollbar scrollbar;

    public string Message
    {
        set
        {
            if (!this.gameObject.activeSelf)
                return;
            this.messageText.text = value;
            //MainThreadDispatcher.StartUpdateMicroCoroutine(SetMessage(value));
        }
    }


    ///// <summary>
    ///// 设置信息
    ///// </summary>
    ///// <param name="value"></param>
    ///// <returns></returns>
    //IEnumerator SetMessage(string value)
    //{
    //    yield return this.messageText.text = value;
    //    this.scrollbar.value = 0;
    //}
}
