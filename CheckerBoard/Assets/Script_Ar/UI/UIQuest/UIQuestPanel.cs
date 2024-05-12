using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestPanel : MonoBehaviour
{
    [SerializeField, LabelText("任务UI"), ReadOnly]
    public List<List<UIQuest>> uIQuest=new List<List<UIQuest>>(2) {new List<UIQuest>(),new List<UIQuest>() };
    //[SerializeField, LabelText("支线UI"), ReadOnly]
    //public Dictionary<int,UIQuest> uiQuestSecond=new Dictionary<int, UIQuest>();
    [SerializeField, LabelText("自适应的窗口"), Tooltip("放入需要只适应的窗口")]
    public List<RectTransform> rectTransforms = new List<RectTransform>();

    public void OnEnable()
    {
        this.UpdateAllUIQuest();
    }

    /// <summary>
    /// 更新任务UI
    /// </summary>
    public void UpdateAllUIQuest()
    {
        List<Dictionary<int,bool>> questOpen= new List<Dictionary<int, bool>>(2) {new Dictionary<int, bool>(), new Dictionary<int, bool>() };
        for (int i = 0; i < this.uIQuest.Count; i++)
        {
            foreach (var quest in uIQuest[i])
            {
                questOpen[i].Add(quest.questId, quest.switchToggle.isOn);//记录任务开关状态
            }
        }
        this.RemoveAllUIQuest();
        for(int i=0;i<QuestManager.Instance.curQuestIds.Count;i++)
        {
            foreach (var questId in QuestManager.Instance.curQuestIds[i])
            {
                if (!questOpen[i].ContainsKey(questId))
                {
                    questOpen[i].Add(questId, true);
                }

                this.SetQuest(i,DataManager.QuestDefines[questId], questOpen[i][questId]);
            }
        }

        foreach (var rectTransform in rectTransforms)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
        //foreach (var item in QuestManager.Instance.curSecondQuestIds)
        //{
        //    SetQuestSecond(DataManager.QuestDefines[item]);
        //}
    }

    /// <summary>
    /// 设置任务
    /// </summary>
    /// <param name="isMain"></param>
    /// <param name="questDefine"></param>
    /// <param name="isOpen"></param>
    public void SetQuest(int isMain,QuestDefine questDefine,bool isOpen)
    {

        GameObject go = GameObjectPool.Instance.UIQuestItems.Get();
        go.transform.SetParent(this.transform);
        UIQuest uiQuest = go.GetComponent<UIQuest>();
        uiQuest.SetInfo(questDefine,isOpen);
        this.uIQuest[isMain].Add(uiQuest);
    }


    /// <summary>
    /// 移除所有任务UI
    /// </summary>
    public void RemoveAllUIQuest()
    {
        for(int i=0;i<this.uIQuest.Count;i++)
        {
            for (int j = 0; j < this.uIQuest[i].Count; j++)
            {
                GameObjectPool.Instance.UIQuestItems.Release(this.uIQuest[i][j].gameObject);
            }
            this.uIQuest[i].Clear();
        }
    }
}
