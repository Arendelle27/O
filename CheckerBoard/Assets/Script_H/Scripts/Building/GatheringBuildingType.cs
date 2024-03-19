using ENTITY;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/�Զ��ɼ���������")]
public class GatheringBuildingType : BuildingType
{
    [SerializeField, LabelText("�ɼ���Դ����"), ReadOnly]
    public int GatherResourceType;
    [SerializeField, LabelText("�ɼ���Դ����"), ReadOnly]
    public int GatherResourceAmount;
    [SerializeField, LabelText("�ɼ���Դ����غ���"), ReadOnly]
    public int GatherResourceRounds;    

}
