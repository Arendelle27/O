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
using System.Linq;

namespace MANAGER
{
    public class PlotManager : MonoSingleton<PlotManager>
    {

        [SerializeField, LabelText("��ͼ"), Tooltip("��Ƭ��ͼ")]
        public Tilemap map;

        [SerializeField, LabelText("��ͼ��Χ"), Tooltip("0Ϊ��߽磬1Ϊ�ұ߽磬2Ϊ�±߽磬3Ϊ�ϱ߽�")]
        public List<int> mapScope = new List<int>(4) {-10,10,-10,10 };
        //��ͼ��Χ,0Ϊ��߽磬1Ϊ�ұ߽磬2Ϊ�±߽磬3Ϊ�ϱ߽�

        [SerializeField, LabelText("���е��������ɵĵؿ�"), ReadOnly]
        public Dictionary<Vector2Int,int> plotTypeSepical;//keyΪλ�ã�valueΪ��������

        #region ÿ����Ϸ��ʼʱ��ʼ��������
        //���浱ǰ���ڵĸ���
        public Dictionary<Vector2Int, Plot> plots = new Dictionary<Vector2Int, Plot>();

        [SerializeField, LabelText("��ǰ���ڵķ��������ɵؿ�"), ReadOnly]
        internal Dictionary<int, PlotDefine> plotTypeDesepical = new Dictionary<int, PlotDefine>();

        [SerializeField, LabelText("��ǰ���ڵĵؿ�����"), ReadOnly]
        public List<HashSet<int>> plotType = new List<HashSet<int>>(2) {new HashSet<int>(), new HashSet<int>()};
        //�洢��������,0Ϊ��Դ�ؿ飬1Ϊ�¼��ؿ�

        [SerializeField, LabelText("��ǰ����Ȩ���ܺ�"),ReadOnly]
        int weightTotal = 0;//����Ȩ���ܺ�

        [SerializeField, LabelText("������������"), ReadOnly]
        internal List<Dictionary<int,string>> plotCondition = new List<Dictionary<int, string>>(3) {new Dictionary<int, string>(),new Dictionary<int, string>(),new Dictionary<int, string>() };
        //�洢��������,0Ϊ���,1Ϊ�غ�,2Ϊ����

        [SerializeField, LabelText("�������ӵĵ���"), ReadOnly]
        internal Dictionary<string, bool> unloadProp;

        //�غ�����������
        IDisposable unlockByRound;
        #endregion

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
                            if (value.HaveExploratoryTeam&& value.wanderer == null)
                            {
                                this.MoveWanderer(value);
                            }
                            break;
                        case Map_Mode.��չ̽��С��:
                            if(ResourcesManager.Instance.levelPromptionAmount>0)
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
            this.ObserveEveryValueChanged(_ => this.plotType[0].Count).Subscribe(_ =>
            {
                this.UpdateWeightTotalsAndDespeicalType();


            });

            this.ObserveEveryValueChanged(_ => this.plotType[1].Count).Subscribe(_ =>
            {
                this.UpdateWeightTotalsAndDespeicalType();
            });
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public void Init()
        {
            if (this.plotTypeSepical == null)
            {
                this.plotTypeSepical = new Dictionary<Vector2Int, int>();
                foreach (var pD in DataManager.PlotDefines.Values)
                {
                    if (pD.IsSpecialGeneration)
                    {
                        for (int i = pD.HorizontalMin; i <= pD.HorizontalMax; i++)
                        {
                            for (int j = pD.VerticalMin; j <= pD.VerticalMax; j++)
                            {
                                Vector2Int v2 = new Vector2Int(i, j);
                                if (this.plotTypeSepical.ContainsKey(v2))
                                {
                                    this.plotTypeSepical[v2] = pD.ID;
                                }
                                else
                                {
                                    this.plotTypeSepical.Add(new Vector2Int(i, j), pD.ID);
                                }
                            }
                        }
                    }
                }
            }//��ʼ���������ɵĸ�������

            if(this.unlockByRound!=null)//ȡ�����ĻغϽ�������
            {
                unlockByRound.Dispose();
            }

            this.RemoveAllPlot();

            this.plotType = new List<HashSet<int>>(3) { new HashSet<int>(), new HashSet<int>() };
            this.weightTotal = 0;
            this.plotCondition = new List<Dictionary<int, string>>(3) { new Dictionary<int, string>(), new Dictionary<int, string>(), new Dictionary<int, string>() };
            this.unloadProp = new Dictionary<string, bool>();

            this.map_Mode = Map_Mode.����;
        }

