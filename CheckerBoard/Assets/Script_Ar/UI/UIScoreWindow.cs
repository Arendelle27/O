using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreWindow : UIWindow
{
    [SerializeField, LabelText("存活天数"), Tooltip("重新开始游戏")]
    public Text roundnumber;

    private void OnEnable()
    {
        this.UpdateUI();
    }

    /// <summary>
    /// 更新UI
    /// </summary>
    void UpdateUI()
    {
        this.roundnumber.text = RoundManager.Instance.roundNumber.ToString();
    }

}
