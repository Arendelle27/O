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
    [SerializeField, LabelText("������ѡ���б�"), Tooltip("��ʾ������ѡ��")]
    public ListView strengthCapabilityList;

    [SerializeField, LabelText("���������ı�"), Tooltip("��ʾ���������ı�")]
    public Text boostPOintsText;

    [SerializeField, LabelText("������ť"), Tooltip("������ť")]
    public Button upgadeButton;

    [SerializeField, LabelText("�رհ�ť"), Tooltip("�رհ�ť")]
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

            gO.transform.SetParent(this.strengthCapabilityList.content);//�ڽ����б��һҳ����
            var ui = gO.GetComponent<UIStrengthenCapabilityItem>();
            ui.SetInfo(i);//���ý���UIItem��Ϣ
            this.strengthCapabilityList.AddItem(ui);
        }

        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        this.UpdateUpgradePointHaveBuy();
    }

    /// <summary>
    /// ��ʾ��������,������������ˣ��رչ��򰴼�
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
    ///// ��������
    ///// </summary>
    ///// <param name="item"></param>
    //public void StrengthCapability(ListView.ListViewItem item)
    //{
    //    UIStrengthenCapabilityItem ui = item as UIStrengthenCapabilityItem;
    //}

    /// <summary>
    /// ����ǿ������ѡ����Ϣ
    /// </summary>
    /// <param name="sort"></param>
    public void UpdateStrengthenCapabilityItemInfo(int sort)
    {
        (this.strengthCapabilityList.items[sort] as UIStrengthenCapabilityItem).SetInfo();
    }

}
