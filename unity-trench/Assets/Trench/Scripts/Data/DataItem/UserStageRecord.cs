using System;
using System.Collections.Generic;

using UnityEngineEx.Net;

//关卡记录状态
public enum StageRecordStatus
{
    Failure, //失败
    Progress,//进行
    Success  //成功
}
[Serializable]
public class UserStageRecord
{
    
    public uint dwUserId;                   //用户
    public uint dwStageId;                  //关卡记录ID
    public int Status;                      //过关状态 0 失败 1 进行中 2 通关
    public int LastStatus;                  //
    public int BestScore;                   //最好成绩
    public int LastScore;
    
    public uint CondId1;
    public uint CondVal1;
    public Dictionary<string, uint> Cond2;
    public uint CondId3;
    public uint CondVal3;
}

public class PlayerStageRecord
{
    Dictionary<string, UserStageRecord> mUserStageRecord = new Dictionary<string, UserStageRecord>();


    public void Load()
    {
        string record = COMMON_FUNC.GetUserStageRecord();
        if(record != COMMON_CONST.NullString)
        {
            mUserStageRecord = record.FromJSON<Dictionary<string, UserStageRecord>>();
        }
    }

    public void WriteRecord()
    {
        if (mUserStageRecord.Count == 0) return;
        COMMON_FUNC.SetUserStageRecord(mUserStageRecord.ToJSON());
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
    public void SetUserRecord(uint stage, int status, int score, uint condid1, uint condval1,
        uint condid3, uint condval3,params uint[] cond2)
    {
        //StageCondData cond_data = new StageCondData();
        UserStageRecord record = new UserStageRecord();
        record.dwUserId = DataBase.Instance.PLAYER.mUser.dwUserID;
        record.dwStageId = stage;
        record.Status = status;
        record.LastStatus = status;
        record.BestScore = score;
        record.LastScore = score;
        record.CondId1 = condid1;
        record.CondVal1 = condval1;
        record.CondId3 = condid3;
        record.CondVal3 = condval3;

        if(cond2.Length%2 != 0) return;
        for(int i = 0;i< cond2.Length;i+=2)
        {
           record.Cond2.Add(cond2[i].ToString(),cond2[i]);

        }

        if (record.Status == (int)StageRecordStatus.Failure)
            return;
        record.dwUserId = DataBase.Instance.PLAYER.mUser.dwUserID;
        if (mUserStageRecord.ContainsKey(record.dwStageId.ToString()) == false)
        {
            record.Status = record.LastStatus;
            mUserStageRecord.Add(record.dwStageId.ToString(), record);
        }
        else
        {
            record.Status = mUserStageRecord[record.dwStageId.ToString()].Status;
            if (mUserStageRecord[record.dwStageId.ToString()].BestScore > record.BestScore)
                record.BestScore = mUserStageRecord[record.dwStageId.ToString()].BestScore;

            mUserStageRecord[record.dwStageId.ToString()] = record;
        }     
    }

    public void SetUserRecord(UserStageRecord record)
    {
        
        record.dwUserId = DataBase.Instance.PLAYER.mUser.dwUserID;
        if (mUserStageRecord.ContainsKey(record.dwStageId.ToString()) == false) 
        {
            record.Status = record.LastStatus;
            mUserStageRecord.Add(record.dwStageId.ToString(), record);
        }
        else
        {
            record.Status = mUserStageRecord[record.dwStageId.ToString()].Status;
            if (mUserStageRecord[record.dwStageId.ToString()].BestScore > record.BestScore)
                record.BestScore = mUserStageRecord[record.dwStageId.ToString()].BestScore;

            mUserStageRecord[record.dwStageId.ToString()] = record;
        }
    }
    /// <summary>
    /// 获取所有记录
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, UserStageRecord> GetUserAllStageRecord()
    {
        return mUserStageRecord;
    }

    /// <summary>
    /// 获取关卡记录
    /// </summary>
    /// <param name="stage">关卡编号</param>
    /// <returns></returns>
    public UserStageRecord GetUserStageRecord(uint stage)
    {
        if (mUserStageRecord.ContainsKey(stage.ToString()))
            return mUserStageRecord[stage.ToString()];
        else
            return null;
    }

    /// <summary>
    /// 获取所有关卡星星数和
    /// </summary>
    /// <returns></returns>
    public int GetTotalStar()
    {
        int Stars = 0;
        foreach(var e in mUserStageRecord.Values)
        {
            if ((StageRecordStatus)e.Status == StageRecordStatus.Success)
                Stars += COMMON_FUNC.GetStarByScore(e.dwStageId, e.BestScore);
        }

        return Stars;
    }
}
