using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngineEx.CMD.i3778;
using UnityEngineEx.LogInterface;
using UnityEngineEx.MThread;
using UnityEngineEx.Common;

using BYTE = System.Byte;
using DWORD = System.UInt32;
using WORD = System.UInt16;


/// <summary>
/// 游戏辅助
/// </summary>
public class GameHelper : SingleClass<GameHelper>
{
    GlobalService.tagGameServiceOption mGameServercieOption = new GlobalService.tagGameServiceOption();
    public GlobalService.tagGameServiceOption GameServerOption { get { return mGameServercieOption; } }
    /// <summary>
    /// 玩家手牌
    /// </summary>
    public Dictionary<ushort, List<byte>> PlayerHandCard { get; private set; }

    //同桌用户
    public Player[] mPlayers = new Player[CMD_Trench.GAME_PLAYER];

    //当前玩家
    public ushort mCurrentChair { get; private set; }
    //黑挖玩家
    public ushort mBlendTrenchChair { get; private set; }
    //首叫用户和黑挖用户
    public ushort mFirstUser { get; private set; }
    /// <summary>
    /// 坑主用户
    /// </summary>
    public ushort mBankerUser { get; private set; }

    //本轮最大者
    public ushort mTurnWiner { get; private set; }
    public ushort OutCardUser { get { return mTurnWiner; } }

    public byte[] mBackCard; // 底牌

    List<byte> mTurnOutCard = new List<byte>(); //用户已出的牌
    public List<byte> TurnOutCard { get { return mTurnOutCard; } }

    int mGameStatus;        //游戏状态
    public int GameStatus { get { return mGameStatus; } }

    public byte[] mTrenchScore = new byte[CMD_Trench.GAME_PLAYER];

    byte mCallScore;    //叫分

    byte[] mOutCardCount = new byte[CMD_Trench.GAME_PLAYER];         //出牌次数

    public int mBombTime;  //炸弹的倍数

    bool[] mUserTrustee = new bool[CMD_Trench.GAME_PLAYER];     //用户托管

    AndroidUserMgr mAndroidUserMgr = new AndroidUserMgr();
    public AndroidUserMgr ANDROID_USER_MGR { get { return mAndroidUserMgr; } }

    //是否启用炸弹
    public bool IsBomb { get; private set; }
    //是否启用黑挖
    public bool IsBlendTrench { get; private set; }

    /// <summary>
    /// 房间倍率
    /// </summary>
    public int lCellScore { get; private set; }


    //提示出牌变量
    bool m_bAutoSucceed;
    CMD_C_OutCard m_AutoOutCard = new CMD_C_OutCard();
    int m_nPromptPos;						//自动出牌位置 wjt 2007-07-20	
    int m_nPromptType;						//提示类型	
    int m_nPromptCount;						//循环提示次数 -1一次循环提示完毕,0第一次循环，1第二次循环,2第三次循环.	
    bool m_bPromptSelected;					//手中有选择的牌
    byte[] m_bPrompted = new byte[CMD_Trench.ZHUANG_CARD_NUM];					//已经提示过的牌
    int m_nPromptedCount;					//已经提示过的牌的个数

    //public byte[] mPlayerStatus;            

    //-----------------------------------------------------------------


    //-----------------------------------------------------------------

    /// <summary>
    /// 游戏逻辑
    /// </summary>
    public GameLogic mGameLogic;

    //-----------------------------------------------------------------------------------------------------
    //游戏接口
    //-----------------------------------------------------------------------------------------------------
    IGameSink[] GameSink = new IGameSink[CMD_Trench.GAME_PLAYER];

    public GameHelper()
    {
        mGameLogic = new GameLogic();                                                   //游戏逻辑
        //PlayerHandCard = new byte[CMD_Trench.GAME_PLAYER, GLogicDef.MAX_COUNT];       //玩家手牌
        PlayerHandCard = new Dictionary<ushort, List<byte>>();                          //玩家手牌
        mBackCard = new byte[GLogicDef.BACK_COUNT];                                      //底牌
        //mPlayerStatus = new byte[CMD_Trench.GAME_PLAYER];

        mCurrentChair = unchecked((ushort)GlobalDef.Deinfe.INVALID_CHAIR);
        mBlendTrenchChair = unchecked((ushort)GlobalDef.Deinfe.INVALID_CHAIR);

        for (ushort i = 0; i < CMD_Trench.GAME_PLAYER; i++)
        {
            PlayerHandCard[i] = new List<byte>();
        }

        mGameServercieOption.szGameRoomName = new char[GlobalDef.SERVER_LEN];
        mGameServercieOption.lCellScore = 1;
        mGameServercieOption.cbRevenue = 0;

        ResetData();

        GameSink.ArraySetAll(null);

        //////////////////////////////////////////////////////////////////////////////////
        MThreadPool ThreadPool = MThreadPool.Instance;
    }

    void ResetData()
    {
        mBombTime = 0;
        mCallScore = 0;
        mGameStatus = CMD_Trench.GS_WK_FREE;
        mTurnWiner = (ushort)GlobalDef.Deinfe.INVALID_CHAIR;
        mBankerUser = (ushort)GlobalDef.Deinfe.INVALID_CHAIR;
        mBlendTrenchChair = (ushort)GlobalDef.Deinfe.INVALID_CHAIR;
        mTurnOutCard.Clear();

       

        //GameSink.ArraySetAll(null);
        mOutCardCount.ArraySetAll((byte)0);
        mUserTrustee.ArraySetAll(false);

        mTrenchScore.ArraySetAll((byte)0);

        //设置用户状态
        for (ushort i = 0; i < CMD_Trench.GAME_PLAYER; i++)
        {
            SetUserStatus(i, GlobalDef.enUserStatus.US_SIT);
        }

        if(DataBase.Instance.IsStageRoom)
        {
             //炸弹设置
            IsBomb = true;
            //黑挖设置
            IsBlendTrench = false;
            lCellScore = (int)DataBase.Instance.CurStage.RoomRate;
        }
        else
        {
            IsBomb = DataBase.Instance.ROOM_MGR.GetRoomType(DataBase.Instance.CurRoom) == enRoomType.enBomb ? true : false;
            IsBlendTrench = DataBase.Instance.CurRoom.bTrench;
            lCellScore = DataBase.Instance.CurRoom.lCellScore;
        }

        mGameLogic.ResetData();
        mGameLogic.SetBomb(IsBomb);
        
    }    

    /// <summary>
    /// 黑挖开始
    /// </summary>
    public void SetTrenchStart()
    {
        mCallScore = 4;//叫分4分
        mBlendTrenchChair = mBankerUser = 1;//坑主

        InitCardHeap();
        GetMasterDesk();


    }

    void SetGameStatus(int gameStatus)
    {
        mGameStatus = gameStatus;
    }

    /// <summary>
    /// 初始化牌堆，洗牌
    /// </summary>
    void InitCardHeap()
    {
        byte[] cardlist = new byte[GLogicDef.FULL_COUNT];
        GLogicDef.CardData.CopyTo(cardlist, 0);

        for (ushort i = 0; i < CMD_Trench.GAME_PLAYER; i++)
        {
            PlayerHandCard[i].Clear();
        }

        //--------------------------------------------------------

        RandomCardList(cardlist);       
        
        //--------------------------------------------------------
        
        //排序扑克
        foreach (var e in PlayerHandCard)
        {
            mGameLogic.SortCardList(e.Value,(byte)e.Value.Count);
            mGameLogic.SetUserCard(e.Key, e.Value);
        }
    }


    private void RandomCardList(byte[] cardlist)
    {
        //关卡配牌逻辑
        bool result = GameLogicUtil.AddStageCondCard(mGameLogic,1);

        if (result)
        {
            byte cbHandCount = 0;
            mGameLogic.GetConfigCard(cardlist, ref cbHandCount);
            for (ushort j = 0; j < CMD_Trench.GAME_PLAYER; j++)
            {
                byte[] playhand = new byte[16];
                System.Array.Copy(cardlist, j * 16, playhand, 0, 16);
                PlayerHandCard[j].AddRange(playhand);
            }
        }
        else
        {
            cardlist.Shuffle();
            //发牌到玩家
            for (int i = 0; i < GLogicDef.FULL_COUNT - GLogicDef.BACK_COUNT; i += CMD_Trench.GAME_PLAYER)
            {
                for (ushort j = 0; j < CMD_Trench.GAME_PLAYER; j++)
                {
                    PlayerHandCard[j].Add(cardlist[i + j]);
                }
            }
        }
        

        //if(DataBase.Instance.PLAYER.Property == PropertyMgr.PROPERTY_BOM)
        //{
           // PropertyBomRandomCard(cardlist);
        //}             

        //拉出底牌
        System.Array.Copy(cardlist, GLogicDef.FULL_COUNT - GLogicDef.BACK_COUNT, mBackCard, 0, mBackCard.Length);

        
    }

    public void PropertyBomRandomCard(byte[] cardlist)
    {
        byte randBom        = (byte)Random.Range(0x4, 0xC);
        byte category       = 0;
        int offsetindex     = 1;
        Debug.Log("bom" + randBom);
        
        for (int i = 0; i < 4; i++ , category++)
        {
            byte bomtemp = (byte)((category << 4) | randBom);

           
            for (int j = 0; j < cardlist.Length; j++ )
            {
                if (cardlist[j] == bomtemp)
                {
                    byte temp               = cardlist[offsetindex];
                    cardlist[offsetindex]   = cardlist[j];
                    cardlist[j]             = temp;

                    offsetindex             += 3;

                    break;
                }
            }
        }
    }


    /// <summary>
    /// 坐下动作
    /// </summary>
    /// <param name="wTable"></param>
    /// <param name="usChair"></param>
    /// <param name="player"></param>
    /// <param name="pSink"></param>
    public void PerformSitDownAction(ushort usChair, Player player, IGameSink pSink)
    {
        ushort wTable = 0;

        if (usChair == (ushort)GlobalDef.Deinfe.INVALID_CHAIR)
        {
            Debuger.Instance.LogError("错误的椅子号");
            return;
        }

        mPlayers[usChair] = player;
        GameSink[usChair] = pSink;

        //更新用户状态,桌子和椅子号
        player.SetUserStatus(GlobalDef.enUserStatus.US_SIT, wTable, usChair);

        //金币游戏设置积分为金币
        player.GameScore = (int)player.mUser.dwGoldCount;

        SendUserStatus(player);
    }

