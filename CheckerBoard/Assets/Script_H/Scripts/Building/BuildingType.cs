using ENTITY;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/建筑类型")]
public class BuildingType : ScriptableObject
{
    //public string nameString; // 建筑类型的名称字符串
    [SerializeField, LabelText("建筑类型"), Tooltip("该建筑物的类型")]
    public Building_Type type; // 建筑类型对应的预制体
    [SerializeField, LabelText("建筑的图标"), Tooltip("该建筑的图标")]
    public Sprite sprite;//建筑的图标


}
