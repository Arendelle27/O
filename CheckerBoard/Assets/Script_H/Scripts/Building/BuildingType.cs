using ENTITY;
using Sirenix.OdinInspector;
using UnityEngine;

public class BuildingType : ScriptableObject
{
    public int TID;
    [SerializeField, LabelText("机械名称"), ReadOnly]
    public string Name;
    [SerializeField, LabelText("类型"), ReadOnly]
    public Building_Type Class;
    [SerializeField, LabelText("子类型"),ReadOnly]
    public Building_Type Type;
    [SerializeField, LabelText("模型资源名称"), ReadOnly]
    public string Resource;
    [SerializeField, LabelText("描述"), ReadOnly]
    public string Description;
    [SerializeField, LabelText("解锁条件"), ReadOnly]
    public string Condition;
    [SerializeField, LabelText("条件数值"), ReadOnly]
    public int NumericalValue;
    [SerializeField, LabelText("建造时消耗资源"), ReadOnly]
    public int[] ResourcesCost = new int[3];
    [SerializeField, LabelText("战力"), ReadOnly]
    public int Attack;
    [SerializeField, LabelText("建造时增加机器人敌意值"), ReadOnly]
    public int HostilityToRobot;
    [SerializeField, LabelText("建造时增加人类敌意值"), ReadOnly]
    public int HostilityToHuman;

    [SerializeField, LabelText("建筑的图标"),ReadOnly]
    public Sprite sprite;//建筑的图标

}
