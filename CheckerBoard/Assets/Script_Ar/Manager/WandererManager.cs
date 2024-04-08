using ENTITY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Sirenix.OdinInspector;
using System.Linq;

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
            //this.Init();
            //this.GetWanderer(PlotManager.Instance.plots[ArchiveManager.archive.wandererData.pos]);
            //this.wanderer.level = ArchiveManager.archive.wandererData.level;

            //this.exploratoryTeams = ArchiveManager.archive.wandererData.exploratoryTeams;

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
                this.GetWanderer(PlotManager.Instance.plots[new Vector2Int(0, 0)]);
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
                Wanderer wanderer = go.GetComponent<Wanderer>();
                this.wanderer = wanderer;
            }
            else
            {
                this.wanderer.gameObject.SetActive(true);
            }

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

            PlotManager.Instance.AddOrRemoveExpTeamPlot(pos,false);
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

            this.wanderer.transform.position = amiPlot.transform.position - new Vector3(0, 0, 0.3f);
            this.wanderer.plot = amiPlot;

            PlotManager.Instance.WanderEnter(amiPlot);
            MessageManager.Instance.AddMessage(Message_Type.������, string.Format("�������ƶ���({0},{1})", amiPlot.pos.x, amiPlot.pos.y));
            //�����ж���
        }

        ///// <summary>
        ///// Ŀ����ʾ���ƶ���ָ���İ��
        ///// </summary>
        ///// <param name="des"></param>
        //public void DestinationSignMoveTo( Plot des)
        //{
        //    this.destinationSign.plot = des;
        //    this.destinationSign.transform.position = des.transform.position-new Vector3(0,0,0.1f);
        //    this.destinationSign.gameObject.SetActive(true);
        //}
        /// <summary>
        /// ����������
        /// </summary>
        public void Upgrade()
        {
            ResourcesManager.Instance.ChangeWealth(-this.wanderer.level * 10);
            ResourcesManager.Instance.levelPromptionAmount++;
            this.wanderer.level++;
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
                    ResourcesManager.Instance.levelPromptionAmount--;

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
                ResourcesManager.Instance.levelPromptionAmount++;
            }
        }

        //�����غ��ж�
        public void RoundOver()
        {
            foreach (var expTeam in this.exploratoryTeams.Keys)
            {
                Plot aimPlot = PlotManager.Instance.plots[this.wanderer.plot.pos + expTeam];
                aimPlot.TeamExp();
            }
        }
    }
}

