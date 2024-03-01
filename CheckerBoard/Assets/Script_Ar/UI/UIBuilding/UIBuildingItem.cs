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

        [SerializeField, LabelText("����ͼ��"), Tooltip("���뱳��ͼ��")]
        public Image background;
        [SerializeField, LabelText("δ��ѡ��ʱ�ı���"), Tooltip("����δ��ѡ��ʱ�ı���")]
        public Sprite normalBg;
        [SerializeField, LabelText("��ѡ��ʱ�ı���"), Tooltip("��ѡ��ʱ�ı���")]
        public Sprite selectedBg;

        //uiͼ����ʾ
        [SerializeField, LabelText("uiͼ����ʾ"), Tooltip("���뽨��ͼ��")]
        public Image buildingImage;

        private void OnEnable()
        {
            this.background.sprite = normalBg;
        }

        public override void onSelected(bool selected)
        {
            this.background.sprite = selected ? selectedBg : normalBg;
        }


        public void SetInfo(Building_Type type,Sprite sprite )
        {
            this.type = type;
            this.buildingImage.sprite = sprite;
        }

    }
}

