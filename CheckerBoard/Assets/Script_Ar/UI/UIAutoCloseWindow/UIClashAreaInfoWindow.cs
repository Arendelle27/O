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
    [SerializeField, LabelText("CGͼƬ"), Tooltip("�¼�ͼƬ")]
    public Image image;
    [SerializeField, LabelText("��ͻ������"), Tooltip("�ó�ͻ������")]
    public Text title;
    [SerializeField, LabelText("��ͻ������"), Tooltip("�ó�ͻ��������")]
    public Text description;
    [SerializeField, LabelText("����ֵ��ʾ"), Tooltip("�������ֵ��ʾ�ı�")]
    public Text hotilityValue;

    [SerializeField, LabelText("�¼�����"), Tooltip("�����¼�����")]
    public List<Button> buttons;
    //0Ϊ���ף�1Ϊ����,�����Կ��¼�

    //[SerializeField, LabelText("���������ť"), Tooltip("���������ť")]
    //public Button destoryBuilding;

    private void Start()
    {
        //���װ���
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
        //StartCoroutine(BeSelected());//0.1���ѡ��
    }

    public void SetInfo(ClashArea clashArea)
    {
        //this.image.sprite = eventArea.SR.sprite;
        this.title.text = clashArea.plot.plotDefine.Name;
        this.description.text = clashArea.plot.plotDefine.Description;
        this.hotilityValue.text = EventAreaManager.Instance.hotility[int.Parse(clashArea.plot.plotDefine.EventValue)].ToString();
        //this.SetButton((int)eventArea.plot.plotDefine.EventType);//���ð���
        
    }


}
