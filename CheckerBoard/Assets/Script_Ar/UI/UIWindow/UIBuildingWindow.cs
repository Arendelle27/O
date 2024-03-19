using ENTITY;
using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UILIST;
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
            foreach(var listView in this.tabView.tabPages)
            {
                listView.onItemSelected += this.OnBuildingItemSelected;
            }
            this.ClearBuildingList();
            this.InitBuildingList();
        }

        private void OnEnable()
        {
            StartCoroutine(BeSelected());
        }

        private void OnDisable()
        {
            this.buildingtypeSelected=Building_Type.无;
        }


        /// <summary>
        /// 选中想要建造的建筑,第二次点击后确认建造
        /// </summary>
        /// <param name="item"></param>
        public void OnBuildingItemSelected(ListView.ListViewItem item)
        {
            UIBuildingItem buildingItem = item as UIBuildingItem;
            if (this.buildingtypeSelected != buildingItem.type) 
            {
                this.buildingtypeSelected = buildingItem.type;
            }
            else
            {
                Debug.LogFormat("建造建筑：{0}", buildingtypeSelected);
                BuildingManager.Instance.Build(buildingtypeSelected, PlotManager.Instance.SelectedPlot);
            }
        }

        /// <summary>
        /// 清空建筑列表UI
        /// </summary>
        void ClearBuildingList()
        {
            foreach (var listView in this.tabView.tabPages)
            {
                listView.RemoveAll();
            }
        }

        /// <summary>
        /// 初始化建筑列表UI
        /// </summary>
        void InitBuildingList()
        {
            foreach(var buType in BuildingManager.Instance.GatheringTypes)
            {
                GameObject go = GameObjectPool.Instance.UIBuildingItems.Get();
                go.transform.parent = this.tabView.tabPages[0].transform;//在建筑列表第一页生成
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(0,buType);//设置建筑UIItem信息
                this.tabView.tabPages[0].AddItem(ui);
            }

            foreach (var buType in BuildingManager.Instance.ProductionTypes)
            {
                GameObject go = GameObjectPool.Instance.UIBuildingItems.Get();
                go.transform.parent = this.tabView.tabPages[1].transform;//在建筑列表第二页生成
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(1, buType);//设置建筑UIItem信息
                this.tabView.tabPages[1].AddItem(ui);
            }

            foreach (var buType in BuildingManager.Instance.BattleTypes)
            {
                GameObject go = GameObjectPool.Instance.UIBuildingItems.Get();
                go.transform.parent = this.tabView.tabPages[2].transform;//在建筑列表第二页生成
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(2,buType);//设置建筑UIItem信息
                this.tabView.tabPages[1].AddItem(ui);
            }
        }

        #region 实现关闭建筑UI选择界面
        /// <summary>
        /// 0.1秒后被选中
        /// </summary>
        /// <returns></returns>
        IEnumerator BeSelected()
        {
            yield return new WaitForSeconds(0.1f);
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
