using System.Collections.Generic;
using UnityEngine;
using UnityEngineEx.CMD.i3778;
using UnityEngineEx.LogInterface;
using UnityEngineEx.Common;

using WORD = System.UInt16;
using BYTE = System.Byte;
using System.Collections;

public class AndroidUser : Player, IGameSink
{
	//出牌参数
// 	class OutCardParam
// 	{
// 		public tagOutCardResult OutCardResult ;
// 		//扑克分析                        
// 		public byte cbHandCardCount;
// 		public byte[] cbHandCards;
// 		public byte[] cbTurnCard;
// 		public byte cbTurnCardCount;
// 		public WORD m_wOutCardUser;
// 	}

    uint AndroidID;

	public uint ID { get { return AndroidID; } }

	ushort m_wBankerUser;
	byte m_cbCurrentLandScore;				//已叫分数
	//WORD m_wBlankTrenchUser;                //黑挖用户


	uint mTimerID = 0;

	//timer id

	//辅助时间
	const uint TIME_LESS = 1;									//最少时间
	const uint TIME_DISPATCH = 3;								//发牌时间

	//游戏时间
	const uint TIME_OUT_CARD = 0;								//出牌时间
	const uint TIME_START_GAME = 3;								//开始时间
	const uint TIME_CALL_SCORE = 0;							    //叫分时间
	const uint TIME_SEND_DARK_CARD = 6;                         //暗牌时间

	//游戏时间
	const uint IDI_OUT_CARD = (uint)(GameExport.Define.IDI_ANDROID_ITEM_SINK + 0);			//出牌时间
	const uint IDI_START_GAME = (uint)(GameExport.Define.IDI_ANDROID_ITEM_SINK + 1);		//开始时间
	const uint IDI_CALL_SCORE = (uint)(GameExport.Define.IDI_ANDROID_ITEM_SINK + 2);		//叫分时间
	const uint IDI_BLANK_TRENCH = (uint)(GameExport.Define.IDI_ANDROID_ITEM_SINK + 3);	//黑挖时间


	//
	GameLogic m_GameLogic = new GameLogic ();						//游戏逻辑

	public AndroidUser ()
	{        
		mIsAndroid = true;
        m_GameLogic.ResetData();        
	}

	public void Init (uint UserID)
	{
        AndroidID = UserID;
		//PlayerData data = new PlayerData();      
		m_wBankerUser = (ushort)GlobalDef.Deinfe.INVALID_CHAIR;
		mPlayerDevice.cbGender = (byte)UnityEngine.Random.Range (GlobalDef.GENDER_NULL, GlobalDef.GENDER_GIRL + 1);
        mPlayerDevice.mName = Name = "玩家" + UserID;
		mPlayerDevice.DeviceID = "android" + ID.ToString ();
		SetUserData ();
		mUser_.dwGoldCount = (uint)UnityEngine.Random.Range (10000, 900000);   
		mUser_.dwUserID = 1000 + UserID;
		m_cbCurrentLandScore = 255;

		//启动准备定时器
		mTimerID = IDI_START_GAME;
		float Elapse = 1;//Random.Range(TIME_LESS, TIME_LESS + TIME_CALL_SCORE + 1);
		//SetTimer(IDI_START_GAME, Elapse);        
	}

	void SetGameTimer (uint id, float elapse)
	{
		Timer.Instance.SetTimer (MakeTimerID (id), elapse, OnEventTimer);
	}

	void KillGameTimer (uint id)
	{
		Timer.Instance.KillTimer (MakeTimerID (id));
	}

	public void KillAllGameTimer ()
	{
		Timer.Instance.KillTimer (MakeTimerID (IDI_OUT_CARD));
		Timer.Instance.KillTimer (MakeTimerID (IDI_START_GAME));
		Timer.Instance.KillTimer (MakeTimerID (IDI_CALL_SCORE));
		Timer.Instance.KillTimer (MakeTimerID (IDI_BLANK_TRENCH));
	}

