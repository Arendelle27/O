using Managers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISelectedWindow : UIWindow, IDeselectHandler
{
    [SerializeField, LabelText("��ѡ�����Զ��ر�"), Tooltip("��ѡ�����Զ��رյ�UI����")]
    public List<UISelectWindow> uISelectedWindows = new List<UISelectWindow>();
    //0Ϊ����ѡ��1Ϊ������Ϣ��2Ϊ�¼���Ϣ

    //public Dictionary<string,UISelectWindow> uISelectedWindows=new Dictionary<string, UISelectWindow>()
    //{
    //    {"UIBuildingWindow",null },
    //    {"UIBuildingInfoWindow",null },
    //    {"UIEventAreaInfoWindow",null }
    //};
    //0Ϊ����ѡ��1Ϊ������Ϣ��2Ϊ�¼���Ϣ

    void Start()
    {
        foreach (var window in this.uISelectedWindows)
        {
            window.selectedWindow = this;
        }

        this.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        StartCoroutine(BeSelected());//0.1���ѡ��
    }

    private void OnDisable()
    {
        foreach (var item in this.uISelectedWindows)
        {
            item.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// �򿪶�Ӧ����
    /// </summary>
    /// <param name="windowName"></param>
    public void OpenWindow(int id)
    {
        this.uISelectedWindows[id].gameObject.SetActive(true);
    }

    #region ʵ�ֹرս���UIѡ�����
    /// <summary>
    /// 0.1���ѡ��
    /// </summary>
    /// <returns></returns>
    public IEnumerator BeSelected()
    {
        yield return new WaitForSeconds(0.01f);
        this.GetComponent<Selectable>().Select();
    }

    /// <summary>
    /// δѡ�н���UIʱ�رս���UIѡ�����
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDeselect(BaseEventData eventData)
    {
        var ed = eventData as PointerEventData;
        if (ed.hovered.Contains(this.gameObject))
        {
            return;
        }
        this.OnCloseClick();
    }
    #endregion
}