using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MANAGER;

public class UIStartPanel : MonoBehaviour
{
    [SerializeField, LabelText("开始新游戏"), Tooltip("重新开始游戏")]
    public Button restartGame;

    [SerializeField, LabelText("继续游戏"), Tooltip("继续已经保存的游戏")]
    public Button continueGame;

    [SerializeField, LabelText("设置"), Tooltip("更改游戏设置")]
    public Button setting;

    [SerializeField, LabelText("退出游戏"), Tooltip("退出游戏")]
    public Button exitGame;


    void Start()
    {
        this.restartGame.OnClickAsObservable().Subscribe(_ =>
        {
            //重新开始游戏
            MainThreadDispatcher.StartUpdateMicroCoroutine(Main.Instance.Init());
        });

        this.continueGame.OnClickAsObservable().Subscribe(_ =>
        {
            //加载上次保存的游戏进度

        });

        this.setting.OnClickAsObservable().Subscribe(_ =>
        {
            //打开设置界面

        });

        this.exitGame.OnClickAsObservable().Subscribe(_ =>
        {
            //退出游戏
            Application.Quit();
        });
    }

}