	uint MakeTimerID (uint id)
	{
		return (uint)(Timer.GameTimerIDBase + mUser.wChairID * 1000 + id);
	}

	uint ParseTimerID (uint id)
	{
		long _id = id - Timer.GameTimerIDBase - mUser.wChairID * 1000;
		return (uint)(_id < 0 ? 0 : _id);
	}

	public void OnGameMessage (GlobalDef.CMD_Head cmd, object o)//游戏处理函数
	{
		//用户消息
		if (cmd.CommandInfo.wMainCmdID == CMD_Game.MDM_GR_USER) {
			OnMainUser (cmd, o);
		}

		//道具消息

		//框架消息
		//游戏消息
		if (cmd.CommandInfo.wMainCmdID == GlobalFrame.MDM_GF_GAME) {
			OnMainGame (cmd, o);
		}

	}

	public void ResetData ()//重置游戏数据
	{
		m_GameLogic.ResetData();
        m_GameLogic.SetBomb(GameHelper.Instance.IsBomb);
	}

	bool OnMainUser (GlobalDef.CMD_Head cmd, object o)
	{
		switch (cmd.CommandInfo.wSubCmdID) {
		case CMD_Game.SUB_GR_USER_STATUS:
			{
				return OnSubUserStatus ((CMD_Game.CMD_GR_UserStatus)o);
			}
		}

		return true;
	}

	bool OnMainGame (GlobalDef.CMD_Head cmd, object o)
	{
		switch (cmd.CommandInfo.wSubCmdID) {
		case enCmdTrench.SUB_S_SEND_CARD://发牌
			{
				return OnSubSendCard ((CMD_S_SendCard)o);
			}
		case enCmdTrench.SUB_S_LAND_SCORE://叫分
			{
				return OnCallScore ((CMD_S_LandScore)o);
			}
		case enCmdTrench.SUB_S_GAME_START://游戏开始
			{
				return OnSubGameStart ((CMD_S_GameStart)o);
			}
		case enCmdTrench.SUB_S_OUT_CARD://游戏出牌
			{
				return OnSubGameOutCard ((CMD_S_OutCard)o);
			}
		case enCmdTrench.SUB_S_PASS_CARD://不出
			{
				return OnSubGamePassCard ((CMD_S_PassCard)o);
				//break;
			}
		case enCmdTrench.SUB_C_TRUSTEE://游戏托管
			{
				break;
			}
		case enCmdTrench.SUB_S_GAME_END://游戏结束
			{
				return OnSubGameEnd ((CMD_S_GameEnd)o);
			}

		}
		return true;
	}

	bool OnSubUserStatus (CMD_Game.CMD_GR_UserStatus cmd)
	{
		//是自己
		if (cmd.dwUserID == mUser.dwUserID) {
			if (mUser.cbUserStatus == (byte)GlobalDef.enUserStatus.US_SIT) {
				float Elapse = 0.1f;//Random.Range(TIME_LESS, TIME_LESS + TIME_CALL_SCORE + 1);
				SetGameTimer (IDI_START_GAME, Elapse);
                m_GameLogic.SetBomb(GameHelper.Instance.IsBomb);
			}
		} else {

		}

		return true;
	}

