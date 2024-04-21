using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MANAGER;
using Managers;

public class UIStartPanel : UIPanel
{
    [SerializeField, LabelText("开始新游戏"), Tooltip("重新开始游戏")]
    public Button restartGame;

    [SerializeField, LabelText("继续游戏"), Tooltip("继续已经保存的游戏")]
    public Button continueGame;

    [SerializeField, LabelText("设置"), Tooltip("更改游戏设置")]
    public Button setting;

    [SerializeField, LabelText("退出游戏"), Tooltip("退出游戏")]
    public Button exitGame;

    [SerializeField, LabelText("是否进行新手教程"), Tooltip("放入是否进行新手教程开关")]
    public Toggle isNovicGuideToggle;

    void Start()
    {
        this.restartGame.OnClickAsObservable().Subscribe(_ =>
        {
            //重新开始游戏
            MainThreadDispatcher.StartUpdateMicroCoroutine(Main.Instance.Restart());
        });

        this.continueGame.OnClickAsObservable().Subscribe(_ =>
        {
            //加载上次保存的游戏进度
            if(ArchiveManager.archive!=null)
            {
                MainThreadDispatcher.StartUpdateMicroCoroutine(Main.Instance.ReadArchive());
            }
            else
            {
                Debug.Log("没有存档");
            }

        });

        this.setting.OnClickAsObservable().Subscribe(_ =>
        {
            UIManager.Instance.Show<UISettingWindow>();
            //打开设置界面

        });

        this.exitGame.OnClickAsObservable().Subscribe(_ =>
        {
            //退出游戏
            Application.Quit();
        });
    }

}
