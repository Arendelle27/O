using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UIBUILDING;
using UnityEngine;

public class UIMain : MonoBehaviour
{
    [SerializeField, LabelText("������ѡ��UI"), Tooltip("���뽨����ѡ��UI����")]
    public GameObject uiBuilding;
    void Start()
    {
        this.uiBuilding.SetActive(false);
    }

}
