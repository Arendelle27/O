using ENTITY;
using Sirenix.OdinInspector;
using UnityEngine;

public class BuildingType : ScriptableObject
{
    public int TID;
    [SerializeField, LabelText("��е����"), ReadOnly]
    public string Name;
    [SerializeField, LabelText("����"), ReadOnly]
    public Building_Type Class;
    [SerializeField, LabelText("������"),ReadOnly]
    public Building_Type Type;
    [SerializeField, LabelText("ģ����Դ����"), ReadOnly]
    public string Resource;
    [SerializeField, LabelText("����"), ReadOnly]
    public string Description;
    [SerializeField, LabelText("��������"), ReadOnly]
    public string Condition;
    [SerializeField, LabelText("������ֵ"), ReadOnly]
    public int NumericalValue;
    [SerializeField, LabelText("����ʱ������Դ"), ReadOnly]
    public int[] ResourcesCost = new int[3];
    [SerializeField, LabelText("ս��"), ReadOnly]
    public int Attack;
    [SerializeField, LabelText("����ʱ���ӻ����˵���ֵ"), ReadOnly]
    public int HostilityToRobot;
    [SerializeField, LabelText("����ʱ�����������ֵ"), ReadOnly]
    public int HostilityToHuman;

    [SerializeField, LabelText("������ͼ��"),ReadOnly]
    public Sprite sprite;//������ͼ��

}
