using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoviceGuideDefine
{
    public int Id {get; set;}
    public string GuideDescription { get; set; }
    public PlayCondition_Type PlayCondition { get; set; }
    public bool IsArrow { get; set; }
    public float NoviceGuidePosX { get; set; }
    public float NoviceGuidePosY { get; set; }
    public float NoviceGuideWindowPosX { get; set; }
    public float NoviceGuideWindowPosY { get; set; }
    public float NoviceGuideArrowPosX { get; set; }
    public float NoviceGuideArrowPosY { get; set; }
    public float NoviceGuideArrowRotZ { get; set; }
}
