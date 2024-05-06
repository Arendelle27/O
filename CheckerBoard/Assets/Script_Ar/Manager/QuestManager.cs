using ENTITY;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UniRx;
using UnityEngine;

namespace MANAGER
{
    public class QuestManager :Singleton<QuestManager>
    {

        public int curMainQuestId;
        public int curMainQuestRound;

        //任务解锁条件
        public HashSet<int> questConditions = new HashSet<int>();

        ////当前主线任务ID 
        //public HashSet<int> curMainQuestIds;
        ////当前主线任务完成回合
        //public Dictionary<int,int> curMainQuestRound;
        ////当前主线任务完成Npc
        //public Dictionary<int, Npc_Name> curMainQuestNpc;
        ////当前主线任务完成板块位置
        //public Dictionary<int, Vector2Int> curMainQuestPos;

        ////当前支线任务ID
        //public HashSet<int> curSecondQuestIds=new HashSet<int>();
        ////当前支线任务完成回合
        //public Dictionary<int, int> curSecondQuestsRound=new Dictionary<int, int>();
        ////当前支线任务完成Npc
        //public Dictionary<int, Npc_Name> curSecondQuestNpc;
        ////当前支线任务完成板块位置
        //public Dictionary<int, Vector2Int> curSecondQuestPos;

        public void Restart()
        {
            foreach (var item in DataManager.QuestDefines)
            {
                if(item.Value.RoundCondition==0)
                    continue;
                this.questConditions.Add(item.Value.Id);
            }
        }

