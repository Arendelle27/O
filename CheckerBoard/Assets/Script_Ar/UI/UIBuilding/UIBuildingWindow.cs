using ENTITY;
using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UILIST;
using UnityEngine;

namespace UIBUILDING
{
    public class UIBuildingWindow : UIWindow
    {
        //建筑UI图像
        UIBuildingSprites define;
        [SerializeField, LabelText("建筑UI预制体"),ReadOnly]
        public GameObject itemPrefab;

        [SerializeField, LabelText("建筑列表"), Tooltip("收集建筑和生产建筑列表")]
        public TabView tabView;

        [SerializeField, LabelText("被选中的建筑UI类型"), ReadOnly]
        Building_Type buildingtypeSelected;//被选中的建筑UI类型

        private void Awake()
        {
            this.define = new UIBuildingSprites();
            this.itemPrefab =  Resources.Load<GameObject>(PathConfig.UI_BuildingItem_Prefab_Path);
        }

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
                BuildingManager.Instance.GetBuilding(buildingtypeSelected, PlotManager.Instance.SelectedPlot);
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
                GameObject go = Instantiate(this.itemPrefab.gameObject, this.tabView.tabPages[0].transform);//在建筑列表第一页生成
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(buType, this.define.sprites[(int)buType]);//设置建筑UIItem信息
                this.tabView.tabPages[0].AddItem(ui);
            }

            foreach (var buType in BuildingManager.Instance.ProductionTypes)
            {
                GameObject go = Instantiate(this.itemPrefab.gameObject, this.tabView.tabPages[1].transform);//在建筑列表第二页生成
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(buType, this.define.sprites[(int)buType]);//设置建筑UIItem信息
                this.tabView.tabPages[1].AddItem(ui);
            }
        }
    }
}
