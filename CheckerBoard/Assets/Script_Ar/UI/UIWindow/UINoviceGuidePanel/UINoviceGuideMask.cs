using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINoviceGuideMask : MonoBehaviour
{
    [SerializeField, LabelText("��������"), Tooltip("�����������")]
    public List<Transform> uIMasks = new List<Transform>();
    //0Ϊ���ã�1Ϊ������2Ϊ�ƶ���3Ϊ�غϽ�����4Ϊ����,5Ϊ�����б�,6Ϊ�����������
}
