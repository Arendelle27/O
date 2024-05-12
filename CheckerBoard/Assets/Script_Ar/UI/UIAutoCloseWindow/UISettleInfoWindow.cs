using ENTITY;
using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using Managers;
using Unity.VisualScripting;

public class UISettleInfoWindow : UISelectWindow
{
    [SerializeField, LabelText("CG图片"), Tooltip("事件图片")]
    public Image image;
    [SerializeField, LabelText("建筑名称"), Tooltip("该建筑名称")]
    public Text title;
    [SerializeField, LabelText("建筑描述"), Tooltip("该建筑的描述")]
    public Text description;
    [SerializeField, LabelText("敌意值显示"), Tooltip("放入敌意值显示文本")]
    public Text hotilityValue;

    [SerializeField, LabelText("事件按键"), Tooltip("所有事件按键")]
    public List<Button> buttons;
    //0为交易，1为挑衅,触发对抗事件

    //[SerializeField, LabelText("拆除建筑按钮"), Tooltip("拆除建筑按钮")]
    //public Button destoryBuilding;

    private void Start()
    {
        //交易按键
        this.buttons[0].OnClickAsObservable().Subscribe(_ =>
        {
            UITransactionWindow uTW = UIManager.Instance.Show<UITransactionWindow>();
            EventAreaManager.Instance.uITransactionWindow??= uTW;//显示交易界面
            //this.selectedWindow.OnCloseClick();
        });

        this.buttons[1].OnClickAsObservable().Subscribe(_ =>
        {
            if (!ResourcesManager.Instance.CanCopyConfront())
            {
                return;
            }
            EventManager.Instance.SetConfrontEvent(0, EventAreaManager.Instance.hotility[0]);
            //this.selectedWindow.OnCloseClick();
        });

    }

    private void OnEnable()
    {
        if (EventAreaManager.Instance.selectedEventArea != null)
            this.SetInfo(EventAreaManager.Instance.selectedEventArea);
    }

    public void SetInfo(EventArea eventArea)
    {
        //this.image.sprite = eventArea.SR.sprite;
        this.image.sprite = SpriteManager.plotSprites[eventArea.plot.plotDefine.Name];
        this.title.text = eventArea.plot.plotDefine.Name;
        this.description.text = eventArea.plot.plotDefine.Description;

        this.hotilityValue.text = EventAreaManager.Instance.hotility[int.Parse( eventArea.plot.plotDefine.EventValue)].ToString(); 
        //this.SetButton((int)eventArea.plot.plotDefine.EventType);//设置按键

    }

    /// <summary>
    /// 根据按键
    /// </summary>
    /// <param name="sort"></param>
    void SetButton(int sort)
    {
        for (int i=0;i<this.buttons.Count;i++)
        {
            buttons[i].gameObject.SetActive(sort==i);
        }
    }

}
