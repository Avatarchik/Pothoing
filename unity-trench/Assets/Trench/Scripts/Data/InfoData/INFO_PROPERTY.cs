using System.Collections.Generic;
using UnityEngine;

public class INFO_PROPERTY
{
    public int Id;              //道具ID
    public string Name;         //道具名称
    public int Type;            //类型0-大厅，1-游戏，2-通用
    public float Price;         //价格
    public int PriceType;       //价格类型0-银币，金币 1-钻石 2-cash
    public string Describe;     //描述
    public int GivenValue;      //赠送的值（赠送的值，只有道具填写ID）
    public int GivenType;       //赠送类型（0-金币 1-钻石 2-道具，没有-1）
    public int GivenCount;      //赠送的个数
    public uint Period;         //有效期(有效期，单位小时永久-1，立即生效0)
    public byte Nullity;        //是否有效(0-有效 1-无效)
}
