using ENTITY;
using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UILIST;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIBUILDING
{
    public class UIBuildingWindow : UIWindow, IDeselectHandler
    {
        [SerializeField, LabelText("建筑列表"), Tooltip("收集建筑和生产建筑列表")]
        public TabView tabView;

        [SerializeField, LabelText("被选中的建筑UI类型"), ReadOnly]
        Building_Type buildingtypeSelected;//被选中的建筑UI类型


        private void Start()
        {
            tabView.OnTabSelect = this.OnTabSelect;
            foreach(var listView in this.tabView.tabPages)
            {
                listView.onItemSelected += this.OnBuildingItemSelected;
            }
            //MainThreadDispatcher.StartUpdateMicroCoroutine(BeSelected());

            this.OnCloseClick();
        }

        void OnEnable()
        {
            StartCoroutine(BeSelected());
        }

        /// <summary>
        /// 按下q切换按键
        /// </summary>
        /// <param name="index"></param>
        void OnTabSelect(int index)
        {
            StartCoroutine(BeSelected());
        }

        /// <summary>
        /// 选中想要建造的建筑,第二次点击后确认建造
        /// </summary>
        /// <param name="item"></param>
        public void OnBuildingItemSelected(ListView.ListViewItem item)
        {
            UIBuildingItem buildingItem = item as UIBuildingItem;
            if (this.buildingtypeSelected != buildingItem.type)//第一次点击选中
            {
                this.buildingtypeSelected = buildingItem.type;
            }
            else//第二次点击确认建造
            {
                Debug.LogFormat("建造建筑：{0}", buildingtypeSelected);
                BuildingManager.Instance.Build(buildingtypeSelected, PlotManager.Instance.SelectedPlot);
            }
        }

        /// <summary>
        /// 清空建筑列表UI
        /// </summary>
        void ClearBuildingList(int sort)
        {
            this.tabView.tabPages[sort].RemoveAll();
        }

        /// <summary>
        /// 初始化建筑列表UI
        /// </summary>
        public void UpdateBuildingList(int sort)
        {
            ClearBuildingList(sort);
            foreach (var buType in BuildingManager.Instance.buildingTypes[sort])
            {
                GameObject go = GameObjectPool.Instance.UIBuildingItems.Get();
                go.transform.SetParent(this.tabView.tabPages[sort].transform);//在建筑列表第一页生成
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(sort, buType);//设置建筑UIItem信息
                this.tabView.tabPages[sort].AddItem(ui);
            }
        }

        #region 实现关闭建筑UI选择界面
        /// <summary>
        /// 0.1秒后被选中
        /// </summary>
        /// <returns></returns>
        IEnumerator BeSelected()
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
}
