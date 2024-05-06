using ENTITY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Sirenix.OdinInspector;
using System.Linq;
using DG.Tweening;

namespace MANAGER
{
    public class WandererManager : MonoSingleton<WandererManager>
    {
        //���浱ǰ���ڵ�������
        [SerializeField, LabelText("������"), ReadOnly]
        public Wanderer wanderer;

        [SerializeField, LabelText("����̽��С�ӵ������������λ��"), ReadOnly]
        public Dictionary<Vector2Int,GameObject> exploratoryTeams=new Dictionary<Vector2Int, GameObject>();

        [SerializeField, LabelText("Ŀ�ĵ���ʾ��"), ReadOnly]
        public DestinationSign destinationSign;


        //���汾����չ�İ��
        [SerializeField, LabelText("������չ�İ��"), ReadOnly]
        public Stack<Vector2Int> exploredV2 = new Stack<Vector2Int>();


        /// <summary>
        /// ��ʼ��
        /// </summary>
        void Init()
        {

            if (this.destinationSign == null)
            {
                this.destinationSign = GameObjectPool.Instance.DestinationSign.GetComponent<DestinationSign>();
                this.destinationSign.transform.SetParent(this.transform);
            }
            this.destinationSign.gameObject.SetActive(false);

        }

        /// <summary>
        /// �ؿ�
        /// </summary>
        public void Restart()
        {
            this.Init();
            this.GetExplotryTeam(new Vector2Int(0, 0));
            this.GetWanderer(PlotManager.Instance.plots[new Vector2Int(0, 0)]);
        }

        /// <summary>
        /// ��ȡ�浵
        /// </summary>
        public void ReadArchive()
        {
            this.Init();
            ArchiveManager. WandererManagerData wandererManagerData = ArchiveManager.archive.wandererManagerData;
            foreach (var item in wandererManagerData.exploratoryTeams)
            {
                this.GetExplotryTeam(item);
            }
            this.GetWanderer(PlotManager.Instance.plots[wandererManagerData.wandererData.pos]);

        }

        public void GameOver()
        {
            for (int i = 0; i < exploratoryTeams.Count;)
            {
                var item = this.exploratoryTeams.ElementAt(i);
                this.RemoveExplotryTeam(item.Key);
            }

            if (this.wanderer != null)
            {
                this.wanderer.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// �ڸ����İ������������
        /// </summary>
        /// <param name="plot"></param>
        public void GetWanderer(Plot plot)
        {
            if(this.wanderer==null)
            {
                GameObject go = GameObjectPool.Instance.Wanderer;
                go.transform.SetParent(this.transform);
                go.gameObject.SetActive(true);
                Wanderer wanderer = go.GetComponent<Wanderer>();
                this.wanderer = wanderer;
            }
            else
            {
                this.wanderer.gameObject.SetActive(true);
            }
            //this.wanderer.transform.position = plot.transform.position + new Vector3(0, 0, ParameterConfig.entityHigh);
            MainThreadDispatcher.StartUpdateMicroCoroutine( this.WandererMoveTo(plot));
        }

        /// <summary>
        /// ����̽��С��
        /// </summary>
        /// <param name="pos"></param>
        public void GetExplotryTeam(Vector2Int pos)
        {
            GameObject gO = GameObjectPool.Instance.ExploratoryTeams.Get();
            this.exploratoryTeams.Add(pos, gO);
            if (this.wanderer!=null&&this.wanderer.gameObject.activeSelf)
            {
                pos += this.wanderer.plot.pos;
                PlotManager.Instance.ExpendPlot(this.wanderer.plot.pos);
            }
            gO.transform.position = PlotManager.Instance.plots[pos].transform.position;
            gO.transform.SetParent(this.transform);

            PlotManager.Instance.AddOrRemoveExpTeamPlot(pos,true);
        }

        /// <summary>
        /// ����̽��С��
        /// </summary>
        /// <param name="pos"></param>
        public void RemoveExplotryTeam(Vector2Int pos)
        {
            GameObjectPool.Instance.ExploratoryTeams.Release(this.exploratoryTeams[pos].gameObject);
            this.exploratoryTeams.Remove(pos);

            Vector2Int v2=pos+this.wanderer.plot.pos;
            PlotManager.Instance.AddOrRemoveExpTeamPlot(v2, false);
        }

        /// <summary>
        /// �������ƶ���ָ���İ��
        /// </summary>
        /// <param name="amiPlot"></param>
        public IEnumerator WandererMoveTo(Plot amiPlot)
        {

            if (this.wanderer.plot != null)
            {
                PlotManager.Instance.WanderLeave(this.wanderer.plot);
            }

            yield return null;
            Tweener tw = this.wanderer.transform.DOMove(amiPlot.transform.position + new Vector3(0, ParameterConfig.entityForward, ParameterConfig.entityHigh), 0.5f);
            tw.OnComplete(() =>
            {
                this.wanderer.plot = amiPlot;

                PlotManager.Instance.WanderEnter(amiPlot);
                PlotManager.Instance.EnterMoveWanderer(false);
            });


        }


        /// <summary>
        /// ��չ̽��С��
        /// </summary>
        /// <param name="aimPlot"></param>
        public void ExtendExpTeam(Plot aimPlot)
        {
            if (aimPlot.HaveExploratoryTeam||aimPlot.wanderer!=null)
                return;

            foreach (var expTeam in this.exploratoryTeams)
            {
                Vector2Int v2 = aimPlot.pos- this.wanderer.plot.pos;
                if ((v2 - expTeam.Key).magnitude <= 1)//�ж�Ŀ���������̽��С������,�Ҳ��������������ڵİ��
                {
                    this.GetExplotryTeam(v2);

                    this.exploredV2.Push(v2);//���汾����չ�İ��
                    CapabilityManager.Instance.expendExploratoryAmount--;

                    break;
                }
            }
        }

        /// <summary>
        /// ������һ��̽��С�ӵ���չ
        /// </summary>
        public void WithdrawExpTeam()
        {
            if (this.exploredV2.Count > 0)
            {
                Vector2Int v2 = this.exploredV2.Pop();
                this.RemoveExplotryTeam(v2);
                CapabilityManager.Instance.expendExploratoryAmount++;
            }
        }

        //�����غ��ж�
        public void RoundOver()
        {
            foreach (var expTeam in this.exploratoryTeams.Keys)
            {
                Vector2Int pos = this.wanderer.plot.pos + expTeam;
                if (PlotManager.Instance.plots.ContainsKey(pos))
                {
                    PlotManager.Instance.plots[pos].TeamExp();
                }
            }
        }
    }
}

