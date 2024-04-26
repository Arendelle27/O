using MANAGER;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIExecutionPanel : MonoBehaviour
{
    [SerializeField, LabelText("行动点"), ReadOnly]
    public List<UIExecution> uiExecutions = new List<UIExecution>();
    [SerializeField, LabelText("父对象"), Tooltip("放入父对象组件")]
    public Transform content;

    [SerializeField, LabelText("需要自适应的UI"), Tooltip("放入需要自适应的UI")]
    public List<RectTransform> rectTransforms = new List<RectTransform>();

    /// <summary>
    /// 更新UI行动点上限
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
    /// 更新UI行动点
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
    /// 清除所有行动点UI
    /// </summary>
    void RemoveAllUIExecution(int index)
    {
        this.uiExecutions[index].transform.SetParent(null);
        GameObjectPool.Instance.UIExecutionItems.Release(this.uiExecutions[index].gameObject);
        this.uiExecutions.RemoveAt(index);
    }
}
