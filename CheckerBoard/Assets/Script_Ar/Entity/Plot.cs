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
        public Plot_Type plot_Type;
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
                case Plot_Type.δ̽��:
                    ChangeToUndiscoverd();
                    break;
                case Plot_Type.��̽��:
                    ChangeToCanDisCover();
                    break;
                case Plot_Type.��̽��:
                    ChangeToDiscovered();
                    break;
                case Plot_Type.�ѿ���:
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
            this.plot_Type = Plot_Type.δ̽��;
            this.SR.color = Color.black;
        }

        /// <summary>
        /// �ı�Ϊ��̽��״̬
        /// </summary>
        void ChangeToCanDisCover()
        {
            this.plot_Type = Plot_Type.��̽��;
            this.SR.color = Color.red;
        }

        /// <summary>
        /// �ı�Ϊ��̽��״̬
        /// </summary>
        void ChangeToDiscovered()
        {
            this.plot_Type = Plot_Type.��̽��;
            this.SR.color = Color.yellow;
        }

        /// <summary>
        /// �ı�Ϊ�ѿ���״̬
        /// </summary>
        void ChangeToDeveloped()
        {
            this.plot_Type = Plot_Type.�ѿ���;
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
            if(this.plot_Type!=Plot_Type.δ̽��)
            {
                this.selectedSubject.OnNext(this.pos);
            }
            //Debug.Log("�����");
        }
    }
}
