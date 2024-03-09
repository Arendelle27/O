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
        public Dictionary<Vector2Int, Plot> grids = new Dictionary<Vector2Int, Plot>();

        [SerializeField, LabelText("��ͼ"), Tooltip("��Ƭ��ͼ")]
        public Tilemap map;

        [SerializeField, LabelText("��ͼģʽ"), ReadOnly]
        public Map_Mode map_Mode = Map_Mode.����;

        //�Ƿ��Ҽ�ѡ���ƶ������е�������
        IDisposable select;
        //�Ƿ�ȡ���ƶ������е�������
        IDisposable cancel;

        //[SerializeField, LabelText("�Ƿ��ƶ�����е�������"),ReadOnly]
        //bool isMove;//�Ƿ��ƶ������е�������

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

        void Awake()
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
            this.SelectedPlot = this.grids[new Vector2Int(0, 0)];
            for (int i = -4; i < 5; i++)
                for (int j = -4; j < 5; j++)
                {
                    this.grids[new Vector2Int(i, j)].Init();
                }

            this.map_Mode = Map_Mode.����;
        }

        

        /// <summary>
        /// �ڸ�����λ�ò�������
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void GetGrid(Vector2Int pos)
        {
            GameObject gO = Instantiate(GameObjectPool.Instance.Plots.Get(), this.transform);
            Vector3Int v3= new Vector3Int(pos.x, pos.y, 0);
            gO.transform.position = this.map.GetCellCenterWorld(v3);

            Plot plot = gO.GetComponent<Plot>();
            plot.pos = pos;

            this.grids.Add(pos, plot);

            plot.clickSelectedSubject.Subscribe(v2 =>
            {
                if(this.grids[v2].plot_Type==Plot_Type.��̽��)
                {
                    UIManager.Instance.Show<UIBuildingWindow>();//�򿪽���UIѡ�����
                }
                else
                {
                    UIManager.Instance.Close<UIBuildingWindow>();//�رս���UIѡ�����
                }
                this.SelectedPlot = this.grids[v2];
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

                    //if((UIMain.Instance.uiPanels[1] as UIGamePanel).gameObject.activeSelf)
                    //{
                    //    (UIMain.Instance.uiPanels[1] as UIGamePanel).gameObject.SetActive(false);//ѡ�����ʱ�ر�UI����
                    //}

                    this.select.Dispose();
                });
        }

        /// <summary>
        /// ȷ���ƶ�������
        /// </summary>
        void MoveWanderer(Plot des)
        {
            this.map_Mode = Map_Mode.����;
            WandererManager.Instance.DestinationSignMoveTo(des);//��Ŀ�ĵ���ʾ���ƶ���ָ���İ��

            UIMain.Instance.ChangeToGamePanel(1);//ѡ��������UI����
            //(UIMain.Instance.uiPanels[1] as UIGamePanel).gameObject.SetActive(true);//ѡ��������UI����

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
                    //(UIMain.Instance.uiPanels[1] as UIGamePanel).gameObject.SetActive(true);//ȡ��ѡ�������UI����

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
            enterPlot.wanderer = WandererManager.Instance.wanderer;
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
                if (this.grids.ContainsKey(v2))
                {
                    this.grids[v2].HaveExploratoryTeam = isEnter;
                }
            }
        }
    }
}
