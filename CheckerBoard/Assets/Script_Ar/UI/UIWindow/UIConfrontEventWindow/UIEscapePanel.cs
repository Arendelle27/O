using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIEscapePanel : UIConfrontEventPanel
{
    [SerializeField, LabelText("逃跑结果显示"), Tooltip("放入逃跑结果显示文本")]
    public Text escapeAddHostility;

    protected override void Start()
    {
        base.Start();
        this.insureButton.OnClickAsObservable().Subscribe(_ =>
        {
            if (!ResourcesManager.Instance.CanCopyConfront())
            {
                return;
            }
            if (!EventManager.Instance.Escape(this.successRate))
            {
                int rate = EventManager.Instance.calculateRates(0);
                EventManager.Instance.Confront(rate);
            }
            UIManager.Instance.Close<UIConfrontEventWindow>();
        }).AddTo(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void CalculateRates()
    {
        base.CalculateRates();
        int rate=EventManager.Instance.calculateRates(2);
        if(this.successRate!=rate)
        {
            this.successRate=rate;
        }
    }

    protected override void SetInfo()
    {
        base.SetInfo();

        if(this.curConfrontDefine.EscapeAddHostilityDescription!="")
        {
            this.escapeAddHostility.text = string.Format("{0}", this.curConfrontDefine.EscapeAddHostilityDescription);
            this.escapeAddHostility.gameObject.SetActive(true);
        }
        else
        {
            this.escapeAddHostility.gameObject.SetActive(false);
        }
    }
}
