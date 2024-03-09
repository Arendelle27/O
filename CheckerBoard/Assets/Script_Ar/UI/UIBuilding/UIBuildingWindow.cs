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
        //����UIͼ��
        UIBuildingSprites define;
        [SerializeField, LabelText("����UIԤ����"),ReadOnly]
        public GameObject itemPrefab;

        [SerializeField, LabelText("�����б�"), Tooltip("�ռ����������������б�")]
        public TabView tabView;

        [SerializeField, LabelText("��ѡ�еĽ���UI����"), ReadOnly]
        Building_Type buildingtypeSelected;//��ѡ�еĽ���UI����

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
            this.buildingtypeSelected=Building_Type.��;
        }

        /// <summary>
        /// ѡ����Ҫ����Ľ���,�ڶ��ε����ȷ�Ͻ���
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
                Debug.LogFormat("���콨����{0}", buildingtypeSelected);
                BuildingManager.Instance.GetBuilding(buildingtypeSelected, PlotManager.Instance.SelectedPlot);
            }
        }

        /// <summary>
        /// ��ս����б�UI
        /// </summary>
        void ClearBuildingList()
        {
            foreach (var listView in this.tabView.tabPages)
            {
                listView.RemoveAll();
            }
        }

        /// <summary>
        /// ��ʼ�������б�UI
        /// </summary>
        void InitBuildingList()
        {
            foreach(var buType in BuildingManager.Instance.GatheringTypes)
            {
                GameObject go = Instantiate(this.itemPrefab.gameObject, this.tabView.tabPages[0].transform);//�ڽ����б��һҳ����
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(buType, this.define.sprites[(int)buType]);//���ý���UIItem��Ϣ
                this.tabView.tabPages[0].AddItem(ui);
            }

            foreach (var buType in BuildingManager.Instance.ProductionTypes)
            {
                GameObject go = Instantiate(this.itemPrefab.gameObject, this.tabView.tabPages[1].transform);//�ڽ����б�ڶ�ҳ����
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(buType, this.define.sprites[(int)buType]);//���ý���UIItem��Ϣ
                this.tabView.tabPages[1].AddItem(ui);
            }
        }
    }
}
