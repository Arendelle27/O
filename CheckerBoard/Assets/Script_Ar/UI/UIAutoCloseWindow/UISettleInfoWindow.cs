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
    [SerializeField, LabelText("CGͼƬ"), Tooltip("�¼�ͼƬ")]
    public Image image;
    [SerializeField, LabelText("��������"), Tooltip("�ý�������")]
    public Text title;
    [SerializeField, LabelText("��������"), Tooltip("�ý���������")]
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
            UITransactionWindow uTW = UIManager.Instance.Show<UITransactionWindow>();
            EventAreaManager.Instance.uITransactionWindow??= uTW;//��ʾ���׽���
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
        //this.SetButton((int)eventArea.plot.plotDefine.EventType);//���ð���

    }

    /// <summary>
    /// ���ݰ���
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
