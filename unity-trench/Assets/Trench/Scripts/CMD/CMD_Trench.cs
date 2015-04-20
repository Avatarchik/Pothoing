using System;
using System.Collections.Generic;
using UnityEngineEx.CMD.i3778;
using System.Runtime.InteropServices;

public class CMD_Trench
{
    //公共宏定义
    public const int TAX_MIN_NUM = 10;													    //要抽税的最少点

    public const int KIND_ID = 291;									                        //游戏 I D
    public const int GAME_PLAYER = 3;									                    //游戏人数
    public const string GAME_NAME = "挖坑";						                            //游戏名字
    public const int GAME_GENRE = GlobalDef.GAME_GENRE_SCORE | GlobalDef.GAME_GENRE_GOLD;   //游戏类型

    public const int LEFT_CARD_NUM = 4;                                                     //留牌
    public const int ONE_USER_GET_CARD_NUM = 16;                                            //每人得牌
    public const int ZHUANG_CARD_NUM = ONE_USER_GET_CARD_NUM + LEFT_CARD_NUM;               //庄家牌数
    public const int JOKER_NUM = 2;                                                         //大小王
    public const int ALL_CARD_NUM = ONE_USER_GET_CARD_NUM * GAME_PLAYER + LEFT_CARD_NUM + JOKER_NUM;//留牌	

    public const int RED_4 = 0x24;								                            //红心4

    public const ushort CHAIR_COUNT=3;                                                     //椅子数

    //游戏状态
    public const int GS_WK_FREE = GlobalFrame.GS_FREE;								        //等待开始
    public const int GS_WK_TRENCH = GlobalFrame.GS_PLAYING;							        //黑挖状态
    public const int GS_WK_SCORE = GlobalFrame.GS_PLAYING + 1;						        //叫分状态
    public const int GS_WK_PLAYING = GlobalFrame.GS_PLAYING + 2;						    //游戏进行
    public const int GS_WK_GETCARD = GlobalFrame.GS_FREE + 1;							    //发牌状态，郭鹏添加，仅发牌中使用
}

public static class enCmdTrench
{
    //客户端命令
    public const ushort SUB_C_LAND_SCORE = 1;								        //用户叫分
    public const ushort SUB_C_OUT_CART = 2;							                //用户出牌
    public const ushort SUB_C_PASS_CARD = 3;									    //放弃出牌
    public const ushort SUB_C_TRUSTEE = 4;								            //托管消息
    public const ushort SUB_C_BLANK_TRENCH = 5;									    //用户黑挖

    //服务端命令
    public const ushort SUB_S_SEND_CARD = 100;									    //发牌命令
    public const ushort SUB_S_LAND_SCORE = 101;									    //叫分命令
    public const ushort SUB_S_GAME_START = 102;								        //游戏开始
    public const ushort SUB_S_OUT_CARD = 103;								        //用户出牌
    public const ushort SUB_S_PASS_CARD = 104;									    //放弃出牌
    public const ushort SUB_S_GAME_END = 105;							            //游戏结束
    public const ushort SUB_S_BLANK_TRENCH = 106;							        //黑挖命令
    public const ushort SUB_S_MATCH_STATUS = 107;								    //比赛状态
}
//-------------------------------------------------------------------------------------------------------------------------------
// 服务端命令
//-------------------------------------------------------------------------------------------------------------------------------
//////////////////////////////////////////////////////////////////////////
//服务器命令结构

//游戏状态
[Serializable]
[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
public struct CMD_S_StatusFree
{
    public int lBaseScore;							//基础积分
    public ushort wGameGenre;							//服务器类型
    public uint dwMatchMode;						//比赛模式
};

//游戏状态
[Serializable]
[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
public struct CMD_S_StatusScore
{
    public byte bLandScore;							//地主分数
    public int lBaseScore;							//基础积分
    public ushort wCurrentUser;						//当前玩家
    public ushort wBlankTrenchUser;					//黑挖玩家
    public ushort wGameGenre;							//服务器类型
    public uint dwMatchMode;						//比赛模式
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.GAME_PLAYER)]
    public byte[] bScoreInfo;			                //叫分信息
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.LEFT_CARD_NUM)]
    public byte[] bBackCard;			                //底牌扑克
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.GAME_PLAYER)]
    public bool[] bUserTrustee;			            //玩家托管
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.GAME_PLAYER*CMD_Trench.ZHUANG_CARD_NUM)]
    public byte[,] bCardData;                          //手上扑克
    public byte cbRemaindTimer;						//剩余时间
};


