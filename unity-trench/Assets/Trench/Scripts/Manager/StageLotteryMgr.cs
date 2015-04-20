using System.Collections.Generic;
using UnityEngine;

public class StageLotteryMgr
{
    StageLottery lottery = new StageLottery();

    public void Load()
    {
        string data = Resources.Load(COMMON_CONST.PathStageLotteryData).ToString();
        lottery.XmlLoad(data, "StageLottery");
    }

    public List<INFO_STAGELOTTERY> GetAllLottery()
    {
        List<INFO_STAGELOTTERY> list = new List<INFO_STAGELOTTERY>();
        list.AddRange(lottery.INFO.Values);
        return list;
    }

    public INFO_STAGELOTTERY GetLottery(int lotteryId)
    {
        foreach (var e in lottery.INFO.Values)
        {
            if (e.Id == lotteryId)
                return e;
        }

        return null;
    }

    //根据抽奖配置获得所有抽奖道具
    public List<INFO_PROPERTY> GetLotteryProp()
    {
        List<INFO_PROPERTY> propes = new List<INFO_PROPERTY>();
        foreach (INFO_STAGELOTTERY info in lottery.INFO.Values)
        {
            INFO_PROPERTY prop = DataBase.Instance.PROP_MGR.GetProperty(info.AwardProID);
            propes.Add(prop);
        }
        
        return propes;
    }

    //根据配置获得抽奖结果
    public INFO_PROPERTY GetLotteryResult(INFO_STAGE stage)
    {
        //扣除抽奖费用
        uint score = stage.LotteryCost;
        if (stage.Pricetype == COMMON_CONST.PriceGold)
        {
            if (DataBase.Instance.PLAYER.Gold < score)
            {
                //失败
                LobbyDialogManager.inst.ShowLowCoinDialog(GoodType.Coin);
                return null;
            }
            DataBase.Instance.PLAYER.Gold -= score;
        }
        else
        {
            if (DataBase.Instance.PLAYER.ZScore < score)
            {
                LobbyDialogManager.inst.ShowLowCoinDialog(GoodType.Diamond);
                return null;
            }
            DataBase.Instance.PLAYER.ZScore -= score;
        }

        
        int toturChance = 0;
        foreach (INFO_STAGELOTTERY info in lottery.INFO.Values)
        {
            toturChance += info.Chance;
        }
        if (toturChance > 0)
        { 
            int chanceIndex = Random.Range(1, toturChance+1);
            int currChance = 0;
            foreach (INFO_STAGELOTTERY info in lottery.INFO.Values)
            {
                currChance += info.Chance;
                if (chanceIndex <= currChance)
                {
                    //记录用户获得的道具ID
                    DataBase.Instance.PLAYER.Property = info.AwardProID;
                    DataBase.Instance.PLAYER.IsUsableProperty = true;
                    //保存用户信息
                    DataBase.Instance.PLAYER.SaveUserInfo();
                    return DataBase.Instance.PROP_MGR.GetProperty(info.AwardProID);
                }
            }
        }

        return null;
    
    }
}