        public void ReadArchive()
        {
            ArchiveManager.QuestManagerData questManagerData = ArchiveManager.archive.questManagerData;

            for(int i=1;i<questManagerData.questIds.Count;i++)
            {
                if (DataManager.QuestDefines[questManagerData.questIds[i]].IsMain)//如果为主线
                {
                    this.curMainQuestId = questManagerData.questIds[i];
                    this.curMainQuestRound = questManagerData.questsRound[i];
                }
                else
                {
                    //this.curSecondQuestIds.Add(questManagerData.questIds[i]);
                    //this.curSecondQuestsRound.Add(questManagerData.questIds[i], questManagerData.questsRound[i]);
                }
            }
            foreach (var item in questManagerData.questConditions)
            {
                this.questConditions.Add(item);
            }
            (UIMain.Instance.uiPanels[1] as UIGamePanel).uIQuestPanel.UpdateAllUIQuest();
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        public void GameOver()
        {
            this.curMainQuestId = -1;
            this.curMainQuestRound = -1;
            this.questConditions.Clear();

            ////清除所有任务
            //for (int i = 0; i < this.curMainQuestIds.Count;)
            //{
            //    this.RemoveQuest(true, this.curMainQuestIds.ElementAt(i));
            //}

            //for (int i = 0; i < this.curSecondQuestIds.Count;)
            //{
            //    this.RemoveQuest(false, this.curSecondQuestIds.ElementAt(i));
            //}
        }

        /// <summary>
        /// 解锁任务
        /// </summary>
        public void QuestUnlock(int questConditionId)
        {
            int id = -1;
            foreach (var item in this.questConditions)
            {
                QuestDefine questDefine= DataManager.QuestDefines[item];
                if (questDefine.QuestConditionId == questConditionId)
                {
                    id = questConditionId;
                    this.GetQuest(id);
                    break;
                }
            }
            if(id!=-1)
            {
                this.questConditions.Remove(id);
            }

         
        }

        /// <summary>
        /// 承接任务
        /// </summary>
        /// <param name="preQuestId"></param>
        public void GetQuest(int preQuestId)
        {
            foreach (var item in DataManager.QuestDefines)
            {
                if (item.Value.QuestConditionId == preQuestId)
                {
                    this. SetQuest(item.Value.Id);
                    MessageManager.Instance.AddMessage(Message_Type.任务, string.Format("开启{0}要任务:{1}",item.Value.IsMain ? "主":"次",item.Value.QuestName));
                    break;
                }
            }
            (UIMain.Instance.uiPanels[1] as UIGamePanel).uIQuestPanel.UpdateAllUIQuest();
        }

        /// <summary>
        /// 到达一定回合结束任务
        /// </summary>
        /// <param name="round"></param>
        public void QuestEndByRound(int round)
        {
            QuestDefine questDefine = DataManager.QuestDefines[this.curMainQuestId];
            if(questDefine.RoundCondition!=round)
            {
                return;
            }

            ResourcesManager.Instance.wealth -= questDefine.CurrencyCondition;
            if (ResourcesManager.Instance.wealth < 0)
            {
                this.EndQuest(true, questDefine.Id, false);
            }
            else
            {
                this.EndQuest(true, questDefine.Id, true);
            }

            //else if (this.curSecondQuestsRound.Values.Contains(round))
            //{
            //    foreach (var id in this.curSecondQuestsRound)
            //    {
            //        if (id.Value == round)
            //        {
            //            QuestDefine questDefine = DataManager.QuestDefines[id.Key];
            //            ResourcesManager.Instance.wealth -= questDefine.CurrencyCondition;
            //            if (ResourcesManager.Instance.wealth < 0)
            //            {
            //                this.EndQuest(false, questDefine.Id, false);
            //            }
            //            else
            //            {
            //                this.EndQuest(false, questDefine.Id, true);
            //            }
            //            break;
            //        }
            //    }
            //}

        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="isMain"></param>
        /// <param name="questId"></param>
        public void SetQuest( int questId)
        {
            QuestDefine questDefine = DataManager.QuestDefines[questId];
            if (questDefine.IsMain)//主线任务
            {
                this.curMainQuestId= questId;
                this.curMainQuestRound = RoundManager.Instance.roundNumber+questDefine.RoundCondition;
            }
            //else//支线任务
            //{
            //    this.curSecondQuestIds.Add(questId);
            //    this.curSecondQuestsRound.Add(questId, RoundManager.Instance.roundNumber + questDefine.RoundCondition);
            //}
            //(UIMain.Instance.uiPanels[1] as UIGamePanel).uIQuestPanel.UpdateAllUIQuest();
        }

        /// <summary>
        /// 结束任务
        /// </summary>
        /// <param name="isMain"></param>
        /// <param name="questId"></param>
        /// <param name="isSuccess"></param>
        public void EndQuest(bool isMain, int questId,bool isSuccess)
        {
            QuestDefine questDefine = DataManager.QuestDefines[questId];
            if(isSuccess)
            {
                if (questDefine.GainCurrency > 0)
                {
                    ResourcesManager.Instance.ChangeWealth(questDefine.GainCurrency);
                }
                if (questDefine.GainUpgradePoint > 0)
                {
                    CapabilityManager.Instance.upgradePoint += questDefine.GainUpgradePoint;
                }
                int[] resources = new int[3] { questDefine.GainResource1, questDefine.GainResource2, questDefine.GainResource3 };
                ResourcesManager.Instance.ChangeBuildingResources(resources, true);

                NpcManager.Instance.NPCLeaveUnlock(0, this.curMainQuestId);
                NpcManager.Instance.NPCAppearUnlock(0, this.curMainQuestId);
                ChatManager.Instance.ChatConditionUnlock(3, this.curMainQuestId);
            }
            else
            {
                if(questDefine.QuestFallResult==0)//失败惩罚为游戏结束
                {
                    MainThreadDispatcher.StartUpdateMicroCoroutine(Main.Instance.GameOver());
                    return;
                }
                else 
                {

                }
            }

            this.RemoveQuest(isMain, questId);//移除任务

            this.GetQuest(questId);//继续下一个任务
        }


        /// <summary>
        /// 移除任务
        /// </summary>
        /// <param name="isMain"></param>
        /// <param name="questId"></param>
        void RemoveQuest(bool isMain, int questId=-1)
        {
            if (isMain)
            {
                this.curMainQuestId = -1;
                this.curMainQuestRound = -1;
                //this.curMainQuestIds.Remove(questId);
                //this.curMainQuestRound.Remove(questId);
            }
            //else
            //{
            //    this.curSecondQuestIds.Remove(questId);
            //    this.curSecondQuestsRound.Remove(questId);
            //}
            (UIMain.Instance.uiPanels[1] as UIGamePanel).uIQuestPanel.UpdateAllUIQuest();
        }

    }
}
