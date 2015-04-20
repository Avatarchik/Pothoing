using System;
using System.Collections.Generic;

/// <summary>
/// 关卡信息
/// </summary>
public class INFO_STAGE
{
    public uint Id;          //关卡编号
    public string Name;      //关卡名称
    public uint LotteryCost; //关卡抽奖价格
    public int Pricetype;    //抽奖货币类型 0 银币 1 金币 2钻石
    public uint StageCost;   //闯关费用
    public uint StageCostPricetype; //闯关费用货币类型 0 银币 1 金币 2钻石
    public uint ComditionId; //条件ID
    public uint RoomRate;    //房间倍率
    public uint Star1;       //一星条件（0-无条件，通关即可）
    public uint Star2;       //二星条件
    public uint Star3;       //三星条件
    public uint OpenCondition; //前置开启条件
    public uint NeedStar;    //开启本关卡需要的星星数
    public uint RewardId;    //关卡奖励
    public float X;          //关卡节点X坐标
    public float Y;          //关卡节点Y坐标
    public string Describe;  //关卡说明
}

/// <summary>
/// 关卡条件信息
/// </summary>
public class INFO_STAGE_COND
{
    public uint Id;          //条件编号
    public uint GroupId;     //组编号
    public string Describe;  //条件描述
}

/// <summary>
/// 关卡和条件关联
/// </summary>
public class INFO_STAGE_COND_LINK
{
    public uint ID;                //主键ID
    public uint ConditionId;        //关联条件编号
    public uint StageConditionId;   //条件编号
    public uint Condition;          //条件值
    public uint FixedHand;          //指定手
    public string Describe;         //具体条件说明
}

