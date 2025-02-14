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
            //this.gameObject.SetActive(false);
            //this.gameObject.SetActive(true);
            //MainThreadDispatcher.StartUpdateMicroCoroutine(BeSelected());

        }

        void OnEnable()
        {
            if (WandererManager.Instance.wanderer == null)
            {
                return;

            }
            this.uiBuildingItemInfo.gameObject.SetActive(false);

            bool canBuildGathering = true;
            Plot plot = WandererManager.Instance.wanderer.plot;
            if (plot.plotType==1)
            {
                canBuildGathering = false;
            }
            else
            {
                if (plot.buildingResources[0]==-1)
                {
                    canBuildGathering = false;
                }
                else if (plot.buildingResources[0]==0&& this.tabView.tabPages[0].items.Count==0)
                {
                    canBuildGathering = false;
                }
                else if(plot.buildingResources[0] == 1 && this.tabView.tabPages[3].items.Count == 0)
                {
                    canBuildGathering = false;
                }
                else if (plot.buildingResources[0] == 2 && this.tabView.tabPages[4].items.Count == 0)
                {
                    canBuildGathering = false;
                }
            }
            for(int i = 0; i < this.tabView.tabButtons.Length; i++)
            {
                if(i==0)
                {
                    this.tabView.tabButtons[0].gameObject.SetActive(canBuildGathering);
                }
                else
                {
                    this.tabView.tabButtons[i].gameObject.SetActive(this.tabView.tabPages[i].items.Count != 0);
                }
            }
            foreach (var rectTransform in rectTransforms)//自适应窗口
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
            this.tabView.tabButtons[1].OnClick();//生产建筑
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
                    if (NoviceGuideManager.Instance.isGuideStage[3])//是否处于新手指引阶段
                    {
                        NoviceGuideManager.Instance.NoviceGuideStage++;
                    }
                    this.selectedWindow.OnCloseClick();
                }
                
            }
        }

        /// <summary>
        /// 清空建筑列表UI
        /// </summary>
        void ClearBuildingList(int sort)
        {
            if (sort == 0)
            {
                this.tabView.tabPages[3].RemoveAll();
                this.tabView.tabPages[4].RemoveAll();
            }
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
                int buildingSort = sort;
                if (sort == 0)
                {
                    if (buType <= Building_Type.电缆卷机)
                    {
                        buildingSort = 0;
                    }
                    else if (buType <= Building_Type.金属提纯器)
                    {
                        buildingSort = 3;
                    }
                    else
                    {
                        buildingSort = 4;
                    }
                }

                go.transform.SetParent(this.tabView.tabPages[buildingSort].content);//在建筑列表第一页生成
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(sort, buType);//设置建筑UIItem信息
                this.tabView.tabPages[buildingSort].AddItem(ui);
            }
        }
    }
}
