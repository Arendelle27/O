using ENTITY;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

namespace MANAGER
{
    public class QuestManager :Singleton<QuestManager>
    {
        //��ǰ����ID,0Ϊ���ߣ�1Ϊ֧��
        public List<HashSet<int>> curQuestIds=new List<HashSet<int>>(2) {new HashSet<int>(),new HashSet<int>() };

        //�����������,0Ϊ�Ի���1Ϊ����
        public List<HashSet<int>> questConditions = new List<HashSet<int>>(2) { new HashSet<int>() ,new HashSet<int>()};

        public void Restart()
        {
            foreach (var item in DataManager.QuestDefines)
            {
                if (item.Value.IsExist != 0)
                    continue;
                this.questConditions[item.Value.PreContent].Add(item.Value.Id);
            }
        }

        public void ReadArchive()
        {
            ArchiveManager.QuestManagerData questManagerData = ArchiveManager.archive.questManagerData;

            foreach (var item in questManagerData.questMainIds)
            {
                this.curQuestIds[0].Add(item);
            }
            foreach (var item in questManagerData.questSecondIds)
            {
                this.curQuestIds[1].Add(item);
            }
            foreach (var item in questManagerData.questChatConditions)
            {
                this.questConditions[0].Add(item);
            }
            foreach (var item in questManagerData.questQuestConditions)
            {
                this.questConditions[1].Add(item);
            }
            (UIMain.Instance.uiPanels[1] as UIGamePanel).uIQuestPanel.UpdateAllUIQuest();
        }

