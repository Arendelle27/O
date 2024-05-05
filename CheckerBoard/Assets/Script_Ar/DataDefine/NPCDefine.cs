using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDefine
{
    public int Id { get; set; }
    public Npc_Name Name { get; set; }
    public int AppearCondition { get; set; }
    public int AppearConditionValue { get; set; }
    public int AppearRound { get; set; }
    public int LeaveCondition { get; set; }
    public int LeaveConditionValue { get; set; }
    public int LeaveRound { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
}
