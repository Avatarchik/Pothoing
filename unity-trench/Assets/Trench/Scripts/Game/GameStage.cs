using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngineEx.LogInterface;

//

/// <summary>
/// 游戏关卡处理
/// </summary>
public class GameStage : MonoBehaviour
{
    [SerializeField]
    WindowData Windata;           //返回窗口数据
    [SerializeField]
    MissionDialog MissionInfo;    //关卡任务条件说明
    [SerializeField]
    BottomLab StageTip;           //任务提示

    //是否有出牌时间任务
    public bool IsOutCardTime
    {
        get;
        private set;
    }

    uint currOutCardNum = 0;      //当前出牌次数
    bool isLastHand = false;      //是否最后一手牌
    uint currBout = 0;            //当前回合数
    float currGameTime = 0;       //当前游戏进行时间
    float outCardTime = 0;        //玩家一次出牌时间
    int playerType = 0;           //0贫民 1 地主
    byte[] currOutCard = null;    //当前出牌
    byte currOutCardType = 0;     //当前出牌类型
    bool IsBeginStage = false;    //是否开始关卡游戏
    bool IsBeginOutCard = false;  //是否开始出牌
    bool IsFailure = false;       //本局是否失败
    bool IsHaveTime = false;      //是否有时限任务

    INFO_STAGE_COND_LINK stageCondOne;       //条件一有且只有一个
    List<INFO_STAGE_COND_LINK> stageCondTwo; //条件二有多个
    INFO_STAGE_COND_LINK stageCondThree;     //条件三有且只有一个

    INFO_STAGE_COND_LINK outCardTimeCond;    //出牌时间条件

    List<StageMissionData> missionData;//条件显示
    //已达成的条件记录
    List<uint> condSuccess = null;
    //已经失败的条件
    List<uint> condFailed = null;
    //玩家关卡记录
    uint curStage;
    INFO_STAGE curStageInfo;//当前关卡信息
    UserStageRecord userStageRecord;

    //条件处理
    private void outCardHandler(INFO_STAGE_COND_LINK cond, byte cardType, bool IsLink = false)
    {
        if (cond.FixedHand > 0)
        {
            if ((cond.FixedHand == 255 && isLastHand)
                || (cond.FixedHand != 255 && cond.FixedHand == currOutCardNum))
            {
                if (currOutCardType != cardType)
                {
                    //直接失败
                    editStageMissionInfo(cond.ID, 0, MissionProgressState.Failed);
                    return;
                }
                if (cond.Condition == 0)
                {
                    stageCondStatus(cond, true);
                    return;
                }
                byte beginCardVal = (byte)(currOutCard[currOutCard.Length - 1] & 0x0f);
                byte endCardVal = (byte)(currOutCard[0] & 0x0f);
                byte taskHigh = (byte)(cond.Condition >> 4);
                byte taskLength = (byte)(cond.Condition & 0x0f);
                if (IsLink)//连牌
                {

                    if ((endCardVal - beginCardVal + 1) == taskLength
                         && (taskHigh == 0 || (taskHigh > 0 && beginCardVal == taskHigh))
                       )
                    {
                        stageCondStatus(cond, true);
                        return;
                    }
                    editStageMissionInfo(cond.ID, 0, MissionProgressState.Failed);
                    return;
                }
                else//非连牌
                {
                    if ((taskHigh == 0 && endCardVal == cond.Condition)
                        || (taskHigh > 0 && currOutCard[0] == (cond.Condition - 0x10))) //任务 0x11 方片1 实际 0x01 需要-0x10
                    {
                        stageCondStatus(cond, true);
                        return;
                    }
                    editStageMissionInfo(cond.ID, 0, MissionProgressState.Failed);
                    return;
                }


            }
        }
        else
        {

            if (currOutCardType == cardType)
            {
                if (cond.Condition == 0)
                {
                    stageCondStatus(cond, true);
                    return;
                }

                byte beginCardVal = (byte)(currOutCard[currOutCard.Length - 1] & 0x0f);
                byte endCardVal = (byte)(currOutCard[0] & 0x0f);
                byte taskHigh = (byte)(cond.Condition >> 4);
                byte taskLength = (byte)(cond.Condition & 0x0f);
                if (IsLink)//连牌
                {
                    
                    if ((endCardVal - beginCardVal + 1) == taskLength
                         && (taskHigh == 0 || (taskHigh > 0 && beginCardVal == taskHigh))
                       )
                    {
                        stageCondStatus(cond, true);
                        return;
                    }
                }
                else//非连牌
                {
                    if ((taskHigh == 0 && endCardVal == cond.Condition)
                        || (taskHigh > 0 && currOutCard[0] == (cond.Condition - 0x10))) //任务 0x11 方片1 实际 0x01 需要-0x10
                    {
                        stageCondStatus(cond, true);
                        return;
                    }
                }
            }
        }
    }

