using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData : MonoBehaviour
{
	public bool isBombable;
	public int betLevel;
	public bool isAllowedBlackTrench;
//	public int item;
//	public int mission;
	public List<INFO_ROOM> normalRoomInfo = new List<INFO_ROOM> ();
	public List<INFO_ROOM> bombRoomInfo = new List<INFO_ROOM> ();
//	public Dictionary<int,int> normalBetLevels = new Dictionary<int, int> ();
//	public Dictionary<int,int> bombBetLevels = new Dictionary<int, int> ();

	public void Init ()
	{
		normalRoomInfo = DataBase.Instance.ROOM_MGR.GetRoomsByType (enRoomType.enNormal);
		bombRoomInfo = DataBase.Instance.ROOM_MGR.GetRoomsByType (enRoomType.enBomb);
//		foreach (INFO_ROOM room in normalRoomInfo) {
//			normalBetLevels.Add (room.GetRoomCfg ().Id, room.GetRoomCfg ().Rate);
//		}
//		foreach (INFO_ROOM room in bombRoomInfo) {
//			bombBetLevels.Add (room.GetRoomCfg ().Id, room.GetRoomCfg ().Rate);
//		}
//		Debug.Log (DataBase.Instance.ROOM_MGR.GetRoomsByType (enRoomType.enNormal));
	}

	public void SelectRoom (INFO_ROOM data)
	{
		DataBase.Instance.EnterNomalRoom (data.wServerID);
		isBombable = DataBase.Instance.CurRoom.GetRoomCfg ().Type == 2 ? true : false;
		betLevel = DataBase.Instance.CurRoom.GetRoomCfg ().Rate;
		isAllowedBlackTrench=DataBase.Instance.CurRoom.GetRoomCfg ().bTrench;
	}
	
}
