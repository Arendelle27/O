using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIConfrontEventWindow : UIWindow
{
    [SerializeField, LabelText("�Կ��¼��ȼ���ʾ"), Tooltip("����Կ��¼��ȼ���ʾ�ı�")]
    public Text confrontLevelText;

    [SerializeField, LabelText("�Կ��¼�����"), Tooltip("����Կ��¼�������ʾ�ı�")]
    public Text confrontDescribeText;

    public List<UIConfrontEventPanel> uIConfrontEventPanels=new List<UIConfrontEventPanel>(3) { };

    public List<Button> copeWayButton = new List<Button>(3) { };

    private void Start()
    {
        this.copeWayButton[0].OnClickAsObservable().Subscribe(_ =>
        {
            SetPanel(0);
        });

        this.copeWayButton[1].OnClickAsObservable().Subscribe(_ =>
        {
            SetPanel(1);
        });

        this.copeWayButton[2].OnClickAsObservable().Subscribe(_ =>
        {
            SetPanel(2);
        });
    }

    private void OnEnable()
    {
        SetInfo();
        SetPanel(0);
    }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="confrontDefine"></param>
    public void SetInfo()
    {
        this.confrontLevelText.text = EventManager.Instance.curConfrontEvent.Level.ToString();
        this.confrontDescribeText.text = EventManager.Instance.curConfrontEvent.Description;
    }

    /// <summary>
    /// �򿪶�Ӧ���
    /// </summary>
    /// <param name="index"></param>
    public void SetPanel(int index)
    {
        for (int i = 0; i < this.uIConfrontEventPanels.Count; i++)
        {
            this.uIConfrontEventPanels[i].gameObject.SetActive(i == index);
        }
    }
}
