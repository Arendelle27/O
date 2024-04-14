using ENTITY;
using MANAGER;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class UIBuildingInfoWindow : UISelectWindow
{
    [SerializeField, LabelText("CG图片"), Tooltip("事件图片")]
    public Image image;
    [SerializeField, LabelText("建筑名称"), Tooltip("该建筑名称")]
    public Text title;
    [SerializeField, LabelText("建筑描述"), Tooltip("该建筑的描述")]
    public Text description;
    [SerializeField, LabelText("拆除建筑按钮"), Tooltip("拆除建筑按钮")]
    public Button destoryBuilding;

    private void Start()
    {
        this.destoryBuilding.OnClickAsObservable().Subscribe(_ =>
        {
            Building_Type type = BuildingManager.Instance.selectedBuilding.type;
            int sort=BuildingManager.BuildingTypeToIndex(type);

            Building building = BuildingManager.Instance.selectedBuilding;

            if (PlotManager.Instance.plots[building.pos].plotDefine.CanBuild)//板块可以建造
            {
                (UIMain.Instance.uiPanels[1] as UIGamePanel).buildButton.gameObject.SetActive(true);//显示建造按钮
            }

            BuildingManager.Instance.RemoveBuilding(sort, building);

            this.selectedWindow.OnCloseClick();
        });

    }

    private void OnEnable()
    {
        if(BuildingManager.Instance.selectedBuilding != null)
        this.SetInfo(BuildingManager.Instance.selectedBuilding);
    }

    public void SetInfo(Building building)
    {
        this.image.sprite = building.SR.sprite;
        this.title.text = building.buildingname;
        this.description.text = building.description;
    }
}
