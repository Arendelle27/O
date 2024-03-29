using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UILIST;

namespace UIBUILDING
{
    public class UIBuildingItem : ListView.ListViewItem
    {
        [SerializeField, LabelText("��������"), ReadOnly]
        public Building_Type type;

        //[SerializeField, LabelText("����ͼ��"), Tooltip("���뱳��ͼ��")]
        //public Image background;
        //[SerializeField, LabelText("δ��ѡ��ʱ�ı���"), Tooltip("����δ��ѡ��ʱ�ı���")]
        //public Sprite normalBg;
        //[SerializeField, LabelText("��ѡ��ʱ�ı���"), Tooltip("��ѡ��ʱ�ı���")]
        //public Sprite selectedBg;

        //uiͼ����ʾ
        [SerializeField, LabelText("uiͼ����ʾ"), Tooltip("���뽨��ͼ��")]
        public Image buildingImage;

        //public override void OnEnable()
        //{
        //    base.OnEnable();
        //}

        ///// <summary>
        ///// ��ѡ��
        ///// </summary>
        ///// <param name="selected"></param>
        //public override void onSelected(bool selected)
        //{
        //    base.onSelected(selected);
        //    //this.background.sprite = selected ? selectedBg : normalBg;
        //}

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="type"></param>
        public void SetInfo(int sort,Building_Type type)
        {
            this.type = type;

            switch (sort)
            {
                case 0:
                    this.buildingImage.sprite = DataManager.BuildingScriptLists[0][(int)type - (int)Building_Type.�Զ��ɼ����� - 1].sprite;//���ý�����ͼƬ
                    break;
                case 1:
                    this.buildingImage.sprite = DataManager.BuildingScriptLists[1][(int)type - (int)Building_Type.�������� - 1].sprite;//���ý�����ͼƬ
                    break;
                case 2:
                    this.buildingImage.sprite = DataManager.BuildingScriptLists[2][(int)type - (int)Building_Type.ս������ - 1].sprite;//���ý�����ͼƬ
                    break;
            }
        }

    }
}

