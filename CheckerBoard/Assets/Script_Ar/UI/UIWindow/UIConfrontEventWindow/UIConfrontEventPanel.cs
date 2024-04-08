using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MANAGER;

public class UIConfrontEventPanel : MonoBehaviour
{
    [SerializeField, LabelText("成功概率"), Tooltip("放入成功概率显示文本")]
    public Text successRateText;

    [SerializeField, LabelText("确认按键"), Tooltip("放入确认按键")]
    public Button insureButton;

    [SerializeField, LabelText("成功概率"), ReadOnly]
    protected int successRate=0;

    [SerializeField, LabelText("当前冲突事件信息"), ReadOnly]
    protected ConfrontDefine curConfrontDefine;

    protected virtual void Start()
    {
        this.ObserveEveryValueChanged(_ => this.successRate)
            .Subscribe(_ =>
            {
                this.successRateText.text = this.successRate.ToString();
            });
    }

    protected virtual void OnEnable()
    {
        if(EventManager.Instance.curConfrontEvent==null)
        {
            return;
        }
        this.CalculateRates();
        this.SetInfo();
    }

    /// <summary>
    /// 计算概率
    /// </summary>
    /// <param name="settleSort"></param>
    protected virtual void CalculateRates()
    {

    }

    /// <summary>
    /// 设置信息
    /// </summary>
    protected virtual void SetInfo()
    {
        ConfrontDefine cD= EventManager.Instance.curConfrontEvent;
        if(this.curConfrontDefine!=cD)
        {
            this.curConfrontDefine=cD;
        }
        else
        {
            return;
        }
    }
}
