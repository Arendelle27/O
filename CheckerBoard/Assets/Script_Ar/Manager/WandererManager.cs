using ENTITY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Sirenix.OdinInspector;

namespace MANAGER
{
    public class WandererManager : MonoSingleton<WandererManager>
    {
        //���浱ǰ���ڵ�������
        [SerializeField, LabelText("������"), ReadOnly]
        public Wanderer wanderer;

        [SerializeField, LabelText("����̽��С�ӵ������������λ��"), ReadOnly]
        public List<Vector2Int> exploratoryTeams;

        ////���浱ǰ���ڵ�Ŀ�ĵ���ʾ��
        //[SerializeField, LabelText("Ŀ�ĵ���ʾ��"), ReadOnly]
        //public DestinationSign destinationSign;

        //���汾����չ�İ��
        [SerializeField, LabelText("������չ�İ��"), ReadOnly]
        public Stack<Vector2Int> exploredV2 = new Stack<Vector2Int>();

        /// <summary>
        /// ��ʼ��
        /// </summary>
        void Init()
        {
            this.exploredV2.Clear();

            if (this.wanderer != null)
            {
                this.wanderer.gameObject.SetActive(false);
            }

            //if(this.destinationSign==null)
            //{
            //    GameObject des=Instantiate(GameObjectPool.Instance.DestinationSigns, this.transform);
            //    this.destinationSign = des.GetComponent<DestinationSign>();
            //}
            //this.destinationSign.gameObject.SetActive(false);

        }

        /// <summary>
        /// �ؿ�
        /// </summary>
        public void Restart()
        {
            this.Init();
            this.GetWanderer(PlotManager.Instance.plots[new Vector2Int(0, 0)]);

            this.exploratoryTeams = new List<Vector2Int>() {
                new Vector2Int(1,1),
                new Vector2Int(1,0),
                new Vector2Int(1,-1),
                new Vector2Int(0,1),
                new Vector2Int(0,-1),
                new Vector2Int(-1,1),
                new Vector2Int(-1,0),
                new Vector2Int(-1,-1)
            };
        }

        /// <summary>
        /// ��ȡ�浵
        /// </summary>
        public void ReadArchive()
        {
            this.Init();
            this.GetWanderer(PlotManager.Instance.plots[ArchiveManager.archive.wandererData.pos]);
            this.wanderer.level = ArchiveManager.archive.wandererData.level;

            this.exploratoryTeams = ArchiveManager.archive.wandererData.exploratoryTeams;

        }

        /// <summary>
        /// �ڸ����İ������������
        /// </summary>
        /// <param name="plot"></param>
        public void GetWanderer(Plot plot)
        {
            if(this.wanderer==null)
            {
                GameObject go = Instantiate(GameObjectPool.Instance.Wanderer, this.transform);
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
        /// �������Ƿ��ƶ���Ŀ�ĵ���ʾ���ڵ�λ��
        /// </summary>
        /// <param name="ini"></param>
        //public void WandererMoveToDestination()
        //{
        //    if(this.destinationSign.plot!=null)
        //    {
        //        StartCoroutine(this.WandererMoveTo(this.destinationSign.plot));
        //        this.destinationSign.plot = null;
        //        this.destinationSign.gameObject.SetActive(false);
        //    }
        //}
        

        /// <summary>
        /// �������ƶ���ָ���İ��
        /// </summary>
        /// <param name="des"></param>
        public IEnumerator WandererMoveTo(Plot des)
        {

            if (this.wanderer.plot != null)
            {
                PlotManager.Instance.WanderLeave(this.wanderer.plot);
            }

            yield return null;

            this.wanderer.transform.position = des.transform.position - new Vector3(0, 0, 0.3f);
            this.wanderer.plot = des;

            PlotManager.Instance.WanderEnter(des);
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
            if (aimPlot.HaveExploratoryTeam)
                return;

            foreach (var expTeam in this.exploratoryTeams)
            {
                Vector2Int v2 = aimPlot.pos- this.wanderer.plot.pos;
                if ((v2 - expTeam).magnitude <= 1&& v2.magnitude>0)//�ж�Ŀ���������̽��С������,�Ҳ��������������ڵİ��
                {
                    this.exploratoryTeams.Add(v2);
                    aimPlot.HaveExploratoryTeam = true;
                    aimPlot.ShowSelectedColor(true);

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
                this.exploratoryTeams.Remove(v2);
                Plot aimPlot = PlotManager.Instance.plots[this.wanderer.plot.pos + v2];
                aimPlot.HaveExploratoryTeam = false;
                ResourcesManager.Instance.levelPromptionAmount++;
            }
        }

        //�����غ��ж�
        public void RoundOver()
        {
            foreach (var expTeam in this.exploratoryTeams)
            {
                Plot aimPlot = PlotManager.Instance.plots[this.wanderer.plot.pos + expTeam];
                aimPlot.TeamExp();
            }
        }
    }
}

