using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINoviceGuideMask : MonoBehaviour
{
    [SerializeField, LabelText("界面遮罩"), Tooltip("放入界面遮罩")]
    public List<Transform> uIMasks = new List<Transform>();
    //0为设置，1为建筑，2为移动，3为回合结束，4为升级,5为建筑列表,6为能力提升面板
}
