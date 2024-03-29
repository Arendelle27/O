using Managers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISelectedWindow : UIWindow, IDeselectHandler
{
    [SerializeField, LabelText("不选择则自动关闭"), Tooltip("不选择则自动关闭的UI窗口")]
    public List<UISelectWindow> uISelectedWindows = new List<UISelectWindow>();
    //0为建筑选择，1为建筑信息，2为事件信息

    //public Dictionary<string,UISelectWindow> uISelectedWindows=new Dictionary<string, UISelectWindow>()
    //{
    //    {"UIBuildingWindow",null },
    //    {"UIBuildingInfoWindow",null },
    //    {"UIEventAreaInfoWindow",null }
    //};
    //0为建筑选择，1为建筑信息，2为事件信息

    void Start()
    {
        //UISelectWindow uSW = UIManager.Instance.Show<UIBuildingWindow>();
        //uSW.selectedWindow = this;
        //this.uISelectedWindows["UIBuildingWindow"]= uSW;
        ////uSW.transform.position=uSW.transform.position+this.transform.position;
        //uSW.transform.SetParent(this.transform);
        //uSW.gameObject. .RectTransform.localPosition = uSW.transform.position;


        //uSW = UIManager.Instance.Show<UIBuildingInfoWindow>();
        //uSW.selectedWindow = this;
        //this.uISelectedWindows["UIBuildingInfoWindow"] = uSW;
        ////uSW.transform.position = uSW.transform.position + this.transform.position;
        //uSW.transform.SetParent(this.transform);
        //uSW.transform.localPosition = uSW.transform.position;


        //uSW = UIManager.Instance.Show<UIEventAreaInfoWindow>();
        //uSW.selectedWindow = this;
        //this.uISelectedWindows["UIEventAreaInfoWindow"] = uSW;
        ////uSW.transform.position = uSW.transform.position + this.transform.position;
        //uSW.transform.SetParent(this.transform);
        //uSW.transform.localPosition = uSW.transform.position;
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

    /// <summary>
    /// 未选中建筑UI时关闭建筑UI选择界面
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
