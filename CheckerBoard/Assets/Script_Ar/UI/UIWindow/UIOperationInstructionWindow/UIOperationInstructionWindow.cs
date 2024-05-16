using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIOperationInstructionWindow : UIWindow
{
    [SerializeField, LabelText("����ָʾ�б�"), Tooltip("����ָʾ�б�")]
    public TabView tabView;
    [SerializeField, LabelText("�رհ���"), Tooltip("����رհ���")]
    public Button closeButton;

    [SerializeField, LabelText("����Ӧ�Ĵ���"), Tooltip("������Ҫֻ��Ӧ�Ĵ���")]
    public List<RectTransform> rectTransforms = new List<RectTransform>();

    void Start()
    {
        tabView.OnTabSelect = this.OnTabSelect;

        this.closeButton.OnClickAsObservable().Subscribe(_ =>
        {
            this.OnCloseClick();
        });

        foreach (var rectTransform in rectTransforms)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }

    private void OnEnable()
    {
        //foreach (var rectTransform in rectTransforms)
        //{
        //    LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        //}
    }

    void OnTabSelect(int index)
    {

    }
}
