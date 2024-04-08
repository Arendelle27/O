using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIMessage : MonoBehaviour
{
    [SerializeField, LabelText("��Ϣ�ı�"), Tooltip("��ʾ��Ϣ�ı�")]
    public Text messageText;
    [SerializeField, LabelText("��Ϣ���ݻ���"), Tooltip("��Ϣ���ݻ���")]
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
    ///// ������Ϣ
    ///// </summary>
    ///// <param name="value"></param>
    ///// <returns></returns>
    //IEnumerator SetMessage(string value)
    //{
    //    yield return this.messageText.text = value;
    //    this.scrollbar.value = 0;
    //}
}
