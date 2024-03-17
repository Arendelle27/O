using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/建筑类型列表")]
public class BuildingTypeList : ScriptableObject
{
    [SerializeField, LabelText("建筑类型列表"), Tooltip("放入建筑类型列表")]
    public List<BuildingType> buildingTypeList; // 建筑类型列表
}