    //初始化关卡任务说明
    private void initStageMissionInfo()
    {
        missionData = new List<StageMissionData>();
        //1,3条件作为一个
        StageMissionData sdata = new StageMissionData();
        sdata.condId = stageCondThree.ID;
        sdata.title = stageCondOne.Describe + "，" + stageCondThree.Describe;
        if (stageCondOne.StageConditionId == StageCond.COND_XINNING)
            sdata.desc = "当前第" + userStageRecord.CondVal1 + "局";
        else
            sdata.desc = "";
        sdata.targetProgress = (int)stageCondThree.Condition;
        sdata.currentProgress = (int)userStageRecord.CondVal3;
        sdata.state = MissionProgressState.Progressing;
        missionData.Add(sdata);
        foreach (INFO_STAGE_COND_LINK cond in stageCondTwo)
        {
            StageMissionData mdata = new StageMissionData();
            mdata.condId = cond.ID;
            mdata.title = cond.Describe;
            mdata.desc = "";
            mdata.targetProgress = 1;
            mdata.currentProgress = 0;
            mdata.state = MissionProgressState.Progressing;
            missionData.Add(mdata);
        }
        MissionInfo.Initial(missionData.ToArray());
    }

    //修改条件状态
    private void editStageMissionInfo(uint condId,int curr,MissionProgressState status)
    {
        StageMissionData mdata = missionData.Find((x) => x.condId == condId);
        if (mdata != null)
        {
            mdata.currentProgress = curr;
            mdata.state = status;
            MissionInfo.Refresh(missionData.ToArray());
            if (status == MissionProgressState.Completed)
                StageTip.ShowMissionStageTip(true, mdata.title);
            else if(status == MissionProgressState.Failed)
                StageTip.ShowMissionStageTip(false, mdata.title);

        }
    }
    //初始化关卡任务
    private void InitStageCond()
    {

        currOutCardNum = 0; //当前出牌次数
        isLastHand = false;
        currBout = 0; // 当前回合数
        currGameTime = 0; //当前游戏进行时间
        outCardTime = 0;  //玩家一次出牌时间
        playerType = 0; // 0贫民 1 地主
        currOutCard = null; //当前出牌
        currOutCardType = 0; //当前出牌类型
        IsBeginStage = false; //是否开始关卡游戏
        IsBeginOutCard = false; //是否开始出牌
        IsFailure = false; //本局是否已经失败
        IsHaveTime = false;
        condSuccess = new List<uint>();
        condFailed = new List<uint>();
        if (!DataBase.Instance.PLAYER.IsUsableProperty)
        {
            DataBase.Instance.PLAYER.Property = 0;
        }
        DataBase.Instance.PLAYER.IsUsableProperty = false;
        curStageInfo = DataBase.Instance.CurStage;
        curStage = curStageInfo.Id;
        
        userStageRecord = DataBase.Instance.PLAYER.GetUserStageRecord(curStage);
        if (userStageRecord == null || userStageRecord.LastStatus != (int)StageRecordStatus.Progress)
        {
            userStageRecord = new UserStageRecord();
        }
        userStageRecord.dwStageId = curStage;
        //开始只处理条件1（条件一有且只有一个）
        List<INFO_STAGE_COND_LINK> stageConds =
            DataBase.Instance.STAGE_MGR.GetStageCondInfoByGroup(curStage, StageCond.COND_GROUPONE);
        if (stageConds.Count > 0) stageCondOne = stageConds[0];
        //条件二
        stageCondTwo = DataBase.Instance.STAGE_MGR.GetStageCondInfoByGroup(curStage, StageCond.COND_GROUPTWO);
        //条件三
        stageConds = DataBase.Instance.STAGE_MGR.GetStageCondInfoByGroup(curStage, StageCond.COND_GROUPTHREE);
        if (stageConds.Count > 0) stageCondThree = stageConds[0];
        //条件一 处理x局内
        if (stageCondOne.StageConditionId == StageCond.COND_XINNING)
        {
            userStageRecord.CondId1 = stageCondOne.StageConditionId;
            userStageRecord.CondVal1 += 1;
        }
        //条件三
        userStageRecord.CondId3 = stageCondThree.StageConditionId;
        //开始记录失败，中途退出直接闯关失败
        userStageRecord.LastStatus = (int)StageRecordStatus.Failure;

        INFO_STAGE_COND_LINK cond = stageCondTwo.Find((x) => x.StageConditionId == StageCond.COND_NOOUTSECOND);
        if (cond != null && cond.Condition > 0)
        {
            outCardTimeCond = cond;
            IsOutCardTime = true;
        }    
        else
            IsOutCardTime = false;

        //显示任务提示
        StageTip.ToggleStageMissionBtn(true);
        //时间限制任务
        if (stageCondOne.StageConditionId == StageCond.COND_XSECOND)
        {
            currGameTime += userStageRecord.CondVal1;
            StageTip.ToggleStageMissionBtn(true, (int)(stageCondOne.Condition - userStageRecord.CondVal1));
            IsHaveTime = true;
        }
            
        else
            StageTip.ToggleStageMissionBtn(true);
        initStageMissionInfo();   
        Debuger.Instance.Log("关卡初始化~~~~~~~~~~~~");
    }
    //出牌判断
    private void condGroupTwo(INFO_STAGE_COND_LINK cond)
    {
        //已达成的条件不用再次判断
        if (condSuccess.Contains(cond.ID)) return;
        switch (cond.StageConditionId)
        {
            
            case StageCond.COND_OUTSINGLE: //出单张
                    outCardHandler(cond, GLogicDef.CT_SINGLE);
                    break;

            case StageCond.COND_OUTDOUBLE: //出对子
                    outCardHandler(cond, GLogicDef.CT_DOUBLE);
                    break;
                
            case StageCond.COND_OUTTHREE :  //出三条
                    outCardHandler(cond, GLogicDef.CT_THREE);
                    break;

            case StageCond.COND_SINGLELINK ://出单连
                    outCardHandler(cond, GLogicDef.CT_SINGLE_LINE,true);
                    break;

            case StageCond.COND_DOUBLELINK://双连
                    outCardHandler(cond, GLogicDef.CT_DOUBLE_LINE, true);
                    break;

            case StageCond.COND_THREELINK://三连
                    outCardHandler(cond, GLogicDef.CT_THREE_LINE, true);
                    break;

            case StageCond.COND_FOURLINK://四连
                    //TODO 没有定义需要修改
                    outCardHandler(cond, 8, true);
                    break;

            case StageCond.COND_OUTBOMB ://出炸弹
                    outCardHandler(cond, GLogicDef.CT_BOMB_CARD);
                    break;

            case StageCond.COND_XBOUT : //x回合
                {
                    if (cond.Condition > 0 && currBout > cond.Condition)
                    {
                        stageCondStatus(cond, false);
                    }
                        
                    break;
                }
            default :
                break;
        }
           
    }
    
