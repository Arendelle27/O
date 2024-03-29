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
            int sort;
            if ((int)type > (int)Building_Type.�Զ��ɼ����� && (int)type < (int)Building_Type.��������)
            {
                sort = 0;
            }
            else if ((int)type > (int)Building_Type.�������� && (int)type < (int)Building_Type.ս������)
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
