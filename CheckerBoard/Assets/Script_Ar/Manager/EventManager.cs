using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    //��ǰ�¼�id
    public int CurGameEventId;

    //�¼��ֵ�
    Dictionary<int, GameEvent> gameEventDic = new Dictionary<int, GameEvent>();

    void AddGameEvent(int eventId)
    {

    }
}

