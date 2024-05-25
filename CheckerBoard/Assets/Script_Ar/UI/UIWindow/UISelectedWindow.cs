using MANAGER;
using Managers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISelectedWindow : UIWindow, IDeselectHandler, ISelectHandler
{
    [SerializeField, LabelText("不选择则自动关闭"), Tooltip("不选择则自动关闭的UI窗口")]
    public List<UISelectWindow> uISelectedWindows = new List<UISelectWindow>();
    //0为建筑选择，1为建筑信息，2为事件信息,3为冲突区信息,4为资源区信息

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
        StartCoroutine(BeSelected());//0.1秒后被选中
    }

    private void OnDisable()
    {
        foreach (var item in this.uISelectedWindows)
        {
            item.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 打开对应窗口
    /// </summary>
    /// <param name="windowName"></param>
    public void OpenWindow(int id)
    {
        this.uISelectedWindows[id].gameObject.SetActive(true);
    }

    #region 实现关闭建筑UI选择界面
    /// <summary>
    /// 0.1秒后被选中
    /// </summary>
    /// <returns></returns>
    public IEnumerator BeSelected()
    {
        yield return new WaitForSeconds(0.01f);
        this.GetComponent<Selectable>().Select();
    }

    public void OnSelect(BaseEventData eventData)
    {

    }

    /// <summary>
    /// 未选中建筑UI时关闭建筑UI选择界面
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDeselect(BaseEventData eventData)
    {
        var ed = eventData as PointerEventData;

        //if(ed==null)
        //{
        //    return;
        //}
        if (NoviceGuideManager.Instance.NoviceGuideStage>-1)//是否处于新手指引阶段
        {
            return;
        }
        if (ed == null||ed.hovered.Contains(this.gameObject))
        {
            StartCoroutine(BeSelected());//0.1秒后被选中
            return;
        }
        this.OnCloseClick();
    }



    #endregion
}
