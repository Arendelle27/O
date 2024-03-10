using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIExtendExpTeamPanel : UIPanel
{
    [SerializeField, LabelText("��ѡ������"), Tooltip("��ʾʣ���ѡ������")]
    public Text selectPlotAmount;

    [SerializeField, LabelText("��ɰ���"), Tooltip("���̽��С�ӵ�����")]
    public Button finishgrade;

    [SerializeField, LabelText("��������"), Tooltip("�����ղŵ�ѡ��")]
    public Button withdrawgrade;

    private void Start()
    {
        this.finishgrade.OnClickAsObservable().Subscribe(_ =>
        {
            //���̽��С�ӵ�����
            if (PlotManager.Instance.map_Mode == Map_Mode.��չ̽��С��)
            {
                WandererManager.Instance.exploredV2.Clear();

                PlotManager.Instance.EnterSelectExtendExpTeam(false);//����ѡ����չ̽��С�ӵ�ģʽ
                UIMain.Instance.ChangeToGamePanel(1);//�ָ�����Ϸ����
            }
        });

        this.withdrawgrade.OnClickAsObservable().Subscribe(_ =>
        {
            //�����ղŵ�ѡ��
            WandererManager.Instance.WithdrawExpTeam();
        });
    }



    /// <summary>
    /// ����UI
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
