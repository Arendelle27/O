using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MANAGER;

public class UIConfrontEventPanel : MonoBehaviour
{
    [SerializeField, LabelText("�ɹ�����"), Tooltip("����ɹ�������ʾ�ı�")]
    public Text successRateText;

    [SerializeField, LabelText("ȷ�ϰ���"), Tooltip("����ȷ�ϰ���")]
    public Button insureButton;

    [SerializeField, LabelText("�ɹ�����"), ReadOnly]
    protected int successRate=0;

    [SerializeField, LabelText("��ǰ��ͻ�¼���Ϣ"), ReadOnly]
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
    /// �������
    /// </summary>
    /// <param name="settleSort"></param>
    protected virtual void CalculateRates()
    {

    }

    /// <summary>
    /// ������Ϣ
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
