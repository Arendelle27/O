using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;


public class UIClashAreaInfoWindow : UISelectWindow
{
    [SerializeField, LabelText("CG图片"), Tooltip("事件图片")]
    public Image image;
    [SerializeField, LabelText("冲突区名称"), Tooltip("该冲突区名称")]
    public Text title;
    [SerializeField, LabelText("冲突区描述"), Tooltip("该冲突区的描述")]
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
            PlotDefine pD=EventManager.Instance.curClashArea.plot.plotDefine;
            EventManager.Instance.SetConfrontEvent(int.Parse(pD.EventValue),EventAreaManager.Instance.hotility[int.Parse(pD.EventValue)]+500f, EventManager.Instance.curClashArea);
            this.selectedWindow.OnCloseClick();
        });

    }

    private void OnEnable()
    {
        if (EventManager.Instance.curClashArea != null)
            this.SetInfo(EventManager.Instance.curClashArea);
        //StartCoroutine(BeSelected());//0.1秒后被选中
    }

    public void SetInfo(ClashArea clashArea)
    {
        //this.image.sprite = eventArea.SR.sprite;
        this.title.text = clashArea.plot.plotDefine.Name;
        this.description.text = clashArea.plot.plotDefine.Description;
        this.hotilityValue.text = EventAreaManager.Instance.hotility[int.Parse(clashArea.plot.plotDefine.EventValue)].ToString();
        //this.SetButton((int)eventArea.plot.plotDefine.EventType);//设置按键
        
    }


}
