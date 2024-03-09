using MANAGER;
using Managers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePanel : UIPanel
{
    //[SerializeField, LabelText("������"), Tooltip("��ǰ����ֵ��ʾ")]
    //public Slider energyValue;
    [SerializeField, LabelText("��ǰ�ȼ�"), Tooltip("��ǰ�ȼ���ʾ")]
    public Text leaveValue;

    [SerializeField, LabelText("��ǰʣ��Ƹ�"), Tooltip("��ǰʣ��Ƹ�����ʾ")]
    public Text wealthAmount;

    [SerializeField, LabelText("������ť"), Tooltip("���ѡ������")]
    public Button upgrade;

    [SerializeField, LabelText("������Դ������"), Tooltip("������Դ����������ʾ")]
    public List<Text> resourcesAmounts = new List<Text>();

    [SerializeField, LabelText("��ǰʣ���ж���"), Tooltip("��ǰʣ���ж������ʾ")]
    public Text executionAmount;

    [SerializeField, LabelText("��ǰ�غ���"), Tooltip("��ǰ�غ�������ʾ")]
    public Text roundNumber;

    [SerializeField, LabelText("�غϽ���"), Tooltip("���������һ���غ�")]
    public Button roundOver;

    private void Start()
    {
        this.upgrade.OnClickAsObservable().Subscribe(_ =>
        {
            //����������
            UIManager.Instance.Show<UIUpgradeWindow>();
        });

        this.roundOver.OnClickAsObservable().Subscribe(_ =>
        {
            RoundManager.Instance.RoundOver();
        });
    }
}
