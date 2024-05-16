using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotDefine
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool CanBuild { get; set; }
    public int Type { get; set; }
    public Event_Area_Type EventType { get; set; }
    public int EventValue { get; set; }
    public Plot_Condition_Type Condition { get; set; }
    public int UnlockValue { get; set; }
    public int ResourceType { get; set; }
    public int ResourceFristtime { get; set; }
    public int ResourceByRound { get; set; }
    public int ResourceTotal { get; set; }
    public bool IsSpecialGeneration { get; set; }
    public int Weights { get; set; }
    public int HorizontalMin { get; set; }
    public int HorizontalMax { get; set; }
    public int VerticalMin { get; set; }
    public int VerticalMax { get; set; }
}
