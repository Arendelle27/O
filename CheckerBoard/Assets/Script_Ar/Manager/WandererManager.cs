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

        //���浱ǰ���ڵ�Ŀ�ĵ���ʾ��
        [SerializeField, LabelText("Ŀ�ĵ���ʾ��"), ReadOnly]
        public DestinationSign destinationSign;

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public void Init()
        {
            if(this.wanderer!=null)
            {
                GameObjectPool.Instance.Wanderers.Release(this.wanderer.gameObject);
            }

            this.GetWanderer(PlotManager.Instance.grids[new Vector2Int(0, 0)]);

            if(this.destinationSign==null)
            {
                GameObject des=Instantiate(GameObjectPool.Instance.DestinationSigns, this.transform);
                this.destinationSign = des.GetComponent<DestinationSign>();
            }
            this.destinationSign.gameObject.SetActive(false);

        }

        /// <summary>
        /// �ڸ����İ������������
        /// </summary>
        /// <param name="plot"></param>
        public void GetWanderer(Plot plot)
        {
            GameObject go=Instantiate(GameObjectPool.Instance.Wanderers.Get(), this.transform);
            Wanderer wanderer = go.GetComponent<Wanderer>();
            this.wanderer = wanderer;

            this.WandererMoveTo(plot);
        }

        /// <summary>
        /// �������Ƿ��ƶ���Ŀ�ĵ���ʾ���ڵ�λ��
        /// </summary>
        /// <param name="ini"></param>
        public void WandererMoveToDestination()
        {
            if(this.destinationSign.plot!=null)
            {
                this.WandererMoveTo(this.destinationSign.plot);
                this.destinationSign.plot = null;
                this.destinationSign.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// �������ƶ���ָ���İ��
        /// </summary>
        /// <param name="des"></param>
        void WandererMoveTo(Plot des)
        {
            if (this.wanderer.plot != null)
            {
                this.wanderer.plot.wanderer = null;
            }

            this.wanderer.transform.position = des.transform.position - new Vector3(0, 0, 0.3f);
            des.wanderer = this.wanderer;
            this.wanderer.plot = des;

            PlotManager.Instance.PlotsChangeType(des.pos, Plot_Type.��̽��);
            PlotManager.Instance.PlotsChangeType(des.pos, Plot_Type.��̽��);


        }

        /// <summary>
        /// Ŀ����ʾ���ƶ���ָ���İ��
        /// </summary>
        /// <param name="des"></param>
        public void DestinationSignMoveTo( Plot des)
        {
            this.destinationSign.plot = des;
            this.destinationSign.transform.position = des.transform.position-new Vector3(0,0,0.1f);
            this.destinationSign.gameObject.SetActive(true);
        }
    }
}

