using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class StageMgr
{
    Stage mStage = new Stage();
    StageCond mStageCond = new StageCond();
    StageCondInfo mStageCondInfo = new StageCondInfo();

    public void LoadAll()
    {
        LoadStage();
        LoadStageCond();
        LoadStageCondInfo();
    }

    public void LoadStage()
    {
        string data = Resources.Load(COMMON_CONST.PathStageInfo).ToString();
        mStage.XmlLoad(data, "StageInfo");
    }

    public void LoadStageCond()
    {
        string data = Resources.Load(COMMON_CONST.PathStageCondition).ToString();
        mStageCond.XmlLoad(data, "StageCondition");
    }

    public void LoadStageCondInfo()
    {
        string data = Resources.Load(COMMON_CONST.PathStageConditionInfo).ToString();
        mStageCondInfo.XmlLoad(data, "StageConditionInfo");
    }

    /// <summary>
    /// 获取关卡信息
    /// </summary>
    /// <param name="id">关卡编号</param>
    /// <returns></returns>
    public INFO_STAGE GetStage(uint id)
    {
        return mStage.INFO[id];
    }

    /// <summary>
    /// 获取所有关卡信息
    /// </summary>
    /// <returns></returns>
    public List<INFO_STAGE> GetAllStage()
    {
        List<INFO_STAGE> list = new List<INFO_STAGE>();
        list.AddRange(mStage.INFO.Values);

        return list;
    }

    /// <summary>
    /// 获取关卡条件
    /// </summary>
    /// <param name="id">关卡条件编号</param>
    /// <returns></returns>
    public INFO_STAGE_COND GetStageCond(uint id)
    {
        return mStageCond.INFO[id];
    }

    /// <summary>
    /// 获取关卡和关卡条件关联数据
    /// </summary>
    /// <param name="id">关卡编号</param>
    /// <returns></returns>
    public List<INFO_STAGE_COND_LINK> GetStageCondInfo(uint id)
    {
        uint ConditionId = GetStage(id).ComditionId;
        return mStageCondInfo.INFO[ConditionId];
    }

    //获得关卡分组条件
    public List<INFO_STAGE_COND_LINK> GetStageCondInfoByGroup(uint id,int group)
    {
        List<INFO_STAGE_COND_LINK> list = GetStageCondInfo(id);
        List<INFO_STAGE_COND_LINK> groupCond = new List<INFO_STAGE_COND_LINK>();

        foreach(INFO_STAGE_COND_LINK info in list)
        {
            INFO_STAGE_COND cond = GetStageCond(info.StageConditionId);
            if (cond.GroupId == group)
                groupCond.Add(info);
        }
        return groupCond;
    }

    //获得当前关卡任务特定条件
    public INFO_STAGE_COND_LINK GetStageCondById(uint condId)
    {
        if (DataBase.Instance.IsStageRoom)
        {
            uint stageId = DataBase.Instance.CurStage.Id;
            List<INFO_STAGE_COND_LINK> list = GetStageCondInfo(stageId);
            return list.Find((x) => x.StageConditionId == condId);
        }
        return null;
    }

    //扣除闯关费用
    public bool StageCharge(INFO_STAGE stage)
    {
        uint score = stage.StageCost;
        if (stage.StageCostPricetype == COMMON_CONST.PriceGold)
        {
            if (DataBase.Instance.PLAYER.Gold < score)
            {
                //失败
                LobbyDialogManager.inst.ShowLowCoinDialog(GoodType.Coin);
                return false;
            }
            DataBase.Instance.PLAYER.Gold -= score;
        }
        else
        {
            if (DataBase.Instance.PLAYER.ZScore < score)
            {
                LobbyDialogManager.inst.ShowLowCoinDialog(GoodType.Diamond);
                return false;
            }
            DataBase.Instance.PLAYER.ZScore -= score;
        }
        DataBase.Instance.PLAYER.SaveUserInfo();
        return true;
    }


    //重新开始关卡
    public void ReplayStage(uint stageId)
    {
        // 获取关卡当前进行状态
        INFO_STAGE openStage = GetStage(stageId);
        UserStageRecord record = DataBase.Instance.PLAYER.GetUserStageRecord(openStage.Id);
        if (record == null || record.LastScore != (int)StageRecordStatus.Progress)
        {
            if (!StageCharge(openStage))
                return;

        }
        DataBase.Instance.EnterStageRoom(openStage.Id);
        
    }


}
