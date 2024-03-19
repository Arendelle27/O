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
        [SerializeField, LabelText("�����б�"), Tooltip("�ռ����������������б�")]
        public TabView tabView;

        [SerializeField, LabelText("��ѡ�еĽ���UI����"), ReadOnly]
        Building_Type buildingtypeSelected;//��ѡ�еĽ���UI����


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
                BuildingManager.Instance.Build(buildingtypeSelected, PlotManager.Instance.SelectedPlot);
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
                GameObject go = GameObjectPool.Instance.UIBuildingItems.Get();
                go.transform.parent = this.tabView.tabPages[0].transform;//�ڽ����б��һҳ����
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(0,buType);//���ý���UIItem��Ϣ
                this.tabView.tabPages[0].AddItem(ui);
            }

            foreach (var buType in BuildingManager.Instance.ProductionTypes)
            {
                GameObject go = GameObjectPool.Instance.UIBuildingItems.Get();
                go.transform.parent = this.tabView.tabPages[1].transform;//�ڽ����б�ڶ�ҳ����
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(1, buType);//���ý���UIItem��Ϣ
                this.tabView.tabPages[1].AddItem(ui);
            }

            foreach (var buType in BuildingManager.Instance.BattleTypes)
            {
                GameObject go = GameObjectPool.Instance.UIBuildingItems.Get();
                go.transform.parent = this.tabView.tabPages[2].transform;//�ڽ����б�ڶ�ҳ����
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(2,buType);//���ý���UIItem��Ϣ
                this.tabView.tabPages[1].AddItem(ui);
            }
        }

        #region ʵ�ֹرս���UIѡ�����
        /// <summary>
        /// 0.1���ѡ��
        /// </summary>
        /// <returns></returns>
        IEnumerator BeSelected()
        {
            yield return new WaitForSeconds(0.1f);
            this.GetComponent<Selectable>().Select();
        }

        /// <summary>
        /// δѡ�н���UIʱ�رս���UIѡ�����
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
