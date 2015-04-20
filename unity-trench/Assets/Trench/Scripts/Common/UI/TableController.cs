using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngineEx.CMD.i3778;

public enum CurrentPlayer
{
	Last,
	Self,
	Next,
}

public class TableController : MonoBehaviour, IGameView
{

	[SerializeField]
	PlayerPanel[]
		mAllPlayerPanel;
	[SerializeField]
	Clock
		clock;
	[SerializeField]
	ResultTip
		resultTip;
	[SerializeField]
	TableButtonManager
		btns;
	[SerializeField]
	CardStack
		cardStack;
	[SerializeField]
	HandCardMgr
		handManager;
	[SerializeField]
	TablePit
		pit;
	[SerializeField]
	VfxManager
		vfx;
	[SerializeField]
	BottomLab
		bottomBar;


	/// <summary>
	/// 倒计时时钟回调
	/// </summary>
	public delegate void ClockCallBack ();

	void Awake ()
	{
		handManager.InitOnSelected (RefreshPlayBtns);
	}

	IEnumerator Start ()
	{        
		pit.Show ();
		yield return 0;
		SOUND.Instance.PlayBGM (Bgm.inst.game);
	}


	//清除所有玩家打出的牌（包括不出提示）
	public void ClearOutCards ()
	{
		for (int i = 0; i < mAllPlayerPanel.Length; i++)
			SetState (i, PlayerStateText.None);
		handManager.ClearOutCards ();
	}
	//清理单个玩家出牌
	public void ClearOutCards (PlayerPanelPosition p)
	{
		SetState ((int)p, PlayerStateText.None);
		handManager.ClearOutCards (p);
	}

	public void ReStart ()
	{
		isFirstTime = true;
		bottomBar.ToggleReminderBtn (false);
		//隐藏重新开始按钮
		ToggleBtns (TableState.Result, false, 0);
		resultTip.Hide (delegate() {
			resultTip.gameObject.SetActive (false);
		});
        
		//清除手牌
		handManager.ClearHandCards ();
		ClearOutCards ();
		foreach (PlayerPanel p in mAllPlayerPanel) {
			p.SetMaster (PlayerPanel.PlayerMaster.NONE);
			p.SetRestCard (0);
		}
		//清除底牌
		pit.HideCards ();
		//显示准备按钮
		//ToggleBtns (TableState.Ready, true, 0);
		MainGame.inst.OnReady ();
		SOUND.Instance.PlayBGM (Bgm.inst.game);
	}
	//输赢显示
	public void ShowRusult (bool isWin)
	{
		resultTip.gameObject.SetActive (true);
		resultTip.SetResultTip (isWin);
		resultTip.ShowResult (isWin);
		if (isWin) {
			vfx.Play (VFX.Win);
			SOUND.Instance.PlayBGM (Bgm.inst.win, false);
		} else {
			vfx.Play (VFX.Lose);
			SOUND.Instance.PlayBGM (Bgm.inst.lose, false);
		}
	}

	public void SetInforPanel (int id, Player player)
	{
		clock.gameObject.SetActive (false);

		mAllPlayerPanel [id].ToggleState (false);
		mAllPlayerPanel [id].SetPlayerName (player.Name);
		mAllPlayerPanel [id].SetPlayerIcon (player.mPlayerDevice.cbGender);


		mAllPlayerPanel [id].SetStateType (PlayerStateText.None);
	}

	public void SwitchGameState (TableState state)
	{
		//Debug.Log ("Change State!" + state);
		btns.State = state;
	}

	void CloseReadyState ()
	{
		for (int i = 0; i < mAllPlayerPanel.Length; i++) {
			mAllPlayerPanel [i].ToggleState (false);
		}
	}

	public void SwitchGameState (int state)
	{
		SwitchGameState ((TableState)state);
	}

	void SwitchPlayer (CurrentPlayer player, float time=0.5f)
	{
		switch (player) {
		case CurrentPlayer.Self:
			clock.SetClockPosition (ClockPosition.Play, time);
			btns.TogglePlayButtons (true);
			break;
		case CurrentPlayer.Next:
			clock.SetClockPosition (ClockPosition.NextPlayer, time);
			btns.TogglePlayButtons (false);
			break;
		case CurrentPlayer.Last:
			clock.SetClockPosition (ClockPosition.LastPlayer, time);
			btns.TogglePlayButtons (false);
			break;
		default:
			break;
		}
	}

	void SwitchPlayer (int player, float time=0.5f)
	{
		//Debug.Log ("player"+player);
		if (player == -1) {
			clock.gameObject.SetActive (true);
			clock.SetClockPosition (ClockPosition.Identify, time);
		} else {
			SwitchPlayer ((CurrentPlayer)player, time);
		}
	}

