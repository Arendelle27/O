using ENTITY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace MANAGER
{
    public class WandererManager : MonoSingleton<WandererManager>
    {
        //���浱ǰ���ڵ�������
        public Wanderer wanderer;


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

            BuildingManager.Instance.GetBuilding(Building_Type.����, plot);//��ʼλ�ò�������
            this.WandererMoveTo(plot,plot);
        }

        /// <summary>
        /// �ƶ���ָ���İ��
        /// </summary>
        /// <param name="des"></param>
        public void WandererMoveTo(Plot ini,Plot des)
        {
            ini.wanderer = null;

            this.wanderer.transform.position = des.transform.position-new Vector3(0,0,0.3f);
            des.wanderer = this.wanderer;

            PlotManager.Instance.PlotsChangeType(des.pos, Plot_Type.��̽��);
            PlotManager.Instance.PlotsChangeType(des.pos, Plot_Type.��̽��);
        }


    }
}

