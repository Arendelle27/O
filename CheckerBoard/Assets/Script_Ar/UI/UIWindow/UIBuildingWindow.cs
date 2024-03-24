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
        [SerializeField, LabelText("�����б�"), Tooltip("�ռ����������������б�")]
        public TabView tabView;

        [SerializeField, LabelText("��ѡ�еĽ���UI����"), ReadOnly]
        Building_Type buildingtypeSelected;//��ѡ�еĽ���UI����


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
        /// ����q�л�����
        /// </summary>
        /// <param name="index"></param>
        void OnTabSelect(int index)
        {
            StartCoroutine(BeSelected());
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
            }
            else//�ڶ��ε��ȷ�Ͻ���
            {
                Debug.LogFormat("���콨����{0}", buildingtypeSelected);
                BuildingManager.Instance.Build(buildingtypeSelected, PlotManager.Instance.SelectedPlot);
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
                go.transform.SetParent(this.tabView.tabPages[sort].transform);//�ڽ����б��һҳ����
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(sort, buType);//���ý���UIItem��Ϣ
                this.tabView.tabPages[sort].AddItem(ui);
            }
        }

        #region ʵ�ֹرս���UIѡ�����
        /// <summary>
        /// 0.1���ѡ��
        /// </summary>
        /// <returns></returns>
        IEnumerator BeSelected()
        {
            yield return new WaitForSeconds(0.01f);
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
