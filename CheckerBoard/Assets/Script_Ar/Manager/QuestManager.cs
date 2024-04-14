using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace MANAGER
{
    public class QuestManager :Singleton<QuestManager>
    {
        //��ǰ��������ID 
        public int curMainQuestId;

        //��ǰ֧������ID
        public HashSet<int> curSecondQuestIds;

        public void AddQuest(bool isMain, int questId)
        {
            if (isMain)
            {
                curMainQuestId = questId;
            }
            else
            {
                curSecondQuestIds.Add(questId);
            }
        }

        public void RemoveQuest(bool isMain, int questId)
        {
            if (isMain)
            {
                curMainQuestId = -1;
            }
            else
            {
                curSecondQuestIds.Remove(questId);
            }
        }
    }
}
