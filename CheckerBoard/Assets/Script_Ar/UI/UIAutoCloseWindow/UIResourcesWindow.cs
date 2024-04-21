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
    [SerializeField, LabelText("CG图片"), Tooltip("事件图片")]
    public Image image;
    [SerializeField, LabelText("建筑名称"), Tooltip("该建筑名称")]
    public Text title;
    [SerializeField, LabelText("建筑描述"), Tooltip("该建筑的描述")]
    public Text description;
    [SerializeField, LabelText("拥有资源显示"), Tooltip("放入拥有资源显示")]
    public Text reourceText;

    private void OnEnable()
    {
        if (PlotManager.Instance.SelectedPlot?.plotType==0)//如果是资源区
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
            this.reourceText.text = string.Format("{0}：{1}", (Resource_Type)plot.buildingResources[0], plot.buildingResources[1]);
        }
        else
        {
            this.reourceText.text = "无可采集资源";
        }
    }
}
