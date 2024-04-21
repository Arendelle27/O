using ENTITY;
using MANAGER;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResourcesWindow : UISelectWindow
{
    [SerializeField, LabelText("CGͼƬ"), Tooltip("�¼�ͼƬ")]
    public Image image;
    [SerializeField, LabelText("��������"), Tooltip("�ý�������")]
    public Text title;
    [SerializeField, LabelText("��������"), Tooltip("�ý���������")]
    public Text description;
    [SerializeField, LabelText("ӵ����Դ��ʾ"), Tooltip("����ӵ����Դ��ʾ")]
    public Text reourceText;

    private void OnEnable()
    {
        if (PlotManager.Instance.SelectedPlot?.plotType==0)//�������Դ��
        {
            this.SetInfo(PlotManager.Instance.SelectedPlot);
        }
    }

    public void SetInfo(Plot plot)
    {
        this.image.sprite = SpriteManager.plotSprites[plot.plotDefine.Name];
        this.title.text = plot.plotDefine.Name;
        this.description.text = plot.plotDefine.Description;
        if (plot.buildingResources[0]!=-1)
        {
            this.reourceText.text = string.Format("{0}��{1}", (Resource_Type)plot.buildingResources[0], plot.buildingResources[1]);
        }
        else
        {
            this.reourceText.text = "�޿ɲɼ���Դ";
        }
    }
}
