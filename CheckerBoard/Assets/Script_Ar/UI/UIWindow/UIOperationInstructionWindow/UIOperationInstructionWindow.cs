using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIOperationInstructionWindow : UIWindow
{
    [SerializeField, LabelText("操作指示列表"), Tooltip("操作指示列表")]
    public TabView tabView;
    [SerializeField, LabelText("关闭按键"), Tooltip("放入关闭按键")]
    public Button closeButton;

    [SerializeField, LabelText("自适应的窗口"), Tooltip("放入需要只适应的窗口")]
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
