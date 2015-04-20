using UnityEngine;
using System.Collections.Generic;


public class StageCondUtil
{

    List<INFO_STAGE_COND_LINK> stageCondTwo; //条件二有多个
    public StageCondUtil()
    {
        uint curStage = DataBase.Instance.PLAYER.CurStageId;
        stageCondTwo = DataBase.Instance.STAGE_MGR.GetStageCondInfoByGroup(curStage, StageCond.COND_GROUPTWO);
    }

    /// <summary>
    /// 任务配牌
    /// </summary>
    /// <param name="cardList"></param>
    /// <returns></returns>
    public byte[] StageDealCard(byte[] cardList)
    {
        foreach (INFO_STAGE_COND_LINK cond in stageCondTwo)
        {
            cardList = dealCard(cardList,cond);
        }
        return cardList;
    }
    private byte[] dealCard(byte[] cardList,INFO_STAGE_COND_LINK cond)
    {
        switch (cond.StageConditionId)
        {
            case StageCond.COND_OUTSINGLE: //出单张
                    if (cond.Condition > 0)
                       cardList = dealNoLink(cardList, 1, (int)cond.Condition);
                    break;

            case StageCond.COND_OUTDOUBLE: //出对子
                    cardList = dealNoLink(cardList, 2, (int)cond.Condition);
                    break;

            case StageCond.COND_OUTTHREE:  //出三条
                    cardList = dealNoLink(cardList, 3, (int)cond.Condition);
                    break;

            case StageCond.COND_SINGLELINK://出单连
                    cardList = dealLink(cardList, 1, (int)cond.Condition);
                    break;

            case StageCond.COND_DOUBLELINK://双连
                    cardList = dealLink(cardList, 2, (int)cond.Condition);
                    break;

            case StageCond.COND_THREELINK://三连
                    cardList = dealLink(cardList, 3, (int)cond.Condition);
                    break;

            case StageCond.COND_FOURLINK://四连
                    cardList = dealLink(cardList, 4, (int)cond.Condition);
                    break;

            case StageCond.COND_OUTBOMB://出炸弹
                {
                    //道具必有炸弹
                    if (DataBase.Instance.PLAYER.Property == PropertyMgr.PROPERTY_BOM)
                        cardList = dealNoLink(cardList, 4, (int)cond.Condition);
                    else
                        cardList = dealBomb(cardList, 4, (int)cond.Condition);
                    break;
                }
            default:
                break;
        }

        return cardList;

    }
    //关卡炸弹处理,三家都随机发炸弹
    private byte[] dealBomb(byte[] cardList, int cardNum, int cardValue)
    {
        List<int> ranList = new List<int>() { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        int temp = 0;
        if (cardValue == 0)
        {
            temp = Random.Range(0, ranList.Count - 1);
            cardValue = ranList[temp];
        }
        ranList.Remove(cardValue);

        for (int i = 0; i < 3; i++)
        {
            if (i == 1)
                cardList = dealNoLink(cardList, 4, cardValue);
            else
            {
                temp = Random.Range(0, ranList.Count - 1);
                int tempVal = ranList[temp];
                ranList.RemoveAt(temp);
                cardList = dealNoLink(cardList, 4, cardValue, i);
            }
        }

        return cardList;


    }
    //处理非连牌
    public byte[] dealNoLink(byte[] cardList, int cardNum, int cardValue, int player = 1)
    {
        List<byte> list = new List<byte>();
        list.AddRange(cardList);
        List<byte> tempList = new List<byte>();
        int offsetindex = player;
        int dealNum = 0;
        if (cardValue == 0)
        {
            if(cardNum == 4)
                cardValue = Random.Range(4, 13);
            else
                cardValue = Random.Range(1, 13);
        }
        for (int i = 0; i < cardList.Length; i++)
        {
            if (cardNum > dealNum && (cardList[i] & 0x0f) == cardValue)
            {
                tempList.Add(cardList[i]);
                list.Remove(cardList[i]);
                dealNum++;
            }
            if (dealNum == cardNum) break;
        }
        foreach (byte b in tempList)
        {
            list.Insert(offsetindex, b);
            offsetindex += 3;
        }

        return list.ToArray();

    }

    public byte[] dealLink(byte[] cardList, int LinkNum, int cardValue)
    {
        List<byte> list = new List<byte>();
        list.AddRange(cardList);
        List<byte> tempList = new List<byte>();
        int offsetindex = 1;
        int beignNum = 0;
        int endNum = 0;
        int linkLength = 0;
        if (cardValue == 0)
        {
            //随机连牌4-j
            beignNum = Random.Range(4, 11);
            endNum = beignNum + 2;
            linkLength = 3;
        }
        else
        {
            beignNum = cardValue >> 4;
            endNum = cardValue & 0x0f;
            linkLength = (endNum - beignNum) + 1;
        }
        //连牌长度
        int[] cardNum = new int[linkLength];
        for (int i = 0; i < cardList.Length; i++)
        {
            int cardVal = cardList[i] & 0x0f;
            if (cardVal >= beignNum && cardVal <= endNum && LinkNum > cardNum[cardVal - beignNum])
            {
                tempList.Add(cardList[i]);
                list.Remove(cardList[i]);
                cardNum[cardVal - beignNum]++;
            }

        }

        foreach (byte b in tempList)
        {
            list.Insert(offsetindex, b);
            offsetindex += 3;
        }

        return list.ToArray();
    }

}