        /// <summary>
        /// �ؿ�
        /// </summary>
        public IEnumerator Restart()
        {
            this.Init();
            yield return null;
            this.InitPlotType();
            yield return null;
            for (int i = -5; i <= 5; i++)
                for (int j = -3; j <= 3; j++)
                {
                    this.GetPlotAndDefine(new Vector2Int(i, j));
                }
            yield return null;

            this.SelectedPlot = this.plots[new Vector2Int(0, 0)];
        }

        /// <summary>
        /// ��ȡ�浵
        /// </summary>
        public void ReadArchive()
        {
            //foreach (var plot in ArchiveManager.archive.plotData)
            //{
            //    this.plots[plot.pos].ReadArchive(plot);
            //}
        }

        /// <summary>
        /// ���ɸ���
        /// </summary>
        /// <param name="pos"></param>
        void GetPlotAndDefine(Vector2Int pos)
        {
            Plot plot = this.GetGrid(pos);
            if(pos==new Vector2Int(0,0))
            {
                plot.SetInfo(DataManager.PlotDefines[7]);
            }
            else
            {
                plot.SetInfo(this.GetPlotDefine(pos));
            }
            this.UnlockPlotByPlot(plots[plot.pos]);//���Ľ�������
        }

        /// <summary>
        /// �ڸ�����λ�ò�������
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        Plot GetGrid(Vector2Int pos)
        {
            GameObject gO = GameObjectPool.Instance.Plots.Get();
            gO.transform.parent = this.transform;
            Vector3Int v3= new Vector3Int(pos.x, pos.y, 0);
            gO.transform.position = this.map.GetCellCenterWorld(v3);

            Plot plot = gO.GetComponent<Plot>();
            plot.pos = pos;

            this.plots.Add(pos, plot);

            //���ĵ�������¼�
            plot.clickSelectedSubject.Subscribe(p =>
            {
                if(this.map_Mode == Map_Mode.����)//��ͼģʽΪ����
                {
                    if(p.wanderer == null)//������û��������
                        return;

                    if (p.plotDefine.CanBuild//�����Խ���
                        && p.building == null//�����û�н���
                        )
                    {
                        UISelectedWindow uSW= UIManager.Instance.Show<UISelectedWindow>();
                        uSW.OpenWindow(0);

                    }
                    if (p.building != null)//������н���
                    {
                        BuildingManager.Instance.selectedBuilding = p.building;
                        UISelectedWindow uSW = UIManager.Instance.Show<UISelectedWindow>();
                        uSW.OpenWindow(1);
                    }
                    if(p.eventArea!=null)//��������¼�����
                    {
                        EventAreaManager.Instance.selectedEventArea = p.eventArea;
                        UISelectedWindow uSW = UIManager.Instance.Show<UISelectedWindow>();
                        uSW.OpenWindow(2);
                    }
                }

                this.SelectedPlot = p;
            });

            return plot;
        }