        /// <summary>
        /// ��Ϸ����
        /// </summary>
        public void GameOver()
        {
            for(int i=0;i<this.curQuestIds.Count; i++)
            {
                this.curQuestIds[i].Clear();
            }

            for (int i = 0; i < this.questConditions.Count; i++)
            {
                this.questConditions[i].Clear();
            }

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
        public void QuestUnlock(int sort, int questConditionId)
        {
            List<int> ids = new List<int>();
            foreach (var item in this.questConditions[sort])
            {
                QuestDefine questDefine= DataManager.QuestDefines[item];
                if (questDefine.QuestConditionId == questConditionId)
                {
                    ids.Add(questConditionId);
                    this.GetQuest(questDefine.Id);
                }
            }
            foreach (var id in ids)
            {
                this.questConditions[sort].Remove(id);
            }
        }

        /// <summary>
        /// �н�����
        /// </summary>
        /// <param name="questDefineId"></param>
        public void GetQuest(int questDefineId)
        {
            QuestDefine questDefine = DataManager.QuestDefines[questDefineId];

            this.curQuestIds[questDefine.IsMain].Add(questDefineId);
            MessageManager.Instance.AddMessage(Message_Type.����, string.Format("��ȡ{0}Ҫ����:{1}",questDefine.IsMain==0?"��":"��", questDefine.QuestName));
            //if (questDefine.IsMain)//��������
            //{
            //    this.curQuestIds[0].Add(questDefineId);

            //    MessageManager.Instance.AddMessage(Message_Type.����, string.Format("��ȡ��Ҫ����:{0}", questDefine.QuestName));
            //}
            //else//֧������
            //{
            //    this.curQuestIds[1].Add(questDefineId);
            //    MessageManager.Instance.AddMessage(Message_Type.����, string.Format("��ȡ��Ҫ����:{0}", questDefine.QuestName));
            //}
            (UIMain.Instance.uiPanels[1] as UIGamePanel).uIQuestPanel.UpdateAllUIQuest();
        }

        /// <summary>
        /// ����һ���غϽ�������
        /// </summary>
        /// <param name="round"></param>
        public void QuestEndByRound(int round)
        {
            List<List<int>> removeQuestIds = new List<List<int>>(2) {new List<int>(),new List<int>() };
            for(int j=0;j< this.curQuestIds.Count; j++)
            {
                for (int i = 0; i < this.curQuestIds[j].Count; i++)
                {
                    var quest = this.curQuestIds[j].ElementAt(i);
                    QuestDefine questDefine = DataManager.QuestDefines[quest];
                    if(questDefine.RoundCondition == 0)
                    {
                        continue;
                    }
                    if (questDefine.RoundCondition == round)
                    {
                        if (this.FinishQuest(questDefine)==2)//��Ϊ����ʧ�ܶ�������Ϸ
                        {
                            return;
                        }
                        removeQuestIds[j].Add(quest);
                    }
                }
            }

            for (int j = 0; j < removeQuestIds.Count; j++)
            {
                for (int i = 0; i < removeQuestIds[j].Count; i++)
                {
                    this.curQuestIds[j].Remove(removeQuestIds[j][i]);
                }
            }

            if (removeQuestIds[0].Count > 0 || removeQuestIds[1].Count > 0)
            {
                (UIMain.Instance.uiPanels[1] as UIGamePanel).uIQuestPanel.UpdateAllUIQuest();
            }
        }

        /// <summary>
        /// �����ж�
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="value"></param>
        public void QuestEnd(int sort,int value)//0ΪNpc,1Ϊ���,2Ϊ����
        {
            List<List<int>> removeQuestIds = new List<List<int>>(2) { new List<int>(), new List<int>() };
            for (int j = 0; j < this.curQuestIds.Count; j++)
            {
                for (int i = 0; i < this.curQuestIds[j].Count; i++)
                {
                    var quest = this.curQuestIds[j].ElementAt(i);
                    QuestDefine questDefine = DataManager.QuestDefines[quest];
                    if(questDefine.QuestJudgeType!=sort)
                    {
                        continue;
                    }

                    if (questDefine.QuestJudgeValue == value)
                    {
                        if(this.FinishQuest(questDefine)==0)//��Ϊ����ɹ�
                        {
                            removeQuestIds[j].Add(quest);
                        }
                    }


                }
            }

            for (int j = 0; j < removeQuestIds.Count; j++)
            {
                for (int i = 0; i < removeQuestIds[j].Count; i++)
                {
                    this.curQuestIds[j].Remove(removeQuestIds[j][i]);
                }
            }
            if (removeQuestIds[0].Count > 0 || removeQuestIds[1].Count > 0)
            {
                (UIMain.Instance.uiPanels[1] as UIGamePanel).uIQuestPanel.UpdateAllUIQuest();
            }
        }

        /// <summary>
        /// ��������,0Ϊ�ɹ���1Ϊʧ�ܣ�2Ϊ��Ϸ����
        /// </summary>
        /// <param name="isMain"></param>
        /// <param name="questId"></param>
        /// <param name="isSuccess"></param>
        public int FinishQuest(QuestDefine questDefine)
        {
            bool isSuccess = true;
            if (questDefine.CurrencyCondition > 0)
            {
                isSuccess = ResourcesManager.Instance.wealth >= questDefine.CurrencyCondition;
            }
            if(questDefine.Resource1Condition>0&&isSuccess)
            {
                isSuccess = ResourcesManager.Instance.buildingResources[0] >= questDefine.Resource1Condition;
            }
            if(questDefine.Resource2Condition > 0 && isSuccess)
            {
                isSuccess = ResourcesManager.Instance.buildingResources[1] >= questDefine.Resource2Condition;
            }
            if (questDefine.Resource3Condition > 0 && isSuccess)
            {
                isSuccess = ResourcesManager.Instance.buildingResources[2] >= questDefine.Resource3Condition;
            }

            if (isSuccess)
            {
                MessageManager.Instance.AddMessage(Message_Type.����, string.Format("�������:{0}", questDefine.QuestName));
                ResourcesManager.Instance.ChangeWealth(-questDefine.CurrencyCondition);
                ResourcesManager.Instance.ChangeBuildingResources(new int[3] { questDefine.Resource1Condition, questDefine.Resource2Condition, questDefine.Resource3Condition }, false);

                ResourcesManager.Instance.ChangeWealth(questDefine.GainCurrency);
                CapabilityManager.Instance.upgradePoint += questDefine.GainUpgradePoint;
                ResourcesManager.Instance.ChangeBuildingResources(new int[3] { questDefine.GainResource1, questDefine.GainResource2, questDefine.GainResource3 }, true);

                EventAreaManager.Instance.hotility[0] += questDefine.HumanHostility;
                EventAreaManager.Instance.hotility[1] += questDefine.RobotHostility;

                NpcManager.Instance.NPCLeaveUnlock(0, questDefine.Id);
                ChatManager.Instance.ChatConditionUnlock(3, questDefine.Id);
                this.QuestUnlock(1, questDefine.Id);
                return 0;
            }
            else
            {
                if (questDefine.QuestFallResult == 1)
                {
                    MainThreadDispatcher.StartUpdateMicroCoroutine(Main.Instance.GameOver(2));
                    return 2;
                }
                return 1;
            }

        }


    }
}
