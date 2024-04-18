using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UILIST;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIStrengthenCapabilityWindow : UIWindow
{
    [SerializeField, LabelText("可升级选项列表"), Tooltip("显示可升级选项")]
    public ListView strengthCapabilityList;

    [SerializeField, LabelText("升级点数文本"), Tooltip("显示升级点数文本")]
    public Text boostPOintsText;

    [SerializeField, LabelText("升级按钮"), Tooltip("升级按钮")]
    public Button upgadeButton;

    [SerializeField, LabelText("关闭按钮"), Tooltip("关闭按钮")]
    public Button closeButton;


    private void Start()
    {
        this.closeButton.OnClickAsObservable().Subscribe(_ =>
        {
            this.OnCloseClick();
        });

        this.upgadeButton.OnClickAsObservable().Subscribe(_ =>
        {
            UIManager.Instance.Show<UIUpgradeWindow>();
        });

        //strengthCapabilityList.onItemSelected += this.StrengthCapability;
        //MainThreadDispatcher.StartUpdateMicroCoroutine(BeSelected());

        for (int i= 0;i< CapabilityManager.Instance.curLevels.Count;i++)
        {
            GameObject gO = GameObjectPool.Instance.UIStrengthenCapabilityItems.Get();

            gO.transform.SetParent(this.strengthCapabilityList.content);//在建筑列表第一页生成
            var ui = gO.GetComponent<UIStrengthenCapabilityItem>();
            ui.SetInfo(i);//设置建筑UIItem信息
            this.strengthCapabilityList.AddItem(ui);
        }

        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        this.UpdateUpgradePointHaveBuy();
    }

    /// <summary>
    /// 显示升级点数,如果升级买满了，关闭购买按键
    /// </summary>
    public void UpdateUpgradePointHaveBuy()
    {
        this.boostPOintsText.text = CapabilityManager.Instance.upgradePoint.ToString();
        if (DataManager.UpgradePointCostDefines.ContainsKey(CapabilityManager.Instance.upgradePointHaveBuy))
        {
            this.upgadeButton.gameObject.SetActive(true);
        }
        else
        {
            this.upgadeButton.gameObject.SetActive(false);
        }
    }

    ///// <summary>
    ///// 提升能力
    ///// </summary>
    ///// <param name="item"></param>
    //public void StrengthCapability(ListView.ListViewItem item)
    //{
    //    UIStrengthenCapabilityItem ui = item as UIStrengthenCapabilityItem;
    //}

    /// <summary>
    /// 更新强化能力选项信息
    /// </summary>
    /// <param name="sort"></param>
    public void UpdateStrengthenCapabilityItemInfo(int sort)
    {
        (this.strengthCapabilityList.items[sort] as UIStrengthenCapabilityItem).SetInfo();
    }

}
