using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINoviceGuideMask : MonoBehaviour
{
    [SerializeField, LabelText("界面遮罩"), Tooltip("放入界面遮罩")]
    public List<Transform> uIMasks = new List<Transform>();
}
