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
        //��ǰ��������ID 
        public int curMainQUestId;
        //��ǰ����������ɻغ�
        public int curMainQuestRound;

        //��ǰ֧������ID
        public HashSet<int> curSecondQuestIds=new HashSet<int>();
        //��ǰ֧��������ɻغ�
        public Dictionary<int, int> curSecondQuestsRound=new Dictionary<int, int>();

        public void ReadArchive()
        {
            ArchiveManager.QuestManagerData questManagerData = ArchiveManager.archive.questManagerData;

            for(int i=1;i<questManagerData.questIds.Count;i++)
            {
                if (DataManager.QuestDefines[questManagerData.questIds[i]].Type)//���Ϊ����
                {
                    this.curMainQUestId = questManagerData.questIds[i];
                    this.curMainQuestRound = questManagerData.questsRound[i];
                }
                else
                {
                    this.curSecondQuestIds.Add(questManagerData.questIds[i]);
                    this.curSecondQuestsRound.Add(questManagerData.questIds[i], questManagerData.questsRound[i]);
                }
            }

            (UIMain.Instance.uiPanels[1] as UIGamePanel).uIQuestPanel.UpdateAllUIQuest();
        }

        /// <summary>
        /// �н�����
        /// </summary>
        /// <param name="preQuestId"></param>
        public void GetQuest(int preQuestId)
        {
            foreach (var item in DataManager.QuestDefines)
            {
                if (item.Value.PreQuestId == preQuestId)
                {
                    this. SetQuest(item.Value.Id);
                    MessageManager.Instance.AddMessage(Message_Type.����, string.Format("����{0}Ҫ����:{1}",item.Value.Type?"��":"��",item.Value.QuestName));
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
            if (this.curMainQuestRound == round)
            {
                QuestDefine questDefine = DataManager.QuestDefines[this.curMainQUestId];
                ResourcesManager.Instance.wealth -= questDefine.CurrencyCondition;
                if (ResourcesManager.Instance.wealth < 0)
                {
                    this.EndQuest(true, questDefine.Id, false);
                }
                else
                {
                    this.EndQuest(true, questDefine.Id, true);
                }
            }
            else if (this.curSecondQuestsRound.Values.Contains(round))
            {
                foreach (var id in this.curSecondQuestsRound)
                {
                    if (id.Value == round)
                    {
                        QuestDefine questDefine = DataManager.QuestDefines[id.Key];
                        ResourcesManager.Instance.wealth -= questDefine.CurrencyCondition;
                        if (ResourcesManager.Instance.wealth < 0)
                        {
                            this.EndQuest(false, questDefine.Id, false);
                        }
                        else
                        {
                            this.EndQuest(false, questDefine.Id, true);
                        }
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="isMain"></param>
        /// <param name="questId"></param>
        public void SetQuest( int questId)
        {
            QuestDefine questDefine = DataManager.QuestDefines[questId];
            if (questDefine.Type)//��������
            {
                this.curMainQUestId = questId;
                this.curMainQuestRound = RoundManager.Instance.roundNumber+questDefine.RoundCondition;
            }
            else//֧������
            {
                this.curSecondQuestIds.Add(questId);
                this.curSecondQuestsRound.Add(questId, RoundManager.Instance.roundNumber + questDefine.RoundCondition);
            }
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
        void RemoveQuest(bool isMain, int questId)
        {
            if (isMain)
            {
                this.curMainQUestId = -1;
                this.curMainQuestRound = -1;
            }
            else
            {
                this.curSecondQuestIds.Remove(questId);
                this.curSecondQuestsRound.Remove(questId);
            }
            (UIMain.Instance.uiPanels[1] as UIGamePanel).uIQuestPanel.UpdateAllUIQuest();
        }

        /// <summary>
        /// ��Ϸ����
        /// </summary>
        public void GameOver()
        {
            //�����������
            this.RemoveQuest(true, this.curMainQUestId);//�����������

            for(int i=0;i<this.curSecondQuestIds.Count;)
            {
                this.RemoveQuest(false, this.curSecondQuestIds.ElementAt(i));
            }
        }
    }
}
