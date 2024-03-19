using ENTITY;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/生产建筑类型")]
public class ProductionBuildingType : BuildingType
{
    [SerializeField, LabelText("每回合产出货币数量"), ReadOnly]
    public int Production;

}
