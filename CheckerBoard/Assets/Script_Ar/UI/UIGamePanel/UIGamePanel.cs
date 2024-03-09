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
    //[SerializeField, LabelText("能量槽"), Tooltip("当前能量值显示")]
    //public Slider energyValue;
    [SerializeField, LabelText("当前等级"), Tooltip("当前等级显示")]
    public Text leaveValue;

    [SerializeField, LabelText("当前剩余财富"), Tooltip("当前剩余财富的显示")]
    public Text wealthAmount;

    [SerializeField, LabelText("升级按钮"), Tooltip("点击选择升级")]
    public Button upgrade;

    [SerializeField, LabelText("各种资源的数量"), Tooltip("各种资源的数量的显示")]
    public List<Text> resourcesAmounts = new List<Text>();

    [SerializeField, LabelText("当前剩余行动点"), Tooltip("当前剩余行动点的显示")]
    public Text executionAmount;

    [SerializeField, LabelText("当前回合数"), Tooltip("当前回合数的显示")]
    public Text roundNumber;

    [SerializeField, LabelText("回合结束"), Tooltip("点击进入下一个回合")]
    public Button roundOver;

    private void Start()
    {
        this.upgrade.OnClickAsObservable().Subscribe(_ =>
        {
            //打开升级界面
            UIManager.Instance.Show<UIUpgradeWindow>();
        });

        this.roundOver.OnClickAsObservable().Subscribe(_ =>
        {
            RoundManager.Instance.RoundOver();
        });
    }
}