    //开始关卡
    public void BeginStage()
    { 
        //判断是否在关卡房间
        if(DataBase.Instance.IsStageRoom)
        {   
            InitStageCond();
            StartCoroutine(SaveStage());
            IsBeginStage = true;
            StartCoroutine(StageTimeCount());//关卡计时
            
            
        }
    }
    //设置当局关卡游戏玩家身份
    public void SetPlayerType(int playerType)
    {
        if (!IsBeginStage) return;
        this.playerType = playerType;
        //查询身份条件
        INFO_STAGE_COND_LINK cond = stageCondTwo.Find((x) => x.StageConditionId == StageCond.COND_THETYPE);
        if (cond != null)
        {
            if (this.playerType == cond.Condition)
                stageCondStatus(cond, true);
            else
                stageCondStatus(cond, false);
                
        }
    }
    
    //超时处理
    private void outTime()
    {
        stageCondStatus(outCardTimeCond,false);
    }

    private void stageCondStatus(INFO_STAGE_COND_LINK cond, bool isSuccess)
    {
        if (isSuccess)
        {
            condSuccess.Add(cond.ID);
            editStageMissionInfo(cond.ID, 1, MissionProgressState.Completed);
        }
        else
        {
            condFailed.Add(cond.ID);
            IsFailure = true;
            editStageMissionInfo(cond.ID, 0, MissionProgressState.Failed);
        }
        
    }
    //开始出牌
    public void BeginOutCard()
    {
        if (!IsBeginStage) return;
        IsBeginOutCard = true;
        outCardTime = 0;
        StartCoroutine(OutCardTimeCount()); //出牌计时
    }
    //出牌
    public void EndOutCard(byte[] cards,byte cardsType,bool isLastHand=false)
    {
        if (!IsBeginStage) return;
        IsBeginOutCard = false;
        currOutCard = cards;
        currOutCardType = cardsType;
        currOutCardNum ++;
        this.isLastHand = isLastHand;
        foreach(INFO_STAGE_COND_LINK cond in stageCondTwo)
        {
            condGroupTwo(cond);
        }
    }
    //胜局判断处理
    private void successCond()
    {
        if (userStageRecord.CondVal3 == stageCondThree.Condition)
            //关卡通关
            userStageRecord.LastStatus = (int)StageRecordStatus.Success;
        else if (stageCondOne.Condition - userStageRecord.CondVal1 <
                stageCondThree.Condition - userStageRecord.CondVal3)
            //关卡失败
            userStageRecord.LastStatus = (int)StageRecordStatus.Failure;
        else
            //正常进行
            userStageRecord.LastStatus = (int)StageRecordStatus.Progress;
    }
    //重玩关卡
    public void replayStage(uint stageId = 0)
    {
        Windata.LobbyWin = LobbyState.Map;
        Windata.StageId = stageId;
        MainGame.inst.OnExitGame();
        DataBase.Instance.SCENE_MGR.SetScene(SceneMgr.SC_Hall);
    }

