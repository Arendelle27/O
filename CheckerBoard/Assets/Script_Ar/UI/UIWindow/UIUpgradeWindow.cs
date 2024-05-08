using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeWindow : UIWindow
{
    [SerializeField, LabelText("升级文本"), Tooltip("显示升级文本")]
    public Text upgradeText;

    [SerializeField, LabelText("升级耗费金钱值"), Tooltip("显示升级耗费金钱值")]
    public Text upgradeCost;

    [SerializeField, LabelText("升级结果文本"), Tooltip("显示升级结果")]
    public Text upgradeResult;

    [SerializeField, LabelText("确认升级按钮"), Tooltip("升级按钮")]
    public Button upgradeButton;

    //[SerializeField, LabelText("扩展探索小队按钮"), Tooltip("点击扩展探索小队按钮")]
    //public Button extendExpTeamButton;

    //[SerializeField, LabelText("扩展探索小队数量显示"), Tooltip("显示扩展探索小队数量")]
    //public Text expTeamExtendAmountText;

    [SerializeField, LabelText("返回按钮"), Tooltip("返回按钮")]
    public Button closeButton;
    private void Start()
    {
        this.upgradeButton.OnClickAsObservable().Subscribe(_ =>
        {
            this.OnYesClick();
            Debug.Log("选择升级");
        });

        this.closeButton.OnClickAsObservable().Subscribe(_ =>
        {
            this.OnCloseClick();
        });

        //this.extendExpTeamButton.OnClickAsObservable().Subscribe(_ =>
        //{
        //    //打开扩展探索小队的UI界面
        //    PlotManager.Instance.EnterSelectExtendExpTeam(true);//进入选择扩展探索小队的模式

        //    this.OnCloseClick();
        //});

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
    /// 更新UI
    /// </summary>
    void UpdateUI()
    {
        if(DataManager.UpgradePointCostDefines.ContainsKey(CapabilityManager.Instance.upgradePointHaveBuy))
        {
            this.upgradeText.text = "是否购买，\r\n需要消耗金钱:";

            this.upgradeCost.text = (DataManager.UpgradePointCostDefines[CapabilityManager.Instance.upgradePointHaveBuy]).BuyNextCost.ToString();

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
            this.upgradeText.text = "已经没有可以购买的点数了";

            if (this.upgradeCost.gameObject.activeSelf)
            {
                this.upgradeCost.gameObject.SetActive(false);
            }

            if (this.upgradeButton.gameObject.activeSelf)
            {
                this.upgradeButton.gameObject.SetActive(false);
            }
        }

        //if(ResourcesManager.Instance.levelPromptionAmount>0)
        //{
        //    this.expTeamExtendAmountText.text = ResourcesManager.Instance.levelPromptionAmount.ToString();

        //    if(this.closeButton.gameObject.activeSelf)
        //    {
        //        this.closeButton.gameObject.SetActive(false);
        //    }
        //    if(!this.extendExpTeamButton.gameObject.activeSelf)
        //    {
        //        this.extendExpTeamButton.gameObject.SetActive(true);
        //    }
        //}
        //else
        //{
        //    if (!this.closeButton.gameObject.activeSelf)
        //    {
        //        this.closeButton.gameObject.SetActive(true);
        //    }
        //    if (this.extendExpTeamButton.gameObject.activeSelf)
        //    {
        //        this.extendExpTeamButton.gameObject.SetActive(false);
        //    }
        //}
    }

    /// <summary>
    /// 确认升级
    /// </summary>
    public override void OnYesClick()
    {
        if(CapabilityManager.Instance.BuyUpgradePoint())
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
    /// 显示升级结果
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
            this.upgradeResult.text = "升级成功";

        }
        else
        {
            this.upgradeResult.text = "空间币不足不够";
        }
        this.upgradeResult.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        this.upgradeResult.gameObject.SetActive(false);
    }

}