    /// <summary>
    /// 离开动作
    /// </summary>
    /// <param name="player"></param>
    public void PerformStandUpAction(Player player)
    {
        //单机版直接结束游戏，并且扣去积分
        EndGame(player.mUser.wChairID,(int)GameExport.Define.GER_USER_LEFT);
        
        UnityEngine.Object.Destroy(this.gameObject);
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="wChair">-1为全桌发送</param>
    /// <param name="msgobj">消息对象</param>
    public void SendGameData(ushort wChair, ushort cmd, object msgobj)
    {
        GlobalDef.CMD_Head CMDHead = new GlobalDef.CMD_Head();
        CMDHead.CommandInfo.wMainCmdID = GlobalFrame.MDM_GF_GAME;
        CMDHead.CommandInfo.wSubCmdID = (ushort)cmd;

        if (COMMON_FUNC.IsInvalidChair(wChair))
        {
            for (int i = 0; i < CMD_Trench.GAME_PLAYER; i++)
            {
                GameSink[i].OnGameMessage(CMDHead, msgobj);
            }
        }
        else
        {
            GameSink[wChair].OnGameMessage(CMDHead, msgobj);
        }
    }

    public void SendFrameData(ushort wChair, ushort cmdMain, ushort cmd, object msgobj)
    {
        GlobalDef.CMD_Head CMDHead = new GlobalDef.CMD_Head();
        CMDHead.CommandInfo.wMainCmdID = cmdMain;
        CMDHead.CommandInfo.wSubCmdID = cmd;

        if (COMMON_FUNC.IsInvalidChair(wChair))
        {
            for (int i = 0; i < CMD_Trench.GAME_PLAYER; i++)
            {
                if (GameSink[i] != null) GameSink[i].OnGameMessage(CMDHead, msgobj);
            }
        }
        else
        {
            if (GameSink[wChair] != null) GameSink[wChair].OnGameMessage(CMDHead, msgobj);
        }
    }

    bool SendUserStatus(Player p)
    {
        //效验参数    
        //if (p.mUser.cbUserStatus != (byte)GlobalDef.enUserStatus.US_READY) return false;

        ////变量定义
        CMD_Game.CMD_GR_UserStatus UserStatus = new CMD_Game.CMD_GR_UserStatus();
        //memset(&UserStatus, 0, sizeof(UserStatus));
        //tagServerUserData* pUserData = pIServerUserItem->GetUserData();
        GlobalDef.tagUserData UserDta = p.mUser;

        //构造数据
        UserStatus.dwUserID = UserDta.dwUserID;
        UserStatus.wTableID = UserDta.wTableID;
        UserStatus.wChairID = UserDta.wChairID;
        UserStatus.cbUserStatus = UserDta.cbUserStatus;


        ////发送数据
        SendFrameData((ushort)GlobalDef.Deinfe.INVALID_CHAIR, CMD_Game.MDM_GR_USER, CMD_Game.SUB_GR_USER_STATUS, UserStatus);
        //m_pITCPSocketEngine->SendDataBatch(MDM_GR_USER, SUB_GR_USER_STATUS, &UserStatus, sizeof(UserStatus));
        //m_AndroidUserManager.SendDataToClient(MDM_GR_USER, SUB_GR_USER_STATUS, &UserStatus, sizeof(UserStatus));

        return true;
    }

    //切换椅子
    ushort SwitchViewChairID(ushort wChairID)
    {
        //效验状态
        //if (m_ClientKernelHelper.GetInterface()==NULL) return INVALID_CHAIR;

        //变量定义
        //const tagUserData * pMeUserData=m_ClientKernelHelper->GetMeUserInfo();
        //const tagServerAttribute * pServerAttribute=m_ClientKernelHelper->GetServerAttribute();
        GlobalDef.tagUserData MeUser = DataBase.Instance.PLAYER.mUser;

        //转换椅子
        ushort wViewChairID = (ushort)(wChairID + CMD_Trench.CHAIR_COUNT - MeUser.wChairID);
        switch (CMD_Trench.CHAIR_COUNT)
        {
            case 2: { wViewChairID += 1; break; }
            case 3: { wViewChairID += 1; break; }
            case 4: { wViewChairID += 2; break; }
            case 5: { wViewChairID += 2; break; }
            case 6: { wViewChairID += 3; break; }
            case 7: { wViewChairID += 3; break; }
            case 8: { wViewChairID += 4; break; }
        }
        return (ushort)(wViewChairID % CMD_Trench.CHAIR_COUNT);
    }

    //获得主叫牌人员
    void GetMasterDesk()
    {
        /*
        叫牌规则
        玩家可以根据自己手中牌的强弱按逆时针顺序叫分，可叫1分、2分、3分；抓到红心4者先声明手中有红心4，
        然后叫分，均无红心4时，以持有红心5者开始，以此类推；三家都不叫时，系统则默认有红心4者（或红心5）挖1分的坑。
        */

        //有关卡任务时，按关卡任务走
        INFO_STAGE_COND_LINK cond =
                DataBase.Instance.STAGE_MGR.GetStageCondById(StageCond.COND_THETYPE);
        if (cond != null)
        {
            if (cond.Condition == COMMON_CONST.RoleLand)
            {
                mCurrentChair = 1;
                mFirstUser = 1;
                return;
            }
            else {
                mCurrentChair = 2;
                mFirstUser = 2;
                return;
            }

        }
        for (int k = CMD_Trench.RED_4; k <= (CMD_Trench.RED_4 + 4); k++)
        {
            for (ushort i = 0; i < CMD_Trench.GAME_PLAYER; i++)
            {
                for (int j = 0; j < PlayerHandCard[i].Count; j++)
                {
                    if (PlayerHandCard[i][j] == k)
                    {
                        mCurrentChair = i;
                        mFirstUser = i;
                        return;
                    }
                }
            }
        }
    }

    //判断是否有大于上家的牌
    bool IsOutCard(ushort wOutChair)
    {
        //分析结果
        List<tagAnalyseCardTypeResult> follow = new List<tagAnalyseCardTypeResult>();
        mGameLogic.CommFollowCard(wOutChair,mTurnOutCard.ToArray(), (byte)mTurnOutCard.Count,follow,false) ;
        

        //设置界面
        if (follow.Count > 0)
        {
            //设置控件
            //bool bOutCard=VerdictOutCard();
            //m_GameClientView.m_btOutCard.EnableWindow((bOutCard==true)?TRUE:FALSE);
        }
        else
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    void StartGame()
    {
        mGameStatus = CMD_Trench.GS_WK_SCORE;
        for (ushort i = 0; i < CMD_Trench.GAME_PLAYER; i++)
        {
            mPlayers[i].SetUserStatus(GlobalDef.enUserStatus.US_PLAY,mPlayers[i].mUser.wTableID,mPlayers[i].mUser.wChairID);

            if (GameSink != null)
            {
                //游戏开始
                CMD_S_SendCard cmd      = new CMD_S_SendCard();
                //CMD_S_SendAllCard cmd1 = new CMD_S_SendAllCard();
                cmd.wBlankTrenchUser    = mBlendTrenchChair;
                cmd.wCurrentUser        = mCurrentChair;
                cmd.bCardData           = new byte[CMD_Trench.ZHUANG_CARD_NUM];

                System.Array.Copy(PlayerHandCard[i].ToArray(), cmd.bCardData, PlayerHandCard[i].Count);

                //发送扑克
                SendGameData(i, enCmdTrench.SUB_S_SEND_CARD, cmd);
            }
        }
    }


    
    /// <summary>
    /// 黑挖叫分
    /// </summary>
    public void BlackCallScore()
    {
        ushort usChairID = 1;
        mPlayers[usChairID].SetUserStatus(GlobalDef.enUserStatus.US_READY, mPlayers[usChairID].mUser.wTableID, usChairID);
        SendUserStatus(mPlayers[usChairID]);

        mPlayers[usChairID].HasBlackCall = true;

        //判断游戏开始
        bool bGameStart = true;
        for (int i = 0; i < CMD_Trench.GAME_PLAYER; i++)
        {
            if (mPlayers[i].mUser.cbUserStatus < (byte)GlobalDef.enUserStatus.US_READY)
            {
                bGameStart = false;
                break;
            }
        }

        if (bGameStart)
        {
            //初始化牌堆
            InitCardHeap();

            //获取主叫
            //GetMasterDesk();
            mBlendTrenchChair   = usChairID;  //黑挖用户
            mCurrentChair       = usChairID;  //当前座位
            mBankerUser         = usChairID;  //坑主
            //mCallScore          = 4;

            //游戏开始
            StartGame();
        }
    }

    /// <summary>
    /// 黑挖开始
    /// </summary>
    public void BlackScoreStart()
    {
        CMD_S_LandScore LandScore = new CMD_S_LandScore();
        LandScore.bLandUser     = mCurrentChair;
        LandScore.bLandScore    = mCallScore = 4;
        LandScore.wCurrentUser  = (ushort)GlobalDef.Deinfe.INVALID_CHAIR;
        LandScore.bCurrentScore = mCallScore;
        SendGameData((ushort)GlobalDef.Deinfe.INVALID_CHAIR, enCmdTrench.SUB_S_LAND_SCORE, LandScore);

        //------------------------------------------------

        SetGameStatus(CMD_Trench.GS_WK_PLAYING);       

        CMD_S_GameStart GameStart = new CMD_S_GameStart();
        GameStart.wLandUser     = mBankerUser;
        GameStart.bLandScore    = mCallScore;
        GameStart.wCurrentUser  = mCurrentChair;
        GameStart.bBackCard     = new byte[CMD_Trench.LEFT_CARD_NUM];
        mBackCard.CopyTo(GameStart.bBackCard, 0);
        GameStart.bMeCardData = new byte[GLogicDef.MAX_COUNT];

        for (ushort i = 0; i < CMD_Trench.GAME_PLAYER; i++)
        {

            if(mPlayers[i].HasBlackCall)
            {
                PlayerHandCard[i].AddRange(mBackCard);
                mGameLogic.SortCardList(PlayerHandCard[i], (byte)PlayerHandCard[i].Count);
            }

            PlayerHandCard[i].CopyTo(GameStart.bMeCardData);           
            GameStart.lMeCardCount = (i == mBankerUser) ? CMD_Trench.ZHUANG_CARD_NUM : CMD_Trench.ONE_USER_GET_CARD_NUM;           
            SendGameData(i, enCmdTrench.SUB_S_GAME_START, GameStart);
        }
    }

    void SetUserStatus(ushort wChair, GlobalDef.enUserStatus status)
    {
        if (COMMON_FUNC.IsInvalidChair(wChair))
        {
            Debuger.Instance.LogError(COMMON_CONST.ErrChair);
            return;
        }
        if (mPlayers[wChair] != null)
            mPlayers[wChair].SetUserStatus(status, mPlayers[wChair].mUser.wTableID, wChair);
    }

    void EndGame(ushort wChair, int reason)
    {
        SetGameStatus(CMD_Trench.GS_WK_FREE);

        switch ((GameExport.Define)reason)
        {
            case GameExport.Define.GER_NORMAL:		//常规结束
                {
                    OnNormalEndGame(wChair);
                    break;
                }
            case GameExport.Define.GER_USER_LEFT:   //用户离开
                {
                    break;
                }
        }


    }

    void OnNormalEndGame(ushort wChair)
    {
        int lTax = 0;
        //定义变量	
        CMD_S_GameEnd GameEnd = new CMD_S_GameEnd();
        GameEnd.lGameTax = lTax;
        GameEnd.lGameScore = new int[CMD_Trench.GAME_PLAYER];
        GameEnd.lGameHighLadderScore = new int[CMD_Trench.GAME_PLAYER];
        GameEnd.lGold = new int[CMD_Trench.GAME_PLAYER];
        GameEnd.bGameResult = new bool[CMD_Trench.GAME_PLAYER];
        GameEnd.bCardCount = new byte[CMD_Trench.GAME_PLAYER];
        GameEnd.bCardData = new byte[CMD_Trench.ALL_CARD_NUM];

        GameExport.enScoreKind[] ScoreKind = new GameExport.enScoreKind[CMD_Trench.GAME_PLAYER];


        int lSystemScore = 0;

        //剩余扑克
        byte bCardPos = 0;
        for (ushort i = 0; i < CMD_Trench.GAME_PLAYER; i++)
        {
            GameEnd.bCardCount[i] = (byte)PlayerHandCard[i].Count;
            System.Array.Copy(PlayerHandCard[i].ToArray(), 0, GameEnd.bCardData, bCardPos, PlayerHandCard[i].Count);
            bCardPos += (byte)PlayerHandCard[i].Count;
        }

        //变量定义
        //int lCellScore = DataBase.Instance.CurRoom.lCellScore;
        bool bLandWin = (PlayerHandCard[mBankerUser].Count == 0) ? true : false; //坑主赢?

        //////////////////////////////////////////////////////////////////////////

        //炸弹限制
        //mBombTime = Mathf.Min(mBombTime, 4);
        //if(m_pGameServiceOption->wKindID!=291 && m_pGameServiceOption->wServerID!=53)//配置欢乐炸弹场的时候，wKindID换成对应房间的ID
        //{
        //	m_wBombTime = __min( m_wBombTime, 4 );
        //}
        //m_wBombTime = __min( m_wBombTime, 4 );注释掉，去掉炸弹限制，del by guopeng

        //---------------------------------------------------------------------------------------
        //统计积分
        ClearScore( GameEnd.lGameScore );

        //------------------------------------------------------------
        for (ushort i = 0; i < CMD_Trench.GAME_PLAYER; i++)
        {
            //IServerUserItem * pCurUserItem = m_pITableFrame->GetServerUserItem(i);
            //if(pCurUserItem && pCurUserItem->IsAndroidUser() && pCurUserItem->GetSysUserType() ==SysUserType_AndroidTimerControl) lSystemScore += GameEnd.lGameScore[i];

            ////双倍道具分数
            //if (m_pGameServiceOption->wServerType&GAME_GENRE_SCORE)
            //{
            //    bool bWin=(GameEnd.lGameScore[i]>0L)?enScoreKind_Win:enScoreKind_Lost;
            //    if(enScoreKind_Win==bWin)
            //    {
            //        GameEnd.lGameScore[i]=m_pITableFrame->UserScoreProperty(i,GameEnd.lGameScore[i],enScoreKind_Win);
            //    }
            //    else
            //    {
            //        GameEnd.lGameScore[i]=m_pITableFrame->UserScoreProperty(i,GameEnd.lGameScore[i],enScoreKind_Lost);
            //    }
            //}	

            GameEnd.bGameResult[i] = (mBankerUser == i ? bLandWin : !bLandWin);
            ScoreKind[i] = GameEnd.bGameResult[i] ? GameExport.enScoreKind.enScoreKind_Win : GameExport.enScoreKind.enScoreKind_Lost;

            //保分卡
            GameEnd.bNullity = new int[CMD_Trench.GAME_PLAYER];
            GameEnd.bNullity[i] = 0;

            switch(GetStage())
            {
                case STAGE_BOM:
                case STAGE_NORMAL:
                    //记录成绩
                    mPlayers[i].WriteUserGold(i, GameEnd.lGameScore[i], ScoreKind[i]);
                    break;
                case STAGE_GATE:
                    //记录成绩
                    //mPlayers[i].WriteUserScore(i, GameEnd.lGameScore[i], ScoreKind[i]);
                    break;
            }           
        }

        //重置状态
        for (int i = 0; i < CMD_Trench.GAME_PLAYER; i++)
        {
            GameSink[i].ResetData();
        }
        ResetData();


        //发送信息
        //m_pITableFrame->SendTableData(INVALID_CHAIR, SUB_S_GAME_END, &GameEnd, sizeof(GameEnd));
        //m_pITableFrame->SendLookonData(INVALID_CHAIR, SUB_S_GAME_END, &GameEnd, sizeof(GameEnd));
        SendGameData((ushort)GlobalDef.Deinfe.INVALID_CHAIR, enCmdTrench.SUB_S_GAME_END, GameEnd);

        //切换用户
        mFirstUser = wChair;


        //结束游戏
        //m_pITableFrame->ConcludeGame();

        //return true;
    }

    /// <summary>
    /// 游戏结算
    /// </summary>
    /// <param name="outAllScore"></param>
    private void ClearScore(int[] outAllScore)
    {
        int iHasMasterWin = (PlayerHandCard[mBankerUser].Count == 0) ? 1 : 0; //坑主赢?
        int lCellScore = 0;
        if (DataBase.Instance.CurRoom!=null)
            lCellScore = DataBase.Instance.CurRoom.lCellScore;
        else
            lCellScore = (int)DataBase.Instance.PLAYER.GetCurrStage().RoomRate;
        switch(GetStage())
        {
            case STAGE_BOM:     //
            case STAGE_GATE:    //
                EconomicSystem.RulerClearBomGame(
                    new int[]
                    { 
                            iHasMasterWin,                  //是否坑主赢
                            mBankerUser,                    //坑主
                            mCallScore,                     //叫分
                            mBombTime,                      //炸弹个数
                            lCellScore,                     //底分
                            GameServerOption.cbRevenue      //税收
                     }, 
                     outAllScore);

                PropertyScore2(iHasMasterWin, outAllScore); //道具分数翻倍

                break;
            case STAGE_NORMAL:
                EconomicSystem.RulerClearNormal(
                    new int[]
                    { 
                            iHasMasterWin,                  //是否坑主赢
                            mBankerUser,                    //坑主
                            mCallScore,                     //叫分                        
                            lCellScore,                              //底分
                            GameServerOption.cbRevenue      //税收
                    },
                    outAllScore);
                break;
        }       

        
    }

    /// <summary>
    /// 道具 分数翻倍
    /// </summary>
    /// <param name="hasMasertWin"></param>
    /// <param name="outAllScore"></param>
    private void PropertyScore2(int hasMasertWin, int[] outAllScore)
    {
        if(GetStage() != STAGE_GATE )
        {
            return;
        }
        if(DataBase.Instance.PLAYER.Property != PropertyMgr.PROPERTY_SCORE2)
        {
            return;
        }

        //-----------------------------------------------

        int playerIndex = 1;

        if (hasMasertWin == playerIndex && mBankerUser == playerIndex)
        {
            // 玩家获胜
            outAllScore[playerIndex] *= 2;
            Debug.Log("玩家道具 Score * 2");
        }

    }


    public const int STAGE_NORMAL = 0;
    public const int STAGE_BOM = 1;
    public const int STAGE_GATE = 2;

	
    public int GetStage()
    {
        if (DataBase.Instance.IsNormalRoom && DataBase.Instance.ROOM_MGR.GetRoomType(DataBase.Instance.CurRoom) == enRoomType.enBomb)
        {     //普通场
            Debug.Log("炸弹场");
            return STAGE_BOM;
        }
        else if (DataBase.Instance.IsStageRoom)
        {
            Debug.Log("关卡");
            return STAGE_GATE;
        }

        Debug.Log("普通场");
        return STAGE_NORMAL;
    }

    void OnLeaveEndGame(ushort wChairID)
    {
        //效验参数
        //ASSERT(pIServerUserItem != NULL);
        //ASSERT(wChairID < m_wPlayerCount);

        //如果是关卡直接失败，请实现
        if (DataBase.Instance.IsStageRoom)
        {
            return;
        }

        //构造数据
        CMD_S_GameEnd GameEnd = new CMD_S_GameEnd();        
        //memset(&GameEnd, 0, sizeof(GameEnd));

        //int lCellScore = DataBase.Instance.CurRoom.lCellScore;

        if (mBankerUser == (ushort)GlobalDef.Deinfe.INVALID_CHAIR)
        {
            GameEnd.lGameScore[wChairID] = -3 * lCellScore;
        }
        else
        {
            if (mBankerUser == wChairID)
                GameEnd.lGameScore[wChairID] = -4 * lCellScore * mCallScore;
            else
                GameEnd.lGameScore[wChairID] = -2 * lCellScore * mCallScore;
        }

        //剩余扑克
        BYTE bCardPos = 0;
        for (WORD i = 0; i < CMD_Trench.GAME_PLAYER; i++)
        {
            GameEnd.bCardCount[i] = (byte)PlayerHandCard[i].Count;
            System.Array.Copy(PlayerHandCard[i].ToArray(), 0, GameEnd.bCardData, bCardPos, PlayerHandCard[i].Count);
            bCardPos += (byte)PlayerHandCard[i].Count;
            //保分卡
            GameEnd.bNullity[i] = 0;
        }

        //修改积分
        mPlayers[wChairID].WriteUserScore(wChairID, GameEnd.lGameScore[wChairID], GameExport.enScoreKind.enScoreKind_Flee);        

        //发送信息
        SendGameData((ushort)GlobalDef.Deinfe.INVALID_CHAIR, enCmdTrench.SUB_S_GAME_END, GameEnd);
        //m_pITableFrame->SendTableData(INVALID_CHAIR, SUB_S_GAME_END, &GameEnd, sizeof(GameEnd));
        //m_pITableFrame->SendLookonData(INVALID_CHAIR, SUB_S_GAME_END, &GameEnd, sizeof(GameEnd));

        //结束游戏
        //m_pITableFrame->ConcludeGame();

        //return true;
    }

    /// <summary>
    /// 游戏准备
    /// </summary>
    /// <param name="usChairID"></param>
    public void Ready(ushort usChairID)
    {
        if (usChairID == (ushort)GlobalDef.Deinfe.INVALID_CHAIR)
        {
            Debuger.Instance.LogError("错误的椅子号");
            return;
        }

        mPlayers[usChairID].SetUserStatus(GlobalDef.enUserStatus.US_READY, mPlayers[usChairID].mUser.wTableID, usChairID);
        SendUserStatus(mPlayers[usChairID]);

        //SendFrameData((ushort)GlobalDef.Deinfe.INVALID_CHAIR, (ushort)GlobalFrame.SUB_GF_USER_READY, usChairID);

        //判断游戏开始
        bool bGameStart = true;
        for (int i = 0; i < CMD_Trench.GAME_PLAYER; i++)
        {
            if (mPlayers[i].mUser.cbUserStatus < (byte)GlobalDef.enUserStatus.US_READY)
            {
                bGameStart = false;
                break;
            }
        }
        if (bGameStart)
        {
           
            //初始化牌堆
            InitCardHeap();

            //获取主叫
            GetMasterDesk();

            //游戏开始
            StartGame();
        }

    }

    //获得first牌人员
    int GetFirstOutCardDesk()
    {
        for (int k = CMD_Trench.RED_4; k <= (CMD_Trench.RED_4 + 4); k++)
        {
            for (ushort i = 0; i < CMD_Trench.GAME_PLAYER; i++)
            {
                for (ushort j = 0; j < PlayerHandCard[i].Count; j++)
                {
                    if (PlayerHandCard[i][j] == k) return i;
                }
            }
        }

        return -1;
    }
    /// <summary>
    ///  叫分
    /// </summary>
    /// <param name="wChair">座位号</param>
    /// <param name="score">叫几分</param>
    public void CallScore(ushort wChair, byte score)
    {
        //判断座位号
        if (COMMON_FUNC.IsInvalidChair(wChair)) return;

        //判断用户状态
        if (mPlayers[wChair].mUser.cbUserStatus != (byte)GlobalDef.enUserStatus.US_PLAY) return;

        //判断游戏状态
        if (mGameStatus != CMD_Trench.GS_WK_SCORE)
        {
            Debuger.Instance.LogError(COMMON_CONST.ErrCallScore + "[" + mGameStatus.ToString() + "]"
                + " [" + mPlayers[wChair].mUser.cbUserStatus.ToString() + "]");
            return;
        }

        //效验参数
        if (mCurrentChair != wChair)
        {
            Debuger.Instance.LogError(COMMON_CONST.ErrCurChair);
            return;
        }

        //效验参数

        //设置变量
        if (score != 255)
        {
            mCallScore = score;
            mBankerUser = mCurrentChair;
        }

        mTrenchScore[wChair] = score;

        //开始判断
        ushort wChairID = wChair;
        if ((mTrenchScore[wChair] == 3) || (mFirstUser == (wChair + 1) % CMD_Trench.GAME_PLAYER))
        {
            //设置变量
            if (mCallScore == 0) mCallScore = 1;
            if (mBankerUser == (ushort)GlobalDef.Deinfe.INVALID_CHAIR) mBankerUser = mFirstUser;

            mGameLogic.SetBanker(mBankerUser);
            mGameLogic.SetBackCard(mBackCard, (byte)mBackCard.Length);

            //CopyMemory(GameStart.bBackCard, m_bBackCard, sizeof(m_bBackCard));
            //发送消息
            CMD_S_LandScore LandScore = new CMD_S_LandScore();
            LandScore.bLandUser = wChairID;
            LandScore.bLandScore = score;
            LandScore.wCurrentUser = unchecked((ushort)GlobalDef.Deinfe.INVALID_CHAIR);
            LandScore.bCurrentScore = mCallScore;
            SendGameData((ushort)GlobalDef.Deinfe.INVALID_CHAIR, enCmdTrench.SUB_S_LAND_SCORE, LandScore);
            //m_pITableFrame->SendTableData(INVALID_CHAIR, SUB_S_LAND_SCORE, &LandScore, sizeof(LandScore));
            //m_pITableFrame->SendLookonData(INVALID_CHAIR, SUB_S_LAND_SCORE, &LandScore, sizeof(LandScore));

            //设置状态
            SetGameStatus(CMD_Trench.GS_WK_PLAYING);

            int myInt = GetFirstOutCardDesk();

            //发送底牌
            //m_bCardCount[m_wBankerUser] = ZHUANG_CARD_NUM;
            PlayerHandCard[mBankerUser].AddRange(mBackCard);
            //CopyMemory(&m_bHandCardData[m_wBankerUser][ONE_USER_GET_CARD_NUM], m_bBackCard, sizeof(m_bBackCard));
            //m_GameLogic.SortCardList(m_bHandCardData[m_wBankerUser], m_bCardCount[m_wBankerUser], ST_ORDER);
            //byte[] cards = PlayerHandCard[mBankerUser].ToArray();
            //mGameLogic.SortCardList(ref cards, GLogicDef.ST_ORDER);
            //Debuger.Instance.Log( PlayerHandCard[mBankerUser].ToString<byte>() );
            mGameLogic.SortCardList(PlayerHandCard[mBankerUser], (byte)PlayerHandCard[mBankerUser].Count);
            //Debuger.Instance.Log("====\n"+PlayerHandCard[mBankerUser].ToString<byte>());
            //PlayerHandCard[mBankerUser].Clear();
            //PlayerHandCard[mBankerUser].AddRange(cards);

            //出牌信息
            mTurnOutCard.Clear();
            //m_bTurnCardCount = 0;
            mTurnWiner = mBankerUser;

            //合法
            if ((myInt > -1) && (myInt < CMD_Trench.GAME_PLAYER))
            {
                mCurrentChair = (ushort)myInt;
            }
            else
            {
                mCurrentChair = mBankerUser;
            }
            //////////////////////////////////////////////////////////////////////////
            //if (m_pGameServiceOption->wServerType & GAME_GENRE_HILADDER)
            //{
            //    //叫分完成设置天梯信息
            //    m_HiLadder.SetLandUser(m_wBankerUser, m_bLandScore);

            //    //确定叫分地主后设置天梯
            //    for (int i = 0; i < GAME_PLAYER; i++)
            //        m_HiLadder.SetKVal(i, m_bHandCardData[i]);
            //}
            //////////////////////////////////////////////////////////////////////////
            //发送消息
            CMD_S_GameStart GameStart = new CMD_S_GameStart();
            GameStart.wLandUser = mBankerUser;
            GameStart.bLandScore = mCallScore;
            GameStart.wCurrentUser = mCurrentChair;
            GameStart.bBackCard = new byte[CMD_Trench.LEFT_CARD_NUM];
            mBackCard.CopyTo(GameStart.bBackCard, 0);
            GameStart.bMeCardData = new byte[GLogicDef.MAX_COUNT];

            for (ushort i = 0; i < CMD_Trench.GAME_PLAYER; i++)
            {
                PlayerHandCard[i].CopyTo(GameStart.bMeCardData);
                //CopyMemory(GameStart.bMeCardData, m_bHandCardData[i], sizeof(GameStart.bMeCardData));
                GameStart.lMeCardCount = (i == mBankerUser) ? CMD_Trench.ZHUANG_CARD_NUM : CMD_Trench.ONE_USER_GET_CARD_NUM;
                //m_pITableFrame->SendTableData(i, SUB_S_GAME_START, &GameStart, sizeof(GameStart));
                //m_pITableFrame->SendLookonData(i, SUB_S_GAME_START, &GameStart, sizeof(GameStart));
                SendGameData(i, enCmdTrench.SUB_S_GAME_START, GameStart);
            }

            //剩余时间定时器
            //m_pITableFrame->KillGameTimer(IDI_CALL_SCORE_REMAIN_TIME);
            //m_cbCallScoreRemaindTimer = 0;
            //m_cbOutCardRemaindTimer = TIME_OUT_CARD;
            //m_pITableFrame->SetGameTimer(IDI_OUT_CARD_REMAIN_TIME, 1000, TIME_OUT_CARD * 1000, NULL);

            return;
        }

        //设置用户
        mCurrentChair = (ushort)((wChairID + 1) % CMD_Trench.GAME_PLAYER);

        //发送消息
        CMD_S_LandScore LandScore1 = new CMD_S_LandScore();
        LandScore1.bLandUser = wChairID;
        LandScore1.bLandScore = score;
        LandScore1.wCurrentUser = mCurrentChair;
        LandScore1.bCurrentScore = mCallScore;
        //m_pITableFrame->SendTableData(INVALID_CHAIR, SUB_S_LAND_SCORE, &LandScore, sizeof(LandScore));
        //m_pITableFrame->SendLookonData(INVALID_CHAIR, SUB_S_LAND_SCORE, &LandScore, sizeof(LandScore));
        SendGameData((ushort)GlobalDef.Deinfe.INVALID_CHAIR, (ushort)enCmdTrench.SUB_S_LAND_SCORE, LandScore1);

    }

    //出牌
    public void OutCard(ushort wChair, byte[] cards)
    {
        if (COMMON_FUNC.IsInvalidChair(wChair))
        {
            Debuger.Instance.LogError(COMMON_CONST.ErrChair);
            return;
        }
        if (mGameStatus != CMD_Trench.GS_WK_PLAYING)
        {
            Debuger.Instance.LogError(COMMON_CONST.ErrOutCard);
            return;
        }
        if (mPlayers[wChair].mUser.cbUserStatus != (byte)GlobalDef.enUserStatus.US_PLAY)
        {
            Debuger.Instance.LogError(COMMON_CONST.ErrUserStaus);
            return;
        }
        if (wChair != mCurrentChair)
        {
            Debuger.Instance.LogError(COMMON_CONST.ErrCurChair);
            return;
        }

        mGameLogic.SortCardList(PlayerHandCard[wChair], (byte)PlayerHandCard[wChair].Count);
        mGameLogic.SortCardList(cards, (byte)cards.Length);

        //类型判断
        byte bCardType = mGameLogic.GetCardType(cards, (byte)cards.Length);
        if (bCardType == GLogicDef.CT_INVALID)
        {
            Debuger.Instance.LogError(COMMON_CONST.ErrOutCard);
            return;
        }

        //跟随出牌
        if (mTurnOutCard.Count > 0)
        {
            if (mGameLogic.CompareCard(mTurnOutCard.ToArray(), cards, (byte)mTurnOutCard.Count, (byte)cards.Length) == false)
            {
                Debuger.Instance.LogError(COMMON_CONST.ErrOutCard);
                return;
            }
        }

        //删除扑克
        if (mGameLogic.RemoveCard(cards, (byte)cards.Length, PlayerHandCard[wChair]) == false)
        {
            Debuger.Instance.LogError(COMMON_CONST.ErrOutCard);
            return;
        }

        //出牌记录
        mTurnOutCard.Clear();
        mTurnOutCard.AddRange(cards);

        mOutCardCount[wChair]++;


        mTurnWiner = wChair;

        //炸弹判断
        if (bCardType == GLogicDef.CT_BOMB_CARD)
        {
            //如果出的炸弹设置天梯信息
            //if (m_pGameServiceOption->wServerType & GAME_GENRE_HILADDER)
            //    m_HiLadder.SetOutBomb();
           // mBombTime*=2;
            mBombTime++;
            //if (m_pGameServiceOption->wKindID != 291 && m_pGameServiceOption->wServerID != 53)//配置欢乐炸弹场的时候，wKindID换成对应房间的ID
            //{
            //    m_wBombTime = __min(m_wBombTime, 4);
            //}
            //m_wBombTime=__min( m_wBombTime, 4 );注释掉，去掉炸弹限制，del by guopeng
        }

        //大牌判断
        bool bMostest = false;
        byte bValue = mGameLogic.GetCardValue(cards[0]);
        //if ((bValue==3)||((bValue==13)&&(bCardType>=CT_SINGLE_LINE)&&bCardType != CT_BOMB_CARD)) bMostest=true;

        if (PlayerHandCard[wChair].Count != 0)
        {
            if (!bMostest) mCurrentChair = (ushort)((mCurrentChair + 1) % CMD_Trench.GAME_PLAYER);
        }
        else mCurrentChair = (ushort)GlobalDef.Deinfe.INVALID_CHAIR;

        //构造数据
        CMD_S_OutCard OutCardCmd = new CMD_S_OutCard();
        OutCardCmd.bCardCount = (byte)cards.Length;
        OutCardCmd.wOutCardUser = wChair;
        OutCardCmd.wCurrentUser = mCurrentChair;
        OutCardCmd.bCardData = new byte[CMD_Trench.ZHUANG_CARD_NUM];
        System.Array.Copy(mTurnOutCard.ToArray(), OutCardCmd.bCardData, mTurnOutCard.Count);
        //CopyMemory(OutCard.bCardData, m_bTurnCardData, m_bTurnCardCount * sizeof(BYTE));

        //发送数据
        //WORD wSendSize =  sizeof(OutCard) - sizeof(OutCard.bCardData) + OutCard.bCardCount * sizeof(BYTE);
        //m_pITableFrame->SendTableData(INVALID_CHAIR, SUB_S_OUT_CARD, &OutCard, wSendSize);
        //m_pITableFrame->SendLookonData(INVALID_CHAIR, SUB_S_OUT_CARD, &OutCard, wSendSize);
        SendGameData((ushort)GlobalDef.Deinfe.INVALID_CHAIR, enCmdTrench.SUB_S_OUT_CARD, OutCardCmd);

        //出牌最大
        if (bMostest) mTurnOutCard.Clear();

        //结束判断
        if (mCurrentChair == (ushort)GlobalDef.Deinfe.INVALID_CHAIR)
        {
            EndGame(wChair, (int)GameExport.Define.GER_NORMAL);
        }
        //return true;

    }

    /// <summary>
    /// 不出 过牌
    /// </summary>
    /// <param name="wChair"></param>
    public void PassCard(ushort wChair)
    {
        if (COMMON_FUNC.IsInvalidChair(wChair))
        {
            Debuger.Instance.LogError(COMMON_CONST.ErrChair);
            return;
        }

        if (mPlayers[wChair].mUser.cbUserStatus != (byte)GlobalDef.enUserStatus.US_PLAY)
        {
            Debuger.Instance.LogError(COMMON_CONST.ErrUserStaus);
            return;
        }

        //
        if (mGameStatus != CMD_Trench.GS_WK_PLAYING)
        {
            Debuger.Instance.LogError(COMMON_CONST.ErrGameStatus);
            return;
        }

        if (mCurrentChair != wChair)
        {
            Debuger.Instance.LogError(COMMON_CONST.ErrCurChair);
            return;
        }

        //效验状态
        if (mTurnOutCard.Count == 0)
        {
            Debuger.Instance.LogError(COMMON_CONST.ErrCurChair);
            return;
        }

        //设置变量
        mCurrentChair = (ushort)((mCurrentChair + 1) % CMD_Trench.GAME_PLAYER);
        if (mCurrentChair == mTurnWiner) mTurnOutCard.Clear();

        //发送数据
        CMD_S_PassCard PassCardCmd = new CMD_S_PassCard();
        PassCardCmd.wPassUser = wChair;
        PassCardCmd.wCurrentUser = mCurrentChair;
        PassCardCmd.bNewTurn = (mTurnOutCard.Count == 0) ? (byte)(1) : (byte)(0);
        //m_pITableFrame->SendTableData(INVALID_CHAIR, SUB_S_PASS_CARD, &PassCard, sizeof(PassCard));
        //m_pITableFrame->SendLookonData(INVALID_CHAIR, SUB_S_PASS_CARD, &PassCard, sizeof(PassCard));
        SendGameData((ushort)GlobalDef.Deinfe.INVALID_CHAIR, enCmdTrench.SUB_S_PASS_CARD, PassCardCmd);

    }

    public bool Trustee(ushort wChair, bool bTrustee)
    {
        //效验数据
        if (COMMON_FUNC.IsInvalidChair(wChair))
        {
            Debuger.Instance.LogError(COMMON_CONST.ErrChair);
            return false;
        }
        if (mGameStatus != CMD_Trench.GS_WK_PLAYING)
        {
            return false;
        }
        if (mUserTrustee[wChair] != bTrustee)
        {
            mUserTrustee[wChair] = bTrustee;
            CMD_C_UserTrustee pUserTrustee = new CMD_C_UserTrustee();
            pUserTrustee.wUserChairID = wChair;
            pUserTrustee.bTrustee = bTrustee;

            SendGameData((ushort)GlobalDef.Deinfe.INVALID_CHAIR, enCmdTrench.SUB_C_TRUSTEE, pUserTrustee);
        }
        else
        {
            return false;
        }

        return true;
    }

    public void SortCardList(IList<byte> cbCardData,byte cbCardCount)
    {
        mGameLogic.SortCardList(cbCardData, cbCardCount);
    }


    //检测出牌
    /// <summary>
    /// 判断是否有大过上家的牌
    /// </summary>
    public bool SearchOutCard(ushort wCurrentOutChair)
    {
        WORD wLastCahir =  (WORD)((wCurrentOutChair+2)%CMD_Trench.GAME_PLAYER);

        List<tagAnalyseCardTypeResult> follow = new List<tagAnalyseCardTypeResult>();
        bool isOut = mGameLogic.CommFollowCard(wLastCahir, mTurnOutCard.ToArray(), (byte)mTurnOutCard.Count, follow, false);

        if (isOut && follow.Count > 0)
        {
            return true;
        }
        return false;
    }

    //提示出牌
    public List<byte> GetHelpCard()
    {
        List<byte> returnList = new List<byte>();
        if (mTurnOutCard.Count < 1) { 
            //用户先出牌
            List<byte> handCards = PlayerHandCard[DataBase.Instance.PLAYER.mUser.wChairID];
            returnList.Add(handCards[handCards.Count - 1]);
            return returnList;
        }
        if (GLogicDef.CT_INVALID == m_nPromptType) ClearPrompt();
        if (AutoPlayCards())
        {
            for (int i = 0; i < m_AutoOutCard.bCardCount; i++)
                returnList.Add(m_AutoOutCard.bCardData[i]);
        }

        return returnList;
    }
    // 得到上把出牌的最大值 
    private byte GetMaxCard()
    {
        byte bMaxCard;
        bMaxCard = mGameLogic.GetCardLogicValue(mTurnOutCard[0]);
        for (int i = 1; i < mTurnOutCard.Count; i++)
        {
            if (mGameLogic.GetCardLogicValue(mTurnOutCard[i]) > bMaxCard)
            {
                bMaxCard = mGameLogic.GetCardLogicValue(mTurnOutCard[i]);
            }
        }
        return bMaxCard;
    }
    //清除提示
    public void ClearPrompt()
    {
        m_nPromptPos = -1;
        m_nPromptType = 0;
        m_nPromptCount = 0;
        m_bPromptSelected = false;
        m_bPrompted = new byte[CMD_Trench.ZHUANG_CARD_NUM];
        m_nPromptedCount = 0;
    }
    //检查提示过的牌,已经出过的牌返回true，否则返回false
    private bool CheckPrompted(byte bCardValue)
    {
        for (int i = 0; i < 20; ++i)
        {
            if (m_bPrompted[i] == 0)
            {
                break;
            }
            if (mGameLogic.GetCardLogicValue(m_bPrompted[i]) == mGameLogic.GetCardLogicValue(bCardValue))
            {
                return true;
            }
        }
        return false;
    }

    //统计手中指定相同牌的个数
    private int GetTotalCount(byte bCardValue, int wIndex)
    {

        int nCount = 0;
        byte[] m_bHandCardData = PlayerHandCard[DataBase.Instance.PLAYER.mUser.wChairID].ToArray();
        byte m_bHandCardCount = (byte)m_bHandCardData.Length;
        if (wIndex >= 0 && wIndex < m_bHandCardCount)
        {
            for (int i = wIndex; i >= 0; --i)
            {
                if (mGameLogic.GetCardLogicValue(m_bHandCardData[i]) == mGameLogic.GetCardLogicValue(bCardValue))
                {
                    ++nCount;
                }
                else if (nCount > 0)
                {
                    break;
                }
            }
        }
        return nCount;
    }

    //出牌类型
    public byte GetOutCardType(byte[] cards)
    {
        byte m_bTurnCardCount = (byte)cards.Length;
        return mGameLogic.GetCardType(cards, m_bTurnCardCount);
    }
    //提示功能
    public bool AutoPlayCards()
    {

        //是否检查过
        bool bCheckOver = false;

        m_AutoOutCard.bCardCount = 0;
        m_AutoOutCard.bCardData = new byte[CMD_Trench.ZHUANG_CARD_NUM];
        byte bMaxCardValue = GetMaxCard();
        byte m_bTurnCardCount = (byte)mTurnOutCard.Count;
        byte m_bTurnOutType = mGameLogic.GetCardType(mTurnOutCard.ToArray(), m_bTurnCardCount);

        //用户手牌
        byte[] m_bHandCardData = PlayerHandCard[DataBase.Instance.PLAYER.mUser.wChairID].ToArray();
        byte m_bHandCardCount = (byte)m_bHandCardData.Length;
        switch (m_bTurnOutType)
        {
            case GLogicDef.CT_INVALID:
                {
                    //错误类型，直接返回false

                    return false;
                }

            case GLogicDef.CT_SINGLE://单牌类型
                {
                    //设置循环出牌位置	
                    if (m_nPromptPos < 0)
                    {
                        m_nPromptPos = m_bHandCardCount - 1;
                    }
                    //搜索手中牌
                    while (true)
                    {
                        for (int i = m_nPromptPos; i >= 0; --i)
                        {
                            if (mGameLogic.GetCardLogicValue(m_bHandCardData[i]) > bMaxCardValue)
                            {
                                //检查本牌是否已经出过
                                if (CheckPrompted(m_bHandCardData[i]))
                                {
                                    bCheckOver = true;
                                    continue;
                                }
                                //有可以选择的牌
                                m_bPromptSelected = true;
                                //判断是否成对,如果成对,则继续寻找.
                                int nCount = GetTotalCount(m_bHandCardData[i], i);
                                if (m_nPromptCount == 0 && nCount > 1)
                                {
                                    i = i - (nCount - 1);
                                    continue;
                                }

                                //设置要打的牌和数量
                                m_AutoOutCard.bCardData[0] = m_bHandCardData[i];
                                m_AutoOutCard.bCardCount = 1;
                                //m_AutoOutCard.Add(m_bHandCardData[i]);
                                m_nPromptPos = i - 1;
                                //记录已经检查过的牌
                                m_bPrompted[m_nPromptedCount] = m_bHandCardData[i];
                                ++m_nPromptedCount;
                                m_nPromptType = GLogicDef.CT_SINGLE;
                                return true;
                            }
                        }
                        //重新搜索一遍
                        if (m_nPromptType != GLogicDef.CT_BOMB_CARD)
                            m_nPromptPos = m_bHandCardCount - 1;
                        ++m_nPromptCount;
                        if (m_nPromptCount > 2) break;
                    }
                    //m_nPromptPos = -1;
                    if (bCheckOver)
                    {
                        //一次大的循环结束
                        m_nPromptType = GLogicDef.CT_INVALID;
                    }
                    break;
                    //return false;
                }
            case GLogicDef.CT_DOUBLE:	//对牌类型
                {
                    //设置循环出牌位置	
                    if (m_nPromptPos < 0)
                    {
                        m_nPromptPos = m_bHandCardCount - 1;
                    }

                    while (true)
                    {
                        for (int i = m_nPromptPos; i >= 0; --i)
                        {
                            if (i >= 1 &&
                                mGameLogic.GetCardLogicValue(m_bHandCardData[i]) > bMaxCardValue &&
                                mGameLogic.GetCardLogicValue(m_bHandCardData[i]) == mGameLogic.GetCardLogicValue(m_bHandCardData[i - 1]))
                            {
                                //检查本牌是否已经出过
                                if (CheckPrompted(m_bHandCardData[i]))
                                {
                                    bCheckOver = true;
                                    continue;
                                }
                                m_bPromptSelected = true;
                                int nCount = GetTotalCount(m_bHandCardData[i], i);
                                if (m_nPromptCount == 0 && nCount > 2)
                                {
                                    i = i - (nCount - 1);
                                    continue;
                                }
                                //设置要打的牌
                                m_AutoOutCard.bCardData[0] = m_bHandCardData[i];
                                m_AutoOutCard.bCardData[1] = m_bHandCardData[i - 1];
                                m_AutoOutCard.bCardCount = 2;
                                m_nPromptPos = i - 2;
                                //记录已经检查过的牌
                                m_bPrompted[m_nPromptedCount] = m_bHandCardData[i];
                                ++m_nPromptedCount;
                                m_nPromptType = GLogicDef.CT_DOUBLE;
                                return true;
                            }
                        }
                        //重新开始搜索
                        if (m_nPromptType != GLogicDef.CT_BOMB_CARD)
                            m_nPromptPos = m_bHandCardCount - 1;
                        ++m_nPromptCount;
                        if (m_nPromptCount > 2)
                            break;
                    }
                    //m_nPromptPos = -1;			
                    if (bCheckOver)
                    {
                        //一次大的循环结束
                        m_nPromptType = GLogicDef.CT_INVALID;
                    }
                    break;
                    //return false;
                }
            case GLogicDef.CT_THREE://三条类型	
                {
                    //设置循环出牌位置	
                    if (m_nPromptPos < 0)
                    {
                        m_nPromptPos = m_bHandCardCount - 1;
                    }

                    while (true)
                    {
                        for (int i = m_nPromptPos; i >= 0; --i)
                        {
                            if (i >= 2 &&
                                mGameLogic.GetCardLogicValue(m_bHandCardData[i]) > bMaxCardValue &&
                                mGameLogic.GetCardLogicValue(m_bHandCardData[i]) == mGameLogic.GetCardLogicValue(m_bHandCardData[i - 1]) &&
                                mGameLogic.GetCardLogicValue(m_bHandCardData[i]) == mGameLogic.GetCardLogicValue(m_bHandCardData[i - 2]))
                            {
                                //检查本牌是否已经出过
                                if (CheckPrompted(m_bHandCardData[i]))
                                {
                                    bCheckOver = true;
                                    continue;
                                }
                                m_bPromptSelected = true;
                                int nCount = GetTotalCount(m_bHandCardData[i], i);
                                if (m_nPromptCount == 0 && nCount > 3)
                                {
                                    i = i - (nCount - 1);
                                    continue;
                                }
                                //设置要打的牌
                                m_AutoOutCard.bCardData[0] = m_bHandCardData[i];
                                m_AutoOutCard.bCardData[1] = m_bHandCardData[i - 1];
                                m_AutoOutCard.bCardData[2] = m_bHandCardData[i - 2];
                                m_AutoOutCard.bCardCount = 3;
                                m_nPromptPos = i - 3;

                                //记录已经检查过的牌
                                m_bPrompted[m_nPromptedCount] = m_bHandCardData[i];
                                ++m_nPromptedCount;
                                m_nPromptType = GLogicDef.CT_THREE;
                                return true;
                            }
                        }
                        //重新开始搜索
                        if (m_nPromptType != GLogicDef.CT_BOMB_CARD) m_nPromptPos = m_bHandCardCount - 1;
                        ++m_nPromptCount;
                        if (m_nPromptCount > 2)
                            break;
                    }
                    //m_nPromptPos = -1;
                    if (bCheckOver)
                    {
                        //一次大的循环结束
                        m_nPromptType = GLogicDef.CT_INVALID;
                    }
                    break;
                    //return false;
                }
            case GLogicDef.CT_BOMB_CARD: //四条类型
                {
                    bCheckOver = true;
                    //m_nPromptPos = -1;			
                    if (bCheckOver)
                    {
                        //一次大的循环结束
                        m_nPromptType = GLogicDef.CT_INVALID;
                    }
                    //return false;
                    break;
                }
            case GLogicDef.CT_SINGLE_LINE:
                {
                    //设置循环出牌位置	
                    if (m_nPromptPos < 0)
                    {
                        m_nPromptPos = m_bHandCardCount - 1;
                    }

                    for (int i = m_nPromptPos; i >= 0; --i)
                    {
                        if (mGameLogic.GetCardValue(m_bHandCardData[i]) > 3 &&
                            mGameLogic.GetCardLogicValue(m_bHandCardData[i]) > bMaxCardValue)
                        {
                            //手中牌是否大于等于出牌数
                            if (m_bHandCardCount - i - 1 < m_bTurnCardCount - 1)
                            {
                                continue;
                            }
                            //检查本牌是否已经出过
                            if (CheckPrompted(m_bHandCardData[i]))
                            {
                                continue;
                            }

                            m_AutoOutCard.bCardCount = 0;
                            m_AutoOutCard.bCardData[m_AutoOutCard.bCardCount] = m_bHandCardData[i];
                            m_AutoOutCard.bCardCount++;
                            for (int j = i + 1; j < m_bHandCardCount; j++)
                            {
                                //同牌
                                if (mGameLogic.GetCardLogicValue(m_bHandCardData[j]) ==
                                    mGameLogic.GetCardLogicValue(m_AutoOutCard.bCardData[m_AutoOutCard.bCardCount - 1]))
                                    continue;
                                else
                                    //下一张连牌
                                    if (mGameLogic.GetCardLogicValue(m_bHandCardData[j]) + 1 ==
                                        mGameLogic.GetCardLogicValue(m_AutoOutCard.bCardData[m_AutoOutCard.bCardCount - 1]))
                                    {
                                        //拷贝一张
                                        m_AutoOutCard.bCardData[m_AutoOutCard.bCardCount] = m_bHandCardData[j];
                                        m_AutoOutCard.bCardCount++;
                                        //已经找到完成
                                        if (m_AutoOutCard.bCardCount >= m_bTurnCardCount)
                                        {
                                            m_nPromptPos = i - 1;
                                            m_bPromptSelected = true;
                                            //记录已经检查过的牌
                                            m_bPrompted[m_nPromptedCount] = m_bHandCardData[i];
                                            ++m_nPromptedCount;
                                            m_nPromptType = GLogicDef.CT_SINGLE_LINE;
                                            return true;
                                        }
                                    }
                                    else
                                        break;
                            }
                        }
                    }

                    if (m_nPromptType != GLogicDef.CT_BOMB_CARD)
                        m_nPromptPos = m_bHandCardCount - 1;
                    m_nPromptType = GLogicDef.CT_INVALID;
                    break;
                    //return false;
                }
            case GLogicDef.CT_DOUBLE_LINE:										//对连类型
                {
                    //设置循环出牌位置	
                    if (m_nPromptPos < 0)
                    {
                        m_nPromptPos = m_bHandCardCount - 1;
                    }

                    for (int i = m_nPromptPos; i >= 0; --i)
                    {
                        if (mGameLogic.GetCardValue(m_bHandCardData[i]) > 3 &&
                            mGameLogic.GetCardLogicValue(m_bHandCardData[i]) > bMaxCardValue)
                        {
                            if (m_bHandCardCount - i - 1 < m_bTurnCardCount - 1)
                            {
                                continue;
                            }
                            //检查本牌是否已经出过
                            if (CheckPrompted(m_bHandCardData[i]))
                            {
                                continue;
                            }

                            m_AutoOutCard.bCardData[0] = m_bHandCardData[i];
                            m_AutoOutCard.bCardCount = 1;
                            byte TempValue = mGameLogic.GetCardLogicValue(m_AutoOutCard.bCardData[0]);
                            for (int j = i + 1; j < m_bHandCardCount; j++)
                            {
                                //同牌
                                if (mGameLogic.GetCardLogicValue(m_bHandCardData[j]) == TempValue)
                                {
                                    //拷贝一张
                                    m_AutoOutCard.bCardData[m_AutoOutCard.bCardCount] = m_bHandCardData[j];
                                    m_AutoOutCard.bCardCount++;
                                    if (m_AutoOutCard.bCardCount % 2 == 0) TempValue--;

                                    //已经找到
                                    if (m_AutoOutCard.bCardCount == m_bTurnCardCount)
                                    {
                                        m_nPromptPos = i - 1;
                                        m_bPromptSelected = true;
                                        //记录已经检查过的牌
                                        m_bPrompted[m_nPromptedCount] = m_bHandCardData[i];
                                        ++m_nPromptedCount;
                                        m_nPromptType = GLogicDef.CT_DOUBLE_LINE;
                                        return true;
                                    }
                                    else
                                    {
                                        if (m_AutoOutCard.bCardCount > m_bTurnCardCount)
                                            return false;
                                    }
                                }
                                else
                                {
                                    //下一张连牌
                                    if (mGameLogic.GetCardLogicValue(m_bHandCardData[j]) == TempValue + 1 && m_AutoOutCard.bCardCount % 2 == 0)
                                    {
                                        continue;
                                    }
                                    else break;
                                }
                            }
                        }
                    }

                    if (m_nPromptType != GLogicDef.CT_BOMB_CARD)
                        m_nPromptPos = m_bHandCardCount - 1;
                    m_nPromptType = GLogicDef.CT_INVALID;
                    break;
                }
            case GLogicDef.CT_THREE_LINE:  // by wxh 2011013!@#
                {
                    //设置循环出牌位置
                    if (m_nPromptPos < 0)
                    {
                        m_nPromptPos = m_bHandCardCount - 1;
                    }
                    for (int i = m_nPromptPos; i >= 0; i--)
                    {
                        if (mGameLogic.GetCardValue(m_bHandCardData[i]) > 3 &&
                            mGameLogic.GetCardLogicValue(m_bHandCardData[i]) > bMaxCardValue)
                        {
                            if (m_bHandCardCount - i - 1 < m_bTurnCardCount - 1)
                            {
                                continue;
                            }
                            //检查本牌是否已经出过
                            if (CheckPrompted(m_bHandCardData[i]))
                            {
                                continue;
                            }

                            //找到第一张合适
                            m_AutoOutCard.bCardData[0] = m_bHandCardData[i];
                            m_AutoOutCard.bCardCount = 1;
                            byte TempValue = mGameLogic.GetCardLogicValue(m_AutoOutCard.bCardData[0]);

                            //要找到三张连牌
                            for (int j = i + 1; j < m_bHandCardCount; j++)
                            {
                                if (mGameLogic.GetCardLogicValue(m_bHandCardData[j]) == TempValue)//同牌
                                {
                                    m_AutoOutCard.bCardData[m_AutoOutCard.bCardCount] = m_bHandCardData[j];//拷贝一张
                                    m_AutoOutCard.bCardCount++;
                                    if (m_AutoOutCard.bCardCount % 3 == 0) TempValue--;
                                    //已经找到
                                    if (m_AutoOutCard.bCardCount == m_bTurnCardCount)
                                    {
                                        m_nPromptPos = i - 1;
                                        m_bPromptSelected = true;
                                        //记录已经检查过的牌
                                        m_bPrompted[m_nPromptedCount] = m_bHandCardData[i];
                                        ++m_nPromptedCount;
                                        m_nPromptType = GLogicDef.CT_THREE_LINE;
                                        return true;
                                    }
                                    else
                                    {
                                        if (m_AutoOutCard.bCardCount > m_bTurnCardCount)
                                            return false;
                                    }
                                }
                                else
                                {
                                    //下一张连牌
                                    if (mGameLogic.GetCardLogicValue(m_bHandCardData[j]) == TempValue + 1 && m_AutoOutCard.bCardCount % 3 == 0) { continue; }
                                    else break;
                                }
                            }

                        }
                    }

                    if (m_nPromptType != GLogicDef.CT_BOMB_CARD)
                        m_nPromptPos = m_bHandCardCount - 1;
                    m_nPromptType = GLogicDef.CT_INVALID;
                    break;
                }
            default:
                break;
        }

        //检测炸弹
        if (m_nPromptType == GLogicDef.CT_INVALID)
        {
            //设置循环出牌位置	
            if (m_nPromptPos < 0)
            {
                m_nPromptPos = m_bHandCardCount - 1;
            }

            while (true)
            {
                for (int i = m_nPromptPos; i >= 0; --i)
                {
                    if ((i >= 3) &&
                        (mGameLogic.GetCardLogicValue(m_bHandCardData[i]) == mGameLogic.GetCardLogicValue(m_bHandCardData[i - 1])) &&
                        (mGameLogic.GetCardLogicValue(m_bHandCardData[i]) == mGameLogic.GetCardLogicValue(m_bHandCardData[i - 2])) &&
                        (mGameLogic.GetCardLogicValue(m_bHandCardData[i]) == mGameLogic.GetCardLogicValue(m_bHandCardData[i - 3]))
                        )
                    {
                        if ((m_bTurnOutType != GLogicDef.CT_BOMB_CARD) ||
                            (((m_bTurnOutType == GLogicDef.CT_BOMB_CARD) && (mGameLogic.GetCardLogicValue(m_bHandCardData[i]) > bMaxCardValue)))
                            )
                        {
                            //设置要打的牌
                            m_bPromptSelected = true;
                            m_AutoOutCard.bCardData[0] = m_bHandCardData[i];
                            m_AutoOutCard.bCardData[1] = m_bHandCardData[i - 1];
                            m_AutoOutCard.bCardData[2] = m_bHandCardData[i - 2];
                            m_AutoOutCard.bCardData[3] = m_bHandCardData[i - 3];
                            m_AutoOutCard.bCardCount = 4;
                            m_nPromptPos = i - 4;
                            m_nPromptType = GLogicDef.CT_BOMB_CARD;
                            if (i - 3 == 0)
                            {
                                m_nPromptPos = -1;
                                m_nPromptType = GLogicDef.CT_INVALID;
                            }
                            return true;
                        }
                    }
                }

                //重新开始搜索
                m_nPromptPos = m_bHandCardCount - 1;
                ++m_nPromptCount;
                m_nPromptType = GLogicDef.CT_INVALID;
                if (m_nPromptCount > 2)
                    break;
            }
        }

        return false;
    }
    //辅助功能
    public void StartSubThread(System.Action<object> subCall, object param)
    {
        UnityEngineEx.MThread.MThreadPool.Instance.NewThreadToSub(subCall, param);
    }

    public void StartMainThread(System.Action<object> mainCall, object param)
    {
        UnityEngineEx.MThread.MThreadPool.Instance.NewThreadToMain(mainCall, param);
    }


    void Update()
    {
        UnityEngineEx.MThread.MThreadPool.Instance.OnUpdate();
    }

    //////////////////////////////////////////////////////////


   

}
