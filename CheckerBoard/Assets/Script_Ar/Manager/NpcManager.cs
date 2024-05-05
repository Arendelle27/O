using ENTITY;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace MANAGER
{
    public class NpcManager : MonoSingleton<NpcManager>
    {
        //当前存在npc
        public Dictionary<Npc_Name,Npc> npcs=new Dictionary<Npc_Name,Npc>();
        //npc的解锁条件
        public Dictionary<int,Dictionary<int,List<bool>>> npcAppearConditions=new Dictionary<int, Dictionary<int, List<bool>>>(4) 
        {
            {-1,new Dictionary<int, List<bool>>()},//无
            {0,new Dictionary<int, List<bool>>()},//任务
            {1,new Dictionary<int, List<bool>>()},//板块
            {2,new Dictionary<int, List<bool>>()},//已经相遇过
        };

        //npc的离开条件
        public Dictionary<int, Dictionary<int, List<bool>>> npcLeaveConditions = new Dictionary<int, Dictionary<int, List<bool>>>(3)
        {
            {-1,new Dictionary<int, List<bool>>()},//无
            {0,new Dictionary<int, List<bool>>()},//任务
            {1,new Dictionary<int, List<bool>>()},//对话
        };

        public void Restart()
        {
            foreach(var npc in DataManager.NPCDefines.Values)
            {
                switch (npc.AppearCondition)
                {
                    case -1:
                        if (!npcAppearConditions[-1].ContainsKey(npc.AppearCondition))
                        {
                            npcAppearConditions[-1].Add(npc.Id, new List<bool>(1) { false });
                        }
                        break;
                    default:
                        if (!npcAppearConditions[npc.AppearCondition].ContainsKey(npc.AppearCondition))
                        {
                            if (npc.AppearRound == 0)
                            {
                                npcAppearConditions[npc.AppearCondition].Add(npc.Id, new List<bool>(2) { true, false });
                            }
                            else
                            {
                                npcAppearConditions[npc.AppearCondition].Add(npc.Id, new List<bool>(2) { false, false });
                            }
                        }
                        break;
                }

                switch (npc.LeaveCondition)
                {
                    case -2:
                        break;
                    case -1:
                        if (!npcLeaveConditions[-1].ContainsKey(npc.LeaveCondition))
                        {
                            npcLeaveConditions[-1].Add(npc.Id, new List<bool>(1) { false });
                        }
                        break;
                    default:
                        if (!npcLeaveConditions[npc.LeaveCondition].ContainsKey(npc.LeaveCondition))
                        {
                            if(npc.LeaveRound==0)
                            {
                                npcLeaveConditions[npc.LeaveCondition].Add(npc.Id, new List<bool>(2) { true, false });
                            }
                            else
                            {
                                npcLeaveConditions[npc.LeaveCondition].Add(npc.Id, new List<bool>(2) { false, false });
                            }
                        }
                        break;
                }
            }
            this.RoundStart(1);
        }

        public void ReadArchive()
        {
            ArchiveManager.NpcManagerData npcManagerData = ArchiveManager.archive.npcManagerData;
            foreach (var npcAppearCondition in npcManagerData.npcsData)
            {
                this.NpcAppearOrLeave(true, npcAppearCondition.npcDefineId);
            }
            foreach(var npcCondition in npcManagerData.npcConditionsData)
            {
                if(npcCondition.isAppear)
                {
                    foreach (var npc in npcCondition.npcConditionData)
                    {
                        this.npcAppearConditions[npcCondition.sort].Add(npc.npcDefineId,npc.conditions);
                    }
                }
                else
                {
                    foreach (var npc in npcCondition.npcConditionData)
                    {
                        this.npcLeaveConditions[npcCondition.sort].Add(npc.npcDefineId, npc.conditions);
                    }
                }
            }
        }

        /// <summary>
        /// 回合结束时
        /// </summary>
        /// <param name="round"></param>
        public void RoundOver(int round)
        {
            List<List<int>> unlockList = new List<List<int>>();
            foreach (var npcLeaveCondition in npcLeaveConditions.Values)
            {
                foreach (var npcDefineId in npcLeaveCondition.Keys)
                {
                    NPCDefine npcDefine = DataManager.NPCDefines[npcDefineId];
                    if (npcDefine.LeaveRound == round)
                    {
                        npcLeaveCondition[npcDefineId][0] = true;

                        if(this.IsUnlock(false, npcDefine.LeaveCondition, npcDefineId))
                        {
                            unlockList.Add(new List<int> { npcDefine.LeaveCondition, npcDefineId });
                        }
                    }
                }
            }
            this.RemoveNpcConditions(false, unlockList);
        }

        /// <summary>
        /// 回合开始时
        /// </summary>
        /// <param name="round"></param>
        public void RoundStart(int round)
        {
            List<List<int>> unlockList = new List<List<int>>();
            foreach(var npcAppearCondition in npcAppearConditions.Values)
            {
                foreach(var npcDefineId in npcAppearCondition.Keys)
                {
                    NPCDefine npcDefine = DataManager.NPCDefines[npcDefineId];
                    if(npcDefine.AppearRound==round)
                    {
                        npcAppearCondition[npcDefineId][0] = true;

                        if(this.IsUnlock(true, npcDefine.AppearCondition, npcDefineId))
                        {
                            unlockList.Add(new List<int> { npcDefine.AppearCondition, npcDefineId });
                        }
                    }
                }
            }
            this.RemoveNpcConditions(true, unlockList);
        }

        public void GameOver()
        {
            this.RemoveAllNpcCondition();
            this.ClearAllNPC();
        }

        /// <summary>
        /// npc出现条件解锁
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="conditionValue"></param>
        public void NPCAppearUnlock(int sort,int conditionValue)
        {
            List<List<int>> unlockList = new List<List<int>>();
            foreach (var npc in this.npcAppearConditions[sort])
            {
                NPCDefine npcDefine = DataManager.NPCDefines[npc.Key];
                if(npcDefine.AppearConditionValue==conditionValue)
                {
                    npc.Value[1] = true;
                    if(this.IsUnlock(true, sort, npc.Key))
                    {
                        unlockList.Add(new List<int> { sort, npc.Key });
                    }
                }
            }
            this.RemoveNpcConditions(true, unlockList);
        }

        /// <summary>
        /// npc离开条件解锁
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="conditionValue"></param>
        public void NPCLeaveUnlock(int sort,int conditionValue)
        {
            List<List<int>> unlockList = new List<List<int>>();
            foreach (var npc in this.npcLeaveConditions[sort])
            {
                NPCDefine npcDefine = DataManager.NPCDefines[npc.Key];
                if (npcDefine.LeaveConditionValue == conditionValue)
                {
                    npc.Value[1] = true;
                    if (this.IsUnlock(false, sort, npc.Key))
                    {
                        unlockList.Add(new List<int> { sort, npc.Key });
                    }
                }
            }
            this.RemoveNpcConditions(true, unlockList);
        }

        /// <summary>
        /// 判断npc是否解锁
        /// </summary>
        /// <param name="isAppear"></param>
        /// <param name="sort"></param>
        /// <param name="npcDefineId"></param>
        bool IsUnlock(bool isAppear, int sort, int npcDefineId)
        {
            bool isAllUnlock = true;
            if(isAppear)
            {
                foreach (var isUnlock in this.npcAppearConditions[sort][npcDefineId])
                {
                    if (!isUnlock)
                    {
                        isAllUnlock = false;
                        break;
                    }
                }
                if (isAllUnlock)
                {
                    this.NpcAppearOrLeave(true, npcDefineId);
                }
            }
            else
            {
                foreach (var isUnlock in this.npcLeaveConditions[sort][npcDefineId])
                {
                    if (!isUnlock)
                    {
                        isAllUnlock = false;
                        break;
                    }
                }
                if (isAllUnlock)
                {
                    this.NpcAppearOrLeave(false, npcDefineId);
                }
            }
            return isAllUnlock;
        }

        /// <summary>
        /// npc出现和离开
        /// </summary>
        /// <param name="isAppear"></param>
        /// <param name="sort"></param>
        /// <param name="npcDefineId"></param>
        void NpcAppearOrLeave(bool isAppear,int npcDefineId)
        {
            if(isAppear)
            {
                GameObject npc=GameObjectPool.Instance.NPCs.Get();
                npc.transform.SetParent(this.transform);
                Npc npcScript = npc.GetComponent<Npc>();

                NPCDefine npcDefine = DataManager.NPCDefines[npcDefineId];
                Vector2Int pos = new Vector2Int(npcDefine.PositionX, npcDefine.PositionY);
                Plot plot= PlotManager.Instance.NPCAppear(pos);
                npcScript.SetInfo(npcDefineId, plot);
                plot.npcs.Add(npcScript);

                this.npcs.Add(npcDefine.Name, npcScript);
            }
            else
            {
                NPCDefine npcDefine = DataManager.NPCDefines[npcDefineId];
                GameObjectPool.Instance.NPCs.Release(this.npcs[npcDefine.Name].gameObject);
                Vector2Int pos = new Vector2Int(npcDefine.PositionX, npcDefine.PositionY);
                Plot plot= PlotManager.Instance.NPCAppear(pos);
                plot.npcs.Remove(this.npcs[npcDefine.Name]);

                this.npcs.Remove(npcDefine.Name);
            }
        }

        /// <summary>
        /// 移除npc出现离开条件
        /// </summary>
        /// <param name="isAppear"></param>
        /// <param name="sort"></param>
        /// <param name="npcDefineId"></param>
        void RemoveNpcConditions(bool isAppear,List<List<int>> npcDefineIds)
        {
            if(isAppear)
            {
                foreach (var npc in npcDefineIds)
                {
                    this.npcAppearConditions[npc[0]].Remove(npc[1]);
                }
            }
            else
            {
                foreach (var npc in npcDefineIds)
                {
                    this.npcLeaveConditions[npc[0]].Remove(npc[1]);
                }
            }
        }

        /// <summary>
        /// 移除所有npc出现离开条件
        /// </summary>
        void RemoveAllNpcCondition()
        {
            foreach(var npc in this.npcAppearConditions)
            {
                npc.Value.Clear();
            }
            foreach(var npc in this.npcLeaveConditions)
            {
                npc.Value.Clear();
            }
        }

        void ClearAllNPC()
        {
            for(int i = 0;i < this.npcs.Count;)
            {
                var key = this.npcs.ElementAt(i).Key;
                GameObjectPool.Instance.NPCs.Release(this.npcs[key].gameObject);
                this.npcs.Remove(key);
            }
        }
    }
}
