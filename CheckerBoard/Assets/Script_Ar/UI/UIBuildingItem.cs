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

        /// <summary>
        /// ��ѡ��
        /// </summary>
        /// <param name="selected"></param>
        public override void onSelected(bool selected)
        {
            this.background.sprite = selected ? selectedBg : normalBg;
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="type"></param>
        public void SetInfo(Building_Type type)
        {
            this.type = type;
            this.buildingImage.sprite = ScriptableObjectPool.buildingScriptList[(int)type].sprite;
        }

    }
}

