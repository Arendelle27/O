using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Plot_Statue
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
    自动采集建筑,
    自动采集建筑_1,
    自动采集建筑_2,
    自动采集建筑_3,
    自动采集建筑_4,
    自动采集建筑_5,

    生产建筑,
    生产建筑_1,
    生产建筑_2,
    生产建筑_3,

    战斗建筑,
    战斗建筑_1,
    战斗建筑_2
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

public enum Building_Condition_Type
{
    无=-1,
    资源1,
    资源2,
    资源3,
    回合数,
    蓝图,
    厉害的战斗机器
}

public enum Plot_Condition_Type
{
    无=-1,
    回合,
    板块,
    道具
}

public enum Event_Area_Type
{
    无=-1,
    交易,
    对抗,
    遗迹,
    剧情
}

public enum Resource_Type
{
    断裂电缆,
    废弃金属,
    影像芯片
}

public enum Transaction_Type
{
    无=-1,
    资源,
    蓝图
}
