using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIExtendExpTeamPanel : UIPanel
{
    [SerializeField, LabelText("可选择板块数"), Tooltip("显示剩余可选择板块数")]
    public Text selectPlotAmount;

    [SerializeField, LabelText("完成按键"), Tooltip("完成探索小队的扩张")]
    public Button finishgrade;

    [SerializeField, LabelText("撤销按键"), Tooltip("撤销刚才的选择")]
    public Button withdrawgrade;

    private void Start()
    {
        this.finishgrade.OnClickAsObservable().Subscribe(_ =>
        {
            //完成探索小队的扩张
            if (PlotManager.Instance.map_Mode == Map_Mode.拓展探索小队)
            {
                WandererManager.Instance.exploredV2.Clear();

                PlotManager.Instance.EnterSelectExtendExpTeam(false);//进入选择扩展探索小队的模式
                UIMain.Instance.ChangeToGamePanel(1);//恢复到游戏界面
            }
        });

        this.withdrawgrade.OnClickAsObservable().Subscribe(_ =>
        {
            //撤销刚才的选择
            WandererManager.Instance.WithdrawExpTeam();
        });
    }



    /// <summary>
    /// 更新UI
    /// </summary>
    public void UpdateUI(int proAmount)
    {
        this.selectPlotAmount.text = proAmount.ToString();
        if (proAmount > 0)
        {
            if (this.finishgrade.gameObject.activeSelf)
            {
                this.finishgrade.gameObject.SetActive(false);
            }
        }
        else
        {
            if (!this.finishgrade.gameObject.activeSelf)
            {
                this.finishgrade.gameObject.SetActive(true);
            }
        }
    }

}
