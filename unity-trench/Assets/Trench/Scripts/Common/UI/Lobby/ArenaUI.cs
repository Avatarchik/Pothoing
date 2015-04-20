using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ArenaUI : MonoBehaviour
{
	[SerializeField]
	Image
		title;
	[SerializeField]
	Sprite
		normal;
	[SerializeField]
	Sprite
		bombable;
	//[Header("freshman,advanced,master,expert")]
	[SerializeField]
	ArenaBtn
		betLevelBtn;
//	[SerializeField]
//	List<ArenaBtn>
//		betLevelBtns;
//	[SerializeField]
//	Text[]
//		betLevels;
	[SerializeField]
	GameData
		data;
	[SerializeField]
	UserData
		user;
	[SerializeField]
	WindowData
		navi;
	[SerializeField]
	RectTransform
		scrollViewContent;
	bool isBombEnabled = false;
	//int betLevelBtnIndex = -1;
	List<ArenaBtn> btns = new List<ArenaBtn> ();
	LobbyDialogManager dialogs;
	[SerializeField]
	UnityEvent
		onPlayGame;

	void Start ()
	{
		dialogs = LobbyDialogManager.inst;
	}

	public void SetBombable (bool isBombable)
	{
		//Debug.Log (isBombEnabled.ToString () + " " + btns.Count);
		if (isBombEnabled != isBombable || btns.Count == 0) {
			foreach (ArenaBtn btn in btns) {
				Destroy (btn.gameObject);
			}
			btns.Clear ();
			isBombEnabled = isBombable;
			List<INFO_ROOM> info = new List<INFO_ROOM> ();
			if (isBombEnabled) {
				title.sprite = bombable;
				info = data.bombRoomInfo;
			} else {
				title.sprite = normal;
				info = data.normalRoomInfo;
			}
			//Debug.Log(data.normalRoomInfo);
			foreach (INFO_ROOM room in info) {
				ArenaBtn btn = Instantiate (betLevelBtn) as ArenaBtn;
				btn.transform.SetParent (scrollViewContent.transform);
				btn.gameObject.SetActive (true);
				btn.Init (room, SelectRoom);
				btns.Add (btn);
				//Debug.Log(btn.GetRoomData ());
//				if (user.coin <= btn.GetRoomData ().dwMinBonus || user.coin >= btn.GetRoomData ().dwMaxBonus) {
//					btn.ToggleButton (false);
//				}
			}
		}
	}

	INFO_ROOM room;

	void SelectRoom (INFO_ROOM roomData, ArenaBtn btn)
	{
		if (user.coin < roomData.dwMinBonus) {
			//judge is low coin gift gone
			int getLowCoinGiftRest = PlayerPrefs.GetInt ("LowCoinGift", 2);
			//Debug.Log(roomData.bRelief);
			if (roomData.bRelief == true && getLowCoinGiftRest > 0) {
				dialogs.ShowConfirmDialog (string.Format (TextManager.Get ("lowCoinGiftTip"), (2-getLowCoinGiftRest).ToString()), OnLowCoinGiftGet, null, false, null, null, TextManager.Get ("lowCoinGiftTitle"));
			} else {
				dialogs.ShowLowCoinDialog (GoodType.Coin,string.Format (TextManager.Get ("lowCoinRoomTip"), roomData.dwMinBonus));
			}
		} else if (user.coin >= roomData.dwMaxBonus) {
			dialogs.ShowConfirmDialog (TextManager.Get ("highCoinRoomTip"), null,null,false);
		} else {
			room = roomData;
			navi.LobbyWin=LobbyState.MainMenu;
			onPlayGame.Invoke ();
			SetBetLevel (btn);
		}
		SOUND.Instance.OneShotSound (Sfx.inst.btn);
	}

	void SetBetLevel (ArenaBtn selectedBtn)
	{
		foreach (ArenaBtn btn in btns) {
			btn.ToggleButton (true);
		}
		selectedBtn.ToggleButton (false);
	}

	public void LoadPlayScene ()
	{
		//DataBase.Instance.SCENE_MGR.SetScene ("Game");
		data.SelectRoom (room);
	}

	void OnLowCoinGiftGet ()
	{
		user.coin += 2000;
		user.SaveMoney ();
		UserHud.inst.RefreshMoney ();
		int getLowCoinGiftRest = PlayerPrefs.GetInt ("LowCoinGift", 2);
		getLowCoinGiftRest--;
		PlayerPrefs.SetInt ("LowCoinGift", getLowCoinGiftRest);
	}

	public void QuickStart ()
	{
		ArenaBtn targetRoom = null;
		foreach (ArenaBtn btn in btns) {
			if (user.coin > btn.GetRoomData ().dwMinBonus || btn.GetRoomData ().wServerID.ToString ().Substring (1) == "002") {
				targetRoom = btn;
			}
		}
		if (targetRoom != null) {
			SelectRoom (targetRoom.GetRoomData (), targetRoom);
		} else {
			Debug.Log("No target room!");
		}
	}

}
