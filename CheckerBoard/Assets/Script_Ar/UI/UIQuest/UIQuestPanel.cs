using MANAGER;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestPanel : MonoBehaviour
{
    [SerializeField, LabelText("主线UI"), ReadOnly]
    public UIQuest uIQuestMain;
    [SerializeField, LabelText("支线UI"), ReadOnly]
    public Dictionary<int,UIQuest> uiQuestSecond=new Dictionary<int, UIQuest>();

    /// <summary>
    /// 更新任务UI
    /// </summary>
    public void UpdateAllUIQuest()
    {
        this.RemoveAllUIQuest();
        if(DataManager.QuestDefines.ContainsKey(QuestManager.Instance.curMainQUestId))
        {
            this.SetQuestMain(DataManager.QuestDefines[QuestManager.Instance.curMainQUestId]);//主线任务
        }

        foreach (var item in QuestManager.Instance.curSecondQuestIds)
        {
            SetQuestSecond(DataManager.QuestDefines[item]);
        }
    }

    public void SetQuestMain(QuestDefine questDefine)
    {

        GameObject go = GameObjectPool.Instance.UIQuestItems.Get();
        go.transform.SetParent(this.transform);
        UIQuest uiQuest = go.GetComponent<UIQuest>();
        uiQuest.SetInfo(questDefine);
        this.uIQuestMain = uiQuest;
    }

    public void SetQuestSecond(QuestDefine questDefine)
    {
        if(this.uiQuestSecond.ContainsKey(questDefine.Id))
        {
            this.uiQuestSecond[questDefine.Id].SetInfo(questDefine);
            return;
        }
        //GameObject go = GameObjectPool.Instance.UIQuestItems.Get();
        //go.transform.SetParent(UIMain.Instance.uIQuestPanel.transform);
        //UIQuest uiQuest = go.GetComponent<UIQuest>();
        //uiQuest.SetInfo(questDefine);
        //uiQuestSecond.Add(uiQuest);
    }

    /// <summary>
    /// 移除所有任务UI
    /// </summary>
    public void RemoveAllUIQuest()
    {
        if (this.uIQuestMain != null)
        {
            GameObjectPool.Instance.UIQuestItems.Release(uIQuestMain.gameObject);
            this.uIQuestMain = null;
        }
        foreach (var item in this.uiQuestSecond)
        {
            GameObjectPool.Instance.UIQuestItems.Release(item.Value.gameObject);
        }
        this.uiQuestSecond.Clear();
    }
}
