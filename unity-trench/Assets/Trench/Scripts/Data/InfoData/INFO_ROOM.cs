
using System.Collections.Generic;

using UnityEngineEx.CMD.i3778;

using WORD = System.UInt16;
using DWORD = System.UInt32;
using LONG = System.Int32;
using BYTE = System.Byte;

public class INFO_ROOM
{
	public WORD wSortID;							//排序号码
	public WORD wKindID;							//名称号码
	public WORD wServerID;							//房间号码
	public WORD wStationID;					        //站点号码
	public WORD wServerType;						//房间类型
	public WORD wServerPort;						//房间端口
	public LONG lCellScore;						    //房间倍率						
	public DWORD dwServerAddr;						//房间地址
	public DWORD dwOnLineCount;					    //在线人数
	public DWORD dwAreaType;						//地区号码
	public DWORD dwMinBonus;						//进入房间的最少金币
	public DWORD dwMaxBonus;						//进入房间的最多金币
	public DWORD dwServerKindID;					//房间的RoomKindID
	public DWORD dwRoomLogoID;						//房间的LogID
	public DWORD dwMaxRoomPersonCnt;				//房间最大人数
	public DWORD dwHLScoreMin;						//天梯最小积分
	public DWORD dwHLScoreMax;						//天梯最大积分
	public string szServerName;			            //房间名称
	public bool bTrench;                            //是否启用黑挖
	public bool bRelief;                            //是否启用低保
	public int TurnTax;                             //局税

	public GlobalDef.tagGameServer GetServer ()
	{
		GlobalDef.tagGameServer room;
		room.wSortID = wSortID;
		room.wKindID = wKindID;
		room.wServerID = wServerID;
		room.wStationID = wStationID;
		room.wServerType = wServerType;
		room.wServerPort = wServerPort;
		room.lCellScore = lCellScore;
		room.dwServerAddr = dwServerAddr;
		room.dwOnLineCount = dwOnLineCount;
		room.dwAreaType = dwAreaType;
		room.dwMinBonus = dwMinBonus;
		room.dwMaxBonus = dwMaxBonus;
		room.dwServerKindID = dwServerKindID;
		room.dwRoomLogoID = dwRoomLogoID;
		room.dwMaxRoomPersonCnt = dwMaxRoomPersonCnt;
		room.dwHLScoreMin = dwHLScoreMin;
		room.dwHLScoreMax = dwHLScoreMax;
		room.szServerName = szServerName;		
		return room;
	}

	public void FromServer (GlobalDef.tagGameServer room)
	{
		this.wSortID = room.wSortID;
		this.wKindID = room.wKindID;
		this.wServerID = room.wServerID;
		this.wStationID = room.wStationID;
		this.wServerType = room.wServerType;
		this.wServerPort = room.wServerPort;
		this.lCellScore = room.lCellScore;
		this.dwServerAddr = room.dwServerAddr;
		this.dwOnLineCount = room.dwOnLineCount;
		this.dwAreaType = room.dwAreaType;
		this.dwMinBonus = room.dwMinBonus;
		this.dwMaxBonus = room.dwMaxBonus;
		this.dwServerKindID = room.dwServerKindID;
		this.dwRoomLogoID = room.dwRoomLogoID;
		this.dwMaxRoomPersonCnt = room.dwMaxRoomPersonCnt;
		this.dwHLScoreMin = room.dwHLScoreMin;
		this.dwHLScoreMax = room.dwHLScoreMax;
		this.szServerName = room.szServerName;		
	}

	public INFO_ROOMCFG GetRoomCfg ()
	{
		INFO_ROOMCFG room = new INFO_ROOMCFG ();

		room.Id = this.wServerID;
		room.Name = this.szServerName;
		room.bRelief = this.bRelief;  
		room.Type = this.wServerType;
		room.Rate = this.lCellScore;
		room.TurnTax = this.TurnTax;
		room.MinScore = this.dwMinBonus;
		room.MaxScore = this.dwMaxBonus;
		room.bTrench = this.bTrench;   

		return room;
	}

	public void FromRoomCfg (INFO_ROOMCFG room)
	{
		this.wServerID = room.Id;
		this.szServerName = room.Name;
		this.wServerType = room.Type;
		this.lCellScore = room.Rate;
		this.TurnTax = room.TurnTax;
		this.dwMinBonus = room.MinScore;
		this.dwMaxBonus = room.MaxScore;
		this.bTrench = room.bTrench;
	}
}

public class INFO_ROOMCFG
{
	public WORD Id;             //房间id
	public string Name;         //房间名称
	public bool bRelief;         //低保
	public WORD Type;           //房间类型1-普通场2-炸弹场
	public int Rate;            //房间倍率
	public int TurnTax;         //局税
	public DWORD MinScore;      //最小进入分
	public DWORD MaxScore;      //最大进入分
	public bool bTrench;        //是否有黑挖

	public INFO_ROOM GetInfoRoom ()
	{
		INFO_ROOM room = new INFO_ROOM ();
		room.wServerID = this.Id;
		room.szServerName = this.Name;
		room.wServerType = this.Type;
		room.lCellScore = this.Rate;
		room.TurnTax = this.TurnTax;
		room.dwMinBonus = this.MinScore;
		room.dwMaxBonus = this.MaxScore;
		room.bTrench = this.bTrench;
		room.bRelief = this.bRelief;

		return room;
	}
}
