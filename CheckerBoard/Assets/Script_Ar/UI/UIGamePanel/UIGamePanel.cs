using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
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
    public Button upgradeButton;

    [SerializeField, LabelText("各种资源的数量"), Tooltip("各种资源的数量的显示")]
    public List<Text> resourcesAmounts = new List<Text>();

    //[SerializeField, LabelText("当前剩余行动点"), Tooltip("当前剩余行动点的显示")]
    //public Text executionAmount;

    [SerializeField, LabelText("当前回合数"), Tooltip("当前回合数的显示")]
    public Text roundNumber;

    [SerializeField, LabelText("建造按键"), Tooltip("点击建造建筑")]
    public Button buildButton;

    [SerializeField, LabelText("移动按键"), Tooltip("点击移动流浪者")]
    public Button moveButton;

    [SerializeField, LabelText("回合结束按键"), Tooltip("点击进入下一个回合")]
    public Button roundOverButton;

    [SerializeField, LabelText("设置"), Tooltip("打开设置")]
    public Button settingButton;

    [SerializeField, LabelText("信息开关"), Tooltip("打开信息界面开关")]
    public Toggle messageToggle;

    [SerializeField, LabelText("信息面板"), Tooltip("信息界面")]
    public UIMessage uiMessage;
    [SerializeField, LabelText("信息面板标题"), Tooltip("信息标题")]
    public Transform uiMessageTitle;

    [SerializeField, LabelText("任务面板"), Tooltip("放入任务面板")]
    public UIQuestPanel uIQuestPanel;

    [SerializeField, LabelText("行动点面板"), Tooltip("放入行动点面板")]
    public UIExecutionPanel uIExecutionPanel;
    [SerializeField, LabelText("作弊"), Tooltip("作弊")]
    public Button cheat;

    private void Start()
    {
        this.upgradeButton.OnClickAsObservable().Subscribe(_ =>
        {
            //打开升级界面
            UIManager.Instance.Show<UIStrengthenCapabilityWindow>();
            if (NoviceGuideManager.Instance.isGuideStage[4])//是否处于新手指引阶段
            {
                NoviceGuideManager.Instance.NoviceGuideStage++;
            }
        });

        this.buildButton.OnClickAsObservable().Subscribe(_ =>
        {
            if (NoviceGuideManager.Instance.isGuideStage[2])//是否处于新手指引阶段
            {
                NoviceGuideManager.Instance.NoviceGuideStage++;
            }
            UISelectedWindow uISelectedWindow = UIManager.Instance.Show<UISelectedWindow>();
            uISelectedWindow.OpenWindow(0);//打开建筑选择界面
        });

        this.moveButton.OnClickAsObservable().Subscribe(_ =>
        {
            if (NoviceGuideManager.Instance.isGuideStage[0])//是否处于新手指引阶段
            {
                NoviceGuideManager.Instance.NoviceGuideStage++;
            }
            PlotManager.Instance.IsMoveWanderer();
        });

        this.roundOverButton.OnClickAsObservable().Subscribe(_ =>
        {
            RoundManager.Instance.RoundOver();
        });

        this.settingButton.OnClickAsObservable().Subscribe(_ =>
        {
            UIManager.Instance.Show<UISettingWindow>();
        });

        this.messageToggle.OnPointerClickAsObservable().Subscribe(_ =>
        {
            this.uiMessage.gameObject.SetActive(messageToggle.isOn);
            this.uiMessageTitle.gameObject.SetActive(messageToggle.isOn);
        });

        this.cheat.OnClickAsObservable().Subscribe(_ =>
        {
            ResourcesManager.Instance.ChangeWealth(1000000);
        });
    }
}
