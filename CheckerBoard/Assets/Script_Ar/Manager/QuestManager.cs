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

        //�����������
        public HashSet<int> questConditions = new HashSet<int>();

        ////��ǰ��������ID 
        //public HashSet<int> curMainQuestIds;
        ////��ǰ����������ɻغ�
        //public Dictionary<int,int> curMainQuestRound;
        ////��ǰ�����������Npc
        //public Dictionary<int, Npc_Name> curMainQuestNpc;
        ////��ǰ����������ɰ��λ��
        //public Dictionary<int, Vector2Int> curMainQuestPos;

        ////��ǰ֧������ID
        //public HashSet<int> curSecondQuestIds=new HashSet<int>();
        ////��ǰ֧��������ɻغ�
        //public Dictionary<int, int> curSecondQuestsRound=new Dictionary<int, int>();
        ////��ǰ֧���������Npc
        //public Dictionary<int, Npc_Name> curSecondQuestNpc;
        ////��ǰ֧��������ɰ��λ��
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
                if (DataManager.QuestDefines[questManagerData.questIds[i]].IsMain)//���Ϊ����
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
        /// ��Ϸ����
        /// </summary>
        public void GameOver()
        {
            this.curMainQuestId = -1;
            this.curMainQuestRound = -1;
            this.questConditions.Clear();

            ////�����������
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
        /// ��������
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
        /// �н�����
        /// </summary>
        /// <param name="preQuestId"></param>
        public void GetQuest(int preQuestId)
        {
            foreach (var item in DataManager.QuestDefines)
            {
                if (item.Value.QuestConditionId == preQuestId)
                {
                    this. SetQuest(item.Value.Id);
                    MessageManager.Instance.AddMessage(Message_Type.����, string.Format("����{0}Ҫ����:{1}",item.Value.IsMain ? "��":"��",item.Value.QuestName));
                    break;
                }
            }
            (UIMain.Instance.uiPanels[1] as UIGamePanel).uIQuestPanel.UpdateAllUIQuest();
        }

        /// <summary>
        /// ����һ���غϽ�������
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
        /// �������
        /// </summary>
        /// <param name="isMain"></param>
        /// <param name="questId"></param>
        public void SetQuest( int questId)
        {
            QuestDefine questDefine = DataManager.QuestDefines[questId];
            if (questDefine.IsMain)//��������
            {
                this.curMainQuestId= questId;
                this.curMainQuestRound = RoundManager.Instance.roundNumber+questDefine.RoundCondition;
            }
            //else//֧������
            //{
            //    this.curSecondQuestIds.Add(questId);
            //    this.curSecondQuestsRound.Add(questId, RoundManager.Instance.roundNumber + questDefine.RoundCondition);
            //}
            //(UIMain.Instance.uiPanels[1] as UIGamePanel).uIQuestPanel.UpdateAllUIQuest();
        }

        /// <summary>
        /// ��������
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
                if(questDefine.QuestFallResult==0)//ʧ�ܳͷ�Ϊ��Ϸ����
                {
                    MainThreadDispatcher.StartUpdateMicroCoroutine(Main.Instance.GameOver());
                    return;
                }
                else 
                {

                }
            }

            this.RemoveQuest(isMain, questId);//�Ƴ�����

            this.GetQuest(questId);//������һ������
        }


        /// <summary>
        /// �Ƴ�����
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
