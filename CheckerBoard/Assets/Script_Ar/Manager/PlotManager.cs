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

        //�Ƿ��Ҽ�ѡ���ƶ������е�������
        IDisposable select;
        //�Ƿ�ȡ���ƶ������е�������
        IDisposable cancel;

        [SerializeField, LabelText("�Ƿ��ƶ�����е�������"),ReadOnly]
        bool isMove;//�Ƿ��ƶ������е�������
        [SerializeField, LabelText("������İ��"), ReadOnly]
        Plot ini;//������İ��



        ///��ǰ��ѡ�еĸ���
        Plot SelectedPlot
        {
            set
            {
                if (value != null)
                {
                    if(value.wanderer!=null)
                    {
                        this.IsMoveWanaderer(value);
                    }
                    else
                    {
                        if(this.select!=null)
                        {
                            select.Dispose();
                        }
                        if (this.isMove)
                        {
                            this.MoveWanderer(value);
                        }
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
                    this.grids[new Vector2Int(i, j)].ChangeType(Plot_Type.δ̽��);
                }

            this.grids[new Vector2Int(0,0)].ChangeType(Plot_Type.�ѿ���);
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

            plot.Selected.Subscribe(v2 =>
            {
                if(this.grids[v2].plot_Type==Plot_Type.��̽��)
                {
                    UIManager.Instance.Show<UIBuilding>();//�򿪽���UIѡ�����
                }
                else
                {
                    UIManager.Instance.Close<UIBuilding>();//�رս���UIѡ�����
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
                    Debug.Log("ѡ����");
                    this.isMove = true;
                    this.ini = ini;

                    this.CancelMoveWanderer();
                });
        }

        /// <summary>
        /// ȷ���ƶ�������
        /// </summary>
        void MoveWanderer(Plot des)
        {
            this.isMove = false;
            WandererManager.Instance.WandererMoveTo(this.ini, des);
            if(this.cancel!=null)
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
                    this.isMove = false;
                    cancel.Dispose();
                });
        }
        #endregion

        #region �ı�������
        /// <summary>
        /// �ı�������
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="plot_Type"></param>
        public void PlotsChangeType(Vector2Int pos, Plot_Type plot_Type)
        {
            switch (plot_Type)
            {
                case Plot_Type.��̽��:
                    this.PlotsChangeToCanDiscover(pos);
                    break;
                case Plot_Type.��̽��:
                    this.PlotsChangeToDiscovered_Plot(pos);
                    break;
                case Plot_Type.�ѿ���:
                    this.PlotsChangeToDeveloped_Plot(pos);
                    break;
            }
        }
        /// <summary>
        /// ʹ��ΧһȦ���תΪ��̽��
        /// </summary>
        /// <param name="pos"></param>
        void PlotsChangeToCanDiscover(Vector2Int pos)
        {
            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                {
                    int x= pos.x + i;
                    int y= pos.y + j;
                    if (x == pos.x && y == pos.y)
                        continue;

                    Vector2Int v2 = new Vector2Int(x, y);
                    if (this.grids.ContainsKey(v2))
                    {
                        if (this.grids[v2].plot_Type==Plot_Type.δ̽��)
                        {
                            this.grids[v2].ChangeType(Plot_Type.��̽��);
                        }
                    }

                }
        }
        /// <summary>
        /// ��ǰ���תΪ��̽��
        /// </summary>
        /// <param name="pos"></param>
        void PlotsChangeToDiscovered_Plot(Vector2Int pos)
        {
            if (grids[pos].plot_Type == Plot_Type.��̽��)
            {
                this.grids[pos].ChangeType(Plot_Type.��̽��);
            }

        }
        /// <summary>
        /// ��ǰ���תΪ�ѿ���
        /// </summary>
        /// <param name="pos"></param>
        void PlotsChangeToDeveloped_Plot(Vector2Int pos)
        {
            if (grids[pos].plot_Type==Plot_Type.��̽��)
            {
                this.grids[pos].ChangeType(Plot_Type.�ѿ���);
            }
        }
        #endregion
    }
}
