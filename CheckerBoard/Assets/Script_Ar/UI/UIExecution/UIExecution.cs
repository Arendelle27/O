using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIExecution : MonoBehaviour
{
    public int id;

    [SerializeField, LabelText("行动值图像"), Tooltip("放入行动值图像")]
    public Image executionImage;
    [SerializeField, LabelText("满状态"), Tooltip("满状态显示图")]
    public Sprite full;
    [SerializeField, LabelText("空状态"), Tooltip("空状态显示图")]
    public Sprite empty;

    public void SetInfo(bool isFull)
    {
        this.executionImage.sprite = isFull ? this.full : this.empty;
    }
}
