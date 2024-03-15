using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Managers;
using Unity.VisualScripting.FullSerializer;

public class UISettingWindow: UIWindow
{
    [SerializeField, LabelText("音乐开关"), Tooltip("音乐开关")]
    public Toggle toggleMusic;
    [SerializeField, LabelText("音效开关"), Tooltip("音效开关")]
    public Toggle toggleVoice;

    [SerializeField, LabelText("音乐音量"), Tooltip("音乐音量")]
    public Slider sliderMusic;
    [SerializeField, LabelText("音效音量"), Tooltip("音效音量")]
    public Slider sliderVoice;

    [SerializeField, LabelText("保存"), Tooltip("保存游戏")]
    public Button SaveButton;

    [SerializeField, LabelText("返回游戏"), Tooltip("返回游戏")]
    public Button BackToGameButton;

    [SerializeField, LabelText("返回主界面"), Tooltip("返回主界面")]
    public Button BackButton;

    [SerializeField, LabelText("退出按钮"), Tooltip("退出游戏")]
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
