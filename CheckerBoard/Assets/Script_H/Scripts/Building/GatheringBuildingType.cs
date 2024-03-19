using ENTITY;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/自动采集建筑类型")]
public class GatheringBuildingType : BuildingType
{
    [SerializeField, LabelText("采集资源类型"), ReadOnly]
    public int GatherResourceType;
    [SerializeField, LabelText("采集资源数量"), ReadOnly]
    public int GatherResourceAmount;
    [SerializeField, LabelText("采集资源所需回合数"), ReadOnly]
    public int GatherResourceRounds;    

}
