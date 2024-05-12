using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UniRx;
using UnityEngine;

namespace MANAGER
{
    public class ChatManager : Singleton<ChatManager>
    {
        //聊天无需交互解锁条件
        public Dictionary<int, HashSet<int>> chatConditions = new Dictionary<int, HashSet<int>>(4)
        {
            {0,new HashSet<int>()},//0为回合
            {1,new HashSet<int>()},//1为对话结束
            {2,new HashSet < int >() },//2为进入板块
            {3,new HashSet < int >() },//3为完成任务
            {4,new HashSet < int >() }//4为CG
        };

        //聊天需要交互解锁条件
        public Dictionary<int, Dictionary<int, bool>> chatConditionsNpc = new Dictionary<int, Dictionary<int, bool>>(4)
        {
            {0,new Dictionary<int, bool>() },
            {1,new Dictionary<int, bool>() },
            {2,new Dictionary<int, bool>() },
            {3,new Dictionary<int, bool>() },
            {4,new Dictionary<int, bool>()  }//4为CG
        };

        //当前聊天Id
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
        /// 当前对话内容
        /// </summary>
        Dictionary<int,ChatDefine> curChatDefines;

        //当前聊天内容Id
        int curChatContentId;
        public int CurChatContentId
        {
            get { return curChatContentId; }
            set
            {
                curChatContentId = value; 
                this.UpdateChatWindow(value);
                this.PlayChatNextVoice();
            }
        }
        //聊天窗口
        public UIChatWindow uiChatWindow;

        //点击位置去下一步
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
                    this.chatConditionsNpc[item.ChatCondition].Add(item.Id,false);
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
                    this.chatConditionsNpc[item.sort].Add(id.chatDefineId,id.condition);
                }
            }
        }

        public void GameOver()
        {
            foreach (var item in this.chatConditions.Values)
            {
                item.Clear();
            }
            foreach (var item in this.chatConditionsNpc.Values)
            {
                item.Clear();
            }
        }

        public void RoundStart(int round)
        {
            this.ChatConditionUnlock( 0, round);
        }

        /// <summary>
        /// 聊天条件解锁
        /// </summary>
        /// <param name="sort"></param>
        public void ChatConditionUnlock(int sort,int value)
        {
            Dictionary<int,int> npcs = new Dictionary<int, int>();
            for(int i = 0; i < this.chatConditionsNpc[sort].Count;i++)
            {
                var item = this.chatConditionsNpc[sort].ElementAt(i);
                ChatConditionDefine chatConditionDefine = DataManager.ChatConditionDefines[item.Key];
                if (chatConditionDefine.ChatConditionValue == value)
                {
                    this.chatConditionsNpc[sort][item.Key] = true;
                    NpcManager.Instance.npcs[chatConditionDefine.NPC].ShowSwitch(true);

                    npcs.Add(chatConditionDefine.NPC, item.Key);

                }
            }
            //foreach (var npc in WandererManager.Instance.wanderer.plot.npcs)
            //{
            //    if (npcs.ContainsKey(npc.npcDefine.Name))
            //    {
            //        this.CurChatId = npcs[npc.npcDefine.Name];
            //        this.chatConditionsNpc[sort].Remove(npcs[npc.npcDefine.Name]);
            //        break;
            //    }
            //}

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
            if (chatDefineId != -1)
            {
                this.chatConditions[sort].Remove(chatDefineId);

            }


        }

        /// <summary>
        /// 与npc对话
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="npcId"></param>
        public void ChatWithNpc(int npc)
        {
            int chatDefineId = -1;
            foreach (var item in this.chatConditionsNpc.Values)
            {
                foreach(var id in item)
                {
                    if (!id.Value)
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
        /// 显示聊天窗口
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

        int subEventType = -1;//结束后的事件类型
        int subEventValue = 0;
        /// <summary>
        /// 更新聊天内容
        /// </summary>
        /// <param name="chatContentId"></param>
        public void UpdateChatWindow(int CurChatContentId)
        {
            if (this.curChatDefines.ContainsKey(CurChatContentId))
            {
                this.subEventType = -1;//结束后的事件类型
                this.subEventValue = 0;
                ChatDefine chatDefine = this.curChatDefines[CurChatContentId];

                this.uiChatWindow.SetInfo(chatDefine);

                if (chatDefine.SubEventType != 0)
                {
                    this.subEventType = chatDefine.SubEventType;
                    this.subEventValue = chatDefine.SubEventValue;
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
            else//对话结束
            {
                UIManager.Instance.Close<UIChatWindow>();
                NpcManager.Instance.NPCLeaveUnlock(1,this.curChatId);
                NpcManager.Instance.NPCAppearUnlock(0, this.curChatId);
                QuestManager.Instance.QuestUnlock(this.curChatId);
                this.ChatConditionUnlock(1, this.curChatId);

                if(this.subEventType==-1)
                {
                    return;
                }

                switch (this.subEventType)
                {
                    case 1://冲突事件
                        EventManager.Instance.SetConfrontEvent(0, EventAreaManager.Instance.hotility[0]);
                        break;
                    case 2://新手引导
                        NoviceGuideManager.Instance.OnStart();
                        Debug.Log("新手引导");
                        break;
                    case 3://解锁地下室钥匙
                        PlotManager.Instance.unloadProp[Prop_Type.地下室钥匙] = true;
                        MessageManager.Instance.AddMessage(Message_Type.探索, string.Format("获得了{0}", Prop_Type.地下室钥匙.ToString()));
                        break;
                    case 4://播放CG
                        CGManager.Instance.PlayCG((CG_Type) this.subEventValue);
                        break;
                    case 5://拓展探索小队
                        CapabilityManager.Instance.expendExploratoryAmount++;
                        PlotManager.Instance.EnterSelectExtendExpTeam(true);//进入选择扩展探索小队的模式
                        break;
                    case 6://打开黑市界面
                        UITransactionWindow uTW = UIManager.Instance.Show<UITransactionWindow>();
                        EventAreaManager.Instance.uITransactionWindow ??= uTW;//显示交易界面
                        break;
                    case 7://解锁战斗机械蓝图
                        BuildingManager.Instance.bluePrints[1] = true;
                        break;
                    case 8://结局CG
                        MainThreadDispatcher.StartUpdateMicroCoroutine(Main.Instance.GameOver(subEventValue));
                        break;
                    case 9://增加人类阵营敌意
                        EventAreaManager.Instance.hotility[0] += this.subEventValue;
                        break;
                }
            }
        }

        /// <summary>
        /// 点击去下一个步
        /// </summary>
        void ClickToNext()
        {

                this.onClickToNext = Observable.EveryUpdate().Where(_ => Input.GetMouseButtonDown(0)).Subscribe(_ =>
                {
                    this.onClickToNext.Dispose();
                    this.CurChatContentId= this.curChatDefines[this.CurChatContentId].SubAnswerId;
                    //this.PlayChatNextVoice();
                });
        }

        void PlayChatNextVoice()
        {
            SoundManager.Instance.PlayVoice("Chat");
        }
    }
}