    //关卡结束提示
    public void EndStageGame(bool isSuccess,int score=0)
    {
        //score 当局分数
        userStageRecord.LastScore += score;
        StageRecordStatus status = endStage(isSuccess);
        if (status == StageRecordStatus.Success)
        {
            //胜利
            if (userStageRecord.LastScore > userStageRecord.BestScore)
            {
                userStageRecord.BestScore = userStageRecord.LastScore;
            }
            userStageRecord.Status = (int)StageRecordStatus.Success;
            int starNum = COMMON_FUNC.GetStarByScore(userStageRecord.dwStageId, score);
            GameDialog.inst.ShowStageResult(true, starNum, 
                curStageInfo.Describe + "(" + TextManager.Get("StageCondComplete") + ")",
                userStageRecord.BestScore,
                TextManager.Get("StageReplayBut"),
                delegate{
                    replayStage(DataBase.Instance.CurStage.Id);
                },
                TextManager.Get("StageNext"),
                delegate{
                    replayStage(DataBase.Instance.PLAYER.NextStage());
                });
        }
        else if (status == StageRecordStatus.Progress)
        {
            //继续关卡
            GameDialog.inst.ShowDialog((int)GameDialogIndex.FINAL);
            int[] allScore = new int[2];
            allScore[0] = userStageRecord.LastScore;
            allScore[1] = score;
            GameDialog.inst.SetFinalWin(GameDialog.CATEGORY_STAGE, isSuccess, allScore,
                delegate {
                    uint stageId = DataBase.Instance.CurStage.Id;
                    DataBase.Instance.STAGE_MGR.ReplayStage(stageId);
                    MainGame.inst.ReStart();
                });
            GameDialog.inst.ShowDialog((int)GameDialogIndex.FINAL);
        }
        else
        { 
            //失败
            int starNum = COMMON_FUNC.GetStarByScore(userStageRecord.dwStageId, score);
            GameDialog.inst.ShowStageResult(false, 0,
                curStageInfo.Describe + "(" + TextManager.Get("StageCondFailure") + ")",
                score,
                TextManager.Get("Back"),
                delegate
                {
                    replayStage(0);
                },
                TextManager.Get("StageReplay"),
                delegate
                {
                    replayStage(DataBase.Instance.CurStage.Id);
                });

        }
        //结束时保存关卡
        StartCoroutine(SaveStage());
    }

