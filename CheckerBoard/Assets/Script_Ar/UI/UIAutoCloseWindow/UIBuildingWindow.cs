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
            foreach (var rectTransform in rectTransforms)//����Ӧ����
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
            this.tabView.tabButtons[1].OnClick();//��������
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
                    if (NoviceGuideManager.Instance.isGuideStage[3])//�Ƿ�������ָ���׶�
                    {
                        NoviceGuideManager.Instance.NoviceGuideStage++;
                    }
                    this.selectedWindow.OnCloseClick();
                }
                
            }
        }

        /// <summary>
        /// ��ս����б�UI
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
        /// ��ʼ�������б�UI
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
                    if (buType <= Building_Type.���¾��)
                    {
                        buildingSort = 0;
                    }
                    else if (buType <= Building_Type.�����ᴿ��)
                    {
                        buildingSort = 3;
                    }
                    else
                    {
                        buildingSort = 4;
                    }
                }

                go.transform.SetParent(this.tabView.tabPages[buildingSort].content);//�ڽ����б��һҳ����
                UIBuildingItem ui = go.GetComponent<UIBuildingItem>();
                ui.SetInfo(sort, buType);//���ý���UIItem��Ϣ
                this.tabView.tabPages[buildingSort].AddItem(ui);
            }
        }
    }
}
