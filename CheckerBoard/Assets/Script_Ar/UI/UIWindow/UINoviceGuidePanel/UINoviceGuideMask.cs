using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINoviceGuideMask : MonoBehaviour
{
    [SerializeField, LabelText("��������"), Tooltip("�����������")]
    public List<Transform> uIMasks = new List<Transform>();
}
