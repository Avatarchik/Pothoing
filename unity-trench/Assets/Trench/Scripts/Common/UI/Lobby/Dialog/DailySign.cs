using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class DailySign : Dialog
{
	[SerializeField]
	Image[]
		dailyRewards;
	[SerializeField]
	Text[]
		dailyRewardNums;
	[SerializeField]
	Sprite
		notGetYetReward;
	[SerializeField]
	Sprite
		gotReward;
	[SerializeField]
	Image[]
		gotMarks;
	[SerializeField]
	ParticleSystem
		gotVfx;
	[SerializeField]
	Button
		signBtn;
	[SerializeField]
	SignData
		data;
	[SerializeField]
	UserData
		user;
//	Action onGetBtn;

	public override void Init ()
	{
		//onGetBtn = onGet;
		for (int i=0; i<dailyRewards.Length; i++) {
			dailyRewardNums [i].text = "+" + data.signInfo [i].GivenValue;
			if (data.continueSignedDays <= i) {
				dailyRewards [i].sprite = notGetYetReward;
			} else {
				dailyRewards [i].sprite = gotReward;
				gotMarks [i].gameObject.SetActive (true);
			}
		}
		if (data.isTodaySigned) {
			signBtn.interactable = false;
		} else {
			PlayerPrefs.SetInt ("LowCoinGift", 2);
		}
	}

	public void OnGetButton ()
	{
//		if (onGetBtn != null) {
//			onGetBtn ();
//		}
		gotVfx.Play ();
		signBtn.interactable = false;
		if (!data.isTodaySigned) {
			//Debug.Log(data.continueSignedDays);
			user.coin+=data.signInfo[data.continueSignedDays].GivenValue;
			data.Sign ();
			Init();
			user.SaveMoney();
			UserHud.inst.RefreshMoney();
		}
		SOUND.Instance.OneShotSound (Sfx.inst.reward);
	}

}