    //结束一局关卡游戏
    private StageRecordStatus endStage(bool isSuccess)
    {
        if (!IsBeginStage) return 0;
        IsBeginStage = false;
        StageTip.StopTimeCount();//界面停止时间计算
        //结束游戏进行关卡判断
        if (!IsFailure) {
            //条件二是否全满足
            foreach (INFO_STAGE_COND_LINK cond in stageCondTwo)
            {
                if (cond.StageConditionId != StageCond.COND_FIXEDHAND
                    && cond.StageConditionId != StageCond.COND_XBOUT
                    && cond.StageConditionId != StageCond.COND_NOOUTSECOND)
                {
                    if(!condSuccess.Contains(cond.ID))
                        stageCondStatus(cond, false);
                }
                else
                    stageCondStatus(cond, true);
            }
        }

        //判断x局内 100001
        if (stageCondOne.StageConditionId == StageCond.COND_XINNING)
        {
            //判断胜局300001 
            if (stageCondThree.StageConditionId == StageCond.COND_XSUCCESS)
            {
                if (!IsFailure && isSuccess)
                    userStageRecord.CondVal3++; //本局胜
                successCond(); //胜局处理
            }
            else//判断连胜
            {
                if (!IsFailure && isSuccess)
                {
                    userStageRecord.CondVal3++; //本局胜
                    successCond();
                }
                else
                {
                    if (userStageRecord.CondVal3 > 0)
                        //不是连胜直接失败
                        userStageRecord.LastStatus = (int)StageRecordStatus.Failure;
                    else
                        successCond();
                }
            }
        }
        else
        {
            //判断时间100002 x秒内，超时关卡失败
            userStageRecord.CondVal1 = (uint)currGameTime;
            if (userStageRecord.CondVal1 > stageCondOne.Condition)
            {
                //超时失败
                userStageRecord.LastStatus = (int)StageRecordStatus.Failure;
            }
            else
            { 
                //判断胜局300001 
                if (stageCondThree.StageConditionId == StageCond.COND_XSUCCESS)
                {
                    if (!IsFailure && isSuccess)
                        userStageRecord.CondVal3++; //本局胜
                    if (userStageRecord.CondVal3 == stageCondThree.Condition)
                        //关卡通关
                        userStageRecord.LastStatus = (int)StageRecordStatus.Success;
                    else
                        //正常进行
                        userStageRecord.LastStatus = (int)StageRecordStatus.Progress;
                }
                else
                {
                    if (!IsFailure && isSuccess)
                    {
                        userStageRecord.CondVal3++; //本局胜
                        if (userStageRecord.CondVal3 == stageCondThree.Condition)
                            //关卡通关
                            userStageRecord.LastStatus = (int)StageRecordStatus.Success;
                        else
                            //正常进行
                            userStageRecord.LastStatus = (int)StageRecordStatus.Progress;
                    }
                    else
                    {
                        if (userStageRecord.CondVal3 > 0)
                            //不是连胜直接失败
                            userStageRecord.LastStatus = (int)StageRecordStatus.Failure;
                        else
                            //正常进行
                            userStageRecord.LastStatus = (int)StageRecordStatus.Progress;
                    }
                }
                
            }
        }
        //用户关卡道具处理
        if ((StageRecordStatus)userStageRecord.LastStatus == StageRecordStatus.Progress)
        {
            DataBase.Instance.PLAYER.IsUsableProperty = true;
        }
        else
        {
            DataBase.Instance.PLAYER.Property = 0;
            DataBase.Instance.PLAYER.IsUsableProperty = false;
        }

        return (StageRecordStatus)userStageRecord.LastStatus;

    }
    //保存关卡
    IEnumerator SaveStage()
    {
        DataBase.Instance.PLAYER.SetUserRecord(userStageRecord);
        DataBase.Instance.PLAYER.SaveUserStageRecord();
        yield return 0;
    }
    //游戏时间任务
    IEnumerator StageTimeCount()
    {

        while (IsBeginStage)
        {
            yield return new WaitForSeconds(1f);
            currGameTime += 1f;
            if (IsHaveTime && currGameTime > stageCondOne.Condition)
            {
                IsHaveTime = false;
                editStageMissionInfo(stageCondThree.ID, (int)userStageRecord.CondVal3, MissionProgressState.Failed);
                userStageRecord.LastStatus = (int)StageRecordStatus.Failure;
            }
            
        }
            
    }
    //每次出牌时间统计
    IEnumerator OutCardTimeCount()
    {
        while (IsBeginOutCard)
        {
            yield return 0;
            outCardTime += Time.deltaTime;
        }
    }
    
}
