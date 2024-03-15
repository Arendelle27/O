using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Managers;
using Unity.VisualScripting.FullSerializer;

public class UISettingWindow: UIWindow
{
    [SerializeField, LabelText("���ֿ���"), Tooltip("���ֿ���")]
    public Toggle toggleMusic;
    [SerializeField, LabelText("��Ч����"), Tooltip("��Ч����")]
    public Toggle toggleVoice;

    [SerializeField, LabelText("��������"), Tooltip("��������")]
    public Slider sliderMusic;
    [SerializeField, LabelText("��Ч����"), Tooltip("��Ч����")]
    public Slider sliderVoice;

    [SerializeField, LabelText("����"), Tooltip("������Ϸ")]
    public Button SaveButton;

    [SerializeField, LabelText("������Ϸ"), Tooltip("������Ϸ")]
    public Button BackToGameButton;

    [SerializeField, LabelText("����������"), Tooltip("����������")]
    public Button BackButton;

    [SerializeField, LabelText("�˳���ť"), Tooltip("�˳���Ϸ")]
    public Button QuitButton;

    private void Start()
    {
        this.SaveButton.OnClickAsObservable().Subscribe(_ =>
        {
            ArchiveManager.SaveData();
        });

        this.BackToGameButton.OnClickAsObservable().Subscribe(_ =>
        {
            this.OnCloseClick();
        });

        this.BackButton.OnClickAsObservable().Subscribe(_ =>
        {
            UIMain.Instance.ChangeToGamePanel(0);
            this.OnCloseClick();
        });

        this.QuitButton.OnClickAsObservable().Subscribe(_ =>
        {
            Application.Quit();
        });

        this.toggleMusic.isOn = SoundConfig.MusicOn;
        this.toggleVoice.isOn = SoundConfig.VoiceOn;
        this.sliderMusic.value = SoundConfig.MusicVolume;
        this.sliderVoice.value = SoundConfig.VoiceVolume;

        this.toggleMusic.OnValueChangedAsObservable().Subscribe(isOn =>
        {
            SoundManager.Instance.MusicOn = isOn;
        });

        this.toggleVoice.OnValueChangedAsObservable().Subscribe(isOn =>
        {
            SoundManager.Instance.VoiceOn = isOn;
        });

        this.sliderMusic.OnValueChangedAsObservable().Subscribe(value =>
        {
            SoundConfig.MusicVolume = (int)value;
            if (!this.toggleMusic.isOn)
            {
                this.toggleMusic.isOn = true;
            }
        });

        this.sliderVoice.OnValueChangedAsObservable().Subscribe(value =>
        {
            SoundConfig.VoiceVolume = (int)value;
            if (!this.toggleVoice.isOn)
            {
                this.toggleVoice.isOn = true;
            }
        });
    }

    private void OnEnable()
    {
        if(UIMain.Instance.curPanelIndex == 0)
        {
            this.SaveButton.gameObject.SetActive(false);
            this.BackButton.gameObject.SetActive(false);
            this.QuitButton.gameObject.SetActive(false);
        }
        else
        {
            if(!this.SaveButton.gameObject.activeSelf)
            {
                this.SaveButton.gameObject.SetActive(true);
            }
            if(!this.BackButton.gameObject.activeSelf)
            {
                this.BackButton.gameObject.SetActive(true) ;
            }
            if (!this.QuitButton.gameObject.activeSelf)
            {
                this.QuitButton.gameObject.SetActive(true);
            }
        }
    }
}
