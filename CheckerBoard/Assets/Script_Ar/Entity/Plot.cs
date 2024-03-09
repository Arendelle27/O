using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ENTITY
{
    public class Plot : Entity
    {
        [SerializeField, LabelText("����ͼ��"), Tooltip("��ͬ״̬�ĸ��ӵ�ͼ��")]
        public List<Sprite> plot_Sps = new List<Sprite>();

        [SerializeField, LabelText("�������"), ReadOnly]
        public Plot_Type plot_Type;
        //��ʱ ��ǰ��ɫ
        public Color curColor;
        //��ѡ��ʱ����ɫ
        public Color selectedColor=Color.blue;

        [SerializeField, LabelText("λ��"), ReadOnly]
        public Vector2Int pos;

        [SerializeField, LabelText("������"), ReadOnly]
        public Wanderer wanderer;
        [SerializeField, LabelText("����"), ReadOnly]
        public Building building;

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

        #region �¼�
        [SerializeField, LabelText("������"), ReadOnly]
        public Subject<Vector2Int> clickSelectedSubject = new Subject<Vector2Int>();
        //public IObservable<Vector2Int> Selected => selectedSubject;

        [SerializeField, LabelText("������"), ReadOnly]
        public Subject<Vector2Int> enterSelectedSubject = new Subject<Vector2Int>();
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
                this.SR.color = this.curColor;
            });
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public void Init()
        {
            ChangeType(Plot_Type.δ̽��);

            this.wanderer = null;
            this.building = null;
            this.HaveExploratoryTeam = false;
        }

        #region �ı����״̬
        /// <summary>
        /// ��������͸ı���ı�
        /// </summary>
        /// <param name="plot_Type"></param>
        void ChangeType(Plot_Type plot_Type)
        {
            switch (plot_Type)
            {
                case Plot_Type.δ̽��:
                    ChangeToUndiscoverd();
                    break;
                case Plot_Type.��̽��:
                    ChangeToCanDisCover();
                    break;
                case Plot_Type.��̽��:
                    ChangeToDiscovered();
                    break;
                case Plot_Type.�ѿ���:
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
            this.plot_Type = Plot_Type.δ̽��;
            this.curColor = Color.black;
        }

        /// <summary>
        /// �ı�Ϊ��̽��״̬
        /// </summary>
        void ChangeToCanDisCover()
        {
            this.plot_Type = Plot_Type.��̽��;
            this.curColor = Color.red;
        }

        /// <summary>
        /// �ı�Ϊ��̽��״̬
        /// </summary>
        void ChangeToDiscovered()
        {
            this.plot_Type = Plot_Type.��̽��;
            this.curColor = Color.yellow;
        }

        /// <summary>
        /// �ı�Ϊ�ѿ���״̬
        /// </summary>
        void ChangeToDeveloped()
        {
            this.plot_Type = Plot_Type.�ѿ���;
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
                if(this.plot_Type==Plot_Type.��̽��||this.plot_Type==Plot_Type.δ̽��)
                {
                    this.ChangeType(Plot_Type.��̽��);
                }
            }
            else
            {
                if(this.plot_Type==Plot_Type.��̽��)
                {
                    this.ChangeType(Plot_Type.δ̽��);
                }
            }
        }

        /// <summary>
        /// ���ɻ��߲������
        /// </summary>
        /// <param name="isbuild"></param>
        void BuildConstruction(bool isbuild)
        {
            if(isbuild)
            {
                this.ChangeType(Plot_Type.�ѿ���);
            }
            else
            {
                this.ChangeType(Plot_Type.��̽��);
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
                if(this.plot_Type == Plot_Type.δ̽��)
                {
                    this.ChangeType(Plot_Type.��̽��);
                }
            }
            else
            {
                if (this.plot_Type == Plot_Type.��̽��)
                {
                    this.ChangeType(Plot_Type.δ̽��);
                }
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

            this.clickSelectedSubject.OnNext(this.pos);
            //Debug.Log("�����");
        }
    }
}
