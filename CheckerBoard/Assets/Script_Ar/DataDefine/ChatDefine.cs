using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatDefine
{
    public int Id { get; set; }
    public HeadPortrait_Type HeadPortrait { get; set; }
    public string Name { get; set; }
    public int IsOption { get; set; }
    public int SubAnswerId { get; set; }
    public string ChatContent { get; set; }
    public string Option1 { get; set; }
    public int AnswerId1 { get; set; }
    public string Option2 { get; set; }
    public int AnswerId2 { get; set; }
    public string Option3 { get; set; }
    public int AnswerId3 { get; set; }
    public int SubEventType { get; set; }
    public int SubEventValue { get; set; }
}