        /// <summary>
        /// ��ʼ����������
        /// </summary>
        void InitPlotType()
        {
            foreach (var pD in DataManager.PlotDefines.Values)
            {
                switch (pD.Condition)
                {
                    case Plot_Condition_Type.��:
                        this.plotType[pD.Type].Add(pD.ID);
                        break;
                    case Plot_Condition_Type.���:
                        this.plotCondition[0].Add(pD.ID, pD.UnlockValue);
                        break;
                    case Plot_Condition_Type.�غ�:
                        this.plotCondition[1].Add(pD.ID, pD.UnlockValue);
                        break;
                    case Plot_Condition_Type.����:
                        this.plotCondition[2].Add(pD.ID, pD.UnlockValue);
                        this.unloadProp.Add(pD.UnlockValue, false);

                        this.ObserveEveryValueChanged(_=>this.unloadProp[plotCondition[2][pD.ID]])
                            .First()
                            .Subscribe(_ =>
                        {
                            if (this.unloadProp[plotCondition[2][pD.ID]])
                            {
                                this.plotType[pD.Type].Add(pD.ID);
                                this.plotCondition[2].Remove(pD.ID);
                                Debug.Log("����ͨ�����߽�������");
                                if (this.plotTypeSepical.ContainsValue(pD.ID))
                                {
                                    //����������
                                    foreach (var pos in this.plotTypeSepical.Keys)
                                    {
                                        if (this.plots.ContainsKey(pos) && this.plots[pos].plotDefine.ID == pD.ID)//�����ڲ����Ƕ�Ӧ����
                                        {
                                            this.plots[pos].canEnter = true;
                                        }
                                    }
                                }
                                else
                                {
                                    //����������
                                    this.RegenerateDespecialPlotDefine();
                                }
                            }
                        });
                        break;
                }
            }

            unlockByRound=RoundManager.Instance.unlockPlotByRound
                .Subscribe(roundNumber =>
                {
                    for(int i = 0; i < this.plotCondition[1].Count;)
                    {
                        var id = this.plotCondition[1].ElementAt(i).Key;
                        if (int.Parse(this.plotCondition[1][id]) == roundNumber)
                        {
                            this.plotType[DataManager.PlotDefines[id].Type].Add(id);
                            this.plotCondition[1].Remove(id);
                            Debug.Log("����ͨ���غϽ�������");
                            if(this.plotTypeSepical.ContainsValue(id))
                            {
                                //����������
                                foreach (var pos in this.plotTypeSepical.Keys)
                                {
                                    if (this.plots.ContainsKey(pos) && this.plots[pos].plotDefine.ID==id)//�����ڲ����Ƕ�Ӧ����
                                    {
                                        this.plots[pos].canEnter = true;
                                    }
                                }
                            }
                            else
                            {
                                //����������
                                this.RegenerateDespecialPlotDefine();
                            }
                        }
                        else
                        {
                            i++;
                        }
                    }
                    if (this.plotCondition[1].Count==0)
                    {
                        unlockByRound.Dispose();
                    }
                });


        }

        /// <summary>
        /// ���¸���Ȩ���ܺ�
        /// </summary>
        /// <param name="index"></param>
        void UpdateWeightTotalsAndDespeicalType()
        {
            Dictionary<int, PlotDefine> dic = new Dictionary<int, PlotDefine>();
            for (int n = 0; n < this.plotType.Count; n++)
            {
                foreach (var id in this.plotType[n])
                {
                    if (!this.plotTypeSepical.ContainsValue(id))
                    {
                        dic.Add(id, DataManager.PlotDefines[id]);
                    }
                }
            }
            this.plotTypeDesepical=dic;

            this.weightTotal = 0;
            //�������Ȩ���ܺ�
            for(int i = 0;i<plotType.Count;i++)
            {
                foreach (var id in this.plotType[i])
                {
                    this.weightTotal += DataManager.PlotDefines[id].Weights;
                }
            }
        }

        /// <summary>
        /// ���ø�������
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        PlotDefine GetPlotDefine(Vector2Int pos)
        {
            if(this.plotTypeSepical.ContainsKey(pos))
            {
                return DataManager.PlotDefines[this.plotTypeSepical[pos]];
            }

            int r = UnityEngine.Random.Range(0, this.weightTotal);
            int left = 0;
            int right = 0;

            for (int i = 0; i < plotTypeDesepical.Count; i++)
            {
                var item = plotTypeDesepical.ElementAt(i);
                right += item.Value.Weights;
                if (r >= left && r <= right)
                {
                    return item.Value;
                }
                left = right;
            }

            return null;
        }


        /// <summary>
        /// �������ɷ�δ̽����鶨��
        /// </summary>
        /// <param name="sort"></param>
        void RegenerateDespecialPlotDefine()//0������Դ�ؿ飬1�����¼��ؿ�
        {
            foreach (var plot in this.plots.Values)
            {
                if (!plot.plotDefine.IsSpecialGeneration && plot.plot_Statue==Plot_Statue.δ̽��)//����������,״̬Ϊδ̽��,��Ϊָ������
                {
                    int r = UnityEngine.Random.Range(0, this.weightTotal);
                    int left = 0;
                    int right = 0;

                    for (int i = 0; i < plotTypeDesepical.Count; i++)
                    {
                        var item = plotTypeDesepical.ElementAt(i).Value;
                        if (item.IsSpecialGeneration)
                        {
                            continue;
                        }
                        right += item.Weights;
                        if (r >= left && r <= right)
                        {
                            plot.SetInfo(item);
                            break;
                        }
                        left = right;
                    }
                }
            }
        }


