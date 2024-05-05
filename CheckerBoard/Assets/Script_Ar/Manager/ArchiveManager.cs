using ENTITY;
using MANAGER;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class ArchiveManager
{
    [Serializable]
    public class Archive
    {
        public PlotManagerData plotManagerData;//地块管理器数据
        public WandererManagerData wandererManagerData;
        public BuildingManagerData buildingManagerData;
        public EventAreaManagerData eventAreaManagerData;
        public CapabilityManagerData capabilityManagerData;
        public EventManagerData eventManagerData;
        public MessageManagerData messageManagerData;
        public ResourcesManagerData resourcesManagerData;
        public RoundManagerData roundManagerData;
        public QuestManagerData questManagerData;
        public NpcManagerData npcManagerData;
        public ChatManagerData chatManagerData;
        public Vector3 CameraPos;
    }

    [Serializable]
    public class PlotManagerData
    {
        [Serializable]
        public class PlotData
        {
            public Vector2Int pos;
            public int plotDefineId;
            public Plot_Statue plotStatue;
            public bool isFirstExplored;
            public List<int> buildingResources;
            public bool canEnter;
        }

        [Serializable]
        public class PlotTypeData
        {
            public List<int> plotTypes;
        }

        [Serializable]
        public class PlotConditions
        {
            public List<int> ids;
            public List<string> conditions;
        }

        [Serializable]
        public class UnloadPropData
        {
            public string propName;
            public bool isUnloaded;
        }

        public List<Vector2Int> haveExploredPlots = new List<Vector2Int>();//已探索地块
        public List<PlotData> plotsData = new List<PlotData>();//地块数据
        public List<int> plotTypeDesepicalIds=new List<int>();
        public List<PlotTypeData> plotTypes = new List<PlotTypeData>(2) { new PlotTypeData(), new PlotTypeData() };//地块类型
        public List<PlotConditions> plotConditions = new List<PlotConditions>(3){ new PlotConditions(), new PlotConditions(), new PlotConditions()};
        public List<UnloadPropData> unloadPropsData = new List<UnloadPropData>();//地块管理器数据
    }

    [Serializable]
    public class WandererManagerData
    {
        [Serializable]
        public class WandererData
        {
            public Vector2Int pos;
        }

        public WandererData wandererData=new WandererData();
        public List<Vector2Int> exploratoryTeams=new List<Vector2Int>();
    }

    [Serializable]
    public class BuildingManagerData
    {
        [Serializable]
        public class BuildingData
        {
            public Vector2Int pos;
            public Building_Type buildingType;

            public int existRound = 0;
        }

        [Serializable]
        public class BuildingContitionData
        {
            public Building_Type buildingType;
            public int amount;
        }
        [Serializable]
        public class BuildingContitionsData
        {
            public List<BuildingContitionData> buildingContitions = new List<BuildingContitionData>();
        }
        [Serializable]
        public class BuildingTypesData
        {
            public List<Building_Type> buildingTypes = new List<Building_Type>();
        }
        [Serializable]
        public class BluePrintsData
        {
            public int bluePrintId;
            public bool isUnlocked;
        }

        public List<BuildingData> buildingsData = new List<BuildingData>();
        public List<BuildingContitionsData> buildingConditions = new List<BuildingContitionsData>(6){ new BuildingContitionsData(), new BuildingContitionsData(), new BuildingContitionsData(),new BuildingContitionsData(),new BuildingContitionsData(),new BuildingContitionsData()};
        public List<BuildingTypesData> buildingTypes = new List<BuildingTypesData>(3) { new BuildingTypesData(), new BuildingTypesData(), new BuildingTypesData() };
        public List<BluePrintsData> bluePrintsData = new List<BluePrintsData>();
    }

    [Serializable]
    public class EventAreaManagerData
    {
        [Serializable]
        public class PurchaseObjectStatueData
        {
            public int purchaseObjectId;
            public List<int> purchaseObjectStatue =new List<int>(2){0,0};
        }

        [Serializable]
        public class PurchaseObjectsStatueData
        {
            public List<PurchaseObjectStatueData> purchaseObjectStatueDatas = new List<PurchaseObjectStatueData>();
        }

        [Serializable]
        public class SellObjectStatueData
        {
            public int sellObjectId;
            public int sellObjectStatue;
        }

        [Serializable]
        public class SellObjectsStatueData
        {
            public List<SellObjectStatueData> sellObjectStatueDatas = new List<SellObjectStatueData>();
        }

        public List<PurchaseObjectsStatueData> purchaseObjectStatueDatas = new List<PurchaseObjectsStatueData>(2) {new PurchaseObjectsStatueData(),new PurchaseObjectsStatueData()};
        public List<SellObjectsStatueData> sellObjectsStatueDatas = new List<SellObjectsStatueData>(2) {new SellObjectsStatueData(),new SellObjectsStatueData()};
        public List<float> hotility=new List<float>(2) { 0f,0f};
    }

    [Serializable]
    public class CapabilityManagerData
    {
        public int upgradePoint = 0;
        public int upgradePointHaveBuy = 0;
        public List<int> curLevels = new List<int>(3)
        {
            0,0,0
        };
        public int freelyReduceCoolingRound = 0;
        public int executionAmount = 0;
    }

    [Serializable]
    public class EventManagerData
    {
        public List<int> curConfrontEventIndex = new List<int>(2) { -1, 0 };
        public Vector2Int curClashAreaPos;
    }

    [Serializable]
    public class MessageManagerData
    {
        [Serializable]
        public class MessageData
        {
            public Message_Type type;
            public string message;
        }

        public List<MessageData> yesterdayMessages = new List<MessageData>();
        public List<MessageData> todayMessages = new List<MessageData>();
    }

    [Serializable]
    public class ResourcesManagerData
    {
        public int roundNumber;
        public int wealth;
        public List<int> buildingResources;//建筑资源
        public int execution;
    }
    [Serializable]
    public class RoundManagerData
    {
        public int roundNumber;
    }

    [Serializable]
    public class QuestManagerData
    {
        public List<int> questIds = new List<int>();//存在主线时,0为主线任务,后续为支线任务
        public List<int> questsRound = new List<int>();//存在主线时,0为主线任务,后续为支线任务

        public List<int> questConditions = new List<int>();
    }

    [Serializable]
    public class NpcManagerData
    {
        [Serializable]
        public class NpcData
        {
            public Npc_Name npcName;
            public int npcDefineId;
            public Vector2Int pos;
        }

        [Serializable]
        public class NpcConditionData
        {
            public int npcDefineId;
            public List<bool> conditions=new List<bool>();
        }

        [Serializable]
        public class NpcConditionsData
        {
            public bool isAppear;
            public int sort;
            public List<NpcConditionData> npcConditionData=new List<NpcConditionData>();
        }

        public List<NpcData> npcsData =new List<NpcData>();
        public List<NpcConditionsData> npcConditionsData = new List<NpcConditionsData>();
    }

    [Serializable]
    public class ChatManagerData
    {
        [Serializable]
        public class ChatConditionsData
        {
            public int sort;
            public List<int> chatConditionData = new List<int>();
        }


        [Serializable]
        public class ChatConditionNpcData
        {
            public int chatDefineId;
            public bool condition;
        }

        [Serializable]
        public class ChatConditionsNpcData
        {
            public int sort;
            public List<ChatConditionNpcData> chatConditionData = new List<ChatConditionNpcData>();
        }

        public List<ChatConditionsData> chatConditionsData = new List<ChatConditionsData>();
        public List<ChatConditionsNpcData> chatConditionNpcsData = new List<ChatConditionsNpcData>();
    }

    //存档
    public static Archive archive;

    /// <summary>
    /// 保存游戏
    /// </summary>
    public static void SaveData()
    {
        Archive arc = new Archive();//建立存档
        #region 地块管理器数据
        PlotManagerData plotManagerData = new PlotManagerData();
        //地块数据
        foreach (var plot in PlotManager.Instance.plots.Values)
        {
            plotManagerData.plotsData.Add(new PlotManagerData.PlotData
            {
                pos = plot.pos,
                plotDefineId = plot.plotDefine.ID,
                plotStatue = plot.plot_Statue,
                isFirstExplored = plot.isFirstExplored,
                buildingResources = plot.buildingResources,
                canEnter = plot.canEnter
            }); ;
        }
        //已探索地块
        plotManagerData.haveExploredPlots = PlotManager.Instance.haveExploredPlots.ToList();
        //非特殊地块类型Id
        plotManagerData.plotTypeDesepicalIds = PlotManager.Instance.plotTypeDesepical.Keys.ToList();
        //地块类型
        for (int i=0;i<plotManagerData.plotTypes.Count;i++)
        {
            plotManagerData.plotTypes[i].plotTypes = PlotManager.Instance.plotTypes[i].ToList();
        }
        //地块解锁条件
        for(int i=0; i<plotManagerData.plotConditions.Count;i++)
        {
            plotManagerData.plotConditions[i].ids = PlotManager.Instance.plotConditions[i].Keys.ToList();
            plotManagerData.plotConditions[i].conditions = PlotManager.Instance.plotConditions[i].Values.ToList();
        }
        //解锁道具
        foreach (var plot in PlotManager.Instance.unloadProp)
        {
            plotManagerData.unloadPropsData.Add(new PlotManagerData.UnloadPropData
            {
                propName = plot.Key,
                isUnloaded = plot.Value
            });
        }

        arc.plotManagerData = plotManagerData;
        #endregion

        #region 流浪者管理器数据
        WandererManagerData wandererManagerData = new WandererManagerData();

        //流浪者数据
        wandererManagerData.wandererData.pos = WandererManager.Instance.wanderer.plot.pos;
        //探索小队
        wandererManagerData.exploratoryTeams = WandererManager.Instance.exploratoryTeams.Keys.ToList<Vector2Int>();

        arc.wandererManagerData = wandererManagerData;
        #endregion

        #region 建筑管理器数据
        BuildingManagerData buildingManagerData = new BuildingManagerData();

        foreach (var building in BuildingManager.Instance.gatheringBuildings.Values)
        {
            buildingManagerData.buildingsData.Add(new BuildingManagerData.BuildingData
            {
                pos = building.pos,
                buildingType = building.type,
                existRound = building.existRound
            });
        }

        foreach (var building in BuildingManager.Instance.productionBuildings.Values)
        {
            buildingManagerData.buildingsData.Add(new BuildingManagerData.BuildingData
            {
                pos = building.pos,
                buildingType = building.type
            });
        }

        foreach (var building in BuildingManager.Instance.battleBuildings.Values)
        {
            buildingManagerData.buildingsData.Add(new BuildingManagerData.BuildingData
            {
                pos = building.pos,
                buildingType = building.type
            });
        }

        for (int i = 0; i < buildingManagerData.buildingConditions.Count; i++)
        {
            foreach (var building in BuildingManager.Instance.buildingConditions[(Building_Condition_Type)i])
            {
                buildingManagerData.buildingConditions[i].buildingContitions.Add(new BuildingManagerData.BuildingContitionData
                {
                    buildingType = building.Key,
                    amount = building.Value
                });
            }
        }

        for (int i = 0; i < buildingManagerData.buildingTypes.Count; i++)
        {
            buildingManagerData.buildingTypes[i].buildingTypes = BuildingManager.Instance.buildingTypes[i].ToList();
        }

        foreach (var bluePrint in BuildingManager.Instance.bluePrints)
        {
            buildingManagerData.bluePrintsData.Add(new BuildingManagerData.BluePrintsData
            {
                bluePrintId = bluePrint.Key,
                isUnlocked = bluePrint.Value
            });
        }

        arc.buildingManagerData = buildingManagerData;
        #endregion

        #region 事件地区管理数据
        EventAreaManagerData eventAreaManagerData = new EventAreaManagerData();
        //购买物品状态
        for (int i = 0; i < eventAreaManagerData.purchaseObjectStatueDatas.Count; i++)
        {

            foreach (var purchaseObject in EventAreaManager.Instance.purchaseObjectsStatue[i])
            {
                EventAreaManagerData.PurchaseObjectStatueData purchaseObjectStatueData = new EventAreaManagerData.PurchaseObjectStatueData();
                purchaseObjectStatueData.purchaseObjectId = purchaseObject.Key;
                purchaseObjectStatueData.purchaseObjectStatue = purchaseObject.Value;
                
                eventAreaManagerData.purchaseObjectStatueDatas[i].purchaseObjectStatueDatas.Add(purchaseObjectStatueData);
            }
        }
        //出售物品状态
        for (int i = 0; i < eventAreaManagerData.sellObjectsStatueDatas.Count; i++)
        {
            foreach (var sellObject in EventAreaManager.Instance.sellObjectsStatue[i])
            {
                EventAreaManagerData.SellObjectStatueData sellObjectStatueData = new EventAreaManagerData.SellObjectStatueData();
                sellObjectStatueData.sellObjectId = sellObject.Key;
                sellObjectStatueData.sellObjectStatue = sellObject.Value;
                eventAreaManagerData.sellObjectsStatueDatas[i].sellObjectStatueDatas.Add(sellObjectStatueData);
            }
        }
        //敌对度
        eventAreaManagerData.hotility = EventAreaManager.Instance.hotility.ToList();

        arc.eventAreaManagerData = eventAreaManagerData;
        #endregion

        #region 能力管理器数据
        CapabilityManagerData capabilityManagerData = new CapabilityManagerData();

        capabilityManagerData.upgradePoint = CapabilityManager.Instance.upgradePoint;
        capabilityManagerData.upgradePointHaveBuy = CapabilityManager.Instance.upgradePointHaveBuy;
        capabilityManagerData.curLevels = CapabilityManager.Instance.curLevels;
        capabilityManagerData.freelyReduceCoolingRound = CapabilityManager.Instance.freelyReduceCoolingRound;
        capabilityManagerData.executionAmount = CapabilityManager.Instance.executionAmount;

        arc.capabilityManagerData = capabilityManagerData;
        #endregion

        #region 事件管理器数据
        EventManagerData eventManagerData = new EventManagerData();
        if (EventManager.Instance.curConfrontEvent != null)
        {
            eventManagerData.curConfrontEventIndex[0] = EventManager.Instance.curConfrontEvent.SettleType;
            eventManagerData.curConfrontEventIndex[1] = EventManager.Instance.curConfrontEvent.Level - 1;//索引
        }

        if (EventManager.Instance.curClashArea != null)
        {
            eventManagerData.curClashAreaPos = EventManager.Instance.curClashArea.plot.pos;
        }

        arc.eventManagerData = eventManagerData;
        #endregion

        #region 消息管理器数据
        MessageManagerData messageManagerData =new MessageManagerData();

        foreach (var message in MessageManager.Instance.messages[0])
        {
            messageManagerData.yesterdayMessages.Add(new MessageManagerData.MessageData
            {
                type = message.type,
                message = message.message
            });
        }

        foreach (var message in MessageManager.Instance.messages[1])
        {
            messageManagerData.todayMessages.Add(new MessageManagerData.MessageData
            {
                type = message.type,
                message = message.message
            });
        }

        arc.messageManagerData = messageManagerData;

        #endregion

        #region Npc管理器数据
        NpcManagerData npcManagerData = new NpcManagerData();
        foreach (var npc in NpcManager.Instance.npcs)
        {
            npcManagerData.npcsData.Add(new NpcManagerData.NpcData
            {
                npcName = npc.Key,
                npcDefineId = npc.Value.npcDefine.Id,
                pos = npc.Value.pos
            });
        }

        foreach (var npcCondition in NpcManager.Instance.npcAppearConditions)
        {
            NpcManagerData.NpcConditionsData npcConditionsData = new NpcManagerData.NpcConditionsData();
            npcConditionsData.isAppear = true;
            npcConditionsData.sort = npcCondition.Key;
            foreach (var condition in npcCondition.Value)
            {
                NpcManagerData.NpcConditionData npcConditionData = new NpcManagerData.NpcConditionData();
                npcConditionData.npcDefineId = condition.Key;
                npcConditionData.conditions = condition.Value.ToList();
                npcConditionsData.npcConditionData.Add(npcConditionData);
            }

            npcManagerData.npcConditionsData.Add(npcConditionsData);
        }
        foreach (var npcCondition in NpcManager.Instance.npcLeaveConditions)
        {
            NpcManagerData.NpcConditionsData npcConditionsData = new NpcManagerData.NpcConditionsData();
            npcConditionsData.isAppear = false;
            npcConditionsData.sort = npcCondition.Key;
            foreach (var condition in npcCondition.Value)
            {
                NpcManagerData.NpcConditionData npcConditionData = new NpcManagerData.NpcConditionData();
                npcConditionData.npcDefineId = condition.Key;
                npcConditionData.conditions = condition.Value.ToList();
                npcConditionsData.npcConditionData.Add(npcConditionData);
            }

            npcManagerData.npcConditionsData.Add(npcConditionsData);
        }
        arc.npcManagerData = npcManagerData;
        #endregion

        #region 聊天管理器数据
        ChatManagerData chatManagerData = new ChatManagerData();
        foreach (var chatCondition in ChatManager.Instance.chatConditions)
        {
            ChatManagerData.ChatConditionsData chatConditionsData = new ChatManagerData.ChatConditionsData();
            chatConditionsData.sort = chatCondition.Key;
            foreach (var condition in chatCondition.Value)
            {
                chatConditionsData.chatConditionData.Add(condition);
            }

            chatManagerData.chatConditionsData.Add(chatConditionsData);
        }

        foreach (var chatConditionNpc in ChatManager.Instance.chaConditionsNpc)
        {
            ChatManagerData.ChatConditionsNpcData chatConditionsData = new ChatManagerData.ChatConditionsNpcData();
            chatConditionsData.sort = chatConditionNpc.Key;
            foreach (var condition in chatConditionNpc.Value)
            {
                ChatManagerData.ChatConditionNpcData chatConditionData = new ChatManagerData.ChatConditionNpcData();
                chatConditionData.chatDefineId = condition.Key;
                chatConditionData.condition = condition.Value;
                chatConditionsData.chatConditionData.Add(chatConditionData);
            }

            chatManagerData.chatConditionNpcsData.Add(chatConditionsData);
        }
        arc.chatManagerData = chatManagerData;
        #endregion

        #region 任务管理器数据
        QuestManagerData questManagerData = new QuestManagerData();
        if(QuestManager.Instance.curMainQuestId!=-1)
        {
            questManagerData.questIds.Add(QuestManager.Instance.curMainQuestId);
            questManagerData.questsRound.Add(QuestManager.Instance.curMainQuestRound);
        }
        questManagerData.questConditions = QuestManager.Instance.questConditions.ToList();

        //foreach (var questId in QuestManager.Instance.curSecondQuestIds)
        //{
        //    questManagerData.questIds.Add(questId);
        //    questManagerData.questsRound.Add(QuestManager.Instance.curSecondQuestsRound[questId]);
        //}

        arc.questManagerData = questManagerData;
        #endregion

        #region 回合管理器数据
        RoundManagerData roundManagerData = new RoundManagerData();

        roundManagerData.roundNumber = RoundManager.Instance.roundNumber;

        arc.roundManagerData = roundManagerData;
        #endregion

        #region 资源管理器数据
        ResourcesManagerData resourcesManagerData = new ResourcesManagerData();

        resourcesManagerData.wealth = ResourcesManager.Instance.wealth;

        resourcesManagerData.buildingResources = ResourcesManager.Instance.buildingResources;

        resourcesManagerData.execution = ResourcesManager.Instance.execution;

        arc.resourcesManagerData = resourcesManagerData;
        #endregion

        #region 相机位置
        arc.CameraPos = Camera.main.transform.position;
        #endregion

        string json = JsonUtility.ToJson(arc);
        ArchiveTool.SaveByJson("Archive.json", json);

        archive = arc;

    }

    public static void LoadData()
    {
        string a = ArchiveTool.LoadByJson("Archive.json");
        Archive arc = JsonUtility.FromJson<Archive>(a);

        archive = arc;

    }
}
