using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIExecution : MonoBehaviour
{
    public int id;

    [SerializeField, LabelText("�ж�ֵͼ��"), Tooltip("�����ж�ֵͼ��")]
    public Image executionImage;
    [SerializeField, LabelText("��״̬"), Tooltip("��״̬��ʾͼ")]
    public Sprite full;
    [SerializeField, LabelText("��״̬"), Tooltip("��״̬��ʾͼ")]
    public Sprite empty;

    public void SetInfo(bool isFull)
    {
        this.executionImage.sprite = isFull ? this.full : this.empty;
    }
}
