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
    public class UIBuildingWindow : UISelectWindow
    {
        [SerializeField, LabelText("建筑列表"), Tooltip("收集建筑和生产建筑列表")]
        public TabView tabView;

        [SerializeField, LabelText("建筑信息"), Tooltip("放入建筑信息UI")]
        public UIBuildingItemInfo uiBuildingItemInfo;

        [SerializeField, LabelText("被选中的建筑UI类型"), ReadOnly]
        Building_Type buildingtypeSelected;//被选中的建筑UI类型

        [SerializeField, LabelText("自适应的窗口"), Tooltip("放入需要只适应的窗口")]
        public List<RectTransform> rectTransforms = new List<RectTransform>();

        private void Start()
        {
            tabView.OnTabSelect = this.OnTabSelect;
            foreach(var listView in this.tabView.tabPages)
            {
                listView.onItemSelected += this.OnBuildingItemSelected;
            }
            //MainThreadDispatcher.StartUpdateMicroCoroutine(BeSelected());

        }

        void OnEnable()
        {
            this.uiBuildingItemInfo.gameObject.SetActive(false);
            for(int i = 0; i < this.tabView.tabPages.Length; i++)
            {
                //this.tabView.tabPages[i].gameObject.SetActive(this.tabView.tabPages[i].items.Count != 0);
                this.tabView.tabButtons[i].gameObject.SetActive(this.tabView.tabPages[i].items.Count != 0);
            }
            foreach (var rectTransform in rectTransforms)//自适应窗口
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < this.tabView.tabButtons.Length; i++)
            {
                this.tabView.tabButtons[i].Select(false);
            }
            this.buildingtypeSelected=Building_Type.无;
        }

        /// <summary>
        /// 按下切换按键
        /// </summary>
        /// <param name="index"></param>
        void OnTabSelect(int index)
        {
            this.buildingtypeSelected=Building_Type.无;
            StartCoroutine( this.selectedWindow.BeSelected());
            foreach (var rectTransform in rectTransforms)//自适应窗口
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
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

                this.uiBuildingItemInfo.gameObject.SetActive(true);
                this.uiBuildingItemInfo.SetInfo(buildingItem.type);
            }
            else//第二次点击确认建造
            {
                Debug.LogFormat("建造建筑：{0}", buildingtypeSelected);

                if(BuildingManager.Instance.Build(buildingtypeSelected, WandererManager.Instance.wanderer.plot))
                {
                    this.selectedWindow.OnCloseClick();
                }
                
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
                go.transform.SetParent(this.tabView.tabPages[sort].content);//在建筑列表第一页生成
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(sort, buType);//设置建筑UIItem信息
                this.tabView.tabPages[sort].AddItem(ui);
            }
        }
    }
}
