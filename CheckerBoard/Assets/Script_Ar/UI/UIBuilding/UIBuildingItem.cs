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
        [SerializeField, LabelText("建筑类型"), ReadOnly]
        public Building_Type type;

        [SerializeField, LabelText("背景图像"), Tooltip("放入背景图像【")]
        public Image background;
        [SerializeField, LabelText("未被选择时的背景"), Tooltip("放入未被选择时的背景")]
        public Sprite normalBg;
        [SerializeField, LabelText("被选择时的背景"), Tooltip("被选择时的背景")]
        public Sprite selectedBg;

        //ui图形显示
        [SerializeField, LabelText("ui图形显示"), Tooltip("放入建筑图像")]
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

