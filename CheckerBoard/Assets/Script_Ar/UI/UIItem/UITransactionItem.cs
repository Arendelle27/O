using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UILIST;
using UnityEngine.UI;
using MANAGER;
using UniRx;

public class UITransactionItem : ListView.ListViewItem
{
    [SerializeField, LabelText("商品ID"), ReadOnly]
    public int id;

    [SerializeField, LabelText("商品名称"), Tooltip("放入商品名称")]
    public Text nameText;

    [SerializeField, LabelText("商品图标"), Tooltip("放入商品图标")]
    public Image iconImage;

    [SerializeField, LabelText("商品数量"), Tooltip("放入商品数量")]
    public Text countText;

    [SerializeField, LabelText("商品价格"), Tooltip("放入商品价格")]
    public Text priceText;

    [SerializeField, LabelText("交易按键"), Tooltip("放入交易按键")]
    public Button transactButton;

    private void Start()
    {
        this.transactButton.OnClickAsObservable().Subscribe(_ =>
        {
            this.OpenUITransactionAmount();
        });
    }

    /// <summary>
    /// 设置信息
    /// </summary>
    /// <param name="tD"></param>
    public virtual void SetInfo(TransactionDefine tD)
    {
        this.id = tD.ID;
        switch (tD.TransactionType)
        {
            case Transaction_Type.资源:
                this.nameText.text =((Resource_Type)tD.Subtype).ToString();
                this.iconImage.sprite = SpriteManager.buildingResourceSprites[(Resource_Type)tD.Subtype];
                break;
            case Transaction_Type.蓝图:
                this.nameText.text = string.Format("蓝图{0}", tD.Subtype);
                this.iconImage.sprite = SpriteManager.propSprites[Prop_Type.蓝图];
                break;
        }

    }

    /// <summary>
    /// 打开交易数量UI
    /// </summary>
    protected virtual void OpenUITransactionAmount()
    {

    }
}