	bool OnSubSendCard (CMD_S_SendCard cmd)
	{

		//扑克变量
		m_cbCurrentLandScore = 0;

		//所有扑克
        for (WORD wChairID = 0; wChairID < CMD_Trench.GAME_PLAYER; ++wChairID)
        {
            m_GameLogic.SetUserCard(wChairID, GameHelper.Instance.PlayerHandCard[wChairID]);
        }

        //设置底牌
        //m_GameLogic.SetBackCard(GameHelper.Instance.mBackCard, (byte)GameHelper.Instance.mBackCard.Length);

		//叫牌扑克
		//List<BYTE> cbLandScoreCardData = new List<BYTE> ();
		//cbLandScoreCardData.AddRange (GameHelper.Instance.PlayerHandCard [mUser.wChairID]);
		//cbLandScoreCardData.AddRange (GameHelper.Instance.mBackCard);
		//BYTE[] cbLandScoreCardData = new BYTE[GLogicDef.MAX_COUNT] ;
		//CopyMemory(cbLandScoreCardData, pSendCard->bCardData[m_pIAndroidUserItem->GetChairID()], NORMAL_COUNT) ;
		//CopyMemory(cbLandScoreCardData+NORMAL_COUNT, pSendCard->bBackCardData, BACK_COUNT) ;

		//m_GameLogic.SetLandScoreCardData (cbLandScoreCardData.ToArray (), (byte)cbLandScoreCardData.Count);

		//手上扑克
		//m_cbHandCardCount=NORMAL_COUNT;	
		//CopyMemory(m_cbHandCardData, pSendCard->bCardData[m_pIAndroidUserItem->GetChairID()], NORMAL_COUNT) ; 

		//排列扑克

		//玩家处理

		mCurrentCallUser = cmd.wCurrentUser;
		mBlankTrenchUser = cmd.wBlankTrenchUser;

		return true;
	}

	public override void ActionCallScore ()
	{
		base.ActionCallScore ();

		if (mUser.wChairID == mCurrentCallUser && mBlankTrenchUser == (ushort)GlobalDef.Deinfe.INVALID_CHAIR) {
			//UINT nElapse = rand() % TIME_CALL_SCORE + TIME_LESS;
			//m_pIAndroidUserItem->SetGameTimer(IDI_CALL_SCORE, nElapse + TIME_DISPATCH);
			//mTimerID = IDI_CALL_SCORE;
            float Elapse = 1.0f + TIME_DISPATCH;//Random.Range(TIME_LESS, TIME_LESS+TIME_CALL_SCORE);
			//Invoke("OnEventTimer", Elapse);
			//Timer.Instance.SetTimer(IDI_CALL_SCORE, Elapse, OnEventTimer);
			SetGameTimer (IDI_CALL_SCORE, Elapse);
			//Debuger.Instance.LogWarning(">>>>"+System.DateTime.Now.ToString());

		}
	}

	bool OnCallScore (CMD_S_LandScore cmd)
	{
		//变量定义
		CMD_S_LandScore pCallScore = cmd;
		m_cbCurrentLandScore = pCallScore.bCurrentScore;

		//设置庄家
		m_wBankerUser = pCallScore.bLandUser;
		//m_GameLogic.SetBanker (pCallScore.bLandUser);

		//用户处理
		if (mUser.wChairID == pCallScore.wCurrentUser) {
			mTimerID = IDI_CALL_SCORE;
			float Elapse = 1.0f;//Random.Range (TIME_LESS, TIME_LESS + TIME_CALL_SCORE);
			//Invoke("OnEventTimer", Elapse);
			//UINT nElapse = rand() % TIME_CALL_SCORE + TIME_LESS;
			//m_pIAndroidUserItem->SetGameTimer(IDI_CALL_SCORE, nElapse);
			SetGameTimer (IDI_CALL_SCORE, Elapse);
		}
		return true;
	}

	bool OnSubGameStart (CMD_S_GameStart cmd)
	{
		//效验参数
		//ASSERT(wDataSize==sizeof(CMD_S_GameStart));
		//if (wDataSize!=sizeof(CMD_S_GameStart)) return false;

		//变量定义
		CMD_S_GameStart pGameStart = cmd;

		//设置状态
		//m_pIAndroidUserItem->SetGameStatus(GS_WK_PLAYING);

		//设置庄家
		m_wBankerUser = pGameStart.wLandUser;
		m_GameLogic.SetBanker (pGameStart.wLandUser);

		//设置底牌	
        m_GameLogic.SetBackCard(pGameStart.bBackCard, (byte)pGameStart.bBackCard.Length);

		////设置扑克
		//if (pGameStart->wLandUser==m_pIAndroidUserItem->GetChairID())
		//{
		//    //设置扑克
		//    m_cbHandCardCount+=CountArray(pGameStart->bBackCard);
		//    CopyMemory(&m_cbHandCardData[NORMAL_COUNT],pGameStart->bBackCard,sizeof(pGameStart->bBackCard));

		//    //排列扑克
		//    m_GameLogic.SortCardList(m_cbHandCardData,m_cbHandCardCount,ST_ORDER);
		//}

		//玩家设置
		if (pGameStart.wCurrentUser == mUser.wChairID) {
			//mTimerID = IDI_OUT_CARD;
            float Elapse = 1.0f + TIME_SEND_DARK_CARD;//Random.Range (TIME_SEND_DARK_CARD, TIME_SEND_DARK_CARD + TIME_OUT_CARD + TIME_LESS);
			//Invoke("OnEventTimer", Elapse);
			SetGameTimer (IDI_OUT_CARD, Elapse);
		}

		return true;
	}

	bool OnSubGameOutCard (CMD_S_OutCard msg)
	{
		//变量定义
		CMD_S_OutCard pOutCard = msg;
		//WORD wHeadSize=sizeof(CMD_S_OutCard)-sizeof(pOutCard->bCardData);

		//效验数据
		//ASSERT((wDataSize>=wHeadSize)&&(wDataSize==(wHeadSize+pOutCard->bCardCount*sizeof(BYTE))));
		//if ((wDataSize<wHeadSize)||(wDataSize!=(wHeadSize+pOutCard->bCardCount*sizeof(BYTE)))) return false;
		if (GameHelper.Instance.GameStatus != CMD_Trench.GS_WK_PLAYING) {
			return true;
		}

		//m_wOutCardUser = pOutCard->wOutCardUser ;
		//出牌变量
		if (pOutCard.wCurrentUser == pOutCard.wOutCardUser) {
			//m_cbTurnCardCount=0;
			//ZeroMemory(m_cbTurnCardData,sizeof(m_cbTurnCardData));
		} else {
			//m_cbTurnCardCount=pOutCard->bCardCount;
			//CopyMemory(m_cbTurnCardData,pOutCard->bCardData,pOutCard->bCardCount*sizeof(BYTE));
		}

		//玩家设置(自己)
		if (mUser.wChairID == pOutCard.wCurrentUser) {
            float Elapse = 1.0f;// Random.Range(TIME_LESS, TIME_OUT_CARD + TIME_LESS);
			//m_pIAndroidUserItem->SetGameTimer(IDI_OUT_CARD,nElapse);
			SetGameTimer (IDI_OUT_CARD, Elapse);
		}

		//设置变量
		if (mUser.wChairID != pOutCard.wOutCardUser)
			m_GameLogic.RemoveUserCardData (pOutCard.wOutCardUser, pOutCard.bCardData, pOutCard.bCardCount);

		return true;
	}

	bool OnSubGamePassCard (CMD_S_PassCard msg)
	{
		//效验数据
		//ASSERT(wDataSize == sizeof(CMD_S_PassCard));
		//if (wDataSize != sizeof(CMD_S_PassCard)) return false;

		//变量定义
		CMD_S_PassCard pPassCard = (CMD_S_PassCard)msg;
		if (GameHelper.Instance.GameStatus != CMD_Trench.GS_WK_PLAYING)
			return true;

		//一轮判断
		if (pPassCard.bNewTurn == 1) {
			//m_cbTurnCardCount = 0;
			//ZeroMemory(m_cbTurnCardData, sizeof(m_cbTurnCardData));
		}

		//玩家设置
		if (mUser.wChairID == pPassCard.wCurrentUser) {
            float Elapse = 1.0f;// Random.Range(TIME_LESS, TIME_OUT_CARD + TIME_LESS);
			SetGameTimer (IDI_OUT_CARD, Elapse);
		}

		return true;
	}

	bool OnSubGameEnd (CMD_S_GameEnd msg)
	{
		//效验数据
		//ASSERT(wDataSize == sizeof(CMD_S_GameEnd));
		//if (wDataSize != sizeof(CMD_S_GameEnd)) return false;

		//变量定义
		CMD_S_GameEnd pGameEnd = (CMD_S_GameEnd)msg;

		//设置状态
		//m_pIAndroidUserItem->SetGameStatus(GS_WK_FREE);

		//删除时间
		KillGameTimer (IDI_OUT_CARD);
		//KillGameTimer(IDI_CALL_SCORE);

		//开始设置
		//         if (!(m_pIAndroidUserItem->GetGameServiceOption()->wServerType & GAME_GENRE_MATCH)
		//             && !(m_pIAndroidUserItem->GetGameServiceOption()->wServerType & GAME_GENRE_HILADDER))
		{
            float Elapse = 1.0f;// Random.Range(TIME_LESS, TIME_START_GAME + TIME_LESS);
			SetGameTimer (IDI_START_GAME, Elapse);
		}

		return true;
	}

	/// <summary>
	/// 机器人定时处理
	/// </summary>
	void OnEventTimer (uint id)
	{
		switch (ParseTimerID (id)) {
		case IDI_START_GAME:
			{
				GameHelper.Instance.Ready (mUser.wChairID);
				break;
			}
		case IDI_CALL_SCORE:
			{
				//Debuger.Instance.LogWarning(">>>>2" + System.DateTime.Now.ToString());
				//构造变量
				//CMD_C_LandScore CallScore = new CMD_C_LandScore();

				//设置变量
				//CallScore.bLandScore = m_GameLogic.LandScore(mUser.wChairID, m_cbCurrentLandScore);
				byte bLandScore = m_GameLogic.LandScore (mUser.wChairID, m_cbCurrentLandScore);

				//发送数据
				//m_pIAndroidUserItem->SendSocketData(SUB_C_LAND_SCORE, &CallScore, sizeof(CallScore));
				GameHelper.Instance.CallScore (mUser.wChairID, bLandScore);
				break;
			}
		case IDI_OUT_CARD:
			{
                if (DataBase.Instance.PLAYER.HasStopOut)
                {
                    Debug.Log("Pass Property!!!");
                    GameHelper.Instance.PassCard(mUser.wChairID);
                    return;
                }

                List<byte> calcOutCard = new List<byte>();
                m_GameLogic.OutCard(mUser.wChairID,
                    GameHelper.Instance.TurnOutCard.ToArray(),
                    (byte)GameHelper.Instance.TurnOutCard.Count,
                    calcOutCard);

                //结果判断
                if (calcOutCard.Count > 0)
                {
                    //合法判断			

                    //删除扑克

                    //出牌
                    GameHelper.Instance.OutCard(mUser.wChairID, calcOutCard.ToArray());
                }
                else
                {
                    //放弃出牌
                    GameHelper.Instance.PassCard(mUser.wChairID);
                }

                break;
            }
        }
	}

    //void AnalyseOutCard (object obj)
    //{
    //    OutCardParam op = (OutCardParam)obj;

    //    //tagOutCardResult OutCardResult = op.OutCardResult;
    //    ////扑克分析                        
    //    //byte cbHandCardCount = op.cbHandCardCount;
    //    //byte[] cbHandCards = op.cbHandCards;
    //    //byte[] cbTurnCard = op.cbTurnCard;
    //    //byte cbTurnCardCount = (byte)cbTurnCard.Length;
    //    //WORD m_wOutCardUser = op.m_wOutCardUser;
        
    //    try {//手上扑克 扑克数目 出牌列表  出牌数目 出牌玩家 椅子号码
    //        if ((op.cbHandCardCount > 0) && (op.cbHandCardCount <= GLogicDef.MAX_COUNT))
    //            m_GameLogic.SearchOutCard (op.cbHandCards, op.cbHandCardCount, op.cbTurnCard,
    //                op.cbTurnCardCount, op.m_wOutCardUser, mUser.wChairID, op.OutCardResult);
    //    } catch (System.Exception e) {
    //        //这里的设置，使得进入下面的if处理
    //        //op.OutCardResult = new tagOutCardResult();
    //        //ZeroMemory(OutCardResult.cbResultCard, sizeof(OutCardResult.cbResultCard)) ;
    //        op.OutCardResult.cbCardCount = 0;
    //        op.OutCardResult.cbResultCard.ArraySetAll ((byte)0);
    //        //Debuger.Instance.LogError(e.Message);
    //    }

    //    UnityEngineEx.MThread.MThreadPool.Instance.NewThreadToMain (ProcessOutCard, op.OutCardResult);
    //}

    //void ProcessOutCard (object obj)
    //{
        

		

    //    //------------------------------------------------------
    //    tagOutCardResult OutCardResult = (tagOutCardResult)obj;
    //    //扑克分析                        
    //    byte cbHandCardCount = (byte)GameHelper.Instance.PlayerHandCard [mUser.wChairID].Count;
    //    byte[] cbHandCards = GameHelper.Instance.PlayerHandCard [mUser.wChairID].ToArray ();
    //    byte[] cbTurnCard = GameHelper.Instance.TurnOutCard.ToArray ();
    //    byte cbTurnCardCount = (byte)cbTurnCard.Length;
    //    WORD m_wOutCardUser = GameHelper.Instance.OutCardUser;

    //    //牌型合法判断
    //    if (OutCardResult.cbCardCount > 0 && GLogicDef.CT_INVALID == m_GameLogic.GetCardType (OutCardResult.cbResultCard, OutCardResult.cbCardCount)) {
    //        OutCardResult = new tagOutCardResult ();
    //        OutCardResult.cbCardCount = 0;
    //        //ZeroMemory(&OutCardResult, sizeof(OutCardResult)) ;
    //    }

    //    //先出牌不能为空
    //    if (cbTurnCardCount == 0) {
    //        if (OutCardResult.cbCardCount == 0) {
    //            //最小一张
    //            OutCardResult.cbCardCount = 1;
    //            OutCardResult.cbResultCard [0] = cbHandCards [cbHandCardCount - 1];
    //        }
    //    }

    //    //结果判断
    //    if (OutCardResult.cbCardCount > 0) {
    //        //合法判断
    //        if (cbTurnCardCount > 0 && !m_GameLogic.CompareCard (cbTurnCard, OutCardResult.cbResultCard, cbTurnCardCount, OutCardResult.cbCardCount)) {
    //            //放弃出牌
    //            //m_pIAndroidUserItem->SendSocketData(SUB_C_PASS_CARD);
    //            GameHelper.Instance.PassCard (mUser.wChairID);
    //            return;
    //        }

    //        //删除扑克
    //        if (((cbHandCardCount - OutCardResult.cbCardCount) >= 0) && (cbHandCardCount <= GLogicDef.MAX_COUNT)) {
    //            byte[] cards = new byte[OutCardResult.cbCardCount];
    //            System.Array.Copy (OutCardResult.cbResultCard, cards, OutCardResult.cbCardCount);
    //            GameHelper.Instance.OutCard (mUser.wChairID, cards);
    //        }
    //    } else {
    //        //放弃出牌
    //        GameHelper.Instance.PassCard (mUser.wChairID);
    //    }
    //}
}

public class AndroidUserMgr
{
	uint MaxCount = 2;
	//机器人列表
	Dictionary<uint, AndroidUser> AndroidList = new Dictionary<uint, AndroidUser> ();

  

	public AndroidUserMgr ()
	{
		for (uint i = 0; i < MaxCount; i++) {
			AndroidUser android = new AndroidUser ();           
			android.Init (i);
			AndroidList [android.mUser.dwUserID] = android;
		}
	}

	//分配机器人
	public AndroidUser ActiveAndroid ()
	{
		if (AndroidList.Count <= 0)
			return null;
		AndroidUser user = null;
		foreach (var e in AndroidList.Values) {
			user = e;
			break;
		}

		AndroidList.Remove (user.mUser.dwUserID);
		return user;
	}

	//收回机器人
	public void RealseAndroid (AndroidUser user)
	{
		AndroidList [user.mUser.dwUserID] = user;
		AndroidList [user.mUser.dwUserID].KillAllGameTimer ();
	}
}