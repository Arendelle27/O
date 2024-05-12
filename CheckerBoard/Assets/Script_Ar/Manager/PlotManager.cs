using ENTITY;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UniRx;
using System;
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
        [SerializeField, LabelText("���ڵ���̽���ؿ�"), ReadOnly]
        public HashSet<Vector2Int> haveExploredPlots=new HashSet<Vector2Int>();

        [SerializeField, LabelText("���ڵĿ�̽���ؿ�"), ReadOnly]
        HashSet<Vector2Int> canExploredPlots = new HashSet<Vector2Int>();

        //���浱ǰ���ڵĸ���
        public Dictionary<Vector2Int, Plot> plots = new Dictionary<Vector2Int, Plot>();

        [SerializeField, LabelText("��ǰ���ڵķ��������ɵؿ�"), ReadOnly]
        internal Dictionary<int, PlotDefine> plotTypeDesepical = new Dictionary<int, PlotDefine>();

        [SerializeField, LabelText("��ǰ���ڵĵؿ�����"), ReadOnly]
        public List<HashSet<int>> plotTypes = new List<HashSet<int>>(2) {new HashSet<int>(), new HashSet<int>()};
        //�洢��������,0Ϊ��Դ�ؿ飬1Ϊ�¼��ؿ�

        [SerializeField, LabelText("��ǰ����Ȩ���ܺ�"),ReadOnly]
        int weightTotal = 0;//����Ȩ���ܺ�

        [SerializeField, LabelText("������������"), ReadOnly]
        internal List<Dictionary<int,int>> plotConditions = new List<Dictionary<int, int>>(3) { new Dictionary<int, int>(), new Dictionary<int, int>(), new Dictionary<int, int>() };
        //�洢��������,0Ϊ���,1Ϊ�غ�,2Ϊ����

        [SerializeField, LabelText("�������ӵĵ���"), ReadOnly]
        internal Dictionary<Prop_Type, bool> unloadProp=new Dictionary<Prop_Type, bool>();
        [SerializeField, LabelText("���߽����¼�"), ReadOnly]
        Dictionary<Prop_Type, IDisposable> unlockIDisposableProp = new Dictionary<Prop_Type, IDisposable> { };

        //�غ�����������
        IDisposable unlockByRound;
        #endregion

        [SerializeField, LabelText("��ͼģʽ"), ReadOnly]
        public Map_Mode map_Mode = Map_Mode.����;

        ////�Ƿ��Ҽ�ѡ���ƶ������е�������
        //IDisposable select;
        ////�Ƿ�ȡ���ƶ������е�������
        //IDisposable cancel;

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
                        //case Map_Mode.����:
                        //    if (this.select != null)
                        //    {
                        //        select.Dispose();
                        //    }

                        //    if (value.wanderer != null)
                        //    {
                        //        this.IsMoveWanaderer(value);
                        //    }
                        //    break;
                        case Map_Mode.ѡ��Ŀ�ĵ�λ��:
                            if (this.canMovePlots.Contains(value)&& value.wanderer == null)
                            {
                                this.CaculateMoveExecutionCost(value);
                            }
                            break;
                        case Map_Mode.��չ̽��С��:
                            if(CapabilityManager.Instance.expendExploratoryAmount > 0)
                            {
                                WandererManager.Instance.ExtendExpTeam(value);//��չ̽��С��
                            }
                            break;
                    }

                }
            }
        }

        [SerializeField, LabelText("�ƶ����ĵ��ж���"), ReadOnly]
        public int moveExecutionCost;//�ƶ����ĵ��ж���

        void Start()
        {

            this.ObserveEveryValueChanged(_ => this.haveExploredPlots.Count).Subscribe(_ =>
            {
                this.UpdateCanExplorePlot();
            });

            this.ObserveEveryValueChanged(_ => this.moveExecutionCost).Subscribe(_ =>
            {
                (UIMain.Instance.uiPanels[2] as UIMovePanel).curCxecutiontext.text = this.moveExecutionCost.ToString();
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
            for (int i = -3; i <= 3; i++)
                for (int j = -3; j <= 3; j++)
                {
                    this.GetPlotAndDefine(new Vector2Int(i, j));
                }
            yield return null;

            this.plots[new Vector2Int(0, 0)].canEnter = true;

            this.SelectedPlot = this.plots[new Vector2Int(0, 0)];
        }

        /// <summary>
        /// ��ȡ�浵
        /// </summary>
        public IEnumerator ReadArchive()
        {
            this.Init();
            ArchiveManager.PlotManagerData plotManagerData = ArchiveManager.archive.plotManagerData;
            yield return null;

            for(int i=0;i<this.plotTypes.Count;i++)
            {
                HashSet<int> hs=new HashSet<int>(plotManagerData.plotTypes[i].plotTypes);
                //foreach (var id in plotManagerData.plotTypes[i].plotTypes)
                //{
                //    hs.Add(id);
                //}
                this.plotTypes[i] = hs;
            }

            for(int i = 0; i < this.plotConditions.Count; i++)
            {
                Dictionary<int, int> dic = new Dictionary<int, int>();
                for(int j = 0; j < plotManagerData.plotConditions[i].ids.Count; j++)
                {
                    dic.Add(plotManagerData.plotConditions[i].ids[j], plotManagerData.plotConditions[i].conditions[j]);
                }
                this.plotConditions[i] = dic;


            }

            foreach(var plotTypeDespicalId in plotManagerData.plotTypeDesepicalIds)
            {
                this.plotTypeDesepical.Add(plotTypeDespicalId, DataManager.PlotDefines[plotTypeDespicalId]);
            }

            foreach (var prop in plotManagerData.unloadPropsData)
            {
                this.unloadProp.Add(prop.propName, prop.isUnloaded);
                this.unlockIDisposableProp.Add(prop. propName, null);
            }

            foreach (var plot in plotManagerData.plotsData)
            {
                Plot p = this.GetGrid(plot.pos);
                p.SetInfo(DataManager.PlotDefines[plot.plotDefineId]);
                p.ChangeType(plot.plotStatue);
                p.canEnter = plot.canEnter;
                p.buildingResources = plot.buildingResources;
                p.isFirstExplored = plot.isFirstExplored;

                this.UnlockPlotByPlot(p);//���Ľ�������
            }

            yield return null;

            this.CalculateWeight();

            this.haveExploredPlots= new HashSet<Vector2Int>(plotManagerData.haveExploredPlots);

            this.UnlockByRound();

            foreach(var id in this.plotConditions[2].Keys)//���߽�������
            {
                this.unlockIDisposableProp[(Prop_Type)plotConditions[2][id]]= this.ObserveEveryValueChanged(_ => this.unloadProp[(Prop_Type)plotConditions[2][id]])
                    .Where(_ => this.unloadProp[(Prop_Type)plotConditions[2][id]])
                    .First()
                    .Subscribe(_ =>
                    {
                        //if (this.unloadProp[(Prop_Type)plotConditions[2][id]])
                        //{
                        this.plotTypes[DataManager.PlotDefines[id].Type].Add(id);
                        this.unlockIDisposableProp[(Prop_Type)plotConditions[2][id]].Dispose();
                        this.plotConditions[2].Remove(id);
                        Debug.Log("����ͨ�����߽�������");
                        this.PlotUnlock(id);

                        //}
                        QuestManager.Instance.QuestEnd(2, plotConditions[2][id]);
                    });
                break;
            }


            //foreach (var plot in ArchiveManager.archive.plotData)
            //{
            //    this.plots[plot.pos].ReadArchive(plot);
            //}
        }

        /// <summary>
        /// ��Ϸ����
        /// </summary>
        public void GameOver()
        {
            if (this.unlockByRound != null)//ȡ�����ĻغϽ�������
            {
                unlockByRound.Dispose();
            }
            for (int i = 0; i < this.unlockIDisposableProp.Count;)
            {
                var id = this.unlockIDisposableProp.ElementAt(i);
                if (id.Value != null)
                {
                    id.Value.Dispose();
                }
                this.unlockIDisposableProp.Remove(id.Key);
            }
            this.map_Mode = Map_Mode.����;

            this.plotTypeDesepical.Clear();
            this.haveExploredPlots.Clear();
            this.canExploredPlots.Clear();
            this.plotTypes = new List<HashSet<int>>(3) { new HashSet<int>(), new HashSet<int>() };
            this.weightTotal = 0;
            this.plotConditions = new List<Dictionary<int, int>>(3) { new Dictionary<int, int>(), new Dictionary<int, int>(), new Dictionary<int, int>() };
            this.unloadProp = new Dictionary<Prop_Type, bool>();

            this.RemoveAllPlot();
        }

        /// <summary>
        /// ���ɸ���
        /// </summary>
        /// <param name="pos"></param>
        Plot GetPlotAndDefine(Vector2Int pos)
        {
            Plot plot = this.GetGrid(pos);
            if (pos == new Vector2Int(0, 0))
            {
                plot.SetInfo(DataManager.PlotDefines[7]);
            }
            else
            {
                plot.SetInfo(this.GetPlotDefine(pos));
            }
            this.UnlockPlotByPlot(plots[plot.pos]);//���Ľ�������
            return plot;
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
            //plot.Discover(false);//�ر���ͼ

            this.plots.Add(pos, plot);

            //���ĵ�������¼�
            plot.clickSelectedSubject.Subscribe(plot =>
            {
                this.SelectedPlot = plot;
                if (this.map_Mode == Map_Mode.����)//��ͼģʽΪ����
                {
                    if(plot.wanderer == null)//������û��������
                        return;

                    if (plot.plotDefine.CanBuild//�����Խ���
                        && plot.building == null//�����û�н���
                        )
                    {
                        UISelectedWindow uSW= UIManager.Instance.Show<UISelectedWindow>();
                        uSW.OpenWindow(0);

                    }
                    if (plot.building != null)//������н���
                    {
                        BuildingManager.Instance.selectedBuilding = plot.building;
                        UISelectedWindow uSW = UIManager.Instance.Show<UISelectedWindow>();
                        uSW.OpenWindow(1);
                    }
                    if(plot.plotDefine.EventType==Event_Area_Type.����&&plot.wanderer!=null)//��������¼�����
                    {
                        //EventAreaManager.Instance.selectedEventArea = plot.eventArea;
                        UISelectedWindow uSW = UIManager.Instance.Show<UISelectedWindow>();
                        uSW.OpenWindow(2);
                    }
                    if(EventManager.Instance.curClashArea!=null&& plot.eventArea== EventManager.Instance.curClashArea)//������г�ͻ����
                    {
                        UISelectedWindow uSW = UIManager.Instance.Show<UISelectedWindow>();
                        uSW.OpenWindow(3);
                    }
                    if(plot.plotType==0)
                    {
                        UISelectedWindow uSW = UIManager.Instance.Show<UISelectedWindow>();
                        uSW.OpenWindow(4);
                    }
                }
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
                        this.plotTypes[pD.Type].Add(pD.ID);
                        this.UpdateWeightTotalsAndDespeicalType(false);
                        break;
                    case Plot_Condition_Type.���:
                        this.plotConditions[0].Add(pD.ID, pD.UnlockValue);
                        break;
                    case Plot_Condition_Type.�غ�:
                        this.plotConditions[1].Add(pD.ID, pD.UnlockValue);
                        break;
                    case Plot_Condition_Type.����:
                        this.plotConditions[2].Add(pD.ID, pD.UnlockValue);
                        this.unloadProp.Add((Prop_Type)pD.UnlockValue, false);
                        this.unlockIDisposableProp.Add((Prop_Type)pD.UnlockValue, null);

                        this.unlockIDisposableProp[(Prop_Type)plotConditions[2][pD.ID]]=this.ObserveEveryValueChanged(_=>this.unloadProp[(Prop_Type)plotConditions[2][pD.ID]])
                            .Where(_=>this.unloadProp[(Prop_Type)plotConditions[2][pD.ID]])
                            .First()
                            .Subscribe(_ =>
                        {
                            if (this.unloadProp[(Prop_Type)plotConditions[2][pD.ID]])
                            {
                                this.plotTypes[pD.Type].Add(pD.ID);
                                this.unlockIDisposableProp[(Prop_Type)plotConditions[2][pD.ID]].Dispose();
                                this.plotConditions[2].Remove(pD.ID);
                                Debug.Log("����ͨ�����߽�������");
                                this.PlotUnlock(pD.ID);
                            }
                        });
                        break;
                }
            }

            this.UnlockByRound();
        }

        /// <summary>
        /// ͨ���غϽ�������
        /// </summary>
        void UnlockByRound()
        {
            unlockByRound = RoundManager.Instance.unlockPlotByRound.Subscribe(roundNumber =>
            {
                for (int i = 0; i < this.plotConditions[1].Count;)
                {
                    var id = this.plotConditions[1].ElementAt(i).Key;
                    if (this.plotConditions[1][id] <= roundNumber)
                    {
                        this.plotTypes[DataManager.PlotDefines[id].Type].Add(id);
                        this.plotConditions[1].Remove(id);
                        Debug.Log("����ͨ���غϽ�������");
                        this.PlotUnlock(id);
                    }
                    else
                    {
                        i++;
                    }
                }
                if (this.plotConditions[1].Count == 0)
                {
                    unlockByRound.Dispose();
                }
            });
        }

        /// <summary>
        /// ���¸���Ȩ���ܺͷ��������ɵĸ�������,��������Ҫ�������ɷ��������ɵĸ���
        /// </summary>
        /// <param name="index"></param>
        void UpdateWeightTotalsAndDespeicalType(bool isRegenerateDefine=false)
        {
            Dictionary<int, PlotDefine> dic = new Dictionary<int, PlotDefine>();
            for (int n = 0; n < this.plotTypes.Count; n++)
            {
                foreach (var id in this.plotTypes[n])
                {
                    if (!this.plotTypeSepical.ContainsValue(id))
                    {
                        dic.Add(id, DataManager.PlotDefines[id]);
                    }
                }
            }
            this.plotTypeDesepical=dic;

            this.CalculateWeight();

            if(isRegenerateDefine)
            {
                //�������ɷ��������ɵĸ���
                this.RegenerateDespecialPlotDefine();
            }
        }


        /// <summary>
        /// �������Ȩ��
        /// </summary>
        void CalculateWeight()
        {
            this.weightTotal = 0;
            //�������Ȩ���ܺ�
            for (int i = 0; i < plotTypes.Count; i++)
            {
                foreach (var id in this.plotTypes[i])
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
        /// Npc������뿪���
        /// </summary>
        /// <param name="isAppear"></param>
        /// <param name="pos"></param>
        /// <param name="npc"></param>
        public Plot NPCAppear(Vector2Int pos)
        {
            Plot plot = null;
            if(this.plots.ContainsKey(pos))
            {
                plot = this.plots[pos];
            }
            else
            {
                plot=this.GetPlotAndDefine(pos);
            }
            return plot;
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
        public void ExpendPlot(Vector2Int pos)
        {
            foreach (var expTeam in WandererManager.Instance.exploratoryTeams.Keys)
            {
                List<int> xs = new List<int>() { 0 };
                List<int> ys = new List<int>() { 0 };
                if (expTeam.x >= 0)
                {
                    xs.AddRange(new List<int>{-1,1,2});
                }
                if (expTeam.x <= 0)
                {
                    xs.AddRange(new List<int> { -1, -2 ,1});
                }

                if (expTeam.y >= 0)
                {
                    ys.AddRange(new List<int> {-1, 1, 2 });
                }
                if (expTeam.y <= 0)
                {
                    ys.AddRange(new List<int> { -1, -2 ,1});
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
                            //Plot plot= this.GetGrid(v2);//���ɸ���
                            ////PlotDefine pD = this.GetPlotDefine(v2);
                            ////if (pD.IsSpecialGeneration)//�Ƿ�����������
                            ////{
                            ////    if (this.plotType[0].Contains(pD.ID) || this.plotType[1].Contains(pD.ID))//�Ƿ��ѽ���
                            ////    {
                            ////        this.plots[v2].SetInfo(pD);
                            ////    }
                            ////    else
                            ////    {
                            ////        this.plots[v2].SetInfo(pD, false);
                            ////    }
                            ////}
                            ////else
                            ////{
                            ////    this.plots[v2].SetInfo(pD);
                            ////}
                            //this.plots[plot.pos].SetInfo(this.GetPlotDefine(plot.pos));
                            //this.UnlockPlotByPlot(plots[v2]);//���Ľ�������
                            this.GetPlotAndDefine(v2);
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
            if (this.plotConditions[0].ContainsValue(plot.plotDefine.ID))
            {
                //���Ľ��������¼�
                plot.unLoadByPlot=new Subject<int>();
                plot.unLoadByPlot
                    .First()
                    .Subscribe(id =>
                {
                    //this.plotTypes[plot.plotDefine.Type].Add(plot.plotDefine.ID);
                    List<int> unlockPlotDefineIds = new List<int>();
                    for(int i = 0; i < this.plotConditions[0].Count; i++)
                    {
                        var item = this.plotConditions[0].ElementAt(i);
                        if (item.Value == id)
                        {
                            unlockPlotDefineIds.Add(item.Key);
                            this.plotTypes[plot.plotDefine.Type].Add(item.Key);
                            break;
                        }
                    }
                    foreach (var unlockPlotDefineId in unlockPlotDefineIds)
                    {
                        this.plotConditions[0].Remove(unlockPlotDefineId);

                        Debug.Log("����ͨ������������");
                        this.PlotUnlock(unlockPlotDefineId);
                    }
                    plot.unLoadByPlot.Dispose();

                });
            }
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="plotDefineId"></param>
        void PlotUnlock(int plotDefineId)
        {
            if (this.plotTypeSepical.ContainsValue(plotDefineId))
            {
                //����������
                MessageManager.Instance.AddMessage(Message_Type.̽��, string.Format("��ͼ�ϵ�{0}���Խ�����", DataManager.PlotDefines[plotDefineId].Name));
                foreach (var pos in this.plotTypeSepical.Keys)
                {
                    if(this.plotTypeSepical[pos] == plotDefineId) //�Ƕ�Ӧ����
                    {
                        Plot plot = null;
                        if (this.plots.ContainsKey(pos) )//������
                        {
                            plot = this.plots[pos];
                        }
                        else
                        {
                            plot = this.GetPlotAndDefine(pos);
                        }
                        plot.canEnter = true;
                        if (plot.plotDefine.Name == "�����ң�δ������")//����ǵ�����
                        {
                            plot.SR.sprite = SpriteManager.plotSprites["�����ң�������"];
                        }
                    }

                }
            }
            else
            {
                MessageManager.Instance.AddMessage(Message_Type.̽��, string.Format("��ͼ�ϰ�鷢���˾޴�ı仯��������{0}", DataManager.PlotDefines[plotDefineId].Name));
                //����������
                this.UpdateWeightTotalsAndDespeicalType(true);
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

        public void IsMoveWanderer()
        {
            //if (this.map_Mode == Map_Mode.����)
            //{
                //ѡ���ƶ���
            Debug.Log("ѡ���ƶ���");

            if(ResourcesManager.Instance.execution<=0)
            {
                MessageManager.Instance.AddMessage(Message_Type.̽��, "�ж��㲻��");
                return;
            }
            this.EnterMoveWanderer(true);
            //}
        }

        //��¼���ƶ����İ��
        List<Plot> canMovePlots = new List<Plot>();
        //�ƶ�Ŀ����
        Plot moveAimPlot=null;

        /// <summary>
        /// ������ƶ���Χ
        /// </summary>
        void CaculateCanMoveScope()
        {
            if(this.canMovePlots.Count!=0)
            {
                this.canMovePlots.Clear();
            }
            float r = ResourcesManager.Instance.execution * 3 + 0.5f;
            for(int i = -Mathf.CeilToInt(r); i <= Mathf.CeilToInt(r); i++)
            {
                for (int j = -Mathf.CeilToInt(r); j <= Mathf.CeilToInt(r); j++)
                {
                    if (i * i + j * j <= r * r)
                    {
                        Vector2Int v2 = new Vector2Int(i, j)+WandererManager.Instance.wanderer.plot.pos;
                        if(!this.plots.ContainsKey(v2))
                        {
                            continue;
                        }
                        if (this.plots[v2].plot_Statue!=Plot_Statue.δ̽��)
                        {
                            this.plots[v2].CanMoveIn(true);

                            this.canMovePlots.Add(this.plots[v2]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// �������ĵ��ж���
        /// </summary>
        /// <param name="plot"></param>
        public void CaculateMoveExecutionCost(Plot plot)
        {
            this.moveAimPlot=plot;
            WandererManager.Instance.destinationSign.SetInfo(plot);//����Ŀ�ĵر�־
            float distance= Vector2Int.Distance(WandererManager.Instance.wanderer.plot.pos, plot.pos);
            for (int i = ResourcesManager.Instance.execution; i >0; i--)
            {
                if(distance>=(i-1)*3+0.5f)
                {
                    this.moveExecutionCost = i;
                    break;
                }
            }
        }

        /// <summary>
        /// ȷ���ƶ�������
        /// </summary>
        public bool MoveWanderer(bool isMove)
        {
            //this.EnterMoveWanderer(false);
            if (isMove)
            {
                if (this.moveAimPlot == null)//Ŀ�ĵ�Ϊ��
                {
                    return false;
                }
                if (!this.selectedPlot.canEnter)
                {
                    Debug.Log("Ŀ�ĵ���ʱ����ȥ");
                    MessageManager.Instance.AddMessage(Message_Type.̽��, "��֪Ϊ�Σ�Ŀ�ĵؽ���ȥ");
                    return false;
                }

                ResourcesManager.Instance.ChangeExecution(-this.moveExecutionCost);//�����ж���
                MainThreadDispatcher.StartUpdateMicroCoroutine(WandererManager.Instance.WandererMoveTo(this.moveAimPlot));//���������ƶ���ָ���İ��
                MessageManager.Instance.AddMessage(Message_Type.̽��, string.Format("����{0}�ж������ƶ�����{1}��{2}��", this.moveExecutionCost, this.moveAimPlot.pos.x, this.moveAimPlot.pos.y));

            }
            else
            {
                this.EnterMoveWanderer(false);
            }

            WandererManager.Instance.destinationSign.Hide();

            this.moveAimPlot = null;
            return true;
        }


        #endregion

        /// <summary>
        /// �����߽���������
        /// </summary>
        /// <param name="enterPlot"></param>
        public void WanderEnter(Plot enterPlot)
        {
            enterPlot.wanderer = WandererManager.Instance.wanderer;//�����߽����������

            this. ExpendPlot(enterPlot.pos);//��չ���

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
            foreach (var expTeam in WandererManager.Instance.exploratoryTeams.Keys)
            {
                Vector2Int v2 = new Vector2Int(WandererPlot.pos.x + expTeam.x, WandererPlot.pos.y + expTeam.y);
                if (this.plots.ContainsKey(v2))
                {
                    if(isEnter)
                    {
                        this.AddOrRemoveExpTeamPlot(v2, isEnter);
                        Debug.Log(v2 + "̽��С�Ӹ������");
                    }
                    else
                    {
                        this.plots[v2].HaveExploratoryTeam = false;
                    }
                    WandererManager.Instance.exploratoryTeams[expTeam].transform.position = this.plots[v2].transform.position;
                }
            }
            //this.UpdateCanExplorePlot();
        }

        /// <summary>
        /// ���ӻ��Ƴ�̽��С��
        /// </summary>
        /// <param name="v2"></param>
        /// <param name="isEnter"></param>
        public void AddOrRemoveExpTeamPlot(Vector2Int v2,bool isEnter)
        {

                this.plots[v2].HaveExploratoryTeam = isEnter;
                if (isEnter && !this.haveExploredPlots.Contains(v2))
                {
                    this.haveExploredPlots.Add(v2);
                }
                else if (!isEnter && this.haveExploredPlots.Contains(v2))
                {
                    this.haveExploredPlots.Remove(v2);
                }
            
        }

        /// <summary>
        /// ���¿�̽������
        /// </summary>
        void UpdateCanExplorePlot()
        {
            //foreach (var canExploredPlot in this.canExploredPlots)
            //{
            //    if (!this.plots[canExploredPlot].HaveExploratoryTeam/*&& this.plots[canExploredPlot].plot_Statue!=Plot_Statue.��̽��*/)
            //    {
            //        this.plots[canExploredPlot].ChangeType(Plot_Statue.δ̽��);
            //    }
            //}
            this.canExploredPlots.Clear();

            foreach (var exploredPlot in this.haveExploredPlots)
            {

                for (int x=-1;x<=1;x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector2Int pos = exploredPlot + new Vector2Int(x, y);
                        if(!this.plots.ContainsKey(pos))
                        {
                            continue;
                        }
                        if (!this.haveExploredPlots.Contains(pos))
                        {
                            this.plots[pos].ChangeType(Plot_Statue.��̽��);
                            this.canExploredPlots.Add(pos);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ������˳��ƶ�������ģʽ
        /// </summary>
        /// <param name="isEnter"></param>
        public void EnterMoveWanderer(bool isEnter)
        {
            if(isEnter)
            {
                this.map_Mode = Map_Mode.ѡ��Ŀ�ĵ�λ��;

                this.moveExecutionCost = 0;
                UIMain.Instance.ChangeToGamePanel(2);//ѡ�����ʱ���ƶ�UI����
                this.CaculateCanMoveScope();
            }
            else
            {
                this.map_Mode = Map_Mode.����;
                UIMain.Instance.ChangeToGamePanel(1);//ѡ��������UI����

                foreach (var plot in this.canMovePlots)
                {
                    plot.CanMoveIn(false);
                }

                this.canMovePlots.Clear();
            }
        }

        /// <summary>
        /// ������˳���չģʽ
        /// </summary>
        /// <param name="isExtend"></param>
        public void EnterSelectExtendExpTeam(bool isExtend)
        {
            if(isExtend) 
            {
                this.map_Mode = Map_Mode.��չ̽��С��;
                UIMain.Instance.ChangeToGamePanel(3);
                if (NoviceGuideManager.Instance.isGuideStage[5])//�Ƿ�������ָ���׶�
                {
                    NoviceGuideManager.Instance.uINoviceGuidePanel.masks[0].gameObject.SetActive(false);
                }
            }
            else
            {
                this.map_Mode = Map_Mode.����;
                UIMain.Instance.ChangeToGamePanel(1);//�ָ�����Ϸ����
                if (NoviceGuideManager.Instance.isGuideStage[5])//�Ƿ�������ָ���׶�
                {
                    NoviceGuideManager.Instance.NoviceGuideStage++;
                }
                //UIManager.Instance.Show<UIStrengthenCapabilityWindow>();
            }
        }
    }
}
