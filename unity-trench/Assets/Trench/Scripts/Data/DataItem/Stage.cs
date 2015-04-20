using System;
using System.Collections.Generic;
using UnityEngineEx.DataFormat;
using UnityEngine;

public enum enStageCondType
{
    enMustHas1=1,
    enMutiHas2 = 2,
    enMustHas3 = 3,    
}

public enum StageStatus
{
    NotUnlock,//未开锁
    Unlocked, //开锁
    CanPlay,  //开启
    Compelet  //通关

}

/// <summary>
/// 关卡信息
/// </summary>
public class xmlStage : BaseXmlReader<Dictionary<uint, INFO_STAGE>,INFO_STAGE>
{
    protected override void OnRead(INFO_STAGE v)
    {
        m_Info.Add(v.Id, v);
    }
}
public class Stage : BaseXmlNode<Dictionary<uint, INFO_STAGE>, xmlStage>
{
//     public void XmlLoad(string data,string node)
//     {
//         xmlStage xml = new xmlStage();
//         xml.Load(mInfo, data, node);
//     }
// 
//     public void XmlLoad(TextAsset data, string node)
//     {
//         xmlStage xml = new xmlStage();
//         xml.Load(mInfo, data.bytes, node);
//     }

    //---------------------------------------------------------------------------------------------        
}

/// <summary>
/// 关卡条件
/// </summary>

public class xmlStageCond : BaseXmlReader<Dictionary<uint, INFO_STAGE_COND>, INFO_STAGE_COND>
{
    protected override void OnRead(INFO_STAGE_COND v)
    {
        m_Info.Add(v.Id, v);
    }
}

public class StageCond : BaseXmlNode<Dictionary<uint, INFO_STAGE_COND>, xmlStageCond>
{
    //关卡条件
    public const uint COND_NULL = 0; //无限制
    public const uint COND_XINNING = 100001; //X局内
    public const uint COND_XSECOND = 100002; //X秒内
    public const uint COND_FIXEDHAND = 200001;//指定手
    public const uint COND_OUTSINGLE = 200002;//打出某张单牌
    public const uint COND_OUTDOUBLE = 200003;//打出某副对子
    public const uint COND_OUTTHREE = 200004;//打出三条
    public const uint COND_SINGLELINK = 200005;//打出单顺
    public const uint COND_DOUBLELINK = 200006;//打出双顺
    public const uint COND_THREELINK = 200007;//打出三顺
    public const uint COND_FOURLINK = 200008;//打出四顺
    public const uint COND_OUTBOMB = 200009;//打出炸弹
    public const uint COND_XBOUT = 200010;//X回合内
    public const uint COND_NOOUTSECOND = 200011;//出牌时间不超过X秒
    public const uint COND_THETYPE = 200012;//指定身份
    public const uint COND_XSUCCESS = 300001;//胜X局
    public const uint COND_XLINKSUCCESS = 300002;//连胜X局

    //条件分组
    public const int COND_GROUPONE = 1;
    public const int COND_GROUPTWO = 2;
    public const int COND_GROUPTHREE = 3;
}

/// <summary>
/// 关卡条件关联信息
/// </summary>
public class xmlStageCondInfo : BaseXmlReader<Dictionary<uint,List<INFO_STAGE_COND_LINK>>,INFO_STAGE_COND_LINK>
{
    protected override void OnRead(INFO_STAGE_COND_LINK v)
    {
        if (m_Info.ContainsKey(v.ConditionId) == false)m_Info.Add(v.ConditionId,new List<INFO_STAGE_COND_LINK>()); 
        m_Info[v.ConditionId].Add(v);
    }
}

public class StageCondInfo : BaseXmlNode<Dictionary<uint,List<INFO_STAGE_COND_LINK>>,xmlStageCondInfo>
{

}
