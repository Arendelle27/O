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

        //[SerializeField, LabelText("背景图像"), Tooltip("放入背景图像")]
        //public Image background;
        //[SerializeField, LabelText("未被选择时的背景"), Tooltip("放入未被选择时的背景")]
        //public Sprite normalBg;
        //[SerializeField, LabelText("被选择时的背景"), Tooltip("被选择时的背景")]
        //public Sprite selectedBg;

        //ui图形显示
        [SerializeField, LabelText("ui图形显示"), Tooltip("放入建筑图像")]
        public Image buildingImage;

        //public override void OnEnable()
        //{
        //    base.OnEnable();
        //}

        ///// <summary>
        ///// 被选中
        ///// </summary>
        ///// <param name="selected"></param>
        //public override void onSelected(bool selected)
        //{
        //    base.onSelected(selected);
        //    //this.background.sprite = selected ? selectedBg : normalBg;
        //}

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="type"></param>
        public void SetInfo(int sort,Building_Type type)
        {
            this.type = type;

            switch (sort)
            {
                case 0:
                    this.buildingImage.sprite = DataManager.BuildingScriptLists[0][(int)type - (int)Building_Type.自动采集建筑 - 1].sprite;//设置建筑的图片
                    break;
                case 1:
                    this.buildingImage.sprite = DataManager.BuildingScriptLists[1][(int)type - (int)Building_Type.生产建筑 - 1].sprite;//设置建筑的图片
                    break;
                case 2:
                    this.buildingImage.sprite = DataManager.BuildingScriptLists[2][(int)type - (int)Building_Type.战斗建筑 - 1].sprite;//设置建筑的图片
                    break;
            }
        }

    }
}

