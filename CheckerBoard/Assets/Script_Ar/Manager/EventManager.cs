using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    //当前事件id
    public int CurGameEventId;

    //事件字典
    Dictionary<int, GameEvent> gameEventDic = new Dictionary<int, GameEvent>();

    void AddGameEvent(int eventId)
    {

    }
}

