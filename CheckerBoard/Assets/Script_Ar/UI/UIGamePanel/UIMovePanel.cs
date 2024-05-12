using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MANAGER;

public class UIMovePanel : UIPanel
{
    [SerializeField, LabelText("当前行动力文本"), Tooltip("显示当前行动力")]
    public Text executiontext;

    [SerializeField, LabelText("当前移动消耗行动力文本"), Tooltip("显示当前移动消耗行动力")]
    public Text curCxecutiontext;

    [SerializeField, LabelText("确认移动按键"), Tooltip("确认移动按键")]
    public Button insureButton;

    [SerializeField, LabelText("取消移动按键"), Tooltip("取消移动按键")]
    public Button cancelButton;

    private void Start()
    {
        this.insureButton.OnClickAsObservable().Subscribe(_ =>
        {
            //确认移动
            PlotManager.Instance.MoveWanderer(true);
            //if (PlotManager.Instance.MoveWanderer(true))
            //{
            //    if (NoviceGuideManager.Instance.isGuideStage[1])//是否处于新手指引阶段
            //    {
            //        NoviceGuideManager.Instance.NoviceGuideStage++;
            //    }
            //}
        });

        this.cancelButton.OnClickAsObservable().Subscribe(_ =>
        {
            //取消移动
            PlotManager.Instance.MoveWanderer(false);
        }); 
    }

    private void OnEnable()
    {
        this.executiontext.text = ResourcesManager.Instance.execution.ToString();
    }

}
