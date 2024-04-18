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
    [SerializeField, LabelText("��ƷID"), ReadOnly]
    public int id;

    [SerializeField, LabelText("��Ʒ����"), Tooltip("������Ʒ����")]
    public Text nameText;

    [SerializeField, LabelText("��Ʒͼ��"), Tooltip("������Ʒͼ��")]
    public Image iconImage;

    [SerializeField, LabelText("��Ʒ����"), Tooltip("������Ʒ����")]
    public Text countText;

    [SerializeField, LabelText("��Ʒ�۸�"), Tooltip("������Ʒ�۸�")]
    public Text priceText;

    [SerializeField, LabelText("���װ���"), Tooltip("���뽻�װ���")]
    public Button transactButton;

    private void Start()
    {
        this.transactButton.OnClickAsObservable().Subscribe(_ =>
        {
            this.OpenUITransactionAmount();
        });
    }

    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="tD"></param>
    public virtual void SetInfo(TransactionDefine tD)
    {
        this.id = tD.ID;
        switch (tD.TransactionType)
        {
            case Transaction_Type.��Դ:
                this.nameText.text =((Resource_Type)tD.Subtype).ToString();
                this.iconImage.sprite = SpriteManager.buildingResourceSprites[(Resource_Type)tD.Subtype];
                break;
            case Transaction_Type.��ͼ:
                this.nameText.text = string.Format("��ͼ{0}", tD.Subtype);
                this.iconImage.sprite = SpriteManager.propSprites[Prop_Type.��ͼ];
                break;
        }

    }

    /// <summary>
    /// �򿪽�������UI
    /// </summary>
    protected virtual void OpenUITransactionAmount()
    {

    }
}
