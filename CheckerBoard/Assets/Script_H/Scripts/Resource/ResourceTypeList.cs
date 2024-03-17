using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/资源类型列表")]
public class ResourceTypeList : ScriptableObject
{
    public List<ResourceType> list; // 资源类型的列表
}
