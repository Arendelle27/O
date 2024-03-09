using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreWindow : UIWindow
{
    [SerializeField, LabelText("�������"), Tooltip("���¿�ʼ��Ϸ")]
    public Text roundnumber;

    private void OnEnable()
    {
        this.UpdateUI();
    }

    /// <summary>
    /// ����UI
    /// </summary>
    void UpdateUI()
    {
        this.roundnumber.text = RoundManager.Instance.roundNumber.ToString();
    }

}
