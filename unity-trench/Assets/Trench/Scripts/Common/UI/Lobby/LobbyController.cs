using UnityEngine;
using System.Collections;

public enum LobbyState
{
	FinalPhase,//left
	Arena,//right
	Map,//up
	Shop,//down
	MainMenu,//Center
	Game,//HideAll
}

public class LobbyController : MonoBehaviour
{
	[SerializeField]
	LobbyBar
		topBar;
	[SerializeField]
	LobbyBar
		bottomBar;
	[SerializeField]
	ArenaUI
		arena;
	[SerializeField]
	MoveTweener
		backBtn;
	[SerializeField]
	Mover
		bkg;
	[Tooltip("0:final phase,1:arena,2:map,3:shop,4:main,5:Game")]
	[SerializeField]
	MoveTweener[]
		stateBtns;
	LobbyState lastState = LobbyState.MainMenu;
	[SerializeField]
	MoveTweener
		curtain;
	[SerializeField]
	LobbyData
		data;
	UserHud hud;
	[SerializeField]
	Shop
		shop;
	LobbyDialogManager dialogs;

	void Start ()
	{
		dialogs = LobbyDialogManager.inst;
		hud = UserHud.inst;
		Init ();
		curtain.FromTarget ();
		SOUND.Instance.PlayBGM (Bgm.inst.lobby);
	}

	public void Init ()
	{
		data.Init ();

		hud.Init ();

		shop.Init ();

		if (data.navigation.LobbyWin != LobbyState.MainMenu) {
			SwitchState (data.navigation.LobbyWin);
		} else {
			if (!data.sign.isTodaySigned) {
				dialogs.ShowDialog (LobbyDialog.DailySign);
			}
		}
	}

	public void SwitchState (LobbyState lobbyState)
	{
		SwitchState ((int)lobbyState);
	}

	public void SwitchState (int lobbyState)
	{
		SOUND.Instance.OneShotSound (Sfx.inst.btn);
		if (lastState != (LobbyState)lobbyState) {
			if ((LobbyState)lobbyState != LobbyState.Game) {
				bkg.MoveTo ((Direction)lobbyState);
				stateBtns [lobbyState].gameObject.SetActive (true);
				stateBtns [lobbyState].FromTarget ();
				stateBtns [(int)lastState].ToTarget ();
			}
			switch ((LobbyState)lobbyState) {
			case LobbyState.FinalPhase:
				backBtn.gameObject.SetActive (true);
				backBtn.FromTarget ();
				bottomBar.HideBtns ();
				break;
			case LobbyState.Arena:
				backBtn.gameObject.SetActive (true);
				backBtn.FromTarget ();
				bottomBar.HideBtns ();
				break;
			case LobbyState.Map:
				backBtn.gameObject.SetActive (true);
				backBtn.FromTarget ();
				bottomBar.HideBtns ();
				break;
			case LobbyState.Shop:
				backBtn.gameObject.SetActive (true);
				backBtn.FromTarget ();
				bottomBar.HideBtns ();
				break;
			case LobbyState.MainMenu:
				backBtn.ToTarget ();
				bottomBar.ShowBtns ();
				break;
			case LobbyState.Game:
				topBar.HideBtns ();
				bottomBar.HideBtns ();
				curtain.ToTarget (arena.LoadPlayScene);
				break;
			}
			lastState = (LobbyState)lobbyState;
		}
	}

}
