using ENTITY;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/������������")]
public class ProductionBuildingType : BuildingType
{
    [SerializeField, LabelText("ÿ�غϲ�����������"), ReadOnly]
    public int Production;

}
