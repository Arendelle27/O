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
        [SerializeField, LabelText("״̬����"), Tooltip("��ʾ���״̬")]
        public SpriteRenderer statueMask;

        [SerializeField, LabelText("���Ӷ���"), ReadOnly]
        public PlotDefine plotDefine;

        [SerializeField, LabelText("���״̬"), ReadOnly]
        public Plot_Statue plot_Statue;

        [SerializeField, LabelText("�������"), ReadOnly]
        public int plotType;
        //0Ϊ��Դ�࣬1Ϊ�¼���

        [SerializeField, LabelText("����ͼ���б�"), Tooltip("����ͼ���б�")]
        public List<Sprite> maskSprite;
        //0��̽��״̬���֣�1���ƶ�״̬����
        [SerializeField, LabelText("��������"), ReadOnly]
        int statueSort = -1;//-1Ϊ�ر����֣�0Ϊ��̽����1Ϊ���ƶ�

        [SerializeField, LabelText("λ��"), ReadOnly]
        public Vector2Int pos;

        [SerializeField, LabelText("������"), ReadOnly]
        public Wanderer wanderer;
        [SerializeField, LabelText("����"), ReadOnly]
        public Building building;
        [SerializeField, LabelText("�¼�����"), ReadOnly]
        public EventArea eventArea;
        [SerializeField, LabelText("����ϵ�npc"), ReadOnly]
        public List<Npc> npcs=new List<Npc>();

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
        //��Դ����  0��1��2��-1Ϊ����Դ

        [SerializeField, LabelText("�Ƿ���Խ���"), ReadOnly]
        public bool canEnter;

        #region �¼�
        [SerializeField, LabelText("��������"), ReadOnly]
        public Subject<Plot> clickSelectedSubject = new Subject<Plot>();

        //[SerializeField, LabelText("��������"), ReadOnly]
        //public Subject<Vector2Int> enterSelectedSubject = new Subject<Vector2Int>();

        [SerializeField, LabelText("ͨ�����������"), ReadOnly]
        public Subject<int> unLoadByPlot=new Subject<int>();
        #endregion

        private void Start()
        {

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


            this.ObserveEveryValueChanged(_ => this.statueSort).Subscribe(_ =>
            {
                this.ChangeStatueMask(this.statueSort);
            });
        }

        private void OnEnable()
        {
            this.clickSelectedSubject = new Subject<Plot>();
            //this.enterSelectedSubject = new Subject<Vector2Int>();
            this.unLoadByPlot = new Subject<int>();
        }

        private void OnDisable()
        {
            this.clickSelectedSubject.OnCompleted();

            //this.enterSelectedSubject.OnCompleted();

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
        /// <param name="plotDefine"></param>
        public void SetInfo(PlotDefine plotDefine)
        {
            this.Init();

            this.plotDefine= plotDefine;
            this.plotType = plotDefine.Type;
            //this.figure.text = plotDefine.ID.ToString();
            if(SpriteManager.plotSprites.ContainsKey(plotDefine.Name))
            {
                this.SR.sprite = SpriteManager.plotSprites[plotDefine.Name];
            }


            switch (this.plotType)
            {
                case 0://��Դ���

                    this.buildingResources[0]= plotDefine.ResourceType;
                    this.buildingResources[1] = plotDefine.ResourceTotal;
                    break;
                case 1://�¼����
                    this.eventArea=EventAreaManager.Instance.GetEventArea(plotDefine.EventType, this);

                    break;
            }

            ChangeType(Plot_Statue.δ̽��);
            this.isFirstExplored = true;
            if(this.plotDefine.IsSpecialGeneration)
            {
                //�������ɣ������Ƿ�������ж��Ƿ���Խ���
                this.canEnter = PlotManager.Instance.plotTypes[0].Contains(this.plotDefine.ID)|| PlotManager.Instance.plotTypes[1].Contains(this.plotDefine.ID);
            }
            else
            {
                this.canEnter = true;
            }


        }

        /// <summary>
        /// ��Դ����������߽�ֳ�ͻ֮����ӱ�Ϊ��ԭ
        /// </summary>
        public void ChangeToNormalType()
        {
            this.plotDefine = DataManager.PlotDefines[7];
            this.plotType = this.plotDefine.Type;


            if (SpriteManager.plotSprites.ContainsKey(this.plotDefine.Name))
            {
                this.SR.sprite = SpriteManager.plotSprites[this.plotDefine.Name];
            }

            this.eventArea= null;
            this.buildingResources[0] = -1;
            this.canEnter = true;

            this.ChangeType(this.plot_Statue);
        }

        /// <summary>
        /// �ı�Ϊ��ͻ����
        /// </summary>
        /// <param name="clashType"></param>
        public void ChangeToClashType(int clashType)
        {
            switch (clashType)
            {
                case 0:
                    this.plotDefine = DataManager.PlotDefines[12];//�����ͻ��
                    break;
                default:
                    this.plotDefine = DataManager.PlotDefines[8];//�����˳�ͻ��
                    break;
            }
            this.plotType = this.plotDefine.Type;
            //this.figure.text = this.plotDefine.ID.ToString();
            //this.figure.color = Color.gray;

            if (SpriteManager.plotSprites.ContainsKey(this.plotDefine.Name))
            {
                this.SR.sprite = SpriteManager.plotSprites[this.plotDefine.Name];
            }

            this.eventArea = EventAreaManager.Instance.GetEventArea(this.plotDefine.EventType, this);
            this.canEnter = true;

            if(this.building!=null)
            {
                BuildingManager.Instance.RemoveBuilding(BuildingManager.BuildingTypeToIndex(this.building.type), this.building);
                this.building = null;
            }

            this.ChangeType(this.plot_Statue);
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
        /// ���ֻ�������
        /// </summary>
        /// <param name="isDiscover"></param>
        public void Discover(bool isDiscover)
        {
            this.SR.enabled = isDiscover;
        }

        /// <summary>
        /// �ı�״̬������ɫ
        /// </summary>
        void ChangeStatueMask(int maskSort)//-1Ϊ�ر����֣�0Ϊ��̽����1Ϊ���ƶ�
        {
            //Color color=new Color(curColor.r,curColor.g,curColor.b,ParameterConfig.diaphaneity);
            //this.statueMask.color= color;
            if(maskSort==-1)
            {
                this.statueMask.gameObject.SetActive(false);
            }
            else
            {
                this.statueMask.gameObject.SetActive(true);
                this.statueMask.sprite = this.maskSprite[maskSort];
            }
        }

        /// <summary>
        /// ��������͸ı���ı�
        /// </summary>
        /// <param name="plot_Type"></param>
        public void ChangeType(Plot_Statue plot_Type)
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
                //case Plot_Statue.�ѿ���:
                //    ChangeToDeveloped();
                //    break;
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
            //this.curColor = Color.black;
            this.statueSort = -1;
            if(this.plotType==0)
            {
                this.Discover(false);
            }
            else
            {
                this.Discover(true);
            }
        }

        /// <summary>
        /// �ı�Ϊ��̽��״̬
        /// </summary>
        void ChangeToCanDisCover()
        {
            this.plot_Statue = Plot_Statue.��̽��;
            //this.curColor = Color.red;
            this.statueSort = 0;
            if (this.plotType == 0)
            {
                this.Discover(false);
            }
            else
            {
                this.Discover(true);
            }
        }

        /// <summary>
        /// �ı�Ϊ��̽��״̬
        /// </summary>
        void ChangeToDiscovered()
        {
            this.plot_Statue = Plot_Statue.��̽��;
            this.Discover(true);
            //this.curColor = Color.yellow;
            this.statueSort = -1;
        }

        ///// <summary>
        ///// �ı�Ϊ�ѿ���״̬
        ///// </summary>
        //void ChangeToDeveloped()
        //{
        //    this.plot_Statue = Plot_Statue.�ѿ���;
        //    //this.curColor = Color.green;
        //    this.statueSort = -1;
        //}
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

                if (this.plotDefine.CanBuild//�����Խ���
                    && this.building == null//�����û�н���
                    )
                {
                    (UIMain.Instance.uiPanels[1] as UIGamePanel).buildButton.gameObject.SetActive(true);//��ʾ���찴ť
                }
                else
                {
                    (UIMain.Instance.uiPanels[1] as UIGamePanel).buildButton.gameObject.SetActive(false);//�رս��찴ť
                }

                NpcManager.Instance.NPCAppearUnlock(1, this.plotDefine.ID);//��ӶԻ�
                ChatManager.Instance.ChatConditionUnlock(2, this.plotDefine.ID);

                foreach (var npc in this.npcs)
                {
                    npc.ChatWithWander();
                }

                if (isFirstExplored)
                {
                    this.unLoadByPlot.OnNext(this.plotDefine.ID);//ͨ�����ӽ�������
                    if (this.plotDefine.Type == 0)//��Դ���
                    {
                        if (this.buildingResources[0] == -1)
                        {
                            return;
                        }
                        int[] res = new int[3] { 0, 0, 0 };
                        res[this.buildingResources[0]] = this.ReduceResource( this.plotDefine.ResourceFristtime);
                        ResourcesManager.Instance.ChangeBuildingResources(res, true);

                    }

                    this.isFirstExplored = false;
                }

                //EventManager.Instance.curClashArea = null;//��ռ�¼�ĳ�ͻ����

                //������ɫ�����¼�
                this.eventArea?.WandererEnter();
            }
        }

        /// <summary>
        /// ������Դ
        /// </summary>
        /// <param name="amountNeeded"></param>
        /// <returns></returns>
        public int ReduceResource(int amountNeeded)
        {
            int amountResult= 0;
            if (this.buildingResources[1] > amountNeeded)//��Դ�㹻
            {
                amountResult = amountNeeded;
                this.buildingResources[1] -= amountNeeded;
            }
            else if (this.buildingResources[1] > 0)//��Դ����
            {
                amountResult = this.buildingResources[1];
                this.buildingResources[1] = 0;
                this.ChangeToNormalType();//��Ϊ��ԭ
            }
            return amountResult;
        }

        ///// <summary>
        ///// ���ɻ��߲������
        ///// </summary>
        ///// <param name="isbuild"></param>
        //void BuildConstruction(bool isbuild)
        //{
        //    if(isbuild)
        //    {
        //        this.ChangeType(Plot_Statue.�ѿ���);
        //    }
        //    else
        //    {
        //        if(this.plot_Statue==Plot_Statue.�ѿ���)
        //        {
        //            this.ChangeType(Plot_Statue.��̽��);
        //        }
        //    }
        //}

        /// <summary>
        /// ̽��С�ӽ�����뿪
        /// </summary>
        /// <param name="isEnter"></param>
        void ExpTeamEnter(bool isEnter)
        {
            if (isEnter)
            {
                if (this.plot_Statue == Plot_Statue.δ̽��|| this.plot_Statue == Plot_Statue.��̽��)
                {
                    this.ChangeType(Plot_Statue.��̽��);
                }
            }
        }

        /// <summary>
        /// ����ѡ����չ����С���У���ѡ���״̬
        /// </summary>
        /// <param name="canMove"></param>
        public void CanMoveIn(bool canMove)
        {
            if (canMove)
            {
                this.ChangeStatueMask(1);//���ƶ�״̬
            }
            else
            {
                this.ChangeStatueMask(this.statueSort);
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
                if(this.buildingResources[0]==-1)
                {
                    return;
                }
                int[] res = new int[3] { 0, 0, 0 };
                res[this.buildingResources[0]] =this.ReduceResource(this.plotDefine.ResourceByRound);
                ResourcesManager.Instance.ChangeBuildingResources(res, true);
            }
        }
    }
}
