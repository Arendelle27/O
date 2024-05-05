using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UniRx;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace MANAGER
{
    public class ChatManager : Singleton<ChatManager>
    {
        //�������轻����������
        public Dictionary<int, HashSet<int>> chatConditions = new Dictionary<int, HashSet<int>>(4)
        {
            {0,new HashSet<int>()},//0Ϊ�غ�
            {1,new HashSet<int>()},//1Ϊ�Ի�����
            {2,new HashSet < int >() },//2Ϊ������
            {3,new HashSet < int >() }//3Ϊ�������
        };

        //������Ҫ������������
        public Dictionary<int, Dictionary<int, bool>> chaConditionsNpc = new Dictionary<int, Dictionary<int, bool>>(4)
        {
            {0,new Dictionary<int, bool>() },
            {1,new Dictionary<int, bool>() },
            {2,new Dictionary<int, bool>() },
            {3,new Dictionary<int, bool>() }
        };

        //��ǰ����Id
        int curChatId;
        public int CurChatId
        {
            get { return curChatId; }
            set 
            { 
                curChatId = value; 
                this.curChatDefines = DataManager.ChatDefines[curChatId];
                this.ShowChatWindow();
            }
        }

        /// <summary>
        /// ��ǰ�Ի�����
        /// </summary>
        Dictionary<int,ChatDefine> curChatDefines;

        //��ǰ��������Id
        int curChatContentId;
        public int CurChatContentId
        {
            get { return curChatContentId; }
            set
            {
                curChatContentId = value; 
                this.UpdateChatWindow(value);
            }
        }
        //���촰��
        public UIChatWindow uiChatWindow;

        //���λ��ȥ��һ��
        IDisposable onClickToNext;

        public void Restart()
        {
            foreach (var item in DataManager.ChatConditionDefines.Values)
            {
                if(item.Type==1)
                {
                    this.chatConditions[item.ChatCondition].Add(item.Id);
                }
                else
                {
                    this.chaConditionsNpc[item.ChatCondition].Add(item.Id,false);
                }
            }
            this.RoundStart(1);
        }

        public void ReadArchive()
        {
            ArchiveManager.ChatManagerData chatManagerData = ArchiveManager.archive.chatManagerData;

            foreach (var item in chatManagerData.chatConditionsData)
            {
                foreach (var id in item.chatConditionData)
                {
                    this.chatConditions[item.sort].Add(id);
                }
            }

            foreach (var item in chatManagerData.chatConditionNpcsData)
            {
                foreach (var id in item.chatConditionData)
                {
                    this.chaConditionsNpc[item.sort].Add(id.chatDefineId,id.condition);
                }
            }
        }

        public void GameOver()
        {
            foreach (var item in this.chatConditions.Values)
            {
                item.Clear();
            }
            foreach (var item in this.chaConditionsNpc.Values)
            {
                item.Clear();
            }
        }

        public void RoundStart(int round)
        {
            this.ChatConditionUnlock( 0, round);
        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="sort"></param>
        public void ChatConditionUnlock(int sort,int value)
        {

                foreach(var id in this.chaConditionsNpc[sort])
                {
                    ChatConditionDefine chatConditionDefine = DataManager.ChatConditionDefines[id.Key];
                    if(chatConditionDefine.ChatConditionValue==value)
                    {
                        this.chaConditionsNpc[sort][id.Key] = true;

                    }
                }
            

                int chatDefineId = -1;
                foreach (var id in this.chatConditions[sort])
                {
                    ChatConditionDefine chatConditionDefine = DataManager.ChatConditionDefines[id];
                    if (chatConditionDefine.ChatConditionValue == value)
                    {
                        chatDefineId = id;
                        this.CurChatId = id;
                        break;
                    }
                }
                if(chatDefineId!=-1)
                {
                    this.chatConditions[sort].Remove(chatDefineId);
                }
            

        }

        /// <summary>
        /// ��npc�Ի�
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="npcId"></param>
        public void ChatWithNpc(Npc_Name npc)
        {
            int chatDefineId = -1;
            foreach (var item in this.chaConditionsNpc.Values)
            {
                foreach(var id in item)
                {
                    if (id.Value)
                    {
                        continue;
                    }
                    ChatConditionDefine chatConditionDefine = DataManager.ChatConditionDefines[id.Key];
                    if (chatConditionDefine.NPC == npc)
                    {
                        this.CurChatId = id.Key;
                        chatDefineId = id.Key;
                        break;
                    }
                }
                if (chatDefineId != -1)
                {
                    item.Remove(chatDefineId);
                    break;
                }
            }

        }

        /// <summary>
        /// ��ʾ���촰��
        /// </summary>
        void ShowChatWindow()
        { 
            if(DataManager.ChatDefines.ContainsKey(curChatId))
            {
                UIChatWindow uiChatWindow = UIManager.Instance.Show<UIChatWindow>();
                if (this.uiChatWindow == null)
                {
                    this.uiChatWindow = uiChatWindow;
                }
                this.CurChatContentId = 0;
            }
        }

        int subEventType = -1;//��������¼�����
        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="chatContentId"></param>
        public void UpdateChatWindow(int CurChatContentId)
        {
            if (this.curChatDefines.ContainsKey(CurChatContentId))
            {
                ChatDefine chatDefine = this.curChatDefines[CurChatContentId];

                this.uiChatWindow.SetInfo(chatDefine);

                if (chatDefine.SubEventType != 0)
                {
                    this.subEventType = chatDefine.SubEventType;
                }

                if (chatDefine.IsOption==1)
                {
                    return;
                }
                if(this.onClickToNext != null)
                {
                    this.onClickToNext.Dispose();
                    this.onClickToNext = null;
                }
                this.ClickToNext();
            }
            else//�Ի�����
            {
                UIManager.Instance.Close<UIChatWindow>();

                NpcManager.Instance.NPCLeaveUnlock(1,this.curChatContentId);
                QuestManager.Instance.QuestUnlock(this.curChatId);
                this.ChatConditionUnlock(1, this.curChatId);

                if(this.subEventType==-1)
                {
                    return;
                }

                switch (this.subEventType)
                {
                    case 1://��ͻ�¼�
                        EventManager.Instance.SetConfrontEvent(0, EventAreaManager.Instance.hotility[0]);
                        break;
                    case 2://��������
                        NoviceGuideManager.Instance.OnStart();
                        break;
                    case 3://��������
                        PlotManager.Instance.unloadProp["������Կ��"] = true;
                        break;
                }
            }
        }

        /// <summary>
        /// ���ȥ��һ����
        /// </summary>
        void ClickToNext()
        {

                this.onClickToNext = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(0)).Subscribe(_ =>
                {
                    this.onClickToNext.Dispose();
                    this.CurChatContentId= this.curChatDefines[this.CurChatContentId].SubAnswerId;
                });
        }
    }
}
