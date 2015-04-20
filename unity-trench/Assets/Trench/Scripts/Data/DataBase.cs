using UnityEngine;
using System.Collections;
using UnityEngineEx.LogInterface;

public class DataBase : SingleClass<DataBase>
{
	bool mIsFinish = false;

	public bool IsComplete ()
	{
		return mIsFinish;
	}
	//private static readonly string TAG = COMMON_CONST.database_name;

	Player player = null;                    //自己玩家
	public Player PLAYER {
		get {
			if (player == null) {
				player = new Player ();     
                
			}
			return player;
		}
	}

	SceneMgr mSceneMgr = null;

	public SceneMgr SCENE_MGR {
		get {
			if (mSceneMgr == null)
				mSceneMgr = new SceneMgr ();
			return mSceneMgr;
		}
	}

	StageMgr mStageMgr = null;

	public StageMgr STAGE_MGR {
		get {
			if (mStageMgr == null)
				mStageMgr = new StageMgr ();
			return mStageMgr; 
		}
	}

	RoomMgr mRoomMgr = null;

	public RoomMgr ROOM_MGR {
		get {
			if (mRoomMgr == null)
				mRoomMgr = new RoomMgr ();
			return mRoomMgr; 
		}
	}

	PropertyMgr mPropertyMgr = null;
	public PropertyMgr PROP_MGR {
		get {
			if (mPropertyMgr == null)
				mPropertyMgr = new PropertyMgr ();
			return mPropertyMgr;
		}
	}

	SignMgr mSignMgr = null;

	public SignMgr SIGN_MGR {
		get {
			if (mSignMgr == null)
				mSignMgr = new SignMgr ();
			return mSignMgr;
		}
	}

	StageLotteryMgr mLotteryMgr = null;

	public StageLotteryMgr LOTTERY_MGR {
		get {
			if (mLotteryMgr == null)
				mLotteryMgr = new StageLotteryMgr ();
			return mLotteryMgr;
		}
	}

	//-----------------------------------------------------------------------------
	//游戏版本
	public COMMON_DEF.enVerDef mVerDef { get; private set; }

	public bool IsOffline ()
	{
		return mVerDef == COMMON_DEF.enVerDef.Offline;
	}

	public void SetVerDef (COMMON_DEF.enVerDef ver)
	{
		mVerDef = ver;
	}

	int RoomType = COMMON_CONST.RoomNormal;

	public INFO_ROOM CurRoom { get; private set; }                                                                          //当前进入的房间
	public INFO_STAGE CurStage { get; private set; }                                                                        //当前关卡
	public bool IsStageRoom { get { return RoomType == COMMON_CONST.RoomStage; } }                                         //是关卡房间
	public bool IsNormalRoom { get { return RoomType == COMMON_CONST.RoomNormal; } }                                        //是普通房间

	/// <summary>
	/// 开始传统游戏
	/// </summary>
	/// <param name="roomid">房间ID</param>
	public void EnterNomalRoom (ushort roomid)
	{
		RoomType = COMMON_CONST.RoomNormal;
		CurRoom = ROOM_MGR.SearchRoomById (roomid);
		SCENE_MGR.SetScene (SceneMgr.SC_Game);
	}

	/// <summary>
	/// 开始关卡游戏
	/// </summary>
	/// <param name="roomid">房间ID</param>
	public void EnterStageRoom (uint stageid)
	{
		RoomType = COMMON_CONST.RoomStage;
		CurStage = STAGE_MGR.GetStage (stageid);
		SCENE_MGR.SetScene (SceneMgr.SC_Game);
	}


	// Use this for initialization
	void Start ()
	{
		Debuger.Instance.LogEnable = true;        
		Debuger.Instance.Init (Debug.Log, Debug.LogWarning, Debug.LogError);         //设置日志回调

		SetVerDef (COMMON_DEF.enVerDef.Offline);//网络版如果上了自己实现

		StartCoroutine (LoadFromLocal ());
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Application.platform == RuntimePlatform.Android) {
#if UNITY_ANDROID
			if(Input.GetKeyUp(KeyCode.Escape))
			{
				try
				{
					AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
					AndroidJavaObject MainActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
					MainActivity.Call("onBack");
				}
				catch(System.Exception ex)
				{
					//sDeviceManager.Instance.ShowToast(ex.Message, true);
					//bPressedCancel = false;
				}
			}  
#endif
		}
	}

	/// <summary>
	/// 加载数据从本地存储
	/// </summary>
	/// <returns></returns>
	IEnumerator LoadFromLocal ()
	{
		LoadTitleInfo ();
		yield return null;
		LoadUserBaseInfo ();
		yield return null;
		LoadStage ();
		yield return null;
		LoadRoomSetting ();
		yield return null;
		LoadProp ();
		yield return null;
		LoadSign ();
		yield return null;
		LoadLottery ();
		yield return null;
		mIsFinish = true;
	}

	void LoadTitleInfo ()
	{
		PLAYER.LoadTitles ();
	}

	void LoadUserBaseInfo ()
	{
		PLAYER.Load ();
	}

	void LoadStage ()
	{
		STAGE_MGR.LoadAll ();
	}

	void LoadRoomSetting ()
	{
		ROOM_MGR.Load ();
	}

	void LoadProp ()
	{
		PROP_MGR.Load ();
	}

	void LoadSign ()
	{
		SIGN_MGR.Load ();
	}

	void LoadLottery ()
	{
		LOTTERY_MGR.Load ();
	}

	void ExitGame ()
	{
		Application.Quit ();
	}

	void OnCancel ()
	{

	}
}