	public void StartDispatchCards (byte[] data)
	{
		btns.ToggleBtns (TableState.Ready, false);

		cardStack.gameObject.SetActive (true);
		cardStack.InitCardList ();

		StartCoroutine (PlayCardStackAnim (data));

	}

	IEnumerator PlayCardStackAnim (byte[] data)
	{
		cardStack.transform.localScale = Vector3.one * 4f;
		iTween.MoveFrom (cardStack.gameObject, iTween.Hash ("position", Vector3.down * 800f,
		                                               "islocal", true,
		                                                  "easetype", iTween.EaseType.easeOutQuad,
		                                               "time", 0.5f));
		yield return new WaitForSeconds (0.5f);
//		iTween.RotateFrom (cardStack.gameObject, iTween.Hash ("rotation", Vector3.right * 45f,
//		                                                  "islocal", true,
//		                                                    "easetype", iTween.EaseType.linear,
//		                                                  "time", 0.5f));
		iTween.ScaleTo (cardStack.gameObject, iTween.Hash ("scale", Vector3.one * 1f,
		                                                    "islocal", true,
		                                                    "easetype", iTween.EaseType.easeInExpo,
		                                                    "time", 0.5f));
		yield return new WaitForSeconds (0.5f);
		Camera.main.gameObject.SendMessage ("Shake");
		vfx.Play (VFX.Dust);
		CloseReadyState ();
		SOUND.Instance.OneShotSound (Sfx.inst.hitTable);
		PlayVibration ();
		yield return new WaitForSeconds (0.5f);
		cardStack.Run ();
		handManager.Run (data);
	}
	
	public void SetCardStackCallBack (CardStack.CompleteCallBack callback)
	{
		cardStack.callback = callback;
	}
    
	bool isFirstTime = true;
	/// <summary>
	/// 倒计时时钟
	/// </summary>
	/// <param name="id">座位号</param>//-1:identify, 0:last,1:self,2:next
	/// <param name="times">倒计时秒数</param>
	/// <param name="callback">完成回调</param>
	public void ClockStart (int times, ClockCallBack callback, int id=-1, bool isShow=false)
	{
		if (!DataBase.Instance.IsOffline () || isShow) {
			if (isFirstTime) {
				if (GameHelper.Instance.GameStatus == CMD_Trench.GS_WK_SCORE && id == 1) {
					SwitchPlayer (-1, 0f);
				} else {
					SwitchPlayer (id, 0f);
				}
				isFirstTime = false;
			} else {
				if (GameHelper.Instance.GameStatus == CMD_Trench.GS_WK_SCORE && id == 1) {
					SwitchPlayer (-1);
				} else {
					SwitchPlayer (id);
				}
			}
			clock.gameObject.SetActive (true);

			if (callback != null) {
				clock.CountDown (times, (int)(times * 0.2f), callback.Invoke);
			} else {
				clock.CountDown (times, (int)(times * 0.2f), null);
			}
		}
	}


    public void TimeCallBack(int times, ClockPosition cp, ClockCallBack callback)
    {
        clock.SetClockPosition(cp);
        clock.gameObject.SetActive(true);
        if (callback != null)
        {
            clock.CountDown(times, (int)(times * 0.2f), callback.Invoke);
        }
        else
        {
            clock.CountDown(times, (int)(times * 0.2f), null);
        }
    }

    public void SetBlackCallBack(System.Action blackcall, System.Action cancalcall)
    {
        btns.SetBlackCallBack(blackcall, cancalcall);
    }

	/// <summary>
	/// 停止计时，放弃回调
	/// </summary>
	public void ClockStop ()
	{
		clock.StopCountDown ();
	}

	/// <summary>
	/// 设置坑主
	/// </summary>
	/// <param name="id"></param>
	public void SetMaster (int id)
	{
		if (id > 2 || id < 0) {
			return;
		}

		for (int i = 0; i < mAllPlayerPanel.Length; i++) {
			if (id == i) {
				mAllPlayerPanel [i].SetMaster (PlayerPanel.PlayerMaster.MASTER);
			} else {
				mAllPlayerPanel [i].SetMaster (PlayerPanel.PlayerMaster.NORMAL);
			}
		}
	}

	/// <summary>
	/// 设置玩家状态
	/// </summary>
	/// <param name="id"></param>
	/// <param name="stateText"></param>
	public void SetState (int id, PlayerStateText stateText)
	{
		if (stateText != PlayerStateText.None) {
			mAllPlayerPanel [id].ToggleState (true);
			mAllPlayerPanel [id].SetStateType (stateText);
		} else {
			mAllPlayerPanel [id].ToggleState (false);
		}
	}

