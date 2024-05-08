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
        [SerializeField, LabelText("�����б�"), Tooltip("�ռ����������������б�")]
        public TabView tabView;

        [SerializeField, LabelText("������Ϣ"), Tooltip("���뽨����ϢUI")]
        public UIBuildingItemInfo uiBuildingItemInfo;

        [SerializeField, LabelText("��ѡ�еĽ���UI����"), ReadOnly]
        Building_Type buildingtypeSelected;//��ѡ�еĽ���UI����

        [SerializeField, LabelText("����Ӧ�Ĵ���"), Tooltip("������Ҫֻ��Ӧ�Ĵ���")]
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
            foreach (var rectTransform in rectTransforms)//����Ӧ����
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
            this.buildingtypeSelected=Building_Type.��;
        }

        /// <summary>
        /// �����л�����
        /// </summary>
        /// <param name="index"></param>
        void OnTabSelect(int index)
        {
            this.buildingtypeSelected=Building_Type.��;
            StartCoroutine( this.selectedWindow.BeSelected());
            foreach (var rectTransform in rectTransforms)//����Ӧ����
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
        }

        /// <summary>
        /// ѡ����Ҫ����Ľ���,�ڶ��ε����ȷ�Ͻ���
        /// </summary>
        /// <param name="item"></param>
        public void OnBuildingItemSelected(ListView.ListViewItem item)
        {
            UIBuildingItem buildingItem = item as UIBuildingItem;
            if (this.buildingtypeSelected != buildingItem.type)//��һ�ε��ѡ��
            {
                this.buildingtypeSelected = buildingItem.type;

                this.uiBuildingItemInfo.gameObject.SetActive(true);
                this.uiBuildingItemInfo.SetInfo(buildingItem.type);
            }
            else//�ڶ��ε��ȷ�Ͻ���
            {
                Debug.LogFormat("���콨����{0}", buildingtypeSelected);

                if(BuildingManager.Instance.Build(buildingtypeSelected, WandererManager.Instance.wanderer.plot))
                {
                    this.selectedWindow.OnCloseClick();
                }
                
            }
        }

        /// <summary>
        /// ��ս����б�UI
        /// </summary>
        void ClearBuildingList(int sort)
        {
            this.tabView.tabPages[sort].RemoveAll();
        }

        /// <summary>
        /// ��ʼ�������б�UI
        /// </summary>
        public void UpdateBuildingList(int sort)
        {
            ClearBuildingList(sort);
            foreach (var buType in BuildingManager.Instance.buildingTypes[sort])
            {
                GameObject go = GameObjectPool.Instance.UIBuildingItems.Get();
                go.transform.SetParent(this.tabView.tabPages[sort].content);//�ڽ����б��һҳ����
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(sort, buType);//���ý���UIItem��Ϣ
                this.tabView.tabPages[sort].AddItem(ui);
            }
        }
    }
}
