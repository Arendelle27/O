using MANAGER;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ArchiveManager;

namespace ENTITY
{
    public class Plot : Entity
    {
        [SerializeField, LabelText("���Ӷ���"), ReadOnly]
        public PlotDefine plotDefine;

        //[SerializeField, LabelText("����ͼ��"), Tooltip("��ͬ״̬�ĸ��ӵ�ͼ��")]
        //public List<Sprite> plot_Sps = new List<Sprite>();

        [SerializeField, LabelText("���״̬"), ReadOnly]
        public Plot_Statue plot_Statue;

        [SerializeField, LabelText("�������"), ReadOnly]
        public int plotType;
        //0Ϊ��Դ�࣬1Ϊ�¼���

        //��ʱ ��ǰ��ɫ
        public Color curColor;
        //��ѡ��ʱ����ɫ
        public Color selectedColor=Color.blue;

        [SerializeField, LabelText("�������"), Tooltip("��ʾ����")]
        public Text figure;

        [SerializeField, LabelText("λ��"), ReadOnly]
        public Vector2Int pos;

        [SerializeField, LabelText("������"), ReadOnly]
        public Wanderer wanderer;
        [SerializeField, LabelText("����"), ReadOnly]
        public Building building;
        [SerializeField, LabelText("�¼�����"), ReadOnly]
        public EventArea eventArea;

        [SerializeField, LabelText("�Ƿ���̽��С��"), ReadOnly]
        private bool haveExploratoryTeam;//�Ƿ���̽��С��
        public bool HaveExploratoryTeam
        {
            get
            {
                return this.haveExploratoryTeam;
            }
            set
            {
                this.haveExploratoryTeam = value;
                this.ExpTeamEnter(value);
            }
        }

        [SerializeField, LabelText("�Ƿ��ǵ�һ��̽��"), ReadOnly]
        public bool isFirstExplored;

        [SerializeField, LabelText("��Դ����"), ReadOnly]
        public List<int> buildingResources = new List<int>() { 0, 0 };
        //0Ϊ��Դ���ͣ�1Ϊ��Դ����
        //��Դ����  1��2��3��0Ϊ����Դ

        [SerializeField, LabelText("�Ƿ���Խ���"), ReadOnly]
        public bool canEnter;

        #region �¼�
        [SerializeField, LabelText("��������"), ReadOnly]
        public Subject<Plot> clickSelectedSubject = new Subject<Plot>();

        [SerializeField, LabelText("��������"), ReadOnly]
        public Subject<Vector2Int> enterSelectedSubject = new Subject<Vector2Int>();

        [SerializeField, LabelText("ͨ�����������"), ReadOnly]
        public Subject<int> unLoadByPlot=new Subject<int>();
        #endregion

        private void Start()
        {
            this.ObserveEveryValueChanged(_ => this.building).Subscribe(_ =>
            {
                if (this.building != null)
                {
                    this.BuildConstruction(true);
                }
                else
                {
                    this.BuildConstruction(false);
                }
            });

            this.ObserveEveryValueChanged(_ => this.wanderer).Subscribe(_ =>
            {
                if(this.wanderer!=null)
                {
                    this.WandererEnter(true);
                }
                else
                {
                    this.WandererEnter(false);
                }
            });


            this.ObserveEveryValueChanged(_ => this.curColor).Subscribe(_ =>
            {
                if(this.SR.color!=this.selectedColor)
                {
                    this.SR.color = this.curColor;
                }
            });
        }

        private void OnEnable()
        {
            this.clickSelectedSubject = new Subject<Plot>();
            this.enterSelectedSubject = new Subject<Vector2Int>();
            this.unLoadByPlot = new Subject<int>();
        }

