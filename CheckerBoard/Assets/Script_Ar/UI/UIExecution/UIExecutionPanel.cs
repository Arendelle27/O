using MANAGER;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIExecutionPanel : MonoBehaviour
{
    [SerializeField, LabelText("�ж���"), ReadOnly]
    public List<UIExecution> uiExecutions = new List<UIExecution>();
    [SerializeField, LabelText("������"), Tooltip("���븸�������")]
    public Transform content;

    [SerializeField, LabelText("��Ҫ����Ӧ��UI"), Tooltip("������Ҫ����Ӧ��UI")]
    public List<RectTransform> rectTransforms = new List<RectTransform>();

    /// <summary>
    /// ����UI�ж�������
    /// </summary>
    public void UpdatUIExuectionUpperLimit(int exuectionUpperLimit)
    {
        
        int curExuectionUpperLimit = this.uiExecutions.Count;

        if(curExuectionUpperLimit>exuectionUpperLimit)
        {
            for(int i = curExuectionUpperLimit-1;i>=exuectionUpperLimit;i--)
            {
                RemoveAllUIExecution(i);
            }
        }
        else
        { 
            for (int i = curExuectionUpperLimit; i < exuectionUpperLimit; i++)
            {
                GameObject go = GameObjectPool.Instance.UIExecutionItems.Get();
                go.transform.SetParent(this.content);
                UIExecution uiExecution = go.GetComponent<UIExecution>();
                this.uiExecutions.Add(uiExecution);
                uiExecution.id = i;
            }
        }

        //for (int i = 0; i < allExecution; i++)
        //{
        //    GameObject go = GameObjectPool.Instance.UIExecutionItems.Get();
        //    go.transform.SetParent(this.content);
        //    UIExecution uiExecution = go.GetComponent<UIExecution>();
        //    this.uiExecutions.Add(uiExecution);
        //    uiExecution.id = i;
        //}

        foreach (var rectTransform in rectTransforms)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }

    /// <summary>
    /// ����UI�ж���
    /// </summary>
    public void UpdatUIExuection(int execution)
    {
        int curExecution = execution;
        //int curExecution = ResourcesManager.Instance.execution;

        for (int i = 0; i < this.uiExecutions.Count; i++)
        {
            this.uiExecutions[i].SetInfo(i < curExecution);
        }
    }
    /// <summary>
    /// ��������ж���UI
    /// </summary>
    void RemoveAllUIExecution(int index)
    {
        this.uiExecutions[index].transform.SetParent(null);
        GameObjectPool.Instance.UIExecutionItems.Release(this.uiExecutions[index].gameObject);
        this.uiExecutions.RemoveAt(index);
    }
}
