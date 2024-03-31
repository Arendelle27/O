using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildingItemInfo : MonoBehaviour
{
    //[SerializeField, LabelText("建筑选择界面"), Tooltip("放入建筑选择界面")]
    //public UIBuildingWindow uiBuildingWindow;
    [SerializeField, LabelText("建筑名称"), Tooltip("选择到见建筑的建筑名称")]
    public Text buildingName;
    [SerializeField, LabelText("建筑描述文本"), Tooltip("建筑描述文本")]
    public Text descriptionText;
    [SerializeField, LabelText("建筑资源消耗文本"), Tooltip("建筑资源消耗文本")]
    public List<Text> resoucesCostText;

    /// <summary>
    /// 设置建筑信息
    /// </summary>
    /// <param name="type"></param>
    public void SetInfo(Building_Type type)
    {
        BuildingType buildingType= BuildingManager.GetBuildingByType(type);

        this.buildingName.text = buildingType.Name;
        this.descriptionText.text = buildingType.Description;
        for (int i = 0; i < buildingType.ResourcesCost.Length; i++)
        {
            this.resoucesCostText[i].text = string.Format("{0}:{1}",((Resource_Type)i).ToString(), buildingType.ResourcesCost[i]);
            this.resoucesCostText[i].gameObject.SetActive(buildingType.ResourcesCost[i]!=0);
        }
    }
}
