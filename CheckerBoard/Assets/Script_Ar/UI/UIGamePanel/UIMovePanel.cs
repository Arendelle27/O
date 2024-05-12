using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MANAGER;

public class UIMovePanel : UIPanel
{
    [SerializeField, LabelText("��ǰ�ж����ı�"), Tooltip("��ʾ��ǰ�ж���")]
    public Text executiontext;

    [SerializeField, LabelText("��ǰ�ƶ������ж����ı�"), Tooltip("��ʾ��ǰ�ƶ������ж���")]
    public Text curCxecutiontext;

    [SerializeField, LabelText("ȷ���ƶ�����"), Tooltip("ȷ���ƶ�����")]
    public Button insureButton;

    [SerializeField, LabelText("ȡ���ƶ�����"), Tooltip("ȡ���ƶ�����")]
    public Button cancelButton;

    private void Start()
    {
        this.insureButton.OnClickAsObservable().Subscribe(_ =>
        {
            //ȷ���ƶ�
            PlotManager.Instance.MoveWanderer(true);
            //if (PlotManager.Instance.MoveWanderer(true))
            //{
            //    if (NoviceGuideManager.Instance.isGuideStage[1])//�Ƿ�������ָ���׶�
            //    {
            //        NoviceGuideManager.Instance.NoviceGuideStage++;
            //    }
            //}
        });

        this.cancelButton.OnClickAsObservable().Subscribe(_ =>
        {
            //ȡ���ƶ�
            PlotManager.Instance.MoveWanderer(false);
        }); 
    }

    private void OnEnable()
    {
        this.executiontext.text = ResourcesManager.Instance.execution.ToString();
    }

}
