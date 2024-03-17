using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private Dictionary<ResourceType, int> resourceAmountDictionary; // 资源类型与数量的字典

    private void Awake()
    {
        resourceAmountDictionary = new Dictionary<ResourceType, int>(); // 初始化资源字典

        // 加载资源类型列表
        ResourceTypeList resourceTypeList = Resources.Load<ResourceTypeList>("ScriptableObject/资源类型/资源类型列表");

        // 遍历资源类型列表，将每个资源类型添加到资源字典并初始化数量为0
        foreach (ResourceType resourceType in resourceTypeList.list)
        {
            resourceAmountDictionary[resourceType] = 5;
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            // 加载资源类型列表
            ResourceTypeList resourceTypeList = Resources.Load<ResourceTypeList>("ScriptableObject/资源类型/资源类型列表");


        }
    }

    }
