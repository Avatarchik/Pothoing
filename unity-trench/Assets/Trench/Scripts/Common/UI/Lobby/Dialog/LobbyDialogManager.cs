using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

public enum LobbyDialog
{
	Confirm=0,
	UserInfo,
	Mail,
	Setting,
	Activity,
	Friend,
	Ranking,
	Mission,
	LevelInfo,
	DailySign=9
}

public class LobbyDialogManager : DialogManager
{
	//[Tooltip("0 Confirm,1 UserInfo,2 Mail,3 Setting,4 Activity,5 Friend,6 Ranking,7 Mission,8 LevelInfo,9 LevelResult,10 DailySign")]
	public static LobbyDialogManager inst;
	ConfirmDialog confirmDialog;
	[SerializeField]
	UnityEvent
		onLowCoin;
	[SerializeField]
	UnityEvent
		onLowDiamond;
	[SerializeField]
	UnityEvent
		onLowItem;

	void Awake ()
	{
		inst = this;
	}

	void Start ()
	{
		//GetDialog (LobbyDialog.Confirm).GetComponent<ConfirmDialog> ().Init ("您确定要退出游戏吗？",Quit);
		confirmDialog = GetDialog (LobbyDialog.Confirm).GetComponent<ConfirmDialog> ();
	}
	public  Dialog GetDialog (LobbyDialog dialog)
	{
		return dialogs [(int)dialog];
	}

	public  void ShowDialog (LobbyDialog dialog)
	{
		ShowDialog ((int)dialog);
	}
	
	public void HideDialog (LobbyDialog dialog)
	{
		HideDialog ((int)dialog);
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			ShowConfirmDialog (TextManager.Get ("quitTip"), Quit);
		}
	}

	public void ShowConfirmDialog (string descString, Action onConfirmed, string confirmDescString=null, bool hasCancelBtn=true, Action onCancelled=null, string cancelDescString=null, string title=null)
	{
		confirmDialog.Init (descString, onConfirmed, confirmDescString, hasCancelBtn, onCancelled, cancelDescString, title);
		ShowDialog (LobbyDialog.Confirm);
	}

    private void CloseAll()
    {
        foreach (Dialog dia in dialogs)
            if (dia!= null && dia.enabled) dia.Hide();
    }
	public void ShowLowCoinDialog (GoodType goodType, string descString=null)
	{
		string desc = "";
		Action gotoShopPage = onLowCoin.Invoke;
		if (goodType == GoodType.Coin) {
			gotoShopPage = onLowCoin.Invoke;
			desc = TextManager.Get ("lowCoinTip");
		} else if (goodType == GoodType.Diamond) {
			gotoShopPage = onLowDiamond.Invoke;
			desc = TextManager.Get ("lowDiamondTip");
		} else {
			gotoShopPage = onLowItem.Invoke;
			desc = TextManager.Get ("lowItemTip");
		}
		if (string.IsNullOrEmpty (descString)) {
			descString = desc;
		} 
		ShowConfirmDialog (descString, 
            delegate{
                if (gotoShopPage != null) gotoShopPage();
                CloseAll();
            }, 
            TextManager.Get ("Buy"), true, null, null, TextManager.Get ("lowCoinTitle"));
	}

	void Quit ()
	{
		Debug.Log ("Quit!");
		Application.Quit ();
	}
}
