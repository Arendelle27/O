using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildingItemInfo : MonoBehaviour
{
    //[SerializeField, LabelText("����ѡ�����"), Tooltip("���뽨��ѡ�����")]
    //public UIBuildingWindow uiBuildingWindow;
    [SerializeField, LabelText("��������"), Tooltip("ѡ�񵽼������Ľ�������")]
    public Text buildingName;
    [SerializeField, LabelText("���������ı�"), Tooltip("���������ı�")]
    public Text descriptionText;
    [SerializeField, LabelText("������Դ�����ı�"), Tooltip("������Դ�����ı�")]
    public List<Text> resoucesCostText;

    /// <summary>
    /// ���ý�����Ϣ
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
