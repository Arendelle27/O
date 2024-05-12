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
    //[SerializeField, LabelText("������"), Tooltip("��ǰ����ֵ��ʾ")]
    //public Slider energyValue;
    [SerializeField, LabelText("��ǰ�ȼ�"), Tooltip("��ǰ�ȼ���ʾ")]
    public Text leaveValue;

    [SerializeField, LabelText("��ǰʣ��Ƹ�"), Tooltip("��ǰʣ��Ƹ�����ʾ")]
    public Text wealthAmount;

    [SerializeField, LabelText("������ť"), Tooltip("���ѡ������")]
    public Button upgradeButton;

    [SerializeField, LabelText("������Դ������"), Tooltip("������Դ����������ʾ")]
    public List<Text> resourcesAmounts = new List<Text>();

    //[SerializeField, LabelText("��ǰʣ���ж���"), Tooltip("��ǰʣ���ж������ʾ")]
    //public Text executionAmount;

    [SerializeField, LabelText("��ǰ�غ���"), Tooltip("��ǰ�غ�������ʾ")]
    public Text roundNumber;

    [SerializeField, LabelText("���찴��"), Tooltip("������콨��")]
    public Button buildButton;

    [SerializeField, LabelText("�ƶ�����"), Tooltip("����ƶ�������")]
    public Button moveButton;

    [SerializeField, LabelText("�غϽ�������"), Tooltip("���������һ���غ�")]
    public Button roundOverButton;

    [SerializeField, LabelText("����"), Tooltip("������")]
    public Button settingButton;

    [SerializeField, LabelText("��Ϣ����"), Tooltip("����Ϣ���濪��")]
    public Toggle messageToggle;

    [SerializeField, LabelText("��Ϣ���"), Tooltip("��Ϣ����")]
    public UIMessage uiMessage;
    [SerializeField, LabelText("��Ϣ������"), Tooltip("��Ϣ����")]
    public Transform uiMessageTitle;

    [SerializeField, LabelText("�������"), Tooltip("�����������")]
    public UIQuestPanel uIQuestPanel;

    [SerializeField, LabelText("�ж������"), Tooltip("�����ж������")]
    public UIExecutionPanel uIExecutionPanel;
    [SerializeField, LabelText("����"), Tooltip("����")]
    public Button cheat;

    private void Start()
    {
        this.upgradeButton.OnClickAsObservable().Subscribe(_ =>
        {
            //����������
            UIManager.Instance.Show<UIStrengthenCapabilityWindow>();
            if (NoviceGuideManager.Instance.isGuideStage[4])//�Ƿ�������ָ���׶�
            {
                NoviceGuideManager.Instance.NoviceGuideStage++;
            }
        });

        this.buildButton.OnClickAsObservable().Subscribe(_ =>
        {
            if (NoviceGuideManager.Instance.isGuideStage[2])//�Ƿ�������ָ���׶�
            {
                NoviceGuideManager.Instance.NoviceGuideStage++;
            }
            UISelectedWindow uISelectedWindow = UIManager.Instance.Show<UISelectedWindow>();
            uISelectedWindow.OpenWindow(0);//�򿪽���ѡ�����
        });

        this.moveButton.OnClickAsObservable().Subscribe(_ =>
        {
            if (NoviceGuideManager.Instance.isGuideStage[0])//�Ƿ�������ָ���׶�
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
