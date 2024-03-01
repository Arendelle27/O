using ENTITY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace MANAGER
{
    public class WandererManager : MonoSingleton<WandererManager>
    {
        //储存当前存在的流浪者
        public Wanderer wanderer;


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

            BuildingManager.Instance.GetBuilding(Building_Type.基地, plot);//初始位置产生基地
            this.WandererMoveTo(plot,plot);
        }

        /// <summary>
        /// 移动到指定的板块
        /// </summary>
        /// <param name="des"></param>
        public void WandererMoveTo(Plot ini,Plot des)
        {
            ini.wanderer = null;

            this.wanderer.transform.position = des.transform.position-new Vector3(0,0,0.3f);
            des.wanderer = this.wanderer;

            PlotManager.Instance.PlotsChangeType(des.pos, Plot_Type.可探索);
            PlotManager.Instance.PlotsChangeType(des.pos, Plot_Type.已探索);
        }


    }
}

