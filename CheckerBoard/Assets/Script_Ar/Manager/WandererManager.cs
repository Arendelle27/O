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
        //储存当前存在的流浪者
        [SerializeField, LabelText("流浪者"), ReadOnly]
        public Wanderer wanderer;

        //储存当前存在的目的地提示牌
        [SerializeField, LabelText("目的地提示牌"), ReadOnly]
        public DestinationSign destinationSign;

        /// <summary>
        /// 初始化
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
        /// 在给定的板块生成流浪者
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
        /// 流浪者是否移动到目的地提示牌在的位置
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
        /// 流浪者移动到指定的板块
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

            PlotManager.Instance.PlotsChangeType(des.pos, Plot_Type.可探索);
            PlotManager.Instance.PlotsChangeType(des.pos, Plot_Type.已探索);


        }

        /// <summary>
        /// 目的提示牌移动到指定的板块
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