//游戏状态
[Serializable]
[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
public struct CMD_S_StatusPlay
{
    public ushort wLandUser;							//坑主玩家
    public ushort wBombTime;							//炸弹倍数
    public ushort wGameGenre;							//服务器类型
    public uint dwMatchMode;						//比赛模式
    public int lBaseScore;							//基础积分
    public byte bLandScore;							//地主分数
    public ushort wLastOutUser;						//出牌的人
    public ushort wCurrentUser;						//当前玩家
    public ushort wBlankTrenchUser;					//黑挖玩家
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.LEFT_CARD_NUM)]
    public byte[] bBackCard;			                //底牌扑克
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.GAME_PLAYER*CMD_Trench.ZHUANG_CARD_NUM)]
    public byte[,] bCardData;                          //手上扑克
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.GAME_PLAYER)]
    public byte[] bCardCount;			                //扑克数目
    public byte bTurnCardCount;						//基础出牌
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.ZHUANG_CARD_NUM)]
    public byte[] bTurnCardData;		                //出牌列表
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.GAME_PLAYER)]
    public bool[] bUserTrustee;			            //玩家托管
    public byte cbRemaindTimer;						//剩余时间
};


//黑挖状态
[Serializable]
[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
public struct CMD_S_StatusBlankTrench
{
    public ushort wCurrentUser;						//当前玩家
    public ushort wGameGenre;							//服务器类型
    public uint dwMatchMode;						//比赛模式
};

//发送扑克
[Serializable]
[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
public struct CMD_S_SendCard
{
    public ushort wCurrentUser;						//当前玩家
    public ushort wBlankTrenchUser;					//黑挖玩家
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.ZHUANG_CARD_NUM)]
    public byte[] bCardData;			                //扑克列表
};

//发送扑克
[Serializable]
[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
public struct CMD_S_SendAllCard
{
    public ushort wCurrentUser;						//当前玩家
    public ushort wBlankTrenchUser;					//黑挖玩家
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.GAME_PLAYER*CMD_Trench.ZHUANG_CARD_NUM)]
    public byte[,] bCardData;                          //扑克列表
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.LEFT_CARD_NUM)]
    public byte[] bBackCardData;		                //底牌扑克
};

//用户叫分
[Serializable]
[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
public struct CMD_S_LandScore
{
	public ushort bLandUser;						//叫分玩家
    public ushort wCurrentUser;						//当前玩家
    public byte bLandScore;							//上次叫分
    public byte bCurrentScore;						//当前叫分
};

//游戏开始
[Serializable]
[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
public struct CMD_S_GameStart
{
    public ushort wLandUser;							//地主玩家
    public byte bLandScore;							//地主分数
    public ushort wCurrentUser;						//当前玩家
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.LEFT_CARD_NUM)]
    public byte[] bBackCard;			                //底牌扑克
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.ZHUANG_CARD_NUM)]
    public byte[] bMeCardData;		                //自己扑克
    public int lMeCardCount;						//自己的扑克数量
};

//用户出牌
[Serializable]
[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
public struct CMD_S_OutCard
{
    public byte bCardCount;							//出牌数目
    public ushort wCurrentUser;						//当前玩家
    public ushort wOutCardUser;						//出牌玩家
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.ZHUANG_CARD_NUM)]
    public byte[] bCardData;			                //扑克列表
};

//放弃出牌
[Serializable]
[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
public struct CMD_S_PassCard
{
    public byte bNewTurn;							//一轮开始
    public ushort wPassUser;							//放弃玩家
    public ushort wCurrentUser;						//当前玩家
};

//游戏结束
[Serializable]
[StructLayout(LayoutKind.Sequential,CharSet=CharSet.Ansi)]
public struct CMD_S_GameEnd
{
    public int lGameTax;							//游戏税收
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.GAME_PLAYER)]
    public int[] lGameScore;			            //游戏积分
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.GAME_PLAYER)]
    public int[] lGameHighLadderScore;	            //游戏积分
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.GAME_PLAYER)]
    public int[] lGold;					            //输赢金币
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.GAME_PLAYER)]
    public bool[] bGameResult;			            //游戏结果
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.GAME_PLAYER)]
    public byte[] bCardCount;			            //扑克数目
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.ALL_CARD_NUM)]
    public byte[] bCardData;			            //扑克列表 

	//道具应用
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.GAME_PLAYER)]
    public int[] bNullity;				            //保分卡
};


//黑挖命令
[Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct CMD_S_BlankTrench
{
    public ushort wCurrentUser;						//当前玩家
    public int lRevenue;							//税收金币
};

//比赛状态
[Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct CMD_S_MatchStatus
{
    public int nMatchStatus;													//比赛状态
};


//-------------------------------------------------------------------------------------------------------------------------------
// 客户端命令
//-------------------------------------------------------------------------------------------------------------------------------
//托管结构
[Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct CMD_C_UserTrustee 
{
    public ushort wUserChairID;						//玩家椅子
    public bool bTrustee;							//托管标识
};

//用户叫分
[Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct CMD_C_LandScore
{
    public byte bLandScore;							//地主分数
};

//出牌数据包
[Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct CMD_C_OutCard
{
    public byte bCardCount;							//出牌数目
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=CMD_Trench.ZHUANG_CARD_NUM)]
    public byte[] bCardData;			                //扑克列表
};

//是否黑挖
[Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct CMD_C_BlankTrench
{
    public bool bBlankTrench;						//是否黑挖
};
//////////////////////////////////////////////////////////////////////////