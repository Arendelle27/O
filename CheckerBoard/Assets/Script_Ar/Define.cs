using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Plot_Type
{
    无=-1,
    未探明,
    可探索,
    已探索,
    已开发,
}

public enum Building_Type
{
    无=-1,
    自动采集建筑_1,
    自动采集建筑_2,
    自动采集建筑_3,
    生产建筑_1,
    生产建筑_2,
    战斗建筑_1
}

public enum Map_Mode
{
    正常=0,
    选择目的地位置,
    拓展探索小队,
}

public enum Event_Type
{
    正常=0,
    交易,
    战斗,
    特殊
}

