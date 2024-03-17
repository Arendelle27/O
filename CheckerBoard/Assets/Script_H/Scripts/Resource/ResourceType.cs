using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/资源类型")]
public class ResourceType : ScriptableObject
{
    public string nameString; // 资源类型的名称

    public Sprite sprite; // 资源的图标

    public string colorHex; // 提示颜色

}
