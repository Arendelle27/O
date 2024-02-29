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
        //����UIͼ��
        UIBuildingSprites define;
        [SerializeField, LabelText("����UIԤ����"),ReadOnly]
        public GameObject itemPrefab;

        [SerializeField, LabelText("�����б�"), Tooltip("�ռ����������������б�")]
        public TabView tabView;

        [SerializeField, LabelText("��ѡ�еĽ���UI"), ReadOnly]
        UIBuildingItem buildingItemSelected;//��ѡ�еĽ���UI

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
        /// ѡ����Ҫ����Ľ���
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
                Debug.LogFormat("���콨����{0}", buildingItem.type);
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
                ui.SetInfo(buType, this.define.sprites[(int)buType-1]);//���ý���UIItem��Ϣ
                this.tabView.tabPages[0].AddItem(ui);
            }

            foreach (var buType in BuildingManager.Instance.ProductionTypes)
            {
                GameObject go = Instantiate(this.itemPrefab.gameObject, this.tabView.tabPages[1].transform);//�ڽ����б�ڶ�ҳ����
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(buType, this.define.sprites[(int)buType - 1]);//���ý���UIItem��Ϣ
                this.tabView.tabPages[1].AddItem(ui);
            }
        }
    }
}
