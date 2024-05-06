using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Sirenix.OdinInspector;

public class UICGWindow : UIWindow
{
    [SerializeField, LabelText("CGͼ����ʾ"), Tooltip("����CGͼ����ʾ")]
    public Image image;
    [SerializeField, LabelText("CGͼ��"), ReadOnly]
    public Sprite sprite;

    private void Start()
    {
        this.ObserveEveryValueChanged(_ => sprite).Subscribe(_ => SetImage());
    }
    void SetImage()
    {
        this.image.sprite = sprite;
    }
}
