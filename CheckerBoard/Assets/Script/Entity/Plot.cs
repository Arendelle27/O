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
        [SerializeField, LabelText("����ͼ��"), Tooltip("��ͬ״̬�ĸ��ӵ�ͼ��")]
        public List<Sprite> plot_Sps = new List<Sprite>();

        [SerializeField, LabelText("�������"), ReadOnly]
        internal Plot_Type plot_Type;
        [SerializeField, LabelText("λ��"), ReadOnly]
        public Vector2Int pos;

        [SerializeField, LabelText("������"), ReadOnly]
        public Wanderer wanderer;
        [SerializeField, LabelText("����"), ReadOnly]
        public Building building;

        #region �¼�
        private Subject<Vector2Int> selectedSubject = new Subject<Vector2Int>();
        public IObservable<Vector2Int> Selected => selectedSubject;
        #endregion

        #region �ı����״̬
        /// <summary>
        /// ��������͸ı���ı�
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
        /// �ı�Ϊδ̽��״̬
        /// </summary>
        void ChangeToUndiscoverd()
        {
            this.plot_Type = Plot_Type.Undiscovered_Plot;
            this.SR.color = Color.black;
        }

        /// <summary>
        /// �ı�Ϊ��̽��״̬
        /// </summary>
        void ChangeToCanDisCover()
        {
            this.plot_Type = Plot_Type.CanDiscover_Plot;
            this.SR.color = Color.red;
        }

        /// <summary>
        /// �ı�Ϊ��̽��״̬
        /// </summary>
        void ChangeToDiscovered()
        {
            this.plot_Type = Plot_Type.Discovered_Plot;
            this.SR.color = Color.yellow;
        }

        /// <summary>
        /// �ı�Ϊ�ѿ���״̬
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
            //Debug.Log("���ѡ��");
        }

        public void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            if(this.plot_Type!=Plot_Type.Undiscovered_Plot)
            {
                this.selectedSubject.OnNext(this.pos);
            }
            Debug.Log("�����");
        }
    }
}
