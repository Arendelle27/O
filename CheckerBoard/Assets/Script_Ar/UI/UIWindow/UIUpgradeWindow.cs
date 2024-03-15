using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeWindow : UIWindow
{
    [SerializeField, LabelText("�����ı�"), Tooltip("��ʾ�����ı�")]
    public Text upgradeText;

    [SerializeField, LabelText("�����ķѽ�Ǯֵ"), Tooltip("��ʾ�����ķѽ�Ǯֵ")]
    public Text upgradeCost;

    [SerializeField, LabelText("��������ı�"), Tooltip("��ʾ�������")]
    public Text upgradeResult;

    [SerializeField, LabelText("ȷ��������ť"), Tooltip("������ť")]
    public Button upgradeButton;

    [SerializeField, LabelText("��չ̽��С�Ӱ�ť"), Tooltip("�����չ̽��С�Ӱ�ť")]
    public Button extendExpTeamButton;

    [SerializeField, LabelText("��չ̽��С��������ʾ"), Tooltip("��ʾ��չ̽��С������")]
    public Text expTeamExtendAmountText;

    [SerializeField, LabelText("���ذ�ť"), Tooltip("���ذ�ť")]
    public Button closeButton;
    private void Start()
    {
        this.upgradeButton.OnClickAsObservable().Subscribe(_ =>
        {
            this.OnYesClick();
            Debug.Log("ѡ������");
        });

        this.closeButton.OnClickAsObservable().Subscribe(_ =>
        {
            this.OnCloseClick();
        });

        this.extendExpTeamButton.OnClickAsObservable().Subscribe(_ =>
        {
            //����չ̽��С�ӵ�UI����
            UIMain.Instance.ChangeToGamePanel(2);
            PlotManager.Instance.EnterSelectExtendExpTeam(true);//����ѡ����չ̽��С�ӵ�ģʽ

            this.OnCloseClick();
        });

    }

    private void OnEnable()
    {
        this.UpdateUI();

       if(this.upgradeResult.gameObject.activeSelf)
        {
            this.upgradeResult.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ����UI
    /// </summary>
    void UpdateUI()
    {
        if(WandererManager.Instance.wanderer?.level <10)
        {
            this.upgradeText.text = "�Ƿ�Ҫ������\r\n����������Ҫ���Ľ�Ǯ:";

            this.upgradeCost.text = (WandererManager.Instance.wanderer!.level * 10).ToString();

            if (!this.upgradeCost.gameObject.activeSelf)
            {
                this.upgradeCost.gameObject.SetActive(true);
            }

            if (!this.upgradeButton.gameObject.activeSelf)
            {
                this.upgradeButton.gameObject.SetActive(true);
            }
        }
        else
        {
            this.upgradeText.text = "�Ѿ�������";

            if (this.upgradeCost.gameObject.activeSelf)
            {
                this.upgradeCost.gameObject.SetActive(false);
            }

            if (this.upgradeButton.gameObject.activeSelf)
            {
                this.upgradeButton.gameObject.SetActive(false);
            }
        }

        if(DataManager.Instance.levelPromptionAmount>0)
        {
            this.expTeamExtendAmountText.text = DataManager.Instance.levelPromptionAmount.ToString();

            if(this.closeButton.gameObject.activeSelf)
            {
                this.closeButton.gameObject.SetActive(false);
            }
            if(!this.extendExpTeamButton.gameObject.activeSelf)
            {
                this.extendExpTeamButton.gameObject.SetActive(true);
            }
        }
        else
        {
            if (!this.closeButton.gameObject.activeSelf)
            {
                this.closeButton.gameObject.SetActive(true);
            }
            if (this.extendExpTeamButton.gameObject.activeSelf)
            {
                this.extendExpTeamButton.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// ȷ������
    /// </summary>
    public override void OnYesClick()
    {
        if(DataManager.Instance.CanUpgrade())
        {
            this.UpdateUI();

            this.ShowUpgradeResult(true);
        }
        else
        {
            this.ShowUpgradeResult(false);
        }
    }

    /// <summary>
    /// ��ʾ�������
    /// </summary>
    /// <param name="isSuccess"></param>
    void ShowUpgradeResult(bool isSuccess)
    {
        if(this.showUpgradeResultCor!=null)
        {
            StopCoroutine(this.showUpgradeResultCor);
        }
        this.showUpgradeResultCor = StartCoroutine(this.ShowUpgradeResultCor(isSuccess));
    }

    Coroutine showUpgradeResultCor;
    IEnumerator ShowUpgradeResultCor(bool isSuccess)
    {
        if (isSuccess)
        {
            this.upgradeResult.color = Color.green;
            this.upgradeResult.text = "�����ɹ�!";

        }
        else
        {
            this.upgradeResult.color = Color.red;
            this.upgradeResult.text = "����ʧ��,Ǯ����www";
        }
        this.upgradeResult.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        this.upgradeResult.gameObject.SetActive(false);
    }

}