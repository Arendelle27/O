using ENTITY;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/��������")]
public class BuildingType : ScriptableObject
{
    //public string nameString; // �������͵������ַ���
    [SerializeField, LabelText("��������"), Tooltip("�ý����������")]
    public Building_Type type; // �������Ͷ�Ӧ��Ԥ����
    [SerializeField, LabelText("������ͼ��"), Tooltip("�ý�����ͼ��")]
    public Sprite sprite;//������ͼ��


}
