using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDefine
{
    public int Id { get; set; }
    public bool Type { get; set; }
    public string QuestName { get; set; }
    public string QuestDescription { get; set; }
    public int RoundCondition { get; set; }
    public int CurrencyCondition { get; set; }
    public NpcCondition_Type NpcCondition { get; set; }
    public int NpcPositionX { get; set; }
    public int NpcPositionY { get; set; }
    public int GainUpgradePoint { get; set; }
    public int GainCurrency { get; set; }
    public int GainResource1 { get; set; }
    public int GainResource2 { get; set; }
    public int GainResource3 { get; set; }
    public int PreQuestId { get; set; }
    public int QuestFallResult { get; set; }
}
