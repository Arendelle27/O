using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ENTITY
{
    public class Plot : Entity
    {
        [SerializeField, LabelText("格子图像"), Tooltip("不同状态的格子的图像")]
        public List<Sprite> plot_Sps = new List<Sprite>();

        [SerializeField, LabelText("板块类型"), ReadOnly]
        internal Plot_Type plot_Type;
        [SerializeField, LabelText("位置"), ReadOnly]
        public Vector2Int pos;

        [SerializeField, LabelText("流浪者"), ReadOnly]
        public Wanderer wanderer;
        [SerializeField, LabelText("建筑"), ReadOnly]
        public Building building;

        #region 事件
        private Subject<Vector2Int> selectedSubject = new Subject<Vector2Int>();
        public IObservable<Vector2Int> Selected => selectedSubject;
        #endregion

        #region 改变格子状态
        /// <summary>
        /// 随格子类型改变而改变
        /// </summary>
        /// <param name="plot_Type"></param>
        public void ChangeType(Plot_Type plot_Type)
        {
            switch (plot_Type)
            {
                case Plot_Type.Undiscovered_Plot:
                    ChangeToUndiscoverd();
                    break;
                case Plot_Type.CanDiscover_Plot:
                    ChangeToCanDisCover();
                    break;
                case Plot_Type.Discovered_Plot:
                    ChangeToDiscovered();
                    break;
                case Plot_Type.developed_Plot:
                    ChangeToDeveloped();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 改变为未探明状态
        /// </summary>
        void ChangeToUndiscoverd()
        {
            this.plot_Type = Plot_Type.Undiscovered_Plot;
            this.SR.color = Color.black;
        }

        /// <summary>
        /// 改变为可探明状态
        /// </summary>
        void ChangeToCanDisCover()
        {
            this.plot_Type = Plot_Type.CanDiscover_Plot;
            this.SR.color = Color.red;
        }

        /// <summary>
        /// 改变为已探明状态
        /// </summary>
        void ChangeToDiscovered()
        {
            this.plot_Type = Plot_Type.Discovered_Plot;
            this.SR.color = Color.yellow;
        }

        /// <summary>
        /// 改变为已开发状态
        /// </summary>
        void ChangeToDeveloped()
        {
            this.plot_Type = Plot_Type.developed_Plot;
            this.SR.color = Color.green;
        }
        #endregion

        public void OnMouseEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            //Debug.Log("鼠标选择");
        }

        public void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            if(this.plot_Type!=Plot_Type.Undiscovered_Plot)
            {
                this.selectedSubject.OnNext(this.pos);
            }
            Debug.Log("鼠标点击");
        }
    }
}
