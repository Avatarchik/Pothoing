using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngineEx.CMD.i3778;

public class UserData : MonoBehaviour
{
	public string uiName;
	public bool isFemale;
	public Sprite head;
	public string phoneNum;
	public int coin;
	public int diamond;
	public int score;
	public Sprite[] heads;
	public string title = "";
	//public Dictionary<int,string> titleDict = new Dictionary<int, string> ();
	public List<TitleInfoData> titles;
	Player player;

	public void Init ()
	{
		player = DataBase.Instance.PLAYER;
		titles = player.GetAllTitleData ();
		uiName = player.Name;
		if (player.Gender == GlobalDef.GENDER_GIRL) {
			isFemale = true;
		} else {
			isFemale = false;
		}
		head = heads [player.wFaceId];
		phoneNum = player.PhoneNumber;
		coin = (int)player.Gold;
		diamond = (int)player.ZScore;
		score = player.GameScore;
		RefreshTitle ();
	}

	public void RefreshTitle ()
	{
		foreach (TitleInfoData titledData in titles) {
			if (coin >= titledData.coin) {
				title = titledData.title;
				//Debug.Log (title);
			} else {
				break;
			}
		}
	}

	public void RefreshMoney ()
	{
		player = DataBase.Instance.PLAYER;
		coin = (int)player.Gold;
		diamond = (int)player.ZScore;
		score = player.GameScore;
		RefreshTitle ();
	}

	public void SaveMoney ()
	{
		player = DataBase.Instance.PLAYER;
		player.Gold = (uint)coin;
		player.ZScore = (uint)diamond;
		player.GameScore = score;
		player.SaveUserInfo ();
	}

	public void SetUserInfo (string newName, bool isFemaleNow)
	{
		player = DataBase.Instance.PLAYER;
		player.Name = newName;
		uiName = newName;
		isFemale = isFemaleNow;
		if (isFemaleNow) {
			player.Gender = GlobalDef.GENDER_GIRL;
		} else {
			player.Gender = GlobalDef.GENDER_BOY;
		}
		player.SaveUserInfo ();
	}
}
