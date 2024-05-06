using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Sirenix.OdinInspector;

public class UICGWindow : UIWindow
{
    [SerializeField, LabelText("CGÍ¼ÏñÏÔÊ¾"), Tooltip("·ÅÈëCGÍ¼ÏñÏÔÊ¾")]
    public Image image;
    [SerializeField, LabelText("CGÍ¼Ïñ"), ReadOnly]
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