	public void ToggleBtns (TableState tableState, bool isShow, int param)
	{
		btns.ToggleBtns (tableState, isShow);
		if (tableState == TableState.Identify) {
			SetIdentifyBtn (param);
			playBtnsStatus = -1;
		} else if (tableState == TableState.Play) {
			SetPlayBtns (param);
			if (!isShow) {
				playBtnsStatus = -1;
			}
		}
	}

	void SetIdentifyBtn (int score)
	{
		score = Mathf.Clamp (score, 0, 3);
		switch (score) {
		case 0:
			//show all
			btns.DisableBtn (DisableButton.Score1, false);
			btns.DisableBtn (DisableButton.Score2, false);
			break;
		case 1:
			//disable 1 
			btns.DisableBtn (DisableButton.Score1);
			btns.DisableBtn (DisableButton.Score2, false);
			break;
		case 2:
			//disable 2 and 1
			btns.DisableBtn (DisableButton.Score1);
			btns.DisableBtn (DisableButton.Score2);
			break;
		case 3:
			//disable pass and 1
			btns.DisableBtn (DisableButton.Score1);
			btns.DisableBtn (DisableButton.IdentifyPass);
			break;
		}
	}

	int playBtnsStatus = -1;
	//0: can not play ,1: can play,2:just play,3:disable all
	void SetPlayBtns (int canPlay)
	{
		canPlay = Mathf.Clamp (canPlay, 0, 3);
		playBtnsStatus = canPlay;
		switch (canPlay) {
		case 0:
			//disable hint and play
			btns.DisableBtn (DisableButton.Hint);
			btns.DisableBtn (DisableButton.Play);
			btns.DisableBtn (DisableButton.PlayPass, false);
			break;
		case 1:
			//show all
			btns.DisableBtn (DisableButton.Hint, false);
			if (handManager.GetReadyCardsData ().Length > 0) {
				btns.DisableBtn (DisableButton.Play, false);
			} else {
				btns.DisableBtn (DisableButton.Play);
			}
			btns.DisableBtn (DisableButton.PlayPass, false);
			break;
		case 2:
			//just play
			btns.DisableBtn (DisableButton.Hint);
			if (handManager.GetReadyCardsData ().Length > 0) {
				btns.DisableBtn (DisableButton.Play, false);
			} else {
				btns.DisableBtn (DisableButton.Play);
			}
			btns.DisableBtn (DisableButton.PlayPass);
			break;
		case 3:
			//disable all
			btns.DisableBtn (DisableButton.Hint);
			btns.DisableBtn (DisableButton.Play);
			btns.DisableBtn (DisableButton.PlayPass);
			break;
		}
	}

	void RefreshPlayBtns ()
	{
		//Debug.Log ("RefreshPlayBtns  "+btns.State+"   "+handManager.GetReadyCardsData ().Length);
		if (btns.State == TableState.Play && (playBtnsStatus == 1 || playBtnsStatus == 2)) {
			if (handManager.GetReadyCardsData ().Length > 0) {
				btns.DisableBtn (DisableButton.Play, false);
				//Debug.Log ("RefreshPlayBtns");
			} else {
				btns.DisableBtn (DisableButton.Play, true);
			}
		}
	}

	/// <summary>
	/// 出牌
	/// </summary>
	/// <param name="cards"></param>
	public void PlayOthersCard (byte[] cards, PlayerPanelPosition player, bool isFinal=false)
	{
		if (cards.Length > 0) {
			List<Card> cardList = new List<Card> ();
			if (player != PlayerPanelPosition.BOTTOM) {
				foreach (byte bCard in cards) {
					Card obj = Instantiate (cardStack.mCard) as Card;
					obj.SetCard (bCard);
					obj.transform.SetParent (mAllPlayerPanel [(int)player].transform);
					obj.transform.localPosition = Vector3.zero;
					obj.transform.localScale = Vector3.zero;
					cardList.Add (obj);
				}
				handManager.PlayCard (cardList, player);
				if (!isFinal) {
					int restCardsNum = mAllPlayerPanel [(int)player].GetRestCardNum () - cards.Length;
					bool isFemale = GameHelper.Instance.mPlayers [(int)player].Gender == GlobalDef.GENDER_GIRL ? true : false;
					if (restCardsNum == 2) {
						SOUND.Instance.OneShotSound (Sfx.inst.GetVoice (VoiceType.Warning, 0, isFemale));
					} else if (restCardsNum == 1) {
						SOUND.Instance.OneShotSound (Sfx.inst.GetVoice (VoiceType.Warning, 1, isFemale));
					}
					mAllPlayerPanel [(int)player].SetRestCard (restCardsNum);
				}
			} else {
				Debug.Log ("not other player!");
			}
		}
	}