        /// <summary>
        /// ��չ���
        /// </summary>
        /// <param name="pos"></param>
        void ExpendPlot(Vector2Int pos)
        {
            foreach (var expTeam in WandererManager.Instance.exploratoryTeams)
            {
                List<int> xs = new List<int>() { 0 };
                List<int> ys = new List<int>() { 0 };
                if (expTeam.x >= 0)
                {
                    xs.AddRange(new List<int>{1,2});
                }
                else if (expTeam.x <= 0)
                {
                    xs.AddRange(new List<int> { -1, -2 });
                }

                if (expTeam.y >= 0)
                {
                    ys.AddRange(new List<int> { 1, 2 });
                }
                else if (expTeam.y <= 0)
                {
                    ys.AddRange(new List<int> { -1, -2 });
                }

                foreach (var x in xs)
                {
                    foreach (var y in ys)
                    {
                        int posX = pos.x + expTeam.x + x;
                        int posY = pos.y + expTeam.y + y;
                        if(posX < this.mapScope[0] || posX > this.mapScope[1] || posY < this.mapScope[2] || posY > this.mapScope[3])
                        {
                            continue;
                        }
                        Vector2Int v2 = new Vector2Int(posX,posY);
                        if (v2 == expTeam)
                        {
                            continue;
                        }
                        if (!this.plots.ContainsKey(v2))
                        {
                            Plot plot= this.GetGrid(v2);//���ɸ���
                            //PlotDefine pD = this.GetPlotDefine(v2);
                            //if (pD.IsSpecialGeneration)//�Ƿ�����������
                            //{
                            //    if (this.plotType[0].Contains(pD.ID) || this.plotType[1].Contains(pD.ID))//�Ƿ��ѽ���
                            //    {
                            //        this.plots[v2].SetInfo(pD);
                            //    }
                            //    else
                            //    {
                            //        this.plots[v2].SetInfo(pD, false);
                            //    }
                            //}
                            //else
                            //{
                            //    this.plots[v2].SetInfo(pD);
                            //}
                            this.plots[plot.pos].SetInfo(this.GetPlotDefine(plot.pos));
                            this.UnlockPlotByPlot(plots[v2]);//���Ľ�������
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ���ݰ��������������
        /// </summary>
        /// <param name="plot"></param>
        void UnlockPlotByPlot(Plot plot)
        {
            if (this.plotCondition[0].ContainsKey(plot.plotDefine.ID))
            {
                //���Ľ��������¼�
                this.plots[plot.pos].unLoadByPlot
                    .First()
                    .Subscribe(id =>
                {
                    this.plotType[plot.plotDefine.Type].Add(plot.plotDefine.ID);
                    this.plotCondition[0].Remove(plot.plotDefine.ID);
                    Debug.Log("����ͨ������������");
                    if (this.plotTypeSepical.ContainsValue(id))
                    {
                        //����������
                        foreach (var pos in this.plotTypeSepical.Keys)
                        {
                            if (this.plots.ContainsKey(pos) && this.plots[pos].plotDefine.ID == id)//�����ڲ����Ƕ�Ӧ����
                            {
                                this.plots[pos].canEnter = true;
                            }
                        }
                    }
                    else
                    {
                        //����������
                        this.RegenerateDespecialPlotDefine();
                    }
                });
            }
        }

        /// <summary>
        /// ������а��
        /// </summary>
        void RemoveAllPlot()
        {
            for(int i=0;i<this.plots.Count;)
            {
                var plot = this.plots.ElementAt(i).Value;
                GameObjectPool.Instance.Plots.Release(plot.gameObject);
                this.plots.Remove(plot.pos);
            }
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
            UIMain.Instance.ChangeToGamePanel(1);//ѡ��������UI����

            if (this.cancel != null)
            {
                this.cancel.Dispose();
            }

            if (!ResourcesManager.Instance.CanMove(1))//�ж��Ƿ����ж���
            {
                Debug.Log("�ж��㲻��");
                return;
            }

            if (!des.canEnter)//�ж��Ƿ����ж���
            {
                Debug.Log("Ŀ�ĵ���ʱ����ȥ");
                return;
            }

            MainThreadDispatcher.StartUpdateMicroCoroutine(WandererManager.Instance.WandererMoveTo(des));//���������ƶ���ָ���İ��
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

            this.ExpTeamEnterOrLeave(enterPlot, true);//̽��С�Ӱ����������߽���

            this. ExpendPlot(enterPlot.pos);//��չ���
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
