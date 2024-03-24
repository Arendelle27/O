using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UItaskbarItem : MonoBehaviour
{
    [SerializeField, LabelText("是否是主任务"), Tooltip("是否为主任务")]
    public bool isMain;
}
