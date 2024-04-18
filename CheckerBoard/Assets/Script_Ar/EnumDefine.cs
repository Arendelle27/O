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
    电缆收集器,
    电缆卷机,
    金属收集器,
    金属提纯器,
    芯片探测仪,

    生产建筑,
    回收设备,
    快速回收设备,
    超高速回收设备,

    战斗建筑,
    厉害的战斗机器,
    特别厉害的战斗机器
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
    蓝图,
    回合数,
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
    影像芯片,

    建筑资源
}

public enum Transaction_Type
{
    无=-1,
    资源,
    蓝图
}

public enum Prop_Type
{
    蓝图,
    地下室钥匙,

    道具
}

public enum Message_Type
{
    指引 = 0,
    流浪者 = 1,
    机械 = 2,
    交易 = 3,
    冲突 = 4,
    探索=5
}

public enum Upgrade_Type
{
    无=-1,
    小队,
    交易,
    行动力
}

public enum PlayCondition_Type
{
    任意位置,
    移动,
    移动地点,
    建造
}