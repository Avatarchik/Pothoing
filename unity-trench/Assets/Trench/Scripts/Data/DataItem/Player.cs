using UnityEngineEx.CMD.i3778;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineEx.Net;
using UnityEngineEx.LogInterface;
using System.Runtime.InteropServices;

using WORD = System.UInt16;
using DWORD = System.UInt32;
using EncodeTans = System.Text.Encoding;

[Serializable]
public class PlayerDeviceInfo
{
	public string mName;                //名称
	public byte cbGender;               //性别(0-保密 1-男 2-女)
	public string DeviceID;             //设备ID
}
[Serializable]
public class TitleInfoData
{
	public int Id;
	public int coin;
	public string title;                //名称
}


/// <summary>
///  PlayerUserData
/// </summary>
[Serializable]
public class PlayerUserData
{
	public byte cbCompanion{ get; private set; }

	public byte cbGender { get; private set; }

	public byte cbMasterOrder { get; private set; }

	public byte cbMemberOrder { get; private set; }

	public byte cbUserStatus { get; private set; }

	public uint dwAreaType { get; private set; }

	public uint dwGameID { get; private set; }

	public uint dwGoldCount { get; private set; }            //单机：银币 网络：金币
	public uint dwGroupID { get; private set; }

	public uint dwLotteries { get; private set; }

	public uint dwMasterRight { get; private set; }

	public uint dwUserID { get; private set; }

	public uint dwUserRight { get; private set; }

	public uint dwZScore { get; private set; }              //钻石
	public uint dwZZBean { get; private set; }

	public int lDrawCount { get; private set; }

	public int lExperience { get; private set; }

	public int lFleeCount { get; private set; }

	public long llInsureScore { get; private set; }

	public int lLostCount { get; private set; }

	public int lScore { get; private set; }

	public int lWinCount { get; private set; }

	public string szGroupName { get; private set; }

	public string szName { get; private set; }

	public string szUnderWrite { get; private set; }

	public ushort wChairID { get; private set; }

	public ushort wFaceID { get; private set; }

	public ushort wTableID { get; private set; }

	public ushort wUserType { get; private set; }

	public int continueSignedDays { get; private set; }

	//绑定信息
	public string PhoneNumber { get; private set; }      //为null或者“”未绑定
    

	//关卡信息
	public uint nCurStage { get; private set; }

    //用户关卡道具
    public int stageProperty { get; set; }

    //关卡道具可用状态
    public bool IsUsableProperty { get; set; }

	public PlayerUserData ()
	{

	}

	public PlayerUserData (GlobalDef.tagUserData UserData)
	{
		FromUserData (UserData);
	}

	public GlobalDef.tagUserData ToUserData ()
	{
		GlobalDef.tagUserData UserData;
		UserData.cbCompanion = cbCompanion;
		UserData.cbGender = cbGender;
		UserData.cbMasterOrder = cbMasterOrder;
		UserData.cbMemberOrder = cbMemberOrder;
		UserData.dwAreaType = dwAreaType;
		UserData.dwGameID = dwGameID;
		UserData.dwGoldCount = dwGoldCount;
		UserData.dwGroupID = dwGroupID;
		UserData.dwLotteries = dwLotteries;
		UserData.dwMasterRight = dwMasterRight;
		UserData.dwUserID = dwUserID;
		UserData.dwUserRight = dwUserRight;
		UserData.dwZScore = dwZScore;
		UserData.dwZZBean = dwZZBean;
		UserData.lDrawCount = lDrawCount;
		UserData.lExperience = lExperience;
		UserData.lFleeCount = lFleeCount;
		UserData.llInsureScore = llInsureScore;
		UserData.lLostCount = lLostCount;
		UserData.lScore = lScore;
		UserData.lWinCount = lWinCount;
		//UserData.szGroupName = new byte[GlobalDef.GROUP_LEN];
		//UserData.szName = new byte[GlobalDef.NAME_LEN];
		UserData.szGroupName = System.Text.Encoding.Default.GetBytes (szGroupName);
		UserData.szName = System.Text.Encoding.Default.GetBytes (szName);
		UserData.szUnderWrite = System.Text.Encoding.Default.GetBytes (szUnderWrite);
		//szName. CopyTo(UserData.szName, 0);
		//UserData.szUnderWrite = new byte[GlobalDef.UNDER_WRITE_LEN];
		//mUser.szUnderWrite[0]='\0';

		UserData.wChairID = (ushort)GlobalDef.Deinfe.INVALID_CHAIR;
		UserData.wTableID = (ushort)GlobalDef.Deinfe.INVALID_TABLE;
		UserData.cbUserStatus = (byte)GlobalDef.enUserStatus.US_FREE;
		UserData.wFaceID = 0;
		UserData.wUserType = 0;

		return UserData;
	}

	public void FromUserData (GlobalDef.tagUserData UserData)
	{
		this.cbCompanion = UserData.cbCompanion;
		this.cbGender = UserData.cbGender;
		this.cbMasterOrder = UserData.cbMasterOrder;
		this.cbMemberOrder = UserData.cbMemberOrder;
		this.dwAreaType = UserData.dwAreaType;
		this.dwGameID = UserData.dwGameID;
		this.dwGoldCount = UserData.dwGoldCount;
		this.dwGroupID = UserData.dwGroupID;
		this.dwLotteries = UserData.dwLotteries;
		this.dwMasterRight = UserData.dwMasterRight;
		this.dwUserID = UserData.dwUserID;
		this.dwUserRight = UserData.dwUserRight;
		this.dwZScore = UserData.dwZScore;
		this.dwZZBean = UserData.dwZZBean;
		this.lDrawCount = UserData.lDrawCount;
		this.lExperience = UserData.lExperience;
		this.lFleeCount = UserData.lFleeCount;
		this.llInsureScore = UserData.llInsureScore;
		this.lLostCount = UserData.lLostCount;
		this.lScore = UserData.lScore;
		this.lWinCount = UserData.lWinCount;

		this.szGroupName = System.Text.Encoding.Default.GetString (UserData.szGroupName);
		this.szName = System.Text.Encoding.Default.GetString (UserData.szName);
		this.szUnderWrite = System.Text.Encoding.Default.GetString (UserData.szUnderWrite);
		//this.szGroupName = new byte[GlobalDef.GROUP_LEN];
		//this.szName = new byte[GlobalDef.NAME_LEN];
		//UserData.szName.CopyTo(this.szName, 0);
		//this.szUnderWrite = new byte[GlobalDef.UNDER_WRITE_LEN];
		//UserData.szUnderWrite.CopyTo(this.szUnderWrite,0);
		//mUser.szUnderWrite[0]='\0';

		this.wChairID = UserData.wChairID;
		this.wTableID = UserData.wTableID;
		this.cbUserStatus = UserData.cbUserStatus;
		this.wFaceID = UserData.wFaceID;
		this.wUserType = UserData.wUserType;
	}

	public uint NextStage ()
	{
        uint curStageId = DataBase.Instance.CurStage.Id;
        return Math.Min(COMMON_CONST.StageLast, ++curStageId);
	}

	public uint CurStage ()
	{
		nCurStage = Math.Min (COMMON_CONST.StageLast, nCurStage);
		return Math.Max (nCurStage, COMMON_CONST.StageFirst);
	}


    public bool CanOpenStage(uint stage, int stars)
    {
        bool bres = false;
        INFO_STAGE InfoStage = DataBase.Instance.STAGE_MGR.GetStage(stage);
        uint bStageId = InfoStage.OpenCondition;//关卡前置条件关卡
        if (bStageId > 0)
        {
            UserStageRecord record = DataBase.Instance.PLAYER.GetUserStageRecord(bStageId);
            if (record != null && record.Status == (int)StageRecordStatus.Success
                && InfoStage != null && InfoStage.NeedStar <= stars)
                bres = true;
        }
        else if (InfoStage != null && InfoStage.NeedStar <= stars)
            bres = true;

        return bres;
    }

	/////////////////////////////////////////////////////////////////////
	//绑定
	public void SetPhoneNumber (string number)
	{
		PhoneNumber = number;
	}
}


/// <summary>
///  Player
/// </summary>
public class Player //: MonoBehaviour
{
    #region 公有属性
	public GlobalDef.tagUserData mUser { get { return mUser_; } protected set { mUser_ = value; } }                         //用户信息
	public PlayerDeviceInfo mPlayerDevice { get { return mPlayerDevice_; } protected set { mPlayerDevice_ = value; } }      //用户本地信息
	public PlayerUserData mPlayerUserData { get { return mPlayerUserData_; } protected set { mPlayerUserData_ = value; } }  //用户本地真实信息
    public string Name { get { return EncodeTans.Default.GetString(mUser_.szName); } set { mUser_.szName = EncodeTans.Default.GetBytes(value); } }//昵称
	public byte Gender { get { return mUser_.cbGender; } set { mUser_.cbGender = value; } }                                      //性别
	public int GameScore { get { return mUser_.lScore; } set { mUser_.lScore = value; } }                                   //游戏积分
	public uint Gold { get { return mUser_.dwGoldCount; } set { mUser_.dwGoldCount = value; } }                             //金币，银币
	public uint ZScore { get { return mUser_.dwZScore; } set { mUser_.dwZScore = value; } }                                 //钻石，z币
	public string PhoneNumber { get { return mPlayerUserData.PhoneNumber; } set { mPlayerUserData.SetPhoneNumber (value); } }//电话号码
	public ushort wFaceId { get { return mUser_.wFaceID; } set { mUser_.wFaceID = value; } }                                //头像ID
	public bool mIsAndroid { get; protected set; }  //是否是机器人
    public int Property { get { return mPlayerUserData.stageProperty; } set { mPlayerUserData.stageProperty = value; } }
    public bool IsUsableProperty { get { return mPlayerUserData.IsUsableProperty; } set { mPlayerUserData.IsUsableProperty = value; } }

    public bool HasStopOut { get { return hasStopOut; } set { hasStopOut = value; } }
    public int StopOutCount { get { return mStopOutCount; } set { mStopOutCount = value; } }//Debug.Log("mStopOutCount:" + mStopOutCount); } }

    public bool HasBlackCall{get{return mHasBlackCall;} set{mHasBlackCall = value;}}
    #endregion

    #region 保护属性
    
    protected bool hasStopOut;                                                          //是否不允许机器人出牌道具
	protected GlobalDef.tagUserData mUser_;                                             //用户信息
	protected PlayerDeviceInfo mPlayerDevice_;                                          //用户本地信息
	protected PlayerUserData mPlayerUserData_;                                          //用户本地真实信息    
    #endregion

    #region 私有属性
    private bool mHasBlackCall;
    private int mStopOutCount;
	PlayerStageRecord mPlayerStageRecord_;                                              //用户关卡记录
    #endregion
                        

	public Player ()
	{
		mIsAndroid = false;
		mUser_ = new GlobalDef.tagUserData ();
		mPlayerDevice_ = new PlayerDeviceInfo ();
		mPlayerUserData_ = new PlayerUserData ();
		mPlayerStageRecord_ = new PlayerStageRecord ();
        mStopOutCount = 0;

	}

	public void Load ()
	{
		string strUserInfo = COMMON_FUNC.GetUserBaseInfo ();
		SetPlayerData (strUserInfo);

		mPlayerStageRecord_.Load ();
	}

	PlayerTitle titles = new PlayerTitle ();
	public void LoadTitles ()
	{
		string data = Resources.Load (COMMON_CONST.PathTitleInfo).ToString ();
		titles.XmlLoad (data, "TitleInfo");
	}

	/// <summary>
	/// 保存用户信息到本地，请不要频繁调用
	/// </summary>
	public void SaveUserInfo ()
	{
		WriteUserInfo ();
	}

	/// <summary>
	/// 设置用户信息
	/// </summary>
	/// <param name="data"></param>
	protected void SetPlayerData (string data)
	{
		if (data == COMMON_CONST.NullString) {//用户首次进入或用户数据丢失，重新生成用户数据
			Debuger.Instance.Log ("用户首次进入或用户数据丢失，重新生成用户数据");

			mPlayerDevice.mName = "玩家名字";
			mPlayerDevice.cbGender = (byte)GlobalDef.GENDER_NULL;
			mPlayerDevice.DeviceID = DeviceManager.Instance.deviceId;

			data = mPlayerDevice.ToJSON ();
			//data = COMMON_FUNC.AESEncrypt(data);
			COMMON_FUNC.SetUserBaseInfo (data);
		} else {
			//data = COMMON_FUNC.AESDecrypt(data);
			mPlayerDevice = data.FromJSON<PlayerDeviceInfo> ();
			if (mPlayerDevice == null) {
				Debuger.Instance.LogError ("用户数据本地读取失败！");
			}
		}

		SetUserData ();       
	}
	

	protected void SetPlayerSignData (string data)
	{
		if (data == COMMON_CONST.NullString) {//用户首次进入或用户数据丢失，重新生成用户数据
			Debuger.Instance.Log ("用户首次进入或用户数据丢失，重新生成用户数据");
			
			mPlayerDevice.mName = "玩家名字";
			mPlayerDevice.cbGender = (byte)GlobalDef.GENDER_NULL;
			mPlayerDevice.DeviceID = DeviceManager.Instance.deviceId;
			
			data = mPlayerDevice.ToJSON ();
			//data = COMMON_FUNC.AESEncrypt(data);
			COMMON_FUNC.SetUserBaseInfo (data);
		} else {
			//data = COMMON_FUNC.AESDecrypt(data);
			mPlayerDevice = data.FromJSON<PlayerDeviceInfo> ();
			if (mPlayerDevice == null) {
				Debuger.Instance.LogError ("用户数据本地读取失败！");
			}
		}
		
		SetUserData ();       
	}

	public List<TitleInfoData> GetAllTitleData ()
	{
		List<TitleInfoData> list = new List<TitleInfoData> ();
		list.AddRange (titles.INFO.Values);
		return list;
	}

	/// <summary>
	/// 
	/// </summary>
	protected void SetUserData ()
	{
		string strUserInfo = COMMON_FUNC.GetUserInfo ();
		if (strUserInfo == COMMON_CONST.NullString || mIsAndroid) {            
			mUser_.cbCompanion = 0;
			mUser_.cbGender = mPlayerDevice.cbGender;
			mUser_.cbMasterOrder = 0;
			mUser_.cbMemberOrder = 0;
			mUser_.dwAreaType = 0;
			mUser_.dwGameID = 0;
			mUser_.dwGoldCount = 1000;
			mUser_.dwGroupID = 0;
			mUser_.dwLotteries = 0;
			mUser_.dwMasterRight = 0;
			mUser_.dwUserID = 1;
			mUser_.dwUserRight = 0;
			mUser_.dwZScore = 0;
			mUser_.dwZZBean = 0;
			mUser_.lDrawCount = 0;
			mUser_.lExperience = 0;
			mUser_.lFleeCount = 0;
			mUser_.llInsureScore = 0;
			mUser_.lLostCount = 0;
			mUser_.lScore = 0;
			mUser_.lWinCount = 0;
			mUser_.szGroupName = new byte[GlobalDef.GROUP_LEN];
			mUser_.szName = new byte[GlobalDef.NAME_LEN];
			mUser_.szName = System.Text.Encoding.Default.GetBytes (mPlayerDevice.mName);
			mUser_.szUnderWrite = new byte[GlobalDef.UNDER_WRITE_LEN];
			//mUser.szUnderWrite[0]='\0';

			mUser_.wChairID = (ushort)GlobalDef.Deinfe.INVALID_CHAIR;
			mUser_.wTableID = (ushort)GlobalDef.Deinfe.INVALID_TABLE;
			mUser_.cbUserStatus = (byte)GlobalDef.enUserStatus.US_FREE;
			mUser_.wFaceID = 0;
			mUser_.wUserType = 0;

			if (!mIsAndroid) {
				WriteUserInfo ();
			}
		} else {
			Debuger.Instance.Log ("用户本地信息" + strUserInfo);
			ReadUserInfo (strUserInfo);

			mUser_.wChairID = (ushort)GlobalDef.Deinfe.INVALID_CHAIR;
			mUser_.wTableID = (ushort)GlobalDef.Deinfe.INVALID_TABLE;
			mUser_.cbUserStatus = (byte)GlobalDef.enUserStatus.US_FREE;
		}
	}

	/// <summary>
	/// 设置用户状态
	/// </summary>
	/// <param name="status"></param>
	/// <param name="wTableId"></param>
	/// <param name="wChairId"></param>
	public void SetUserStatus (GlobalDef.enUserStatus status, ushort wTableId, ushort wChairId)
	{
		mUser_.cbUserStatus = (byte)status;
		mUser_.wTableID = wTableId;
		mUser_.wChairID = wChairId;
	}

	/// <summary>
	/// 写用户积分
	/// </summary>
	/// <param name="wChair"></param>
	/// <param name="Score"></param>
	/// <param name="ScoreKind"></param>
	public void WriteUserScore (ushort wChair, int Score, GameExport.enScoreKind ScoreKind)
	{        
		mUser_.lScore += Score;
		mUser_.lScore = mUser.lScore < 0 ? 0 : mUser.lScore;
		mUser_.dwGoldCount = (uint)mUser.lScore;

		//判断输赢
		switch (ScoreKind) 
        {
		case GameExport.enScoreKind.enScoreKind_Win:
			mUser_.lExperience += 10;
			mUser_.lWinCount++;
			break;
		case GameExport.enScoreKind.enScoreKind_Lost:
			mUser_.lExperience += 5;
			mUser_.lLostCount++;
			break;
		case GameExport.enScoreKind.enScoreKind_Flee:
			mUser_.lExperience += 5;
			mUser_.lFleeCount++;
			break;
		case GameExport.enScoreKind.enScoreKind_Draw:
			mUser_.lDrawCount++;
			break;
		}

		if (mIsAndroid)
			return;
		WriteUserInfo ();
	}


    /// <summary>
    /// 写用户银币
    /// </summary>
    /// <param name="wChair"></param>
    /// <param name="Score"></param>
    /// <param name="ScoreKind"></param>
    public void WriteUserGold(ushort wChair, int Score, GameExport.enScoreKind ScoreKind)
    {
        mUser_.dwGoldCount += (uint)Score;

        //判断输赢
        switch (ScoreKind)
        {
            case GameExport.enScoreKind.enScoreKind_Win:
                mUser_.lExperience += 10;
                mUser_.lWinCount++;
                break;
            case GameExport.enScoreKind.enScoreKind_Lost:
                mUser_.lExperience += 5;
                mUser_.lLostCount++;
                break;
            case GameExport.enScoreKind.enScoreKind_Flee:
                mUser_.lExperience += 5;
                mUser_.lFleeCount++;
                break;
            case GameExport.enScoreKind.enScoreKind_Draw:
                mUser_.lDrawCount++;
                break;
        }

        if (mIsAndroid)
            return;
        WriteUserInfo();
    }
	/// <summary>
	/// 写用户信息
	/// </summary>
	public void WriteUserInfo ()
	{
		mPlayerUserData.FromUserData (mUser);
		Debuger.Instance.Log ("用户信息:" + mPlayerUserData.ToJSON ());
		COMMON_FUNC.SetUserInfo (mPlayerUserData.ToJSON ());
	}

	/// <summary>
	/// 读用户信息
	/// </summary>
	/// <param name="strUserInfo"></param>
	void ReadUserInfo (string strUserInfo)
	{
		mPlayerUserData = strUserInfo.FromJSON<PlayerUserData> ();
		mUser = mPlayerUserData.ToUserData ();
	}


	///////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 当前关卡
	/// </summary>
	public uint CurStageId {
		get { return mPlayerUserData.CurStage (); } 
	}

    public INFO_STAGE GetCurrStage()
    {
        return DataBase.Instance.STAGE_MGR.GetStage(CurStageId);
    }


	/// <summary>
	/// 下一关
	/// </summary>
	public uint NextStage ()
	{
		return mPlayerUserData.NextStage ();
	}

	/// <summary>
	/// 关卡是否可以开启
	/// </summary>
	/// <param name="nStageId"></param>
	/// <returns></returns>
	public bool IsStageOpen (uint nStageId)
	{
		return mPlayerUserData.CanOpenStage (nStageId, mPlayerStageRecord_.GetTotalStar ());
	}


    public StageStatus GetStageStatus(uint stageId)
    {
        
        StageStatus status = StageStatus.NotUnlock;
        INFO_STAGE InfoStage = DataBase.Instance.STAGE_MGR.GetStage(stageId);
        int stars = mPlayerStageRecord_.GetTotalStar();
        if (InfoStage == null || InfoStage.NeedStar > stars)
            return status;//未开启
        else
            status = StageStatus.Unlocked;

        uint bStageId = InfoStage.OpenCondition;
        if (bStageId > 0)
        {
            UserStageRecord bRecord = GetUserStageRecord(bStageId);
            if (bRecord != null && bRecord.Status == (int)StageRecordStatus.Success)
                status = StageStatus.CanPlay;//可玩
        }
        else
            status = StageStatus.CanPlay;

        UserStageRecord record = GetUserStageRecord(stageId);
        if (record != null && record.Status == (int)StageRecordStatus.Success)
            status = StageStatus.Compelet;

        return status;

    }
	/// <summary>
	/// 获取关卡记录
	/// </summary>
	/// <param name="stage"></param>
	/// <returns></returns>
	public UserStageRecord GetUserStageRecord (uint stage)
	{
		return mPlayerStageRecord_.GetUserStageRecord (stage);
	}

	/// <summary>
	/// 获取所有关卡记录
	/// </summary>
	/// <returns></returns>
	public Dictionary<string,UserStageRecord> GetAllUserStageRecord ()
	{
		return mPlayerStageRecord_.GetUserAllStageRecord ();
	}

	/// <summary>
	/// 设置用户关卡记录
	/// </summary>
	/// <param name="stage"></param>
	/// <param name="status"></param>
	/// <param name="score"></param>
	/// <param name="condid1">条件id</param>
	/// <param name="condval1">条件1实际的值</param>
	/// <param name="condid3">条件id</param>
	/// <param name="condval3">条件3实际的值</param>
	/// <param name="cond2">条件2实际的值</param>
	public void SetUserRecord (uint stage, int status, int score, uint condid1, uint condval1,
        uint condid3, uint condval3, params uint[] cond2)
	{
		mPlayerStageRecord_.SetUserRecord (stage, status, score, condid1, condval1, condid3, condval3, cond2);
	}

    public void SetUserRecord(UserStageRecord record)
    {
        mPlayerStageRecord_.SetUserRecord(record);
    }

    //写关卡记录到本地
    public void SaveUserStageRecord()
    {
        mPlayerStageRecord_.WriteRecord();
    }
	/// <summary>
	/// 获得用户当前总星数
	/// </summary>
	/// <returns></returns>
	public int GetTotalStar ()
	{
		return mPlayerStageRecord_.GetTotalStar ();
	}

	public void Sign ()
	{
		int continueDays = 0;
		if (!string.IsNullOrEmpty (COMMON_FUNC.GetUserSignInfo ()) && !string.IsNullOrEmpty (COMMON_FUNC.GetUserSignInfo ().Split ('#') [0])) {
			continueDays = int.Parse (COMMON_FUNC.GetUserSignInfo ().Split ('#') [0]);
		}
		continueDays++;
		if (continueDays > 7) {
			continueDays = 7;
		}
		string date = System.DateTime.Now.Date.ToShortDateString ();
		COMMON_FUNC.SetUserSignInfo (continueDays.ToString (), date);
	}

	public int GetContinueSignedDays ()
	{
//		Debug.Log ("UserSignInfo"+COMMON_FUNC.GetUserSignInfo ());
//		Debug.Log (System.DateTime.Now.Date);
//		Debug.Log (System.DateTime.Parse (COMMON_FUNC.GetUserSignInfo ().Split ('#') [1]));
//		Debug.Log (System.DateTime.Now.Date-System.DateTime.Parse (COMMON_FUNC.GetUserSignInfo ().Split ('#') [1]));
//		Debug.Log ((System.DateTime.Now.Date - System.DateTime.Parse (COMMON_FUNC.GetUserSignInfo ().Split ('#') [1])).Days);
		if (!string.IsNullOrEmpty (COMMON_FUNC.GetUserSignInfo ()) && !string.IsNullOrEmpty (COMMON_FUNC.GetUserSignInfo ().Split ('#') [0])&& !string.IsNullOrEmpty (COMMON_FUNC.GetUserSignInfo ().Split ('#') [1])) {
			if ((System.DateTime.Now.Date - System.DateTime.Parse (COMMON_FUNC.GetUserSignInfo ().Split ('#') [1])).Days <= 1) {
				return int.Parse (COMMON_FUNC.GetUserSignInfo ().Split ('#') [0]);
			} else {
				ClearSignData ();
				return 0;
			}
		} else {
			return 0;
		}
	}

	public bool IsSignedToday ()
	{
		if (!string.IsNullOrEmpty (COMMON_FUNC.GetUserSignInfo ()) && !string.IsNullOrEmpty (COMMON_FUNC.GetUserSignInfo ().Split ('#') [0])&& !string.IsNullOrEmpty (COMMON_FUNC.GetUserSignInfo ().Split ('#') [1])) {
			//Debug.Log(COMMON_FUNC.GetUserSignInfo ().Split ('#') [1]);
			return System.DateTime.Parse (COMMON_FUNC.GetUserSignInfo ().Split ('#') [1]) == System.DateTime.Now.Date ? true : false;
		} else {
			return false;
		}
	}

	void ClearSignData ()
	{
		COMMON_FUNC.SetUserSignInfo ("0", "");//System.DateTime.Now.Date.ToShortDateString ());
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    public bool hasAndroidPass()
    {
        Debug.Log("Property:" + Property + " \tmStopOutCount:" + mStopOutCount);
        if(Property == PropertyMgr.PROPERTY_STOPOUT &&  mStopOutCount < 1)
        {
            return true;
        }

        return false;
    }

    //-----------------

    protected ushort mCurrentCallUser;//当前叫分玩家
    protected ushort mBlankTrenchUser;//黑挖用户

    public virtual void ActionCallScore()
    {
        
    }
 


    
    
}
