using ENTITY;
using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIBUILDING
{
    public class UIBuilding : UIWindow
    {
        //建筑UI图像
        UIBuildingSprites define;
        [SerializeField, LabelText("建筑UI预制体"),ReadOnly]
        public GameObject itemPrefab;

        [SerializeField, LabelText("建筑列表"), Tooltip("收集建筑和生产建筑列表")]
        public TabView tabView;

        [SerializeField, LabelText("被选中的建筑UI"), ReadOnly]
        UIBuildingItem buildingItemSelected;//被选中的建筑UI

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
            this.buildingItemSelected=null;
        }

        /// <summary>
        /// 选中想要建造的建筑
        /// </summary>
        /// <param name="item"></param>
        void OnBuildingItemSelected(ListView.ListViewItem item)
        {
            UIBuildingItem buildingItem = item as UIBuildingItem;
            if (buildingItem != null) 
            {
                this.buildingItemSelected = buildingItem;
            }
            else
            {
                Debug.LogFormat("建造建筑：{0}", buildingItem.type);
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
                ui.SetInfo(buType, this.define.sprites[(int)buType-1]);//设置建筑UIItem信息
                this.tabView.tabPages[0].AddItem(ui);
            }

            foreach (var buType in BuildingManager.Instance.ProductionTypes)
            {
                GameObject go = Instantiate(this.itemPrefab.gameObject, this.tabView.tabPages[1].transform);//在建筑列表第二页生成
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(buType, this.define.sprites[(int)buType - 1]);//设置建筑UIItem信息
                this.tabView.tabPages[1].AddItem(ui);
            }
        }
    }
}