        private void OnDisable()
        {
            this.clickSelectedSubject.OnCompleted();

            this.enterSelectedSubject.OnCompleted();

            this.unLoadByPlot.OnCompleted();

        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        void Init()
        {
            this.wanderer = null;
            this.building = null;
            this.eventArea = null;
            this.HaveExploratoryTeam = false;
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="plotdefine"></param>
        public void SetInfo(PlotDefine plotdefine)
        {
            this.Init();

            this.plotDefine= plotdefine;
            this.plotType = plotdefine.Type;
            this.figure.text = plotdefine.ID.ToString();

            switch (this.plotType)
            {
                case 0://��Դ���
                    if (plotdefine.ResourceType!=-1)
                    {
                        this.buildingResources[0] = plotdefine.ResourceType;
                    }
                    else
                    {
                        this.buildingResources[0] = 0;
                    }
                    this.buildingResources[1] = plotdefine.ResourceTotal;
                    this.figure.color = Color.white;
                    break;
                case 1://�¼����
                    this.eventArea=EventAreaManager.Instance.GetEventArea(plotdefine.EventType, this);
                    this.figure.color = Color.gray;
                    break;
            }

            ChangeType(Plot_Statue.δ̽��);
            this.isFirstExplored = true;
            if(this.plotDefine.IsSpecialGeneration)
            {
                //�������ɣ������Ƿ�������ж��Ƿ���Խ���
                this.canEnter = PlotManager.Instance.plotType[0].Contains(this.plotDefine.ID)|| PlotManager.Instance.plotType[1].Contains(this.plotDefine.ID);
            }
            else
            {
                this.canEnter = true;
            }

        }

        ///// <summary>
        ///// ����
        ///// </summary>
        ///// <param name="plotData"></param>
        //public void ReadArchive(PlotData plotData)
        //{
        //    this.Init();
        //    this.ChangeType(plotData.plotType);
        //}

        #region �ı����״̬
        /// <summary>
        /// ��������͸ı���ı�
        /// </summary>
        /// <param name="plot_Type"></param>
        void ChangeType(Plot_Statue plot_Type)
        {
            switch (plot_Type)
            {
                case Plot_Statue.δ̽��:
                    ChangeToUndiscoverd();
                    break;
                case Plot_Statue.��̽��:
                    ChangeToCanDisCover();
                    break;
                case Plot_Statue.��̽��:
                    ChangeToDiscovered();
                    break;
                case Plot_Statue.�ѿ���:
                    ChangeToDeveloped();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// �ı�Ϊδ̽��״̬
        /// </summary>
        void ChangeToUndiscoverd()
        {
            this.plot_Statue = Plot_Statue.δ̽��;
            this.curColor = Color.black;
        }

        /// <summary>
        /// �ı�Ϊ��̽��״̬
        /// </summary>
        void ChangeToCanDisCover()
        {
            this.plot_Statue = Plot_Statue.��̽��;
            this.curColor = Color.red;
        }

        /// <summary>
        /// �ı�Ϊ��̽��״̬
        /// </summary>
        void ChangeToDiscovered()
        {
            this.plot_Statue = Plot_Statue.��̽��;
            this.curColor = Color.yellow;
        }

        /// <summary>
        /// �ı�Ϊ�ѿ���״̬
        /// </summary>
        void ChangeToDeveloped()
        {
            this.plot_Statue = Plot_Statue.�ѿ���;
            this.curColor = Color.green;
        }
        #endregion

        /// <summary>
        /// �����߽�����뿪
        /// </summary>
        /// <param name="isEnter"></param>
        void WandererEnter(bool isEnter)
        {
            if(isEnter)
            {
                if(this.plot_Statue==Plot_Statue.��̽��||this.plot_Statue==Plot_Statue.δ̽��)
                {
                    this.ChangeType(Plot_Statue.��̽��);
                }

                if(isFirstExplored)
                {
                    this.unLoadByPlot.OnNext(this.plotDefine.ID);//ͨ�����ӽ�������
                    if (this.plotDefine.Type == 0)//��Դ���
                    {
                        int[] res = new int[3] { 0, 0, 0 };
                        res[this.buildingResources[0]] = this.plotDefine.ResourceFristtime;
                        ResourcesManager.Instance.ChangeBuildingResources(res, true);

                        this.buildingResources[1] -= this.plotDefine.ResourceFristtime;
                    }
                    this.isFirstExplored = false;
                }

                //������ɫ�����¼�
                this.eventArea?.WandererEnter();
            }
            //else
            //{
            //    if(this.plot_Type==Plot_Type.��̽��)
            //    {
            //        this.ChangeType(Plot_Type.δ̽��);
            //    }
            //}
        }

        /// <summary>
        /// ���ɻ��߲������
        /// </summary>
        /// <param name="isbuild"></param>
        void BuildConstruction(bool isbuild)
        {
            if(isbuild)
            {
                this.ChangeType(Plot_Statue.�ѿ���);
            }
            else
            {
                if(this.plot_Statue==Plot_Statue.�ѿ���)
                {
                    this.ChangeType(Plot_Statue.��̽��);
                }
            }
        }

        /// <summary>
        /// ̽��С�ӽ�����뿪
        /// </summary>
        /// <param name="isEnter"></param>
        void ExpTeamEnter(bool isEnter)
        {
            if (isEnter)
            {
                if(this.plot_Statue == Plot_Statue.δ̽��)
                {
                    this.ChangeType(Plot_Statue.��̽��);
                }
            }
            else
            {
                if (this.plot_Statue == Plot_Statue.��̽��)
                {
                    this.ChangeType(Plot_Statue.δ̽��);
                }
            }
        }

        /// <summary>
        /// ����ѡ����չ����С���У���ѡ���״̬
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowSelectedColor(bool isShow)
        {
            if(isShow)
            {
                this.SR.color = this.selectedColor;
            }
            else
            {
                this.SR.color = this.curColor;
            }
        }

        public void OnMouseEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            //Debug.Log("���ѡ��");
        }

        public void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            this.clickSelectedSubject.OnNext(this);
            //Debug.Log("�����");
        }

        /// <summary>
        /// ����̽��
        /// </summary>
        public void TeamExp()
        {
            if ( this.plot_Statue == Plot_Statue.��̽�� && this.plotDefine.Type == 0)
            {
                int[] res = new int[3] { 0, 0, 0 };
                if (this.buildingResources[1] > this.plotDefine.ResourceByRound)//��Դ�㹻
                {
                    res[this.buildingResources[0]] = this.plotDefine.ResourceByRound;
                    ResourcesManager.Instance.ChangeBuildingResources(res, true);
                    this.buildingResources[1] -= this.plotDefine.ResourceByRound;
                }
                else if (this.buildingResources[1] > 0)//��Դ����
                {
                    res[this.buildingResources[0]] = this.buildingResources[1];
                    ResourcesManager.Instance.ChangeBuildingResources(res, true);
                    this.buildingResources[1] = 0;
                }
            }
        }
    }
}
