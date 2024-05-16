using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDefine
{
    public int Id { get; set; }
    public string QuestName { get; set; }
    public int IsMain { get; set; }
    public int IsExist { get; set; }
    public string QuestDescription { get; set; }
    public int PreContent { get; set; }
    public int QuestConditionId { get; set; }
    public int RoundCondition { get; set; }
    public int CurrencyCondition { get; set; }
    public int Resource1Condition { get; set; }
    public int Resource2Condition { get; set; }
    public int Resource3Condition { get; set; }
    public int QuestJudgeType { get; set; }
    public int QuestJudgeValue { get; set; }
    public int GainUpgradePoint { get; set; }
    public int GainCurrency { get; set; }
    public int GainResource1 { get; set; }
    public int GainResource2 { get; set; }
    public int GainResource3 { get; set; }
    public int HumanHostility { get; set; }
    public int RobotHostility { get; set; }
    public int QuestFallResult { get; set; }
}
