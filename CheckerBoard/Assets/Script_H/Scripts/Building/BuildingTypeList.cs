using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/���������б�")]
public class BuildingTypeList : ScriptableObject
{
    [SerializeField, LabelText("���������б�"), Tooltip("���뽨�������б�")]
    public List<BuildingType> buildingTypeList; // ���������б�
}
