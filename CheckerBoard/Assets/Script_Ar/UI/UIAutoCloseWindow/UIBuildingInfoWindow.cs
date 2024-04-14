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
    [SerializeField, LabelText("CGͼƬ"), Tooltip("�¼�ͼƬ")]
    public Image image;
    [SerializeField, LabelText("��������"), Tooltip("�ý�������")]
    public Text title;
    [SerializeField, LabelText("��������"), Tooltip("�ý���������")]
    public Text description;
    [SerializeField, LabelText("���������ť"), Tooltip("���������ť")]
    public Button destoryBuilding;

    private void Start()
    {
        this.destoryBuilding.OnClickAsObservable().Subscribe(_ =>
        {
            Building_Type type = BuildingManager.Instance.selectedBuilding.type;
            int sort=BuildingManager.BuildingTypeToIndex(type);

            Building building = BuildingManager.Instance.selectedBuilding;

            if (PlotManager.Instance.plots[building.pos].plotDefine.CanBuild)//�����Խ���
            {
                (UIMain.Instance.uiPanels[1] as UIGamePanel).buildButton.gameObject.SetActive(true);//��ʾ���찴ť
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
