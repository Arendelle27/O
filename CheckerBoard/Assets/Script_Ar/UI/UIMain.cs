using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using UnityEngine;

public class UIMain : MonoBehaviour
{
    [SerializeField, LabelText("建筑物选择UI"), Tooltip("放入建筑物选择UI界面")]
    public GameObject uiBuilding;
    void Start()
    {
        this.uiBuilding.SetActive(false);
    }

}