	public byte[] GetPlayingCardsData ()
	{
		return handManager.GetReadyCardsData ();
	}

	public void ChangeCard (Card card, byte changedData, byte[] sequencedData)
	{
		RewriteCard (card, changedData);
		handManager.SequenceHands (sequencedData);
	}

	public void ChangeReadyCards (byte[] changedData, byte[] sequencedData)
	{
		RewriteCards (handManager.GetReadyCards (), changedData);
		handManager.SequenceHands (sequencedData);
	}

	public void SearchOutCard ()
	{
		List<byte> helpCards = MainGame.inst.SearchOutCard ();
		if (helpCards != null) {
			handManager.AddCardsToReadyCards (helpCards);
			RefreshPlayBtns ();
		}
        
	}

	public void OnPlaySelfCardSuccess (params byte[] cardsData)
	{
		List<byte> dataList = new List<byte> ();
		dataList.AddRange (cardsData);
		handManager.AddCardsToReadyCards (dataList);
		handManager.OnPlaySelfCardSuccess (cardsData);
		StartCoroutine (RefreshSelfRestCardNum ());
		vfx.Play (VFX.Dust);
	}

	IEnumerator RefreshSelfRestCardNum ()
	{
		yield return 0;
		mAllPlayerPanel [(int)CurrentPlayer.Self].SetRestCard (handManager.HandCards.Count);
	}

	public void OnPlaySelfCardFail ()
	{
		handManager.OnPlaySelfCardFail ();
	}
	
	/// <summary>
	/// 叫分完成开始游戏 底牌与玩家牌数据
	/// </summary>
	/// <param name="pitMaster"></param>
	/// <param name="pitCardsData"></param>
	/// <param name="allCardsData"></param>
	public void AddPitCard (CurrentPlayer pitMaster, byte[] pitCardsData, byte[] allCardsData)
	{
		//Debug.Log (pitMaster);
		pit.ShowCards (pitCardsData);
		byte[] allcards = new byte[allCardsData.Length];
		allCardsData.CopyTo (allcards, 0);
		byte[] pitcards = new byte[pitCardsData.Length];
		pitCardsData.CopyTo (pitcards, 0);
		if (pitMaster == CurrentPlayer.Self) {
			StartCoroutine (TrenchPitCards (allcards));
		} else {
			StartCoroutine (PushPitCardsToOtherPlayer (pitMaster));
		}
	}

	IEnumerator PushPitCardsToOtherPlayer (CurrentPlayer pitMaster)
	{
		yield return new WaitForSeconds (3f);
		cardStack.DispacthCardsTo (pit.GetTrenchedCards (), (int)pitMaster);
	}

	IEnumerator TrenchPitCards (byte[] allCardsData)
	{
		yield return new WaitForSeconds (3f);
		handManager.HandCards.AddRange (pit.GetTrenchedCards ());
		SequenceHands (allCardsData);
		StartCoroutine (RefreshSelfRestCardNum ());
	}

	public void SequenceHands (byte[] allCardsData)
	{
		handManager.SequenceHands (allCardsData);
	}

	void RewriteCard (Card card, byte targetData)
	{
		handManager.RewriteCardData (card, targetData);
	}
	
	void RewriteCards (Card[] cards, byte[] targetData)
	{
		handManager.RewriteCardsData (cards, targetData);
	}

	/// <summary>
	/// 设置手牌组件不可用
	/// </summary>
	/// <param name="isActive"></param>
	public void ToggleCards (CardControlLevel controlLevel)
	{
		handManager.ToggleCards (controlLevel);
	}

	public void ShowTip (RuleTipType tipType, float elipse=2f)
	{
		handManager.ShowTip (tipType, elipse);
	}

	public void MarkBombs (byte[] bombCardsData)
	{
		handManager.MarkAsBomb (bombCardsData);
	}

	public void PlayBombVfx ()
	{
		vfx.Play (VFX.Bomb);
		PlayVibration ();
	}

	public void PlayStraightVfx ()
	{
		vfx.Play (VFX.Straight);
	}

	public void PlayVibration ()
	{
		if (SystemInfo.supportsVibration) {
			if (SOUND.Instance.vibration) {
				Handheld.Vibrate ();
			}
		}
	}

	public void ToggleReminder (bool isOpen)
	{
		Reminder.inst.Init ();
		bottomBar.ToggleReminderBtn (isOpen);
	}

}
