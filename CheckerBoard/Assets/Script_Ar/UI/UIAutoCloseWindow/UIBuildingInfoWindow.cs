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
            int sort;
            if ((int)type > (int)Building_Type.自动采集建筑 && (int)type < (int)Building_Type.生产建筑)
            {
                sort = 0;
            }
            else if ((int)type > (int)Building_Type.生产建筑 && (int)type < (int)Building_Type.战斗建筑)
            {
                sort = 1;
            }
            else
            {
                sort = 2;
            }

            BuildingManager.Instance.RemoveBuilding(sort, BuildingManager.Instance.selectedBuilding);
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
