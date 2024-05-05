using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDefine
{
    public int Id { get; set; }
    public string QuestName { get; set; }
    public bool IsMain { get; set; }
    public string QuestDescription { get; set; }
    public int QuestConditionId { get; set; }
    public int RoundCondition { get; set; }
    public int CurrencyCondition { get; set; }
    public int Resource1Condition { get; set; }
    public int Resource2Condition { get; set; }
    public int Resource3Condition { get; set; }
    public Npc_Name AimNpc { get; set; }
    public int DestinationConditionX { get; set; }
    public int DestinationConditionY { get; set; }
    public int GainUpgradePoint { get; set; }
    public int GainCurrency { get; set; }
    public int GainResource1 { get; set; }
    public int GainResource2 { get; set; }
    public int GainResource3 { get; set; }
    public int HumanHostility { get; set; }
    public int RobotHostility { get; set; }
    public int QuestFallResult { get; set; }
}
