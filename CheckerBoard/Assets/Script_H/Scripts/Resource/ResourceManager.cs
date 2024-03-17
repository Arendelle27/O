using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private Dictionary<ResourceType, int> resourceAmountDictionary; // ��Դ�������������ֵ�

    private void Awake()
    {
        resourceAmountDictionary = new Dictionary<ResourceType, int>(); // ��ʼ����Դ�ֵ�

        // ������Դ�����б�
        ResourceTypeList resourceTypeList = Resources.Load<ResourceTypeList>("ScriptableObject/��Դ����/��Դ�����б�");

        // ������Դ�����б���ÿ����Դ������ӵ���Դ�ֵ䲢��ʼ������Ϊ0
        foreach (ResourceType resourceType in resourceTypeList.list)
        {
            resourceAmountDictionary[resourceType] = 5;
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            // ������Դ�����б�
            ResourceTypeList resourceTypeList = Resources.Load<ResourceTypeList>("ScriptableObject/��Դ����/��Դ�����б�");


        }
    }

    }
