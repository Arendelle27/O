using ENTITY;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Tilemaps;
using UniRx;
using System;
using UnityEngine.EventSystems;
using Managers;
using UIBUILDING;

namespace MANAGER
{
    public class PlotManager : MonoSingleton<PlotManager>
    {
        //���浱ǰ���ڵĸ���
        public Dictionary<Vector2Int, Plot> plots = new Dictionary<Vector2Int, Plot>();

        [SerializeField, LabelText("��ͼ"), Tooltip("��Ƭ��ͼ")]
        public Tilemap map;

        [SerializeField, LabelText("��ͼģʽ"), ReadOnly]
        public Map_Mode map_Mode = Map_Mode.����;

        //�Ƿ��Ҽ�ѡ���ƶ������е�������
        IDisposable select;
        //�Ƿ�ȡ���ƶ������е�������
        IDisposable cancel;

        [SerializeField, LabelText("��ǰ��ѡ�еİ��"), ReadOnly]
        private Plot selectedPlot;//��ǰ��ѡ�еİ��

        ///��ǰ��ѡ�еĸ���
        public Plot SelectedPlot
        {
            get
            {
                return this.selectedPlot;
            }
            set
            {
                if (value != null)
                {
                    this.selectedPlot = value;

                    switch (this.map_Mode)
                    {
                        case Map_Mode.����:
                            if (this.select != null)
                            {
                                select.Dispose();
                            }

                            if (value.wanderer != null)
                            {
                                this.IsMoveWanaderer(value);
                            }
                            break;
                        case Map_Mode.ѡ��Ŀ�ĵ�λ��:
                            if (value.plot_Type != Plot_Type.δ̽��&& value.wanderer == null)
                            {
                                this.MoveWanderer(value);
                            }
                            break;
                        case Map_Mode.��չ̽��С��:
                            if(DataManager.Instance.levelPromptionAmount>0)
                            {
                                WandererManager.Instance.ExtendExpTeam(value);//��չ̽��С��
                            }
                            break;
                    }

                }
            }
        }

        void Start()
        {
            for (int i = -4; i < 5; i++)
                for (int j = -4; j < 5; j++)
                {
                    GetGrid(new Vector2Int(i, j));
                }
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public void Init()
        {
            this.SelectedPlot = this.plots[new Vector2Int(0, 0)];
            this.map_Mode = Map_Mode.����;
        }

        /// <summary>
        /// �ؿ�
        /// </summary>
        public void Restart()
        {
            this.Init();
            for (int i = -4; i < 5; i++)
                for (int j = -4; j < 5; j++)
                {
                    this.plots[new Vector2Int(i, j)].Restart();
                }
        }

        /// <summary>
        /// ��ȡ�浵
        /// </summary>
        public void ReadArchive()
        {
            foreach (var plot in ArchiveManager.archive.plotData)
            {
                this.plots[plot.pos].ReadArchive(plot);
            }
        }


        /// <summary>
        /// �ڸ�����λ�ò�������
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void GetGrid(Vector2Int pos)
        {
            GameObject gO = GameObjectPool.Instance.Plots.Get();
            gO.transform.parent = this.transform;
            Vector3Int v3= new Vector3Int(pos.x, pos.y, 0);
            gO.transform.position = this.map.GetCellCenterWorld(v3);

            Plot plot = gO.GetComponent<Plot>();
            plot.pos = pos;

            this.plots.Add(pos, plot);

            plot.clickSelectedSubject.Subscribe(p =>
            {
                if(p.plot_Type==Plot_Type.��̽��
                    && this.map_Mode == Map_Mode.����
                    /*&& p.settlement==null*/)
                {
                    UIManager.Instance.Show<UIBuildingWindow>();//�򿪽���UIѡ�����
                }
                this.SelectedPlot = p;
            });
        }

        #region �Ƿ��ƶ�����е���������ط���
        /// <summary>
        /// �Ƿ��ƶ������е�������
        /// </summary>
        /// <param name="p"></param>
        void IsMoveWanaderer(Plot ini)
        {
            Debug.Log("��������");
            this.select = Observable
                .EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(1))
                .First()
                .Subscribe(_ =>
                {
                    Debug.Log("ѡ���ƶ���");

                    this.map_Mode = Map_Mode.ѡ��Ŀ�ĵ�λ��;
                    //this.ini = ini;

                    this.CancelMoveWanderer();

                    UIMain.Instance.ChangeToGamePanel(1);//ѡ�����ʱ�ر�UI����

                    this.select.Dispose();
                });
        }

        /// <summary>
        /// ȷ���ƶ�������
        /// </summary>
        void MoveWanderer(Plot des)
        {
            this.map_Mode = Map_Mode.����;
            MainThreadDispatcher.StartUpdateMicroCoroutine( WandererManager.Instance.WandererMoveTo(des));//���������ƶ���ָ���İ��

            UIMain.Instance.ChangeToGamePanel(1);//ѡ��������UI����

            if (this.cancel!=null)
            {
                this.cancel.Dispose();
            }
        }

        /// <summary>
        /// �Ƿ�ȡ���ƶ�������
        /// </summary>
        void CancelMoveWanderer()
        {
            cancel = Observable
                .EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(1))
                .First()
                .Subscribe(_ =>
                {
                    this.map_Mode = Map_Mode.����;

                    UIMain.Instance.ChangeToGamePanel(1);//ȡ��ѡ�������UI����

                    cancel.Dispose();
                });
        }
        #endregion

        /// <summary>
        /// �����߽���������
        /// </summary>
        /// <param name="enterPlot"></param>
        public void WanderEnter(Plot enterPlot)
        {
            enterPlot.wanderer = WandererManager.Instance.wanderer;//�����߽����������
            SettlementManager.Instance.TriggerEvent(Event_Type.����,enterPlot.pos);//�ж�������䴥�������¼�

            this.ExpTeamEnterOrLeave(enterPlot, true);//̽��С�Ӱ����������߽���
        }


        /// <summary>
        /// �������뿪�������
        /// </summary>
        /// <param name="leaverPlot"></param>
        public void WanderLeave(Plot leaverPlot)
        {
            leaverPlot.wanderer = null;
            this.ExpTeamEnterOrLeave(leaverPlot, false);//̽��С�Ӱ������������뿪
        }

        /// <summary>
        /// ̽��С�Ӱ����������߽�����뿪
        /// </summary>
        /// <param name="WandererPlot"></param>
        /// <param name="isEnter"></param>
        void ExpTeamEnterOrLeave(Plot WandererPlot,bool isEnter)
        {
            foreach (var expTeam in WandererManager.Instance.exploratoryTeams)
            {
                Vector2Int v2 = new Vector2Int(WandererPlot.pos.x + expTeam.x, WandererPlot.pos.y + expTeam.y);
                if (this.plots.ContainsKey(v2))
                {
                    this.plots[v2].HaveExploratoryTeam = isEnter;
                }
            }
        }

        /// <summary>
        /// ����ѡ����չ̽��С��ģʽ
        /// </summary>
        /// <param name="isEnter"></param>
        public void EnterSelectExtendExpTeam(bool isEnter)
        {
            this.map_Mode =isEnter? Map_Mode.��չ̽��С��:Map_Mode.����;//�л���ͼģʽ

            WandererManager.Instance.wanderer.plot.ShowSelectedColor(isEnter);
            foreach (var expTeam in WandererManager.Instance.exploratoryTeams)
            {
                var v2 = expTeam + WandererManager.Instance.wanderer.plot.pos;
                if (this.plots[v2] != null)
                {
                    this.plots[v2].ShowSelectedColor(isEnter);
                }
            }
        }
    }
}
