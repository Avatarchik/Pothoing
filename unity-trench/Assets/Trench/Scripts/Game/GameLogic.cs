using System;
using System.Collections.Generic;
using UnityEngineEx.CMD.i3778;
using UnityEngineEx.Common;
using UnityEngineEx.LogInterface;

using WORD = System.UInt16;
using DWORD = System.UInt32;
using LONG = System.Int32;
using BYTE = System.Byte;


#region 牌型注释
//单牌	单张牌。比如一张3。
//对子	相同的两张牌。比如33。
//三条	相同的三张牌。比如444。
//四条（普通场）	相同的四张牌。比如5555。
//单顺	三张或更多的连续单张牌组成的牌型。比如456、45678910JQK等。A、2、3不能组成单顺。
//双顺（蹦蹦）	三对或更多的连续对子组成的牌型，比如445566或445566778899。A、2、3不能组成双顺。
//三顺	三个或更多的连续三条组成的牌型，比如444555666或555666777888等。A、2、3不能组成三顺。
//四顺	三个或更多的连续四条组成的牌型，比如444455556666或5555666677778888等。A、2、3不能组成四顺。
//炸弹（炸弹场）	在有炸弹的倍场中，四条就是炸弹。比如KKKK，3333。
//注意：四顺不算炸弹。
#endregion

/// <summary>
/// 游戏逻辑数据定义类
/// </summary>
public class GLogicDef
{
    //////////////////////////////////////////////////////////////////////////
    //排序类型
    //public const byte ST_ORDER = 0;									//大小排序
    //public const byte ST_COUNT = 1;									//数目排序
    //////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 数目定义
    /// </summary>
    public const int MAX_COUNT = 20;								//最大数目
    public const int FULL_COUNT = 52;								//全牌数目
    //public const int GOOD_CARD_COUTN = 36;						//好牌数目
    public const int BACK_COUNT = 4;							    //底牌数目
    public const int NORMAL_COUNT = 16;							    //常规数目

    /// <summary>
    ///  扑克类型
    /// </summary>
    public const byte CT_INVALID = 0;							    //错误类型
    public const byte CT_SINGLE = 1;							    //单牌类型
    public const byte CT_DOUBLE = 2;							    //对牌类型
    public const byte CT_THREE = 3;							        //三条类型
    public const byte CT_FOUR = 4;                                  //四条类型
    public const byte CT_SINGLE_LINE = 5;							//单连类型
    public const byte CT_DOUBLE_LINE = 6;							//对连类型
    public const byte CT_THREE_LINE = 7;							//三连类型
    public const byte CT_FOUR_LINE = 8;                             //四连类型
    public const byte CT_BOMB_CARD = 9;							    //炸弹类型

    public static readonly string[] CT_NAME_BY_TYPE = new string[10] { "错误类型", "单牌类型", 
        "对牌类型", "三条类型", "四条类型", "单连类型", "对连类型", "三连类型", "四连类型", "炸弹类型" };

    /// <summary>
    /// 数值掩码
    /// </summary>
    public const byte LOGIC_MASK_COLOR = 0xF0;						//花色掩码
    public const byte LOGIC_MASK_VALUE = 0x0F;						//数值掩码

    /// <summary>
    /// 扑克数据
    /// </summary>
    public static readonly byte[] CardData = new byte[FULL_COUNT] {    
        0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,0x0A,0x0B,0x0C,0x0D,	//方块 A - K
        0x11,0x12,0x13,0x14,0x15,0x16,0x17,0x18,0x19,0x1A,0x1B,0x1C,0x1D,	//梅花 A - K
        0x21,0x22,0x23,0x24,0x25,0x26,0x27,0x28,0x29,0x2A,0x2B,0x2C,0x2D,	//红桃 A - K
        0x31,0x32,0x33,0x34,0x35,0x36,0x37,0x38,0x39,0x3A,0x3B,0x3C,0x3D,	//黑桃 A - K
        //0x4E,0x4F,//大小王
    };
}

/// <summary>
/// 分析结构
/// </summary>
public class tagAnalyseResult
{
    public BYTE cbFourCount;						//四条数目
    public BYTE cbThreeCount;						//三条数目
    public BYTE cbDoubleCount;						//对子数目
    public BYTE cbSignedCount;						//单张数目
    public BYTE[] cbFourCardData = new BYTE[GLogicDef.MAX_COUNT];			//四条扑克
    public BYTE[] cbThreeCardData = new BYTE[GLogicDef.MAX_COUNT];			//三条扑克
    public BYTE[] cbDoubleCardData = new BYTE[GLogicDef.MAX_COUNT];		    //对子扑克
    public BYTE[] cbSignedCardData = new BYTE[GLogicDef.MAX_COUNT];		    //单张扑克	
};

/// <summary>
/// 出牌记录
/// </summary>
public class tagOutCardRecord
{
    public WORD wChairID;                                  //出牌用户
    public byte cbAction;                                  //255-无效，预制的 0-不要 1-出牌 2-跟牌
    public List<byte> outCard;                             //出牌数据

   public tagOutCardRecord()
    {
        wChairID = (WORD)GlobalDef.Deinfe.INVALID_CHAIR;
        cbAction = 255;
        outCard = new List<BYTE>();
    }

   public tagOutCardRecord(WORD wChairID, byte cbAction, List<byte> outCard)
   {
       this.wChairID = wChairID;
       this.cbAction = cbAction;
       this.outCard = outCard.ToList();
   }
}

/// <summary>
/// 分析牌型结果
/// </summary>
public class tagAnalyseCardTypeResult
{
    public BYTE cbCardType = GLogicDef.CT_INVALID;      //卡牌类型
    public List<BYTE> CardData = new List<BYTE>();      //卡牌数据
};

/// <summary>
/// 配牌信息
/// </summary>
public class tagConfigCard
{
    public BYTE cbCardType = GLogicDef.CT_INVALID;      //卡牌类型
    public BYTE cbCardData = new BYTE();                  //卡牌数据
    public WORD wChairID = new WORD();
};

/// <summary>
/// 游戏逻辑类
/// </summary>
public class GameLogic
{
    #region 私有类
    class CutCardResult
    {
        //切牌后的手数
        public int mHandNumber = 0;
        //切牌后牌型结果，如果拆牌需重新切牌
        public Dictionary<BYTE, List<tagAnalyseCardTypeResult>> mAllCardType = new Dictionary<BYTE, List<tagAnalyseCardTypeResult>>();
    }
    #endregion
    #region 数据成员
    const WORD wPlayerCount = 3;
    //启用炸弹
    bool bBomb = false;
    //坑主玩家
    WORD wBankerUser = (WORD)GlobalDef.Deinfe.INVALID_CHAIR;
    //当前底分
    byte cbLandScore = 0;
    //所有玩家卡牌
    Dictionary<WORD, List<byte>> mAllCards = new Dictionary<WORD, List<BYTE>> { };
    //底牌
    List<byte> mBackCard = new List<BYTE>();
    //每个玩家切牌后牌型结果，如果拆牌需重新切牌
    Dictionary<WORD, CutCardResult> mCutResult = new Dictionary<WORD, CutCardResult>();   
    //配牌类型
    List<tagConfigCard> mConfigCardType = new List<tagConfigCard>();
    //出牌记录
    List<tagOutCardRecord> mOutCardRecord = new List<tagOutCardRecord>();

    #endregion

    #region 属性函数
    public void SetBanker(WORD wChairID) 
    { 
        this.wBankerUser = wChairID;         
    }
    public void SetLandScore(byte cbLandScore) { this.cbLandScore = cbLandScore; }
    public void SetBomb(bool bBomb) { this.bBomb = bBomb; }
    public void SetUserCard(WORD wChairID, IList<byte> Cards)
    {
        var cbCard = Cards.ToList();
        mAllCards[wChairID].Clear();
        mAllCards[wChairID].AddRange(cbCard);
        SortCardList(mAllCards[wChairID], (byte)mAllCards[wChairID].Count);

        CutUserCard(wChairID, mAllCards[wChairID].ToArray(), (byte)mAllCards[wChairID].Count);
    }
    public List<byte> GetUserCard(WORD wChairID)
    {
        List<byte> lst = new List<byte>();
        lst.AddRange(mAllCards[wChairID]);
        return lst;
    }
    public int GetHandNumber(WORD wChairID) { return mCutResult[wChairID].mHandNumber; }
    public Dictionary<BYTE, List<tagAnalyseCardTypeResult>> GetCutResult(WORD wChairID) 
    {
        Dictionary<BYTE, List<tagAnalyseCardTypeResult>> dic = new Dictionary<BYTE, List<tagAnalyseCardTypeResult>>(mCutResult[wChairID].mAllCardType);
        return dic; 
    }
    public void SetBackCard(IList<byte> cards,byte cbCardCount)
    {
        mBackCard.Clear();
        
        cards.CopyList(mBackCard, cbCardCount);
        SortCardList(mBackCard, cbCardCount);

        mAllCards[wBankerUser].AddRange(mBackCard);
        SetUserCard(wBankerUser, mAllCards[wBankerUser]);         
    }
    #endregion

    //
    public GameLogic()
    {
        for (WORD i = 0; i < wPlayerCount; i++)
        {
            mAllCards.Add(i, new List<BYTE>());
            mCutResult.Add(i, new CutCardResult());
        }
    }

    public void ResetData()
    {
        //启用炸弹
        bBomb = false;
        //坑主玩家
        wBankerUser = (WORD)GlobalDef.Deinfe.INVALID_CHAIR;
        //当前底分
        cbLandScore = 0;

        mBackCard.Clear();
        mAllCards.Clear();
        mCutResult.Clear();
        for (WORD i = 0; i < wPlayerCount; i++)
        {
            mAllCards.Add(i, new List<BYTE>());
            mCutResult.Add(i, new CutCardResult());
        }
    }

    /// <summary>
    /// [min,max)区间随机值
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public int Rand(int min,int max)
    {
        System.Random rm = new Random(System.DateTime.Now.Millisecond);
        return rm.Next(min, max);
    }

    public bool CompareCard(byte[] bFirstList, byte[] bNextList, byte bFirstCount, byte bNextCount)
    {
        //byte bFirstCount = (byte)bFirstList.Length;
        //byte bNextCount = (byte)bNextList.Length;
        //获取类型
        byte bNextType = GetCardType(bNextList, bNextCount);
        byte bFirstType = GetCardType(bFirstList, bFirstCount);

        //类型判断
        if (bFirstType == GLogicDef.CT_INVALID) return false;
        if (bNextType == GLogicDef.CT_INVALID) return false;

        //炸弹判断
        if ((bFirstType != GLogicDef.CT_BOMB_CARD) && (bNextType == GLogicDef.CT_BOMB_CARD)) return true;

        if ((bFirstType == GLogicDef.CT_BOMB_CARD) && (bNextType != GLogicDef.CT_BOMB_CARD)) return false;

        //规则判断
        if ((bFirstType != bNextType) || (bFirstCount != bNextCount)) return false;

        //开始对比
        switch (bNextType)
        {
            case GLogicDef.CT_SINGLE://单
            case GLogicDef.CT_DOUBLE://对
            case GLogicDef.CT_THREE://三
            case GLogicDef.CT_FOUR://四
            case GLogicDef.CT_SINGLE_LINE:
            case GLogicDef.CT_DOUBLE_LINE:
            case GLogicDef.CT_THREE_LINE:
            case GLogicDef.CT_FOUR_LINE:
            case GLogicDef.CT_BOMB_CARD:
                {
                    //获取数值
                    byte cbNextLogicValue = GetCardLogicValue(bNextList[0]);
                    byte cbFirstLogicValue = GetCardLogicValue(bFirstList[0]);

                    //对比扑克
                    return cbNextLogicValue > cbFirstLogicValue;
                }
        }

        return false;

    }

    //获取数值
    public byte GetCardValue(byte bCardData)
    {
        return (byte)(bCardData & GLogicDef.LOGIC_MASK_VALUE);
    }

    //获取花色
    public byte GetCardColor(byte bCardData)
    {
        return (byte)(bCardData & GLogicDef.LOGIC_MASK_COLOR);
    }

    //逻辑数值
    public byte GetCardLogicValue(byte bCardData)
    {
        //点数的大小： 3>2>A>K>Q>J>10>9>8>7>6>5>4
        //扑克属性
        byte bCardColor = GetCardColor(bCardData);
        byte bCardValue = GetCardValue(bCardData);

        //转换数值
        return (bCardValue <= 3) ? (byte)(bCardValue + 13) : (byte)bCardValue;
    }

    //排列扑克
    public void SortCardList(IList<byte> cbCardData, byte cbCardCount, byte fullNum = GLogicDef.MAX_COUNT)
    {
        //byte cbCardCount = (byte)cbCardData.Length;
        //数目过虑
        if (cbCardCount == 0) return;
        if (cbCardCount > fullNum) return;

        //转换数值
        BYTE[] cbSortValue = new BYTE[fullNum];
        for (BYTE i = 0; i < cbCardCount; i++) cbSortValue[i] = GetCardLogicValue(cbCardData[i]);

        //排序操作
        bool bSorted = true;
        BYTE cbThreeCount, cbLast = (byte)(cbCardCount - 1);
        do
        {
            bSorted = true;
            for (BYTE i = 0; i < cbLast; i++)
            {
                if ((cbSortValue[i] < cbSortValue[i + 1]) ||
                    ((cbSortValue[i] == cbSortValue[i + 1]) && (cbCardData[i] < cbCardData[i + 1])))
                {
                    //交换位置
                    cbThreeCount = cbCardData[i];
                    cbCardData[i] = cbCardData[i + 1];
                    cbCardData[i + 1] = cbThreeCount;
                    cbThreeCount = cbSortValue[i];
                    cbSortValue[i] = cbSortValue[i + 1];
                    cbSortValue[i + 1] = cbThreeCount;
                    bSorted = false;
                }
            }
            cbLast--;
        } while (bSorted == false);

        //数目排序
        //if (cbSortType == GLogicDef.ST_COUNT)
        //{
        //    //分析扑克
        //    BYTE cbIndex = 0;
        //    tagAnalyseResult AnalyseResult = new tagAnalyseResult();
        //    AnalysebCardData(cbCardData, cbCardCount, ref AnalyseResult);

        //    //拷贝四牌
        //    System.Array.Copy(AnalyseResult.cbFourCardData, 0, cbCardData, cbIndex, AnalyseResult.cbFourCount * 4);
        //    //CopyMemory(&cbCardData[cbIndex],AnalyseResult.cbFourCardData,sizeof(BYTE)*AnalyseResult.cbFourCount*4);
        //    cbIndex += (byte)(AnalyseResult.cbFourCount * 4);

        //    //拷贝三牌
        //    System.Array.Copy(AnalyseResult.cbFourCardData, 0, cbCardData, cbIndex, AnalyseResult.cbThreeCount * 3);
        //    //CopyMemory(&cbCardData[cbIndex],AnalyseResult.cbThreeCardData,sizeof(BYTE)*AnalyseResult.cbThreeCount*3);
        //    cbIndex += (byte)(AnalyseResult.cbThreeCount * 3);

        //    //拷贝对牌
        //    System.Array.Copy(AnalyseResult.cbFourCardData, 0, cbCardData, cbIndex, AnalyseResult.cbDoubleCount * 2);
        //    //CopyMemory(&cbCardData[cbIndex],AnalyseResult.cbDoubleCardData,sizeof(BYTE)*AnalyseResult.cbDoubleCount*2);
        //    cbIndex += (byte)(AnalyseResult.cbDoubleCount * 2);

        //    //拷贝单牌
        //    System.Array.Copy(AnalyseResult.cbFourCardData, 0, cbCardData, cbIndex, AnalyseResult.cbSignedCount);
        //    //CopyMemory(&cbCardData[cbIndex],AnalyseResult.cbSignedCardData,sizeof(BYTE)*AnalyseResult.cbSignedCount);
        //    cbIndex += (byte)(AnalyseResult.cbSignedCount);
        //}
    }

    //删除扑克
    public bool RemoveCard(byte[] bRemoveCard,byte cbRemoveCount, List<byte> bCardData)
    {
        bool bres = false;

        byte[] cards = bCardData.ToArray();
        bres = RemoveCard(bRemoveCard, cbRemoveCount, cards, (byte)bCardData.Count);
        bCardData.Clear();
        foreach (var e in cards)
        {
            if (e != 0) bCardData.Add(e);
        }

        return bres;
    }

    //删除扑克
    public bool RemoveCard(byte[] bRemoveCard, byte bRemoveCardCnt, byte[] bCardData, byte bCardDataCnt, byte fullNum = GLogicDef.MAX_COUNT)
    {
        int bRemoveCount = bRemoveCardCnt;
        int bCardCount = bCardDataCnt;
        //检验数据
        if (bRemoveCount > bCardCount)
        {
            return false;
        }

        //定义变量
        byte bDeleteCount = 0;
        byte[] bTempCardData = new byte[fullNum];
        if (bCardCount > bTempCardData.Length) return false;
        bCardData.CopyTo(bTempCardData, 0);
        bCardData.ArraySetAll((byte)0);

        //置零扑克
        for (byte i = 0; i < bRemoveCount; i++)
        {
            for (byte j = 0; j < bCardCount; j++)
            {
                if (bRemoveCard[i] == bTempCardData[j])
                {
                    bDeleteCount++;
                    bTempCardData[j] = 0;
                    break;
                }
            }
        }
        if (bDeleteCount != bRemoveCount)
        {
            return false;
        }

        //清理扑克
        byte bCardPos = 0;
        for (byte i = 0; i < bCardCount; i++)
        {
            if (bTempCardData[i] != 0) bCardData[bCardPos++] = bTempCardData[i];
        }

        return true;
    }    

    //获取类型
    public byte GetCardType(byte[] cbCardData, byte cbCardCount)
    {
        //SortCardList(cbCardData,cbCardCount);

        //简单牌型
        switch (cbCardCount)
        {
            case 0:	//空牌
                {
                    return GLogicDef.CT_INVALID;
                }
            case 1: //单牌
                {
                    return GLogicDef.CT_SINGLE;
                }
            case 2:	//对牌
                {
                    //牌型
                    if (GetCardLogicValue(cbCardData[0]) == GetCardLogicValue(cbCardData[1]))
                        return GLogicDef.CT_DOUBLE;

                    return GLogicDef.CT_INVALID;
                }
        }

        //其他牌型
        //分析扑克
        tagAnalyseResult AnalyseResult = new tagAnalyseResult();
        if (!AnalysebCardData(cbCardData, cbCardCount, AnalyseResult))
            return GLogicDef.CT_INVALID;

        //四条|四连 判断
        if (AnalyseResult.cbFourCount > 0)
        {
            //牌型判断四条
            if ((AnalyseResult.cbFourCount == 1) && (cbCardCount == 4))
                return (bBomb ? GLogicDef.CT_BOMB_CARD : GLogicDef.CT_FOUR);

            //过滤其它带牌
            if (AnalyseResult.cbSignedCount > 0 || AnalyseResult.cbDoubleCount > 0 || AnalyseResult.cbThreeCount > 0)
            {
                return GLogicDef.CT_INVALID;
            }

            //四连判断
            if (IsFourLineCardType(cbCardData, cbCardCount))
            {
                return GLogicDef.CT_FOUR_LINE;
            }

            return GLogicDef.CT_INVALID;
        }

        //三条|三连 判断
        if (AnalyseResult.cbThreeCount > 0)
        {
            //三条类型
            if (AnalyseResult.cbThreeCount == 1 && cbCardCount == 3)
                return GLogicDef.CT_THREE;

            //过滤其它带牌
            if (AnalyseResult.cbSignedCount > 0 || AnalyseResult.cbDoubleCount > 0)
            {
                return GLogicDef.CT_INVALID;
            }

            if (IsThreeLineCardType(cbCardData, cbCardCount))
            {
                return GLogicDef.CT_THREE_LINE;
            }

            return GLogicDef.CT_INVALID;
        }

        //两连 判断
        if (AnalyseResult.cbDoubleCount >= 3 && cbCardCount >= 6)
        {
            //过滤其它带牌
            if (AnalyseResult.cbSignedCount > 0)
            {
                return GLogicDef.CT_INVALID;
            }

            if (IsDoubleLineCardType(cbCardData, cbCardCount))
            {
                return GLogicDef.CT_DOUBLE_LINE;
            }
        }

        //单连 判断
        if ((AnalyseResult.cbSignedCount >= 3) && (AnalyseResult.cbSignedCount == cbCardCount))
        {
            //过滤其它带牌
            if (AnalyseResult.cbDoubleCount > 0)
            {
                return GLogicDef.CT_INVALID;
            }

            if (IsSingleLineCardType(cbCardData, cbCardCount))
            {
                return GLogicDef.CT_SINGLE_LINE;
            }
        }
        return GLogicDef.CT_INVALID;
    }

    //分析扑克
    public bool AnalysebCardData(byte[] pcbCardData, byte cbCardCount, tagAnalyseResult AnalyseResult)
    {
        byte[] cbCardData = new byte[cbCardCount];
        System.Array.Copy(pcbCardData, cbCardData, cbCardCount);

        //从大到小排序
        //System.Array.Sort(cbCardData, (a, b) => { return GetCardLogicValue(a).CompareTo(GetCardLogicValue(b)); });

        //扑克分析
        for (BYTE i = 0; i < cbCardCount; i++)
        {
            //变量定义
            BYTE cbSameCount = 1;
            BYTE cbLogicValue = GetCardLogicValue(cbCardData[i]);

            if (cbLogicValue <= 0) return false;

            //搜索同牌
            for (BYTE j = (byte)(i + 1); j < cbCardCount; j++)
            {
                //获取扑克
                if (GetCardLogicValue(cbCardData[j]) != cbLogicValue) break;

                //设置变量
                cbSameCount++;
            }

            //设置结果
            switch (cbSameCount)
            {
                case 1:		//单张
                    {
                        BYTE cbIndex = AnalyseResult.cbSignedCount++;
                        AnalyseResult.cbSignedCardData[cbIndex * cbSameCount] = cbCardData[i];
                        break;
                    }
                case 2:		//两张
                    {
                        BYTE cbIndex = AnalyseResult.cbDoubleCount++;
                        AnalyseResult.cbDoubleCardData[cbIndex * cbSameCount] = cbCardData[i];
                        AnalyseResult.cbDoubleCardData[cbIndex * cbSameCount + 1] = cbCardData[i + 1];
                        break;
                    }
                case 3:		//三条
                    {
                        BYTE cbIndex = AnalyseResult.cbThreeCount++;
                        AnalyseResult.cbThreeCardData[cbIndex * cbSameCount] = cbCardData[i];
                        AnalyseResult.cbThreeCardData[cbIndex * cbSameCount + 1] = cbCardData[i + 1];
                        AnalyseResult.cbThreeCardData[cbIndex * cbSameCount + 2] = cbCardData[i + 2];
                        break;
                    }
                case 4:		//四条
                    {
                        BYTE cbIndex = AnalyseResult.cbFourCount++;
                        AnalyseResult.cbFourCardData[cbIndex * cbSameCount] = cbCardData[i];
                        AnalyseResult.cbFourCardData[cbIndex * cbSameCount + 1] = cbCardData[i + 1];
                        AnalyseResult.cbFourCardData[cbIndex * cbSameCount + 2] = cbCardData[i + 2];
                        AnalyseResult.cbFourCardData[cbIndex * cbSameCount + 3] = cbCardData[i + 3];
                        break;
                    }
            }

            //设置索引
            i += (byte)(cbSameCount - 1);
        }

        return true;
    }

    //是否有对牌//条件：>= 2
    //public bool HaveSameCard(byte[] bCardData, byte bCardCount)
    //{
    //    bool resultBool = false;
    //    if (bCardCount < 2)
    //        return resultBool;

    //    //其他牌型
    //    if (bCardCount >= 2)
    //    {
    //        //分析扑克
    //        tagAnalyseResult AnalyseResult = new tagAnalyseResult();
    //        AnalysebCardData(bCardData, bCardCount, AnalyseResult);

    //        if ((AnalyseResult.cbDoubleCount > 0)
    //            || (AnalyseResult.cbFourCount > 0)
    //            || (AnalyseResult.cbThreeCount > 0))
    //        {
    //            resultBool = true;
    //        }
    //    }

    //    return resultBool;
    //}

    //是否为单联//条件：>= 3 (从大到小已排序)
    public bool IsSingleLineCardType(byte[] bCardData, byte cbCardCount)
    {
        bool resultBool = false;
        if (cbCardCount < 3)
        {
            return resultBool;
        }

        //分析扑克
        tagAnalyseResult AnalyseResult = new tagAnalyseResult();
        AnalysebCardData(bCardData, cbCardCount, AnalyseResult);

        //如果有对牌
        if (AnalyseResult.cbDoubleCount > 0 || AnalyseResult.cbThreeCount > 0 || AnalyseResult.cbFourCount > 0)
        {
            return resultBool;
        }

        //其他牌型
        if (cbCardCount >= 3)
        {
            if ((AnalyseResult.cbSignedCount >= 3) && (AnalyseResult.cbSignedCount == cbCardCount))
            {
                byte bLogicValue = GetCardLogicValue(bCardData[0]);
                //3,2,A不能构成单连
                if (bLogicValue >= 14)
                {
                    return resultBool;
                }
                //连牌判断
                for (byte i = 1; i < AnalyseResult.cbSignedCount; i++)
                {
                    if (GetCardLogicValue(bCardData[i]) != (bLogicValue - i))
                    {
                        return resultBool;
                    }
                }

                resultBool = true;
                return resultBool;
            }
        }

        return resultBool;
    }

    //是否为双联//条件：>= 6
    public bool IsDoubleLineCardType(byte[] CardData, byte cbCardCount)
    {
        bool resultBool = false;
        //bool bThreeLian = false;

        if (cbCardCount < 6)
        {
            return resultBool;
        }

        //分析扑克
        tagAnalyseResult AnalyseResult = new tagAnalyseResult();
        AnalysebCardData(CardData, cbCardCount, AnalyseResult);

        if (AnalyseResult.cbDoubleCount >= 3 && cbCardCount % 2 == 0 && AnalyseResult.cbDoubleCount == cbCardCount / 2)
        {
            BYTE bLogicValue = GetCardLogicValue(CardData[0]);

            if (bLogicValue >= 14)
            {
                return resultBool;
            }

            //连牌判断
            for (BYTE i = 1; i < AnalyseResult.cbDoubleCount; i++)
            {
                BYTE bCardData = AnalyseResult.cbDoubleCardData[i * 2];

                if ((bLogicValue - i) != (GetCardLogicValue(bCardData)))
                {
                    return resultBool;
                }
            }

            resultBool = true;
        }

        return resultBool;
    }

    public bool IsThreeLineCardType(byte[] cbCardData, byte cbCardCount)
    {
        bool resultBool = false;
        if (cbCardCount < 9) return resultBool;

        //分析扑克
        tagAnalyseResult AnalyseResult = new tagAnalyseResult();
        AnalysebCardData(cbCardData, cbCardCount, AnalyseResult);

        if (AnalyseResult.cbThreeCount < 3
            || cbCardCount % 3 != 0
            || AnalyseResult.cbThreeCount != cbCardCount / 3) return resultBool;

        BYTE bLogicValue = GetCardLogicValue(cbCardData[0]);

        if (bLogicValue >= 14) return resultBool;

        //连牌判断
        for (BYTE i = 1; i < AnalyseResult.cbThreeCount; i++)
        {
            BYTE bCardData = AnalyseResult.cbThreeCardData[i * 3];

            if ((bLogicValue - i) != (GetCardLogicValue(bCardData)))
            {
                return resultBool;
            }
        }

        return resultBool;
    }

    public bool IsFourLineCardType(byte[] cbCardData, byte cbCardCount)
    {
        bool resultBool = false;
        if (cbCardCount < 12) return resultBool;

        //分析扑克
        tagAnalyseResult AnalyseResult = new tagAnalyseResult();
        AnalysebCardData(cbCardData, cbCardCount, AnalyseResult);

        if (AnalyseResult.cbFourCount < 3
            || cbCardCount % 4 != 0
            || AnalyseResult.cbFourCount != cbCardCount / 4) return resultBool;

        BYTE bLogicValue = GetCardLogicValue(cbCardData[0]);

        if (bLogicValue >= 14) return resultBool;

        //连牌判断
        for (BYTE i = 1; i < AnalyseResult.cbFourCount; i++)
        {
            BYTE bCardData = AnalyseResult.cbFourCardData[i * 4];

            if ((bLogicValue - i) != (GetCardLogicValue(bCardData)))
            {
                return resultBool;
            }
        }

        return resultBool;
    }

    #region 切牌部分
    //切牌
    /// <summary>
    /// 切牌
    /// </summary>
    /// <param name="cbCardData"></param>
    /// <param name="cbCardCount"></param>
    /// <returns></returns>
    //public int CutCards(IList<byte> cbCardData, byte cbCardCount)
    //{
    //    //1.321作为一手
    //    //2.炸弹作为一手(若存在，4将忽略)
    //    //3.取出所有不重复的单连
    //    //4.取出所有四条(若存在，2将忽略) ,三条 ,对子
    //    //5.合并单牌到单连 
    //    //方法：
    //    //      1.连续2张的边界值与存在的单连边界值绝对值差1
    //    //      2.单张插牌
    //    //      其他不合并

    //    //6.合单连成双连，三连，四连
    //    //List<tagAnalyseCardTypeResult> result = new List<tagAnalyseCardTypeResult>();
    //    mHandNumber = 0;
    //    mAllCardTypeCard.Clear();

    //    List<tagAnalyseCardTypeResult> link = new List<tagAnalyseCardTypeResult>();
    //    List<tagAnalyseCardTypeResult> nolink = new List<tagAnalyseCardTypeResult>();

    //    tagAnalyseCardTypeResult bomb = new tagAnalyseCardTypeResult();
    //    List<byte> remain = new List<BYTE>();

    //    if (bBomb == true) GetAllFourCard(cbCardData.ToList().ToArray(), cbCardCount, bomb, remain);
    //    else cbCardData.CopyList(remain, cbCardCount);
    //    GetAllLineCard(remain.ToArray(), (byte)remain.Count, link, remain);
    //    GetNotLineCard(remain.ToArray(), (byte)remain.Count, nolink); ;
    //    MergeAllLineCard(link, nolink);

    //    if (bomb.cbCardType != GLogicDef.CT_INVALID)
    //    {
    //        bomb.cbCardType = GLogicDef.CT_BOMB_CARD;
    //        nolink.Add(bomb);
    //    }

    //    foreach (var e in link)
    //    {
    //        if (mAllCardTypeCard.ContainsKey(e.cbCardType) == false)
    //            mAllCardTypeCard.Add(e.cbCardType, new List<tagAnalyseCardTypeResult>());

    //        mAllCardTypeCard[e.cbCardType].Add(e);

    //        mHandNumber++;
    //    }

    //    foreach (var e in nolink)
    //    {
    //        if (mAllCardTypeCard.ContainsKey(e.cbCardType) == false)
    //            mAllCardTypeCard.Add(e.cbCardType, new List<tagAnalyseCardTypeResult>());
    //        mAllCardTypeCard[e.cbCardType].Add(e);

    //        switch (e.cbCardType)
    //        {
    //            case GLogicDef.CT_DOUBLE:
    //                mHandNumber += e.CardData.Count / 2;
    //                break;
    //            case GLogicDef.CT_THREE:
    //                mHandNumber += e.CardData.Count / 3;
    //                break;
    //            case GLogicDef.CT_FOUR:
    //                mHandNumber += e.CardData.Count / 4;
    //                break;
    //            case GLogicDef.CT_BOMB_CARD:
    //                mHandNumber += e.CardData.Count / 4;
    //                break;
    //            case GLogicDef.CT_SINGLE:
    //                mHandNumber += e.CardData.Count;
    //                break;
    //        }
    //    }

    //    return mHandNumber;
    //}

    /// <summary>
    /// 临时切牌
    /// </summary>
    /// <param name="cbCardData">要切的牌</param>
    /// <param name="cbCardCount">要切牌的个数</param>
    /// <param name="resAllCardTypeCard">返回切牌结果</param>
    /// <returns>返回切牌的手数</returns>
    public int CutCards(IList<byte> cbCardData, byte cbCardCount, Dictionary<BYTE, List<tagAnalyseCardTypeResult>> resAllCardTypeCard = null)
    {
        //1.321作为一手
        //2.炸弹作为一手(若存在，4将忽略)
        //3.取出所有不重复的单连
        //4.取出所有四条(若存在，2将忽略) ,三条 ,对子
        //5.合并单牌到单连 
        //方法：
        //      1.连续2张的边界值与存在的单连边界值绝对值差1
        //      2.单张插牌
        //      其他不合并

        //6.合单连成双连，三连，四连
        //List<tagAnalyseCardTypeResult> result = new List<tagAnalyseCardTypeResult>();
        int mHandNumber = 0;
        if (resAllCardTypeCard!=null) resAllCardTypeCard.Clear();

        List<tagAnalyseCardTypeResult> link = new List<tagAnalyseCardTypeResult>();
        List<tagAnalyseCardTypeResult> nolink = new List<tagAnalyseCardTypeResult>();

        tagAnalyseCardTypeResult bomb = new tagAnalyseCardTypeResult();
        List<byte> remain = new List<BYTE>();

        if (bBomb == true) GetAllFourCard(cbCardData.ToList().ToArray(), cbCardCount, bomb, remain);
        else cbCardData.CopyList(remain, cbCardCount);
        GetAllLineCard(remain.ToArray(), (byte)remain.Count, link, remain);
        GetNotLineCard(remain.ToArray(), (byte)remain.Count, nolink); ;
        MergeAllLineCard(link, nolink);

        if (bomb.cbCardType != GLogicDef.CT_INVALID)
        {
            bomb.cbCardType = GLogicDef.CT_BOMB_CARD;
            nolink.Add(bomb);
        }

        foreach (var e in link)
        {
            if (resAllCardTypeCard != null)
            {
                if (resAllCardTypeCard.ContainsKey(e.cbCardType) == false)
                    resAllCardTypeCard.Add(e.cbCardType, new List<tagAnalyseCardTypeResult>());

                resAllCardTypeCard[e.cbCardType].Add(e);
            }
            
            mHandNumber++;
        }

        foreach (var e in nolink)
        {
            if (resAllCardTypeCard != null)
            {
                if (resAllCardTypeCard.ContainsKey(e.cbCardType) == false)
                    resAllCardTypeCard.Add(e.cbCardType, new List<tagAnalyseCardTypeResult>());
                resAllCardTypeCard[e.cbCardType].Add(e);
            }
            

            switch (e.cbCardType)
            {
                case GLogicDef.CT_DOUBLE:
                    mHandNumber += e.CardData.Count / 2;
                    break;
                case GLogicDef.CT_THREE:
                    mHandNumber += e.CardData.Count / 3;
                    break;
                case GLogicDef.CT_FOUR:
                    mHandNumber += e.CardData.Count / 4;
                    break;
                case GLogicDef.CT_BOMB_CARD:
                    mHandNumber += e.CardData.Count / 4;
                    break;
                case GLogicDef.CT_SINGLE:
                    mHandNumber += e.CardData.Count;
                    break;
            }
        }

        return mHandNumber;
    }

    /// <summary>
    /// 获取四张（炸弹）
    /// </summary>
    /// <param name="cbHandCardData"></param>
    /// <param name="cbHandCardCount"></param>
    /// <param name="bombResult"></param>
    /// <param name="cbRemainCard"></param>
    public void GetAllFourCard(BYTE[] cbHandCardData, BYTE cbHandCardCount, tagAnalyseCardTypeResult bombResult, List<byte> cbRemainCard)
    {
        bombResult.cbCardType = GLogicDef.CT_INVALID;
        
        byte[] cbTemp = new BYTE[cbHandCardCount];
        Buffer.BlockCopy(cbHandCardData, 0, cbTemp, 0, cbHandCardCount);
        SortCardList(cbTemp, cbHandCardCount);

        cbTemp.CopyList(cbRemainCard, cbHandCardCount, 0);

        tagAnalyseResult analyse = new tagAnalyseResult();
        AnalysebCardData(cbTemp, cbHandCardCount, analyse);
        if (analyse.cbFourCount > 0)
        {
            bombResult.cbCardType = bBomb ? GLogicDef.CT_BOMB_CARD : GLogicDef.CT_FOUR;
            analyse.cbFourCardData.CopyList(bombResult.CardData, analyse.cbFourCount * 4);

            //删除牌型扑克
            for (byte i = 0; i < analyse.cbFourCount; ++i)
            {
                for (int j = 0; j < 4; j++)
                {
                    cbRemainCard.Remove(analyse.cbFourCardData[i + j]);
                }
            }
        }
    }

    /// <summary>
    /// 获取所有三张
    /// </summary>
    /// <param name="cbHandCardData"></param>
    /// <param name="cbHandCardCount"></param>
    /// <param name="resResult"></param>
    /// <param name="cbRemainCard"></param>
    public void GetAllThreeCard(BYTE[] cbHandCardData, BYTE cbHandCardCount, tagAnalyseCardTypeResult resResult, List<byte> cbRemainCard)
    {
        resResult.cbCardType = GLogicDef.CT_INVALID;
        cbHandCardData.CopyList(cbRemainCard, cbHandCardCount, 0);

        byte[] cbTemp = new BYTE[cbHandCardCount];
        Buffer.BlockCopy(cbHandCardData, 0, cbTemp, 0, cbHandCardCount);
        SortCardList(cbTemp, cbHandCardCount);
        tagAnalyseResult analyse = new tagAnalyseResult();
        AnalysebCardData(cbTemp, cbHandCardCount, analyse);
        if (analyse.cbFourCount > 0 || analyse.cbThreeCount > 0)
        {
            resResult.cbCardType = GLogicDef.CT_THREE;
            for (byte i = 0; i < analyse.cbFourCount; ++i)
            {
                analyse.cbFourCardData.CopyList(resResult.CardData, 3, i * 4);
            }

            if (analyse.cbThreeCount > 0)
            {
                analyse.cbThreeCardData.CopyList(resResult.CardData, analyse.cbThreeCount * 3);
            }

            //删除剩余
            for (int i = 0; i < resResult.CardData.Count; ++i)
            {
                cbRemainCard.Remove(resResult.CardData[i]);
            }

        }
    }

    /// <summary>
    /// 获取所有对牌
    /// </summary>
    /// <param name="cbHandCardData"></param>
    /// <param name="cbHandCardCount"></param>
    /// <param name="resResult"></param>
    /// <param name="cbRemainCard"></param>
    public void GetAllTwoCard(BYTE[] cbHandCardData, BYTE cbHandCardCount, tagAnalyseCardTypeResult resResult, List<byte> cbRemainCard)
    {
        resResult.cbCardType = GLogicDef.CT_INVALID;
        cbHandCardData.CopyList(cbRemainCard, cbHandCardCount, 0);

        byte[] cbTemp = new BYTE[cbHandCardCount];
        Buffer.BlockCopy(cbHandCardData, 0, cbTemp, 0, cbHandCardCount);
        SortCardList(cbTemp, cbHandCardCount);
        tagAnalyseResult analyse = new tagAnalyseResult();
        AnalysebCardData(cbTemp, cbHandCardCount, analyse);
        if (analyse.cbFourCount > 0 || analyse.cbThreeCount > 0 || analyse.cbDoubleCount > 0)
        {
            resResult.cbCardType = GLogicDef.CT_DOUBLE;
            if (analyse.cbFourCount > 0)
            {
                analyse.cbFourCardData.CopyList(resResult.CardData, analyse.cbFourCount * 4);
            }
            for (byte i = 0; i < analyse.cbThreeCount; ++i)
            {
                analyse.cbThreeCardData.CopyList(resResult.CardData, 2, i * 3);
            }

            if (analyse.cbDoubleCount > 0)
            {
                analyse.cbDoubleCardData.CopyList(resResult.CardData, analyse.cbDoubleCount * 2);
            }

            //删除剩余
            for (int i = 0; i < resResult.CardData.Count; ++i)
            {
                cbRemainCard.Remove(resResult.CardData[i]);
            }

        }
    }

    /// <summary>
    /// 获取所有对子
    /// </summary>
    /// <param name="cbHandCardData"></param>
    /// <param name="cbHandCardCount"></param>
    /// <param name="cbDoubleCardData"></param>
    /// <param name="cbDoubleCardCount"></param>
    public void GetAllDoubleCard(BYTE[] cbHandCardData, BYTE cbHandCardCount, BYTE[] cbDoubleCardData, ref BYTE cbDoubleCardCount)
    {
        BYTE[] cbTmpCardData = new BYTE[GLogicDef.MAX_COUNT];
        Buffer.BlockCopy(cbHandCardData, 0, cbTmpCardData, 0, cbHandCardCount);
        //CopyMemory(cbTmpCardData, cbHandCardData, cbHandCardCount) ;

        //大小排序
        SortCardList(cbTmpCardData, cbHandCardCount);
        cbDoubleCardCount = 0;

        //扑克分析
        for (BYTE i = 0; i < cbHandCardCount; i++)
        {
            //变量定义
            BYTE cbSameCount = 1;
            BYTE cbLogicValue = GetCardLogicValue(cbTmpCardData[i]);

            //搜索同牌
            for (BYTE j = (BYTE)(i + 1); j < cbHandCardCount; j++)
            {
                //获取扑克
                if (GetCardLogicValue(cbTmpCardData[j]) != cbLogicValue) break;

                //设置变量
                cbSameCount++;
            }

            if (cbSameCount >= 2)
            {
                cbDoubleCardData[cbDoubleCardCount++] = cbTmpCardData[i];
                cbDoubleCardData[cbDoubleCardCount++] = cbTmpCardData[i + 1];
            }

            //设置索引
            i += (BYTE)(cbSameCount - 1);
        }
    }

    /// <summary>
    /// 获取所有单连
    /// </summary>
    /// <param name="cbHandCardData"></param>
    /// <param name="cbHandCardCount"></param>
    /// <param name="lstResult"></param>
    /// <param name="lstRemain"></param>
    public void GetAllLineCard(BYTE[] cbHandCardData, BYTE cbHandCardCount, List<tagAnalyseCardTypeResult> lstResult, List<byte> lstRemain)
    {
        lstRemain.Clear();
        BYTE[] cbTmpCard = new BYTE[GLogicDef.MAX_COUNT];
        //CopyMemory(cbTmpCard, cbHandCardData, cbHandCardCount) ;
        Buffer.BlockCopy(cbHandCardData, 0, cbTmpCard, 0, cbHandCardCount);

        //大小排序
        SortCardList(cbTmpCard, cbHandCardCount); //从大到小

        //数据校验
        if (cbHandCardCount < 3)
        {
            cbTmpCard.CopyList(lstRemain, cbHandCardCount);
            return;
        }
        //去除123
        BYTE cbFirstCard = 0;
        for (BYTE i = 0; i < cbHandCardCount; ++i)
        {
            if (GetCardLogicValue(cbTmpCard[i]) < 14)
            {
                cbFirstCard = i;
                break;
            }
        }
        BYTE[] cbSingleLineCard = new BYTE[12];
        BYTE cbSingleLineCount = 0;
        BYTE cbLeftCardCount = cbHandCardCount;
        bool bFindSingleLine = true;

        //连牌判断
        while (cbLeftCardCount >= 3 && bFindSingleLine)
        {
            cbSingleLineCount = 1;
            bFindSingleLine = false;
            BYTE cbLastCard = cbTmpCard[cbFirstCard];
            cbSingleLineCard[cbSingleLineCount - 1] = cbTmpCard[cbFirstCard];
            for (BYTE i = (byte)(cbFirstCard + 1); i < cbLeftCardCount; i++)
            {
                BYTE cbCardData = cbTmpCard[i];

                //连续判断
                if (1 != (GetCardLogicValue(cbLastCard) - GetCardLogicValue(cbCardData)) && GetCardValue(cbLastCard) != GetCardValue(cbCardData)) //不是连子并且不相等
                {
                    cbLastCard = cbTmpCard[i];
                    if (cbSingleLineCount < 3)
                    {
                        cbSingleLineCount = 1;
                        cbSingleLineCard[cbSingleLineCount - 1] = cbTmpCard[i];
                        continue;
                    }
                    else break;
                }
                //同牌判断
                else if (GetCardValue(cbLastCard) != GetCardValue(cbCardData))
                {
                    cbLastCard = cbCardData;
                    cbSingleLineCard[cbSingleLineCount] = cbCardData;
                    ++cbSingleLineCount;
                }
            }

            //保存数据
            if (cbSingleLineCount >= 3)
            {
                RemoveCard(cbSingleLineCard, cbSingleLineCount, cbTmpCard, cbLeftCardCount);
                tagAnalyseCardTypeResult tagResult = new tagAnalyseCardTypeResult();
                tagResult.cbCardType = GLogicDef.CT_SINGLE_LINE;
                cbSingleLineCard.CopyList(tagResult.CardData, cbSingleLineCount);

                cbLeftCardCount -= cbSingleLineCount;
                bFindSingleLine = true;
                lstResult.Add(tagResult);
            }
        }

        cbTmpCard.CopyList(lstRemain, cbLeftCardCount);
       
        //cbTmpCard.CopyList(lstRemain, cbLeftCardCount);
        //cbRemainCount = cbLeftCardCount;
        //memcpy(cbRemainCard,cbTmpCard,cbLeftCardCount);
    }

    /// <summary>
    /// 获取不是连子所有牌型的牌
    /// </summary>
    /// <param name="cbHandCardData"></param>
    /// <param name="cbHandCardCount"></param>
    /// <param name="lstResult"></param>
    public void GetNotLineCard(BYTE[] cbHandCardData, BYTE cbHandCardCount, List<tagAnalyseCardTypeResult> lstResult)
    {
        lstResult.Clear();
        BYTE[] cbTmpCard = new BYTE[GLogicDef.MAX_COUNT];
        Buffer.BlockCopy(cbHandCardData, 0, cbTmpCard, 0, cbHandCardCount);
        //CopyMemory(cbTmpCard, cbHandCardData, cbHandCardCount) ;
        //大小排序
        SortCardList(cbTmpCard, cbHandCardCount);
        BYTE cbSameCount = 0;
        BYTE cbLeftCardCount = cbHandCardCount;
        bool bEndCard = false;
        int nCurCardPos = 0;
        int nCurCardCount = 0;
        //连牌判断
        while (nCurCardCount < cbLeftCardCount)
        {
            cbSameCount = 1;
            BYTE cbCardData = cbTmpCard[nCurCardPos];
            bEndCard = true;
            for (BYTE i = (byte)(nCurCardPos + 1); i < cbLeftCardCount; i++)
            {
                //同牌判断
                if (GetCardValue(cbTmpCard[i]) == GetCardValue(cbCardData))
                {
                    ++cbSameCount;
                    continue;
                }
                nCurCardPos = i;
                bEndCard = false;
                break;
            }
            BYTE[] cbType = new BYTE[4] { GLogicDef.CT_SINGLE, GLogicDef.CT_DOUBLE, GLogicDef.CT_THREE, (bBomb ? GLogicDef.CT_BOMB_CARD : GLogicDef.CT_FOUR) };
            //保存数据
            if (cbSameCount > 0)
            {
                BYTE cbCardType = cbType[cbSameCount - 1];
                BYTE cbRecordIndex = (byte)(bEndCard ? nCurCardPos : nCurCardPos - cbSameCount);
                bool bAddCardType = true;
                for (int i = 0; i < lstResult.Count; i++)
                {
                    if (cbCardType == lstResult[i].cbCardType)
                    {
                        int nCardCount = lstResult[i].CardData.Count;
                        cbTmpCard.CopyList(lstResult[i].CardData, cbSameCount, cbRecordIndex);

                        //vtResult[i].cbCardCount += cbSameCount;
                        //memcpy(&vtResult[i].pCbCard[nCardCount],&cbTmpCard[cbRecordIndex], sizeof(BYTE)*cbSameCount) ;
                        bAddCardType = false;
                        break;
                    }
                }
                if (bAddCardType)
                {
                    tagAnalyseCardTypeResult tagResult = new tagAnalyseCardTypeResult();
                    tagResult.cbCardType = cbType[cbSameCount - 1];
                    cbTmpCard.CopyList(tagResult.CardData, cbSameCount, cbRecordIndex);

                    //memcpy(tagResult.pCbCard,&cbTmpCard[cbRecordIndex], sizeof(BYTE)*cbSameCount) ;
                    lstResult.Add(tagResult);
                }
                nCurCardCount += cbSameCount;

            }
        }



    }

    /// <summary>
    /// 合并所有的单牌进连牌，连牌成双连三连四连
    /// </summary>
    /// <param name="vtLineResult"></param>
    /// <param name="vtNotLineResult"></param>
    public void MergeAllLineCard(List<tagAnalyseCardTypeResult> vtLineResult, List<tagAnalyseCardTypeResult> vtNotLineResult)
    {
        MergeLineCard(vtLineResult, vtNotLineResult);
        MergeMutiLineCard(vtLineResult);
    }

    /// <summary>
    /// 合并单牌到连牌
    /// </summary>
    /// <param name="vtLineResult"></param>
    /// <param name="vtNotLineResult"></param>
    void MergeLineCard(List<tagAnalyseCardTypeResult> vtLineResult, List<tagAnalyseCardTypeResult> vtNotLineResult)
    {
        BYTE[] cbSigleCard = new BYTE[GLogicDef.MAX_COUNT];
        BYTE cbSigleCardCount = 0;
        for (int i = 0; i < vtNotLineResult.Count; i++)
        {
            if (vtNotLineResult[i].cbCardType == GLogicDef.CT_SINGLE)
            {
                for (int j = 0; j < vtNotLineResult[i].CardData.Count; j++)
                {
                    cbSigleCard[cbSigleCardCount++] = vtNotLineResult[i].CardData[j];
                    if (GetCardValue(vtNotLineResult[i].CardData[j]) > 3)
                    {//插入
                        if (j < vtNotLineResult[i].CardData.Count - 1 && GetCardValue(vtNotLineResult[i].CardData[j]) == GetCardValue(vtNotLineResult[i].CardData[j + 1]) + 1)
                        {
                            for (int k = 0; k < vtLineResult.Count; k++)
                            {
                                int nCount = vtLineResult[k].CardData.Count;
                                if (GetCardValue(vtLineResult[k].CardData[0]) > GetCardValue(vtNotLineResult[i].CardData[j])
                                    && GetCardValue(vtLineResult[k].CardData[nCount - 1]) < GetCardValue(vtNotLineResult[i].CardData[j + 1]))
                                {
                                    BYTE cbFirstCount = (byte)(GetCardValue(vtLineResult[k].CardData[0]) - GetCardValue(vtNotLineResult[i].CardData[j + 1]) + 1);
                                    BYTE cbSecondCount = (byte)(GetCardValue(vtNotLineResult[i].CardData[j]) - GetCardValue(vtLineResult[k].CardData[nCount - 1]) + 1);

                                    tagAnalyseCardTypeResult tagResult = new tagAnalyseCardTypeResult();
                                    tagResult.cbCardType = GLogicDef.CT_SINGLE_LINE;

                                    vtNotLineResult[i].CardData.CopyList(tagResult.CardData, 2, j);
                                    vtLineResult[k].CardData.CopyList(tagResult.CardData, cbSecondCount - 2, cbFirstCount);

                                    vtLineResult.Add(tagResult);
                                    vtLineResult[k].CardData.RemoveRange(cbFirstCount, vtLineResult[k].CardData.Count - cbFirstCount);

                                    cbSigleCardCount -= 1;
                                    ++j;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int k = 0; k < vtLineResult.Count; k++)
                            {
                                int nCount = vtLineResult[k].CardData.Count;
                                if (GetCardValue(vtLineResult[k].CardData[0]) > GetCardValue(vtNotLineResult[i].CardData[j]) + 1
                                   && GetCardValue(vtLineResult[k].CardData[nCount - 1]) < GetCardValue(vtNotLineResult[i].CardData[j]) - 1)
                                {
                                    BYTE cbFirstCount = (byte)(GetCardValue(vtLineResult[k].CardData[0]) - GetCardValue(vtNotLineResult[i].CardData[j]) + 1);
                                    BYTE cbSecondCount = (byte)(GetCardValue(vtNotLineResult[i].CardData[j]) - GetCardValue(vtLineResult[k].CardData[nCount - 1]) + 1);

                                    tagAnalyseCardTypeResult tagResult = new tagAnalyseCardTypeResult();
                                    tagResult.cbCardType = GLogicDef.CT_SINGLE_LINE;

                                    tagResult.CardData.Add(vtNotLineResult[i].CardData[j]);
                                    vtLineResult[k].CardData.CopyList(tagResult.CardData, cbSecondCount - 1, cbFirstCount);
                                    vtLineResult.Add(tagResult);
                                    vtLineResult[k].CardData.RemoveRange(cbFirstCount, vtLineResult[k].CardData.Count - cbFirstCount);
                                    cbSigleCardCount -= 1;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (cbSigleCardCount > 0)
                {
                    vtNotLineResult[i].CardData.Clear();
                    cbSigleCard.CopyList(vtNotLineResult[i].CardData, cbSigleCardCount);
                    //vtNotLineResult[i].cbCardCount =cbSigleCardCount;
                    //memcpy(vtNotLineResult[i].pCbCard,cbSigleCard,sizeof(BYTE)*cbSigleCardCount);
                }
                else
                {
                    vtNotLineResult.RemoveAt(i);
                    //vtNotLineResult.erase(vtNotLineResult.begin() + i);
                }
                break;
            }
        }
    }

    /// <summary>
    /// 合并多连牌（双连、三连、四连）
    /// </summary>
    /// <param name="vtLineResult"></param>
    /// <param name="vtNotLineResult"></param>
    void MergeMutiLineCard(List<tagAnalyseCardTypeResult> vtLineResult)
    {
        if (vtLineResult.Count > 0)
        {
            BYTE[] cbIndex = new BYTE[GLogicDef.MAX_COUNT];
            BYTE cbIndexCount = 0;
            for (int i = 0; i < vtLineResult.Count - 1; i++)
            {
                cbIndexCount = 0;
                cbIndex.ArraySetAll((byte)0);
                //memset(cbIndex,0,sizeof(cbIndex));
                for (int j = i + 1; j < vtLineResult.Count; j++)
                {
                    if (vtLineResult[i].CardData.Count == vtLineResult[j].CardData.Count
                        && GetCardValue(vtLineResult[i].CardData[0]) == GetCardValue(vtLineResult[j].CardData[0]))
                    {
                        cbIndex[cbIndexCount++] = (byte)j;
                    }
                }
                if (cbIndexCount > 0)
                {
                    int nCardCount = vtLineResult[i].CardData.Count;
                    BYTE[] cbCardType = new BYTE[3] { GLogicDef.CT_DOUBLE_LINE, GLogicDef.CT_THREE_LINE, GLogicDef.CT_FOUR_LINE };

                    for (int k = 0; k < cbIndexCount; k++)
                    {
                        vtLineResult[cbIndex[k] - k].CardData.CopyList(vtLineResult[i].CardData,nCardCount);
                        //memcpy(&vtLineResult[i].pCbCard[nCardCount*(k+1)],vtLineResult[i].pCbCard,nCardCount);
                        //vtLineResult[i].cbCardCount += nCardCount;
                        vtLineResult.RemoveAt(cbIndex[k] - k);
                        //vtLineResult.erase(vtLineResult.begin()+(cbIndex[k]-k));
                    }

                    //大小排序
                    SortCardList(vtLineResult[i].CardData, (byte)vtLineResult[i].CardData.Count);
                    if (cbIndexCount <= cbCardType.Length)
                    {
                        vtLineResult[i].cbCardType = cbCardType[cbIndexCount - 1];
                    }
                    else
                    {
                        //ASSERT(FALSE);
                        Debuger.Instance.LogError("False!");
                    }
                }
            }
        }
    }

    #endregion

    #region 拆牌部分
    /// <summary>
    /// 合成多连(拆牌专用)
    /// </summary>
    /// <param name="vtLineResult">必须是连牌集合</param>
    /// <param name="lineNumber">1-单连2-双连3-三连4-四连</param>
    void MergeSpecifiedMutiLineCard(List<tagAnalyseCardTypeResult> vtLineResult)
    {
        //if (lineStyle < GLogicDef.CT_SINGLE_LINE || lineStyle > GLogicDef.CT_FOUR_LINE) return;
        //if (lineStyle == GLogicDef.CT_SINGLE_LINE) return;

        int cardLenA = 0;
        int cardLenB = 0;
        int indexA = 0;
        int indexB = 0;

        List<tagAnalyseCardTypeResult> proccessed = new List<tagAnalyseCardTypeResult>();


        for (byte i = 0; i < vtLineResult.Count; ++i)
        {
            for (byte j = (byte)(i + 1); j < vtLineResult.Count; ++j)
            {
                cardLenA = vtLineResult[i].CardData.Count;
                cardLenB = vtLineResult[j].CardData.Count;
                if (cardLenA > cardLenB)
                {
                    indexA = i;
                    indexB = j;
                }
                else
                {
                    indexA = j;
                    indexB = i;
                    cardLenA = cardLenA + cardLenB;
                    cardLenB = cardLenA - cardLenB;
                    cardLenA = cardLenA - cardLenB;
                }

                if (GetCardValue(vtLineResult[indexB].CardData[0]) <= GetCardValue(vtLineResult[indexA].CardData[0])
                    && GetCardValue(vtLineResult[indexB].CardData[0]) >= GetCardValue(vtLineResult[indexA].CardData[cardLenA - 1])
                    && GetCardValue(vtLineResult[indexB].CardData[cardLenB - 1]) <= GetCardValue(vtLineResult[indexA].CardData[0])
                    && GetCardValue(vtLineResult[indexB].CardData[cardLenB - 1]) >= GetCardValue(vtLineResult[indexA].CardData[cardLenA - 1])
                   )
                {
                    if (proccessed.Count == 0)
                    {
                        //没有就是双连
                        tagAnalyseCardTypeResult tmpRes = new tagAnalyseCardTypeResult();
                        tmpRes.cbCardType = GLogicDef.CT_DOUBLE_LINE;
                        tmpRes.CardData = vtLineResult[indexB].CardData.ToList();

                        byte[] cbLongCard = new byte[cardLenB];
                        Buffer.BlockCopy(vtLineResult[indexA].CardData.ToArray()
                            , GetCardValue(vtLineResult[indexA].CardData[0]) - GetCardValue(vtLineResult[indexB].CardData[0])
                            , cbLongCard, 0, cardLenB);
                        tmpRes.CardData.AddRange(cbLongCard);

                        proccessed.Add(tmpRes);
                    }
                    else
                    {
                        bool bExist = false;
                        //是否已存在相同的连子
                        foreach (var e in proccessed)
                        {
                            if (GetCardValue(e.CardData[0]) == GetCardValue(vtLineResult[j].CardData[0])
                                && GetCardValue(e.CardData[e.CardData.Count - 1]) == GetCardValue(vtLineResult[j].CardData[vtLineResult[j].CardData.Count - 1]))
                            {
                                bExist = true;
                                if (e.cbCardType == GLogicDef.CT_FOUR_LINE) break;

                                e.cbCardType = Math.Min(++e.cbCardType, GLogicDef.CT_FOUR_LINE);
                                e.CardData.AddRange(vtLineResult[j].CardData);
                            }
                        }
                        if (bExist == false)  //不存在相同的连子，新的2连
                        {
                            //没有就是双连
                            tagAnalyseCardTypeResult tmpRes = new tagAnalyseCardTypeResult();
                            tmpRes.cbCardType = GLogicDef.CT_DOUBLE_LINE;
                            tmpRes.CardData = vtLineResult[indexB].CardData.ToList();

                            byte[] cbLongCard = new byte[cardLenB];
                            Buffer.BlockCopy(vtLineResult[indexA].CardData.ToArray()
                                , GetCardValue(vtLineResult[indexA].CardData[0]) - GetCardValue(vtLineResult[indexB].CardData[0])
                                , cbLongCard, 0, cardLenB);
                            tmpRes.CardData.AddRange(cbLongCard);

                            proccessed.Add(tmpRes);
                        }
                    }
                }
            }
        }

        if (proccessed.Count != 0)
        {
            foreach (var e in proccessed)
            {
                SortCardList(e.CardData, (byte)e.CardData.Count);
            }
            vtLineResult.Clear();
            vtLineResult.AddRange(proccessed);
        }
    }

    //获取指定牌型
    public List<tagAnalyseCardTypeResult> GetSpecifiedCard(byte cbCardType, byte[] CardData, byte cbCardCount, List<byte> cbRemainCard)
    {
        List<tagAnalyseCardTypeResult> res = null;
        byte[] cbCardData = new byte[cbCardCount];
        Buffer.BlockCopy(CardData, 0, cbCardData, 0, cbCardCount);
        SortCardList(cbCardData, cbCardCount);
        if (GLogicDef.CT_SINGLE_LINE <= cbCardType && cbCardType <= GLogicDef.CT_FOUR_LINE)
        {
            #region 连牌判断
            res = new List<tagAnalyseCardTypeResult>();
            GetAllLineCard(cbCardData, cbCardCount, res, cbRemainCard);
            if (cbCardType > GLogicDef.CT_SINGLE_LINE) MergeSpecifiedMutiLineCard(res);

            switch (cbCardType)
            {
                case GLogicDef.CT_FOUR_LINE:
                    for (int i = res.Count - 1; i >= 0; --i)
                    {
                        if (res[i].cbCardType != GLogicDef.CT_FOUR_LINE)
                        {
                            res.RemoveAt(i);
                        }
                    }
                    break;
                case GLogicDef.CT_THREE_LINE:
                    for (int i = res.Count - 1; i >= 0; --i)
                    {
                        if (res[i].cbCardType == GLogicDef.CT_FOUR_LINE)
                        {
                            res[i].cbCardType = GLogicDef.CT_THREE_LINE;
                            for (int j = res[i].CardData.Count - 1; j >= 0; --j)
                            {
                                if ((j + 1) % 4 == 0) res[i].CardData.RemoveAt(j);
                            }
                        }
                        else if (res[i].cbCardType == GLogicDef.CT_THREE_LINE)
                        {

                        }
                        else
                        {
                            res.RemoveAt(i);
                        }
                    }
                    break;
                case GLogicDef.CT_DOUBLE_LINE:
                    {
                        List<tagAnalyseCardTypeResult> resTemp = new List<tagAnalyseCardTypeResult>();
                        for (int i = res.Count - 1; i >= 0; --i)
                        {
                            if (res[i].cbCardType == GLogicDef.CT_FOUR_LINE)
                            {
                                tagAnalyseCardTypeResult newCardType = new tagAnalyseCardTypeResult();
                                newCardType.cbCardType = GLogicDef.CT_DOUBLE_LINE;
                                tagAnalyseCardTypeResult oldCardType = new tagAnalyseCardTypeResult();
                                oldCardType.cbCardType = GLogicDef.CT_DOUBLE_LINE;

                                for (int j = 0; j < res[i].CardData.Count; ++j)
                                {
                                    if (j % 4 == 2 || j % 4 == 3)
                                    {
                                        newCardType.CardData.Add(res[i].CardData[j]);
                                    }
                                    else
                                    {
                                        oldCardType.CardData.Add(res[i].CardData[j]);
                                    }
                                }
                                resTemp.Add(oldCardType);
                                resTemp.Add(newCardType);
                            }
                            else if (res[i].cbCardType == GLogicDef.CT_THREE_LINE)
                            {
                                tagAnalyseCardTypeResult newCardType = new tagAnalyseCardTypeResult();
                                newCardType.cbCardType = GLogicDef.CT_DOUBLE_LINE;
                                for (int j = 0; j < res[i].CardData.Count; ++j)
                                {
                                    if (j % 3 != 2)
                                    {
                                        newCardType.CardData.Add(res[i].CardData[j]);
                                    }
                                }

                                resTemp.Add(newCardType);

                            }
                            else if (res[i].cbCardType == GLogicDef.CT_DOUBLE_LINE)
                            {
                                resTemp.Add(res[i]);
                            }
                        }

                        res = resTemp;
                    }
                    break;
                case GLogicDef.CT_SINGLE_LINE:
                    {
                        List<tagAnalyseCardTypeResult> resTemp = new List<tagAnalyseCardTypeResult>();
                        int sameCount = 0;
                        for (int i = res.Count - 1; i >= 0; --i)
                        {
                            if (res[i].cbCardType == GLogicDef.CT_FOUR_LINE)
                            {
                                sameCount = 4;
                                tagAnalyseCardTypeResult[] newCardType = new tagAnalyseCardTypeResult[sameCount];
                                newCardType.ArraySetAll(new tagAnalyseCardTypeResult());
                                for (int j = 0; j < res[i].CardData.Count / sameCount; ++j)
                                {
                                    for (int k = 0; k < sameCount; ++k)
                                    {
                                        newCardType[k].cbCardType = GLogicDef.CT_SINGLE_LINE;
                                        newCardType[k].CardData.Add(res[i].CardData[j * sameCount + k]);
                                    }
                                }
                                resTemp.AddRange(newCardType);
                            }
                            else if (res[i].cbCardType == GLogicDef.CT_THREE_LINE)
                            {
                                sameCount = 3;
                                tagAnalyseCardTypeResult[] newCardType = new tagAnalyseCardTypeResult[sameCount];
                                newCardType.ArraySetAll(new tagAnalyseCardTypeResult());
                                for (int j = 0; j < res[i].CardData.Count / sameCount; ++j)
                                {
                                    for (int k = 0; k < sameCount; ++k)
                                    {
                                        newCardType[k].cbCardType = GLogicDef.CT_SINGLE_LINE;
                                        newCardType[k].CardData.Add(res[i].CardData[j * sameCount + k]);
                                    }
                                }
                                resTemp.AddRange(newCardType);
                            }
                            else if (res[i].cbCardType == GLogicDef.CT_DOUBLE_LINE)
                            {
                                sameCount = 2;
                                tagAnalyseCardTypeResult[] newCardType = new tagAnalyseCardTypeResult[sameCount];
                                newCardType.ArraySetAll(new tagAnalyseCardTypeResult());
                                for (int j = 0; j < res[i].CardData.Count / sameCount; ++j)
                                {
                                    for (int k = 0; k < sameCount; ++k)
                                    {
                                        newCardType[k].cbCardType = GLogicDef.CT_SINGLE_LINE;
                                        newCardType[k].CardData.Add(res[i].CardData[j * sameCount + k]);
                                    }
                                }
                                resTemp.AddRange(newCardType);
                            }
                            else if (res[i].cbCardType == GLogicDef.CT_SINGLE_LINE)
                            {
                                resTemp.Add(res[i]);
                            }
                        }

                        res = resTemp;
                    }
                    break;
            }
            #endregion
        }
        else if (GLogicDef.CT_BOMB_CARD == cbCardType)
        {
            tagAnalyseCardTypeResult tag = new tagAnalyseCardTypeResult();
            GetAllFourCard(cbCardData, cbCardCount, tag, cbRemainCard);
            if (tag.cbCardType == GLogicDef.CT_BOMB_CARD)
            {
                res = new List<tagAnalyseCardTypeResult>();
                SortCardList(tag.CardData, (byte)tag.CardData.Count);
                res.Add(tag);
            }
        }
        else if (GLogicDef.CT_DOUBLE <= cbCardType && cbCardType <= GLogicDef.CT_FOUR)
        {
            tagAnalyseCardTypeResult tag = new tagAnalyseCardTypeResult();
            System.Action<BYTE[], BYTE, tagAnalyseCardTypeResult, List<byte>>[] actions =
                new Action<BYTE[], BYTE, tagAnalyseCardTypeResult, List<BYTE>>[GLogicDef.CT_FOUR - GLogicDef.CT_DOUBLE + 1];
            actions[0] = GetAllTwoCard;
            actions[1] = GetAllThreeCard;
            actions[2] = GetAllFourCard;

            actions[cbCardType - GLogicDef.CT_DOUBLE](cbCardData, cbCardCount, tag, cbRemainCard);
            if (tag.cbCardType != GLogicDef.CT_INVALID)
            {
                res = new List<tagAnalyseCardTypeResult>();
                SortCardList(tag.CardData, (byte)tag.CardData.Count);
                res.Add(tag);
            }
        }
        else
        {
            //Debuger.Instance.LogError("GetSpecifiedCard牌型错误");
        }

        if (res != null && res.Count == 0) res = null;

        return res;
    }

    /// <summary>
    /// 跟牌情况下，是否需要拆牌
    /// </summary>
    /// <param name="cbHandCard">要拆牌数据</param>
    /// <param name="cbHandCardCount">要拆牌个数</param>
    /// <param name="cbOutCard">出牌数据</param>
    /// <param name="cbOutCardCount">出牌个数</param>
    /// <param name="lMeOutCard">不拆，我可以出的牌</param>
    /// <returns>True - Split, False - No Split</returns>
    public bool NeedSplitCard(byte[] cbHandCard,byte cbHandCardCount, byte[] cbOutCard, byte cbOutCardCount, List<tagAnalyseCardTypeResult> follow = null)
    {
        bool res = true;
        if (follow != null) follow.Clear();

        byte[] outCard = new byte[cbOutCardCount];        
        Buffer.BlockCopy(cbOutCard, 0, outCard, 0, cbOutCardCount);
        SortCardList(outCard,cbOutCardCount);

        byte[] handCard = new byte[cbHandCardCount];
        Buffer.BlockCopy(cbHandCard, 0, handCard, 0, cbHandCardCount);
        SortCardList(handCard, cbHandCardCount);

        byte cardType = GetCardType(outCard, cbOutCardCount);
        if(cardType == GLogicDef.CT_INVALID)
        {
            res = false;
            Debuger.Instance.LogError("拆牌错误!");
            return res;
        }

        //切牌
        Dictionary<byte,List<tagAnalyseCardTypeResult>> allCardType = new Dictionary<BYTE,List<tagAnalyseCardTypeResult>>();
        int handNumber = CutCards(cbHandCard, cbHandCardCount, allCardType);

        var mAllCardTypeCard = allCardType;

        if (mAllCardTypeCard.ContainsKey(cardType))
        {
            var cards = mAllCardTypeCard[cardType];
            switch(cardType)
            {
                case GLogicDef.CT_SINGLE_LINE:
                case GLogicDef.CT_DOUBLE_LINE:
                case GLogicDef.CT_THREE_LINE:
                case GLogicDef.CT_FOUR_LINE:
                {
                    //个数相等，最大值大于出牌
                    foreach (var e in cards)
                    {
                        if(e.CardData.Count == cbOutCardCount && GetCardLogicValue ( e.CardData[0]) > GetCardLogicValue(outCard[0]) )
                        {
                            res = false;
                            if (follow != null)
                            {
                                tagAnalyseCardTypeResult tag = new tagAnalyseCardTypeResult();
                                tag.cbCardType = cardType;
                                e.CardData.CopyList(tag.CardData, e.CardData.Count);
                                follow.Add(tag);
                            }
                            else break;
                        }
                    }
                    break;
                }                    
                case GLogicDef.CT_SINGLE:
                case GLogicDef.CT_DOUBLE:
                case GLogicDef.CT_THREE:
                case GLogicDef.CT_FOUR:
                case GLogicDef.CT_BOMB_CARD:
                {
                    //byte cardNum = cardType;
                    //if (GLogicDef.CT_BOMB_CARD == cardType) cardNum = GLogicDef.CT_FOUR;

                    if (cards[0].CardData.Count % cbOutCardCount != 0)
                    {
                        res = false;
                        Debuger.Instance.LogError("非连牌拆牌判断错误!");
                        break;
                    }

                    for (byte i = 0; i < cards[0].CardData.Count; i += cbOutCardCount)
                    {
                        if(GetCardLogicValue(cards[0].CardData[i]) > GetCardLogicValue(outCard[0]) )
                        {
                            res = false;
                            if (follow != null)
                            {
                                if (follow.Count == 0) follow.Add(new tagAnalyseCardTypeResult());
                                follow[0].cbCardType = cardType;
                                cards[0].CardData.CopyList(follow[0].CardData, cbOutCardCount, i);
                            }
                            else break;
                        }
                    }
                    break;
                }                    
            }
        }
        
        return res;
    }

    /// <summary>
    /// 跟牌，拆牌,拆成大于对应出牌的牌集合
    /// </summary>
    /// <param name="cbHandCard">拆牌数据</param>
    /// <param name="cbHandCardCount">拆牌数据个数</param>
    /// <param name="cbOutCard">出牌数据</param>
    /// <param name="cbOutCardCount">出牌个数</param>
    /// <param name="follow">拆牌结果</param>
    /// <param name="isDeterminHandNumber">是否判断手数最优拆牌</param>
    /// <returns>True - 拆牌成功 False - 无对应牌型可拆</returns>
    public bool SplitCard(byte[] cbHandCard, byte cbHandCardCount, byte[] cbOutCard, byte cbOutCardCount, List<tagAnalyseCardTypeResult> follow, bool isDeterminHandNumber = true)
    {
        bool res = false;

        follow.Clear();

        // 1获取指定出牌牌型
        // 2是否有大于的牌型
        // 3切牌
        // 4计算是否合适

        byte[] outCard = new byte[cbOutCardCount];
 
        Buffer.BlockCopy(cbOutCard, 0, outCard, 0, cbOutCardCount);
        SortCardList(outCard,cbOutCardCount);

        //WORD wNextChair = wChairID;// (WORD)((wChairID + 1) % wPlayerCount);
        List<byte> NextUserCard = new List<BYTE>();//GetUserCard(wNextChair);
        cbHandCard.CopyList(NextUserCard, cbHandCardCount);
        int NextUserHandNumber = CutCards(NextUserCard, cbHandCardCount);//GetHandNumber(wNextChair);

        byte cbCardType = GetCardType(outCard, cbOutCardCount);
        if(cbCardType != GLogicDef.CT_INVALID)
        {
            List<byte> remainCard = new List<BYTE>();

            //获取指定出牌牌型
            var result = GetSpecifiedCard(cbCardType, NextUserCard.ToArray(), (byte)NextUserCard.Count, remainCard);
            if(result != null)
            {
                //是否有大于的牌型
                foreach (var e in result)
                {
                    switch (cbCardType)
                    {
                        case GLogicDef.CT_SINGLE_LINE:
                        case GLogicDef.CT_DOUBLE_LINE:
                        case GLogicDef.CT_THREE_LINE:
                        case GLogicDef.CT_FOUR_LINE:
                            {
                                //此连子有大于的连子
                                if (GetCardLogicValue(e.CardData[0]) - GetCardLogicValue(outCard[0]) > 0
                                    && e.CardData.Count >= outCard.Length)
                                {
                                    int splitIndex = 0;
                                    while (splitIndex <= e.CardData.Count - outCard.Length)
                                    {
                                        if (GetCardLogicValue(e.CardData[splitIndex]) <= GetCardLogicValue(outCard[0])) break;
                                        tagAnalyseCardTypeResult tag = new tagAnalyseCardTypeResult();
                                        tag.cbCardType = cbCardType;
                                        e.CardData.CopyList(tag.CardData, outCard.Length, splitIndex++);

                                        //是否判断最优手数
                                        if (isDeterminHandNumber)
                                        {
                                            List<byte> userCardTmp = new List<BYTE>();
                                            userCardTmp.AddRange(NextUserCard);
                                            //切牌
                                            if (RemoveCard(tag.CardData.ToArray(), (byte)outCard.Length, userCardTmp))
                                            {
                                                int splitHandNum = CutCards(userCardTmp, (byte)userCardTmp.Count);
                                                if (splitHandNum <= NextUserHandNumber)
                                                {
                                                    follow.Add(tag);
                                                    res = true;
                                                }
                                            }
                                            else
                                            {
                                                Debuger.Instance.LogError("删除错误");
                                            }
                                        }
                                        else
                                        {
                                            follow.Add(tag);
                                            res = true;
                                        }
                                    }  //end while                                                                      
                                }
                                break;
                            }
                        case GLogicDef.CT_SINGLE:
                        case GLogicDef.CT_DOUBLE:
                        case GLogicDef.CT_THREE:
                        case GLogicDef.CT_FOUR:
                        case GLogicDef.CT_BOMB_CARD:
                            {
                                //是否有大于的非连牌
                                if (e.CardData.Count % cbOutCardCount != 0)
                                {
                                    Debuger.Instance.LogError("非连牌拆牌判断错误!");
                                    break;
                                }

                                tagAnalyseCardTypeResult tag = new tagAnalyseCardTypeResult();
                                for (byte i = 0; i < e.CardData.Count; i += cbOutCardCount)
                                {
                                    if (GetCardLogicValue(e.CardData[i]) > GetCardLogicValue(outCard[0]))
                                    {                                                                                
                                        //是否判断最优手数
                                        if(isDeterminHandNumber)
                                        {
                                            List<byte> userCardTmp = new List<BYTE>();
                                            userCardTmp.AddRange(NextUserCard);
                                            byte[] removeCard = new byte[cbOutCardCount];
                                            Buffer.BlockCopy(e.CardData.ToArray(), i, removeCard, 0, cbOutCardCount);

                                            if (RemoveCard(removeCard, cbOutCardCount, userCardTmp))
                                            {
                                                int splitHandNum = CutCards(userCardTmp, (byte)userCardTmp.Count);
                                                if (splitHandNum <= NextUserHandNumber)
                                                {
                                                    tag.cbCardType = cbCardType;
                                                    e.CardData.CopyList(tag.CardData, cbOutCardCount, i);
                                                    
                                                    res = true;
                                                }
                                            }
                                            else
                                            {
                                                Debuger.Instance.LogError("删除错误");
                                            }    
                                        }
                                        else
                                        {
                                            tag.cbCardType = cbCardType;
                                            e.CardData.CopyList(tag.CardData, cbOutCardCount, i);

                                            res = true;
                                        }  // if(isDeterminHandNumber)

                                    }  //if (GetCardLogicValue(e.CardData[i]) > GetCardLogicValue(outCard[0]))
                                } //for

                                if (tag.cbCardType != GLogicDef.CT_INVALID)
                                {
                                    SortCardList(tag.CardData, (byte)tag.CardData.Count);
                                    follow.Add(tag);
                                }
                                    
                                break;
                            }
                    }
                }                
            }
        }
        return res;
    }    

    /// <summary>
    /// 获取牌的分数
    /// </summary>
    /// <param name="cbCardData">牌数据</param>
    /// <param name="cbCardCount">牌个数</param>
    /// <returns></returns>
    public int GetHandCardScore(byte[] cbCardData,byte cbCardCount)
    {
        int Score = 0;
        byte Score_3 = 6;
        byte Score_2 = 2;
        byte Score_A = 1;
        byte Score_L_K = 3;
        byte Score_BOMB = 15;

        //炸弹优先判断
        List<byte> remain = new List<BYTE>();
        tagAnalyseCardTypeResult tagResult = new tagAnalyseCardTypeResult();
        GetAllFourCard(cbCardData, cbCardCount, tagResult, remain);
        if (tagResult.cbCardType != GLogicDef.CT_INVALID)
        {
            SortCardList(tagResult.CardData, (byte)tagResult.CardData.Count);
            if (GetCardValue(tagResult.CardData[0]) == 3)
            {
                Score += Score_3 * 4;
                tagResult.CardData.RemoveRange(0, 4);
            }

            if (GLogicDef.CT_BOMB_CARD == tagResult.cbCardType)
            {
                Score += tagResult.CardData.Count / 4 * Score_BOMB;
            }
            else
            {
                remain.AddRange(tagResult.CardData);
                SortCardList(remain, (byte)remain.Count);
            }

        }

        //通用判断
        List<tagAnalyseCardTypeResult> tmpResult = new List<tagAnalyseCardTypeResult>();
        GetAllLineCard(remain.ToArray(),(byte)remain.Count,tmpResult,remain);
        foreach(var e in tmpResult)
        {
            if (GetCardLogicValue(e.CardData[0]) == 0xd) Score += Score_L_K;
        }

        SortCardList(remain, (byte)remain.Count);

        //单张分数
        for (int i = 0; i < remain.Count;++i )
        {
            if (GetCardLogicValue(remain[i]) < 14) break;
            if (GetCardLogicValue(remain[i]) == 14) Score += Score_A;
            else if (GetCardLogicValue(remain[i]) == 15) Score += Score_2;
            else if (GetCardLogicValue(remain[i]) == 16) Score += Score_3;
        }


        return Score;
    }
   

    #endregion

    #region AI
    //切用户牌
    public void CutUserCard(WORD wChairID,byte[] cbCardData,byte cbCardCount)
    {
        mCutResult[wChairID].mHandNumber = CutCards(cbCardData, cbCardCount, mCutResult[wChairID].mAllCardType);
    }
    //删除扑克
    public void RemoveUserCardData(WORD wChairID, byte[] cbRemoveCardData, byte cbRemoveCardCount)
    {
        if ((mAllCards[wChairID].Count - cbRemoveCardCount) >= 0)
        {
            bool bSuccess = RemoveCard(cbRemoveCardData, cbRemoveCardCount, mAllCards[wChairID]);
            if (!bSuccess)
            {
                Debuger.Instance.LogError("RemoveUserCardData删除扑克错误!");
            }
        }
    }
    //获取用户天牌
    public uint GetUserHighCard(WORD wChair,
        Dictionary<byte, List<tagAnalyseCardTypeResult>> HighCard = null,
        Dictionary<byte, List<tagAnalyseCardTypeResult>> NoHighCard = null)
    {
        if (HighCard == null) HighCard = new Dictionary<BYTE, List<tagAnalyseCardTypeResult>>();
        if (NoHighCard == null) NoHighCard = new Dictionary<BYTE, List<tagAnalyseCardTypeResult>>();

        HighCard.Clear();
        NoHighCard.Clear();
        WORD NoHighCardNum = 0;
        WORD HighCardNum = 0;

        foreach (var e in GetCutResult(wChair))
        {
            int highCard = 0;
            switch (e.Key)
            {
                case GLogicDef.CT_SINGLE_LINE:
                case GLogicDef.CT_DOUBLE_LINE:
                case GLogicDef.CT_THREE_LINE:
                case GLogicDef.CT_FOUR_LINE:
                    foreach (var f in e.Value)
                    {
                        highCard = GetHighCardType(wChair, f.CardData.ToArray(), (byte)f.CardData.Count);
                        if (highCard != -1)
                        {
                            if (NoHighCard.ContainsKey(e.Key) == false) NoHighCard.Add(e.Key, new List<tagAnalyseCardTypeResult>());
                            tagAnalyseCardTypeResult tag = new tagAnalyseCardTypeResult();
                            tag.cbCardType = e.Key;
                            f.CardData.CopyList(tag.CardData, f.CardData.Count);

                            NoHighCard[e.Key].Add(tag);
                            NoHighCardNum++;
                        }
                        else
                        {
                            if (HighCard.ContainsKey(e.Key) == false) HighCard.Add(e.Key, new List<tagAnalyseCardTypeResult>());
                            HighCard[e.Key].Add(f);
                            HighCardNum++;
                        }
                    }

                    break;
                case GLogicDef.CT_SINGLE:
                case GLogicDef.CT_DOUBLE:
                case GLogicDef.CT_THREE:
                case GLogicDef.CT_FOUR:
                case GLogicDef.CT_BOMB_CARD:
                    {
                        byte cardNum = 1;
                        if (GLogicDef.CT_DOUBLE == e.Value[0].cbCardType) cardNum = 2;
                        if (GLogicDef.CT_THREE == e.Value[0].cbCardType) cardNum = 3;
                        if (GLogicDef.CT_FOUR == e.Value[0].cbCardType) cardNum = 4;
                        if (GLogicDef.CT_BOMB_CARD == e.Value[0].cbCardType) cardNum = 4;
                        for (int i = 0; i < e.Value[0].CardData.Count; i += cardNum)
                        {
                            byte[] tmpCard = new byte[cardNum];
                            e.Value[0].CardData.CopyArr(tmpCard, cardNum, i);
                            highCard = GetHighCardType(wChair, tmpCard, cardNum);
                            if (highCard != -1)
                            {
                                if (NoHighCard.ContainsKey(e.Key) == false)
                                {
                                    tagAnalyseCardTypeResult tag = new tagAnalyseCardTypeResult();
                                    tag.cbCardType = e.Key;
                                    NoHighCard.Add(e.Key, new List<tagAnalyseCardTypeResult>());
                                    NoHighCard[e.Key].Add(tag);
                                }
                                NoHighCard[e.Key][0].CardData.AddRange(tmpCard);
                                NoHighCardNum++;
                            }
                            else
                            {
                                if (HighCard.ContainsKey(e.Key) == false)
                                {
                                    tagAnalyseCardTypeResult tag = new tagAnalyseCardTypeResult();
                                    tag.cbCardType = e.Key;
                                    HighCard.Add(e.Key, new List<tagAnalyseCardTypeResult>());
                                    HighCard[e.Key].Add(tag);
                                }
                                HighCard[e.Key][0].CardData.AddRange(tmpCard);
                                HighCardNum++;
                            }
                        }
                    }
                    break;
            }
        }

        return (uint)((uint)HighCardNum<<16 | NoHighCardNum);
    }

    /// <summary>
    /// 获取天牌，从切牌结果
    /// </summary>
    /// <param name="wChair">天牌玩家</param>
    /// <param name="CutCard">已切好牌的集合</param>
    /// <param name="HighCard">输出天牌集合</param>
    /// <param name="NoHighCard">输出非天牌集合</param>
    /// <returns></returns>
    public uint GetHighCard(WORD wChair,
        Dictionary<byte, List<tagAnalyseCardTypeResult>> CutCard,
        Dictionary<byte, List<tagAnalyseCardTypeResult>> HighCard,
        Dictionary<byte, List<tagAnalyseCardTypeResult>> NoHighCard)
    {
        HighCard.Clear();
        NoHighCard.Clear();
        WORD NoHighCardNum = 0;
        WORD HighCardNum = 0;

        foreach (var e in CutCard)
        {
            int highCard = 0;
            switch (e.Key)
            {
                case GLogicDef.CT_SINGLE_LINE:
                case GLogicDef.CT_DOUBLE_LINE:
                case GLogicDef.CT_THREE_LINE:
                case GLogicDef.CT_FOUR_LINE:
                    foreach (var f in e.Value)
                    {
                        highCard = GetHighCardType(wChair, f.CardData.ToArray(), (byte)f.CardData.Count);
                        if (highCard != -1)
                        {
                            if (NoHighCard.ContainsKey(e.Key) == false) NoHighCard.Add(e.Key, new List<tagAnalyseCardTypeResult>());
                            tagAnalyseCardTypeResult tag = new tagAnalyseCardTypeResult();
                            tag.cbCardType = e.Key;
                            f.CardData.CopyList(tag.CardData, f.CardData.Count);

                            NoHighCard[e.Key].Add(tag);
                            NoHighCardNum++;
                        }
                        else
                        {
                            if (HighCard.ContainsKey(e.Key) == false) HighCard.Add(e.Key, new List<tagAnalyseCardTypeResult>());
                            HighCard[e.Key].Add(f);
                            HighCardNum++;
                        }
                    }

                    break;
                case GLogicDef.CT_SINGLE:
                case GLogicDef.CT_DOUBLE:
                case GLogicDef.CT_THREE:
                case GLogicDef.CT_FOUR:
                case GLogicDef.CT_BOMB_CARD:
                    {
                        byte cardNum = 1;
                        if (GLogicDef.CT_DOUBLE == e.Value[0].cbCardType) cardNum = 2;
                        if (GLogicDef.CT_THREE == e.Value[0].cbCardType) cardNum = 3;
                        if (GLogicDef.CT_FOUR == e.Value[0].cbCardType) cardNum = 4;
                        if (GLogicDef.CT_BOMB_CARD == e.Value[0].cbCardType) cardNum = 4;
                        for (int i = 0; i < e.Value[0].CardData.Count; i += cardNum)
                        {
                            byte[] tmpCard = new byte[cardNum];
                            e.Value[0].CardData.CopyArr(tmpCard, cardNum, i);
                            highCard = GetHighCardType(wChair, tmpCard, cardNum);
                            if (highCard != -1)
                            {
                                if (NoHighCard.ContainsKey(e.Key) == false)
                                {
                                    tagAnalyseCardTypeResult tag = new tagAnalyseCardTypeResult();
                                    tag.cbCardType = e.Key;
                                    NoHighCard.Add(e.Key, new List<tagAnalyseCardTypeResult>());
                                    NoHighCard[e.Key].Add(tag);
                                }
                                NoHighCard[e.Key][0].CardData.AddRange(tmpCard);
                                NoHighCardNum++;
                            }
                            else
                            {
                                if (HighCard.ContainsKey(e.Key) == false)
                                {
                                    tagAnalyseCardTypeResult tag = new tagAnalyseCardTypeResult();
                                    tag.cbCardType = e.Key;
                                    HighCard.Add(e.Key, new List<tagAnalyseCardTypeResult>());
                                    HighCard[e.Key].Add(tag);
                                }
                                HighCard[e.Key][0].CardData.AddRange(tmpCard);
                                HighCardNum++;
                            }
                        }
                    }
                    break;
            }
        }

        return (uint)((uint)HighCardNum << 16 | NoHighCardNum);
    }

    /// <summary>
    /// 通用跟牌(单牌除外)
    /// </summary>
    /// <param name="wChairID">出牌用户</param>
    /// <param name="cbOutCard">出牌数据</param>
    /// <param name="cbOutCardCount">出牌个数</param>
    /// <param name="follow"></param>
    /// <param name="isDeterminHandNumber">是否最优手数</param>
    /// <returns></returns>
    public bool CommFollowCard(WORD wChairID, byte[] cbOutCard, byte cbOutCardCount, List<tagAnalyseCardTypeResult> follow, bool isDeterminHandNumber = true)
    {
        bool res = false;
        //1.是否需要拆牌
        //2.拆牌
        //2.1获取指定出牌牌型
        //2.2是否有大于的牌型
        //2.3切牌
        //2.4计算是否合适

        byte[] outCard = new byte[cbOutCardCount];
        Buffer.BlockCopy(cbOutCard, 0, outCard, 0, cbOutCardCount);
        SortCardList(outCard, cbOutCardCount);

        WORD wNextChair = (WORD)((wChairID + 1) % wPlayerCount);

        //List<tagAnalyseCardTypeResult> lResult = new List<tagAnalyseCardTypeResult>();
        //无需拆牌有牌可出
        if (NeedSplitCard(mAllCards[wNextChair].ToArray(), (byte)mAllCards[wNextChair].Count, outCard, cbOutCardCount, follow) == false)
        {
            res = true;
        }
        else
        {
            res = SplitCard(mAllCards[wNextChair].ToArray(), (byte)mAllCards[wNextChair].Count, cbOutCard, cbOutCardCount, follow, isDeterminHandNumber);
        }

        return res;
    }

    /// <summary>
    /// 通用出牌
    /// </summary>
    /// <param name="wOutChairID">出牌用户</param>
    /// <param name="calcOutCard">出牌结果</param>
    /// <param name="dcHighCard">天牌结果</param>
    /// <param name="dcNoHighCard">非天牌结果</param>
    /// <returns></returns>
    public bool CommOutCard(WORD wOutChairID, List<byte> calcOutCard,
        Dictionary<byte, List<tagAnalyseCardTypeResult>> dcHighCard=null,
        Dictionary<byte, List<tagAnalyseCardTypeResult>> dcNoHighCard=null)
    {
        bool res = false;
        calcOutCard.Clear();

        Dictionary<byte, List<tagAnalyseCardTypeResult>> NoHighCard = null;// new Dictionary<BYTE, List<tagAnalyseCardTypeResult>>();
        Dictionary<byte, List<tagAnalyseCardTypeResult>> HighCard = null;// new Dictionary<BYTE, List<tagAnalyseCardTypeResult>>();
        if (dcHighCard != null) HighCard = dcHighCard;
        else HighCard = new Dictionary<BYTE, List<tagAnalyseCardTypeResult>>();
        if (dcNoHighCard != null) NoHighCard = dcNoHighCard;
        else NoHighCard = new Dictionary<BYTE, List<tagAnalyseCardTypeResult>>();

        uint NoHighCardNum = GetUserHighCard(wOutChairID, HighCard, NoHighCard);
        NoHighCardNum = (WORD)(NoHighCardNum&0x0000ffff);

        //如果 只有一手不是天牌，剩下全是天牌 或者 只剩一手牌时，出之
        if (NoHighCardNum == 1 && HighCard.Count > 0 || GetHandNumber(wOutChairID) == 1)
        {
            //四顺＞三顺＞双顺＞单顺＞四条＞三条＞对子＞单张>炸弹
            List<byte> PriotityOutCard = new List<BYTE>()
            {
                GLogicDef.CT_FOUR_LINE,
                GLogicDef.CT_THREE_LINE,
                GLogicDef.CT_DOUBLE_LINE,
                GLogicDef.CT_SINGLE_LINE,
                GLogicDef.CT_FOUR,
                GLogicDef.CT_THREE,
                GLogicDef.CT_DOUBLE,
                GLogicDef.CT_SINGLE,
                GLogicDef.CT_BOMB_CARD,
            };
            if (PriorityOutCardQueue(HighCard, calcOutCard, PriotityOutCard.ToArray()))
                res = true;            
        }

        return res;
    }

    /// <summary>
    /// 获取指定牌型最小牌型
    /// </summary>
    /// <param name="AllCardType">所有牌型数据</param>
    /// <param name="cbCardType">要出的牌型</param>
    /// <param name="calcOutCard">计算后的出牌结果</param>
    /// <returns></returns>
    bool GetSpecifiedCardTypeOutCard(Dictionary<byte, List<tagAnalyseCardTypeResult>> AllCardType,byte cbCardType, List<byte> calcOutCard,bool minOrder = true)
    {
        bool res = false;
        calcOutCard.Clear();

        switch(cbCardType)
        {
            case GLogicDef.CT_FOUR_LINE:
            case GLogicDef.CT_THREE_LINE:
            case GLogicDef.CT_DOUBLE_LINE:
            case GLogicDef.CT_SINGLE_LINE:
                {
                    if (AllCardType.ContainsKey(cbCardType))
                    {
                        AllCardType[cbCardType].Sort((a, b) =>
                        {
                            return GetCardLogicValue(a.CardData[0]).CompareTo(GetCardLogicValue(b.CardData[0])) * (minOrder? - 1 :1);
                        });
                        calcOutCard.AddRange(AllCardType[cbCardType][0].CardData);
                        res = true;
                    }
                }
                break;
            case GLogicDef.CT_FOUR:
            case GLogicDef.CT_THREE:
            case GLogicDef.CT_DOUBLE:
            case GLogicDef.CT_SINGLE:
            case GLogicDef.CT_BOMB_CARD:
                {
                    int numOutCard = 1;
                    if(cbCardType == GLogicDef.CT_DOUBLE) numOutCard = 2;
                    if(cbCardType == GLogicDef.CT_THREE) numOutCard = 3;
                    if(cbCardType == GLogicDef.CT_FOUR || cbCardType == GLogicDef.CT_BOMB_CARD) numOutCard = 4;
                    if (AllCardType.ContainsKey(cbCardType))
                    {
                        if(minOrder) AllCardType[cbCardType][0].CardData.Reverse();
                        AllCardType[cbCardType][0].CardData.CopyList(calcOutCard, numOutCard);
                        res = true;
                    }
                }
                break;
        }        

        return res;
    }    

    /// <summary>
    /// 按照牌型指定的顺序从小到大出牌
    /// </summary>
    /// <param name="AllCardType">所有牌型数据</param>
    /// <param name="calcOutCard">计算后的出牌结果</param>
    /// <param name="types">出牌牌型顺序</param>
    /// <returns></returns>
    bool PriorityOutCardQueue(Dictionary<byte, List<tagAnalyseCardTypeResult>> AllCardType,List<byte> calcOutCard,params byte[] types)
    {
        bool res = false;
        calcOutCard.Clear();
        for (int i = 0; i < types.Length;++i )
        {
            if(GetSpecifiedCardTypeOutCard(AllCardType,types[i],calcOutCard))
            {
                res = true;
                break;
            }
        }
        return res;
    }

    /// <summary>
    /// 获取天牌类型
    /// </summary>
    /// <param name="wChairID">天牌玩家</param>
    /// <param name="cbOutCard">天牌数据</param>
    /// <param name="cbOutCardCount">天牌数据个数</param>
    /// <returns>0-不是天牌，1-绝对天牌，-1-相对天牌</returns>
    public int GetHighCardType(WORD wChairID, byte[] cbOutCard, byte cbOutCardCount)
    {
        int nRes = 0;

        byte cbCardType = GetCardType(cbOutCard, cbOutCardCount);
        ////绝对天牌  (目前去掉绝对天牌)        
        //if (cbCardType == GLogicDef.CT_INVALID)
        //{
        //    Debuger.Instance.LogError("选牌错误，就不是一个牌类型！");
        //    return nRes;
        //}
        //else
        //{
        //    switch (cbCardType)
        //    {
        //        case GLogicDef.CT_SINGLE:
        //            if (GetCardValue(cbOutCard[0]) == 3) nRes = 1;
        //            break;
        //        case GLogicDef.CT_DOUBLE:
        //            if (GetCardValue(cbOutCard[0]) == 2) nRes = 1;
        //            break;
        //        case GLogicDef.CT_THREE:
        //            if (GetCardValue(cbOutCard[0]) <= 2) nRes = 1;
        //            break;
        //        case GLogicDef.CT_SINGLE_LINE:
        //            if (GetCardValue(cbOutCard[0]) == 0xd) nRes = 1;
        //            break;
        //        case GLogicDef.CT_BOMB_CARD:
        //            nRes = 1;
        //            break;
        //    }
        //}

        //相对天牌
        if (nRes != 1)
        {
            WORD wLastOfBanker = (WORD)((wChairID + 2) % wPlayerCount);
            WORD wNextOfBanker = (WORD)((wChairID + 1) % wPlayerCount);
            List<tagAnalyseCardTypeResult> result = new List<tagAnalyseCardTypeResult>();
            bool bSplit = SplitCard(mAllCards[wNextOfBanker].ToArray(), (byte)mAllCards[wNextOfBanker].Count, cbOutCard, cbOutCardCount, result,false);
            if(bSplit == false)
            {
                bSplit = SplitCard(mAllCards[wLastOfBanker].ToArray(), (byte)mAllCards[wLastOfBanker].Count, cbOutCard, cbOutCardCount, result, false);
                if (bSplit == false)
                {
                    nRes = -1;
                }
            }
            
        }

        return nRes;
    }

    /// <summary>
    /// 获取最后一个最大出牌者
    /// </summary>
    /// <returns></returns>
    public WORD GetLastOutCardWinner()
    {
        WORD wChairID = (WORD)GlobalDef.Deinfe.INVALID_CHAIR;
        List<tagOutCardRecord> list = mOutCardRecord.ToList();
        list.Reverse();
        for (int i = 0; i < list.Count; ++i)
        {
            if(list[i].cbAction == 1 || list[i].cbAction == 2)
            {
                if(list[i].outCard.Count>0)
                {
                    wChairID = list[i].wChairID;
                    break;
                }
            }
        }
        return wChairID;
    }

    /// <summary>
    /// 叫分
    /// </summary>
    /// <param name="wChair">叫分玩家</param>
    /// <param name="cbCurLandScore">当前已叫坑主分</param>
    /// <returns></returns>
    public byte LandScore(WORD wChair,byte cbCurLandScore)
    {
        int callScore = 255;
        bool mustTrench = (GetCardValue(mAllCards[wChair][2]) == 3)  ;
        mustTrench |= GetCardValue(mAllCards[wChair][0]) == 3&&GetCardValue(mAllCards[wChair][1]) == 3
            &&GetCardValue(mAllCards[wChair][2]) == 2&&GetCardValue(mAllCards[wChair][3]) == 2;

        int rmValue = Rand(0,100);//随机值
        int num_3 = 0; //3的个数
        int score = GetHandCardScore(mAllCards[wChair].ToArray(), (byte)mAllCards[wChair].Count); //手牌分数
        int handNum = GetHandNumber(wChair); //手牌数
        int bombNum = 0; //炸弹的个数
        if ( GetCutResult(wChair).ContainsKey(GLogicDef.CT_BOMB_CARD) )
        {
            bombNum = GetCutResult(wChair)[GLogicDef.CT_BOMB_CARD][0].CardData.Count / 4;
        }

        foreach(var e in mAllCards[wChair])
        {
            if(GetCardLogicValue( e )<14) break;
            if (GetCardLogicValue(e) == 16) ++num_3;
        }

        //必挖情况
        if(mustTrench)
        {
            //（1）18≤分数＜24时，50%二分，50%三分；
            //（2）24≤分数时，100%三分。
            if (score >= 18 && score < 24) callScore = rmValue < 50 ? (byte)2 : (byte)3;
            else callScore = 3;
        }
        else
        {
            if (score < 10) callScore = 255;
            else if(score >10 && score <= 15)
            {
                if (handNum <= 6) callScore = 255;
                else if (handNum > 6 && handNum <= 9)
                {

                    if (num_3 > 0) callScore = rmValue < 70 ? 255 : (rmValue < 90 ? 1 : 2);
                    else callScore = 255;
                }
                else callScore = 255;
            }
            else if(score >15 && score <= 20)
            {
                if (handNum <= 6) callScore = 255;
                else if (handNum > 6 && handNum <= 9)
                {
                    if (num_3 == 1) callScore = rmValue < 50 ? 255 : (rmValue < 90 ? 2 : 3);
                    else if (num_3 > 1) callScore = rmValue < 20 ? 255 : (rmValue < 60 ? 2 : 3);
                    else callScore = 255;
                }
                else
                {
                    if (num_3 == 1) callScore = rmValue < 60 ? 255 : 2;
                    else if (num_3 > 1) callScore = rmValue < 20 ? 255 : (rmValue < 60 ? 2 : 3);
                    else callScore = 255;
                }
            }
            else
            {
                //炸弹场
                if(bBomb)
                {
                    if (score > 20 && score <= 25)
                    {
                        if (handNum <= 6) callScore = rmValue < 40 ? 255 : (rmValue < 70 ? 2 : 3);
                        else if (handNum > 6 && handNum <= 9)
                        {
                            if (num_3 == 1)
                            {
                                if (bombNum == 0) callScore = rmValue < 30 ? 255 : (rmValue < 70 ? 2 : 3);
                                else callScore = 255;
                            }
                            else if (num_3 > 1) callScore = 255;
                            else callScore = 255;
                        }
                        else
                        {
                            if (num_3 > 1) callScore = 3;
                            else callScore = 255;
                        }
                    }
                    else if (score > 25 && score <= 30)
                    {
                        if (num_3 > 0)
                        {
                            if (bombNum == 0) callScore = rmValue < 50 ? 255 : 3;
                            else callScore = 3;
                        }
                        else callScore = 255;
                    }
                    else
                    {
                        if (num_3 == 0) callScore = 255;
                        else callScore = 3;
                    }
                }
                //普通场
                else
                {
                    if (score > 20 && score <= 25)
                    {
                        if (handNum <= 6) callScore = rmValue < 40 ? 255 : (rmValue < 70 ? 2 : 3);
                        else if (handNum > 6 && handNum <= 9)
                        {
                            if (num_3 == 1) callScore = rmValue < 30 ? 255 : (rmValue < 70 ? 2 : 3);
                            else if (num_3 > 1) callScore = 3;
                            else callScore = 255;
                        }
                        else
                        {
                            if (num_3 == 1) callScore = rmValue < 60 ? 255 : 2;
                            else if (num_3 > 1) callScore = 3;
                            else callScore = 255;
                        }
                    }
                    else
                    {
                        if (num_3 == 1) callScore = rmValue < 50 ? 255 : 3;
                        else callScore = 3;
                    }
                }                
            }                
        }

        if (callScore <= cbCurLandScore % 255) callScore = 255;

        return (byte)callScore;
    }

    //出牌
    /// <summary>
    /// 出牌处理
    /// </summary>
    /// <param name="wWillOutChairID">将要出牌用户</param>
    /// <param name="outCard">上个玩家出牌数据</param>
    /// <param name="outCardCount">出牌个数</param>
    /// <param name="calcOutCard">出牌计算结果</param>
    public void OutCard(WORD wWillOutChairID,byte[] outCard,byte outCardCount,List<byte> calcOutCard)
    {
        byte cbAction = 255;
        
        //主动出牌
        if(outCardCount == 0)
        {
            cbAction = 1;
            //坑主出牌
            if (wWillOutChairID == wBankerUser) BankerOutCard(calcOutCard); 
            //坑主下家
            else if (wWillOutChairID == (wBankerUser + 1) % wPlayerCount) NextOfBankerOutCard(calcOutCard);
            //坑主上家
            else if (wWillOutChairID == (wBankerUser + 2) % wPlayerCount) LastOfBankerOutCard(calcOutCard);
        }
        //被动出牌
        else
        {
            cbAction = 2;
            //坑主跟牌
            if (wWillOutChairID == wBankerUser) BankerFollowCard(outCard,outCardCount,calcOutCard);
            //坑主下家
            else if (wWillOutChairID == (wBankerUser + 1) % wPlayerCount) NextOfBankerFollowCard(outCard, outCardCount, calcOutCard);
            //坑主上家
            else if (wWillOutChairID == (wBankerUser + 2) % wPlayerCount) LastOfBankerFollowCard(outCard, outCardCount, calcOutCard);

            if(calcOutCard.Count == 0)//不要
            {
                cbAction = 0;
            }
        }

        if ((cbAction==1) && (calcOutCard.Count == 0 ))
        {
            Debuger.Instance.LogError("出牌错误了，机器人SB了，不知道出啥了!");
            return;
        }
        else
        {
            if (cbAction != 0)
            {
                SortCardList(calcOutCard, (byte)calcOutCard.Count);

                //替换底牌 坑主出牌/跟牌
                if (wBankerUser == wWillOutChairID)
                {
                //    List<byte> calcTmp = calcOutCard.ToList();
                //    List<byte> backTmp = mBackCard.ToList();
                //    List<byte> oldAdd = new List<BYTE>();

                //    calcTmp.RemoveAll(a =>
                //    { 
                //        if((backTmp.IndexOf(a) != -1))
                //        {
                //            backTmp.Remove(a);
                //            oldAdd.Add(a);
                //            return true;
                //        }
                //        return false;
                //    });
                //    //和坑底的牌对换
                //    for (int i = 0; i < calcTmp.Count; ++i)
                //    {
                //        //是不是坑底的
                //        int index = backTmp.FindIndex(a => { return GetCardLogicValue(a) == GetCardLogicValue(calcTmp[i]); });
                //        if (index == -1) continue;

                //        calcTmp[i] = backTmp[index];
                //        backTmp.RemoveAt(index);

                //        if (backTmp.Count == 0) break;
                //    }

                //    calcOutCard = calcTmp;
                //    calcOutCard.AddRange(oldAdd);
                //    SortCardList(calcOutCard, (byte)calcOutCard.Count);
                }
                //去掉出牌重新切牌
                RemoveUserCardData(wWillOutChairID, calcOutCard.ToArray(), (byte)calcOutCard.Count);
                CutUserCard(wWillOutChairID, mAllCards[wWillOutChairID].ToArray(), (byte)mAllCards[wWillOutChairID].Count);                
            }
            //记录
            mOutCardRecord.Add(new tagOutCardRecord(wWillOutChairID, cbAction, calcOutCard));
        }
    }

    /// <summary>
    /// 坑主出牌
    /// </summary>
    /// <param name="calcOutCard"></param>
    void BankerOutCard(List<byte> calcOutCard)
    {
        WORD wMeChairID = wBankerUser;
        WORD wLastChairID = (WORD)((wBankerUser + 2) % wPlayerCount);
        WORD wNextChairID = (WORD)((wBankerUser + 1) % wPlayerCount);

        var HighCard = new Dictionary<byte, List<tagAnalyseCardTypeResult>>();
        var NoHighCard = new Dictionary<byte, List<tagAnalyseCardTypeResult>>();
        if (CommOutCard(wMeChairID, calcOutCard, HighCard, NoHighCard) == false)
        {
            var listTmp = GetCutResult(wMeChairID);
            var listTmpLast = GetCutResult(wLastChairID);
            var listTmpNext = GetCutResult(wNextChairID);
            //进入角色出牌判断
            if (mAllCards[wLastChairID].Count == 1 || mAllCards[wNextChairID].Count == 1)
            {                
                //任意一个平民只剩1张牌时
                if ( (listTmp.Count == 1 && listTmp.ContainsKey(GLogicDef.CT_SINGLE))
                    || (listTmp.Count == 2 && listTmp.ContainsKey(GLogicDef.CT_SINGLE)
                        && listTmp.ContainsKey(GLogicDef.CT_BOMB_CARD))
                    )
                {
                    byte cbSinglData = mAllCards[wLastChairID].Count == 1 ? mAllCards[wLastChairID][0] : mAllCards[wNextChairID][0];
                    
                    listTmp[GLogicDef.CT_SINGLE][0].CardData.Reverse();

                    foreach (var f in listTmp[GLogicDef.CT_SINGLE][0].CardData)
                    {
                        if (GetCardLogicValue(f) > GetCardLogicValue(cbSinglData))
                        {
                            calcOutCard.Add(f);
                            return;
                        }
                    }

                    if (listTmp.ContainsKey(GLogicDef.CT_BOMB_CARD))
                    {
                        listTmp[GLogicDef.CT_BOMB_CARD][0].CardData.Reverse();
                        listTmp[GLogicDef.CT_BOMB_CARD][0].CardData.CopyList(calcOutCard, 4);
                        return;
                    }

                    calcOutCard.Add(listTmp[GLogicDef.CT_SINGLE][0].CardData[0]);
                    return;
                }
                else
                {
                    //先出天牌在自己手中的牌型（单牌除外）
                    //普通场 四顺＞三顺＞双顺＞单顺＞四条＞三条＞对子
                    //炸弹场 三顺＞双顺＞单顺＞三条＞对子
                    List<byte> OutCardTypeOrder = new List<BYTE>();
                    OutCardTypeOrder.Add(GLogicDef.CT_FOUR_LINE);
                    OutCardTypeOrder.Add(GLogicDef.CT_THREE_LINE);
                    OutCardTypeOrder.Add(GLogicDef.CT_DOUBLE_LINE);
                    OutCardTypeOrder.Add(GLogicDef.CT_SINGLE_LINE);
                    OutCardTypeOrder.Add(GLogicDef.CT_FOUR);
                    OutCardTypeOrder.Add(GLogicDef.CT_THREE);
                    OutCardTypeOrder.Add(GLogicDef.CT_DOUBLE);
                    if(PriorityOutCardQueue(HighCard,calcOutCard,OutCardTypeOrder.ToArray()) )
                    {
                        return;
                    }

                    //然后出天牌不在自己手中的牌型（单牌除外）
                    if (PriorityOutCardQueue(NoHighCard, calcOutCard, OutCardTypeOrder.ToArray()))
                    {
                        return;
                    }                    
                }
            }
            else
            {
                List<byte> EnemyNoHaveCardType = new List<BYTE>();

                //自己有对家没有的牌型
                foreach(var e in listTmp)
                {
                     if(listTmpLast.ContainsKey( e.Key) == false
                         && listTmpNext.ContainsKey(e.Key) == false)
                     {
                         EnemyNoHaveCardType.Add(e.Key);
                     }
                }

                foreach(var e in EnemyNoHaveCardType)
                {
                    if(GetSpecifiedCardTypeOutCard(listTmp, e, calcOutCard))
                    {
                        return;
                    }
                }

                List<byte> PriorityOutCard = null;
                if(HighCard.Count > 0)
                {                    
                    if(bBomb)
                    {
                        //炸弹场按照“单张＞对子＞单顺＞三条＞双顺＞三顺＞炸弹
                        PriorityOutCard = new List<BYTE>(){
                            GLogicDef.CT_SINGLE,
                            GLogicDef.CT_DOUBLE,
                            GLogicDef.CT_SINGLE_LINE,
                            GLogicDef.CT_THREE,
                            GLogicDef.CT_DOUBLE_LINE,
                            GLogicDef.CT_THREE_LINE,
                            GLogicDef.CT_BOMB_CARD,
                        };
                    }
                    else
                    {
                        //普通场按照“单张＞对子＞单顺＞三条＞双顺＞四条＞三顺＞四顺
                        PriorityOutCard = new List<BYTE>(){
                            GLogicDef.CT_SINGLE,
                            GLogicDef.CT_DOUBLE,
                            GLogicDef.CT_SINGLE_LINE,
                            GLogicDef.CT_THREE,
                            GLogicDef.CT_DOUBLE_LINE,
                            GLogicDef.CT_FOUR,
                            GLogicDef.CT_THREE_LINE,
                            GLogicDef.CT_FOUR_LINE,
                        };                        
                    }
                    if (PriorityOutCardQueue(HighCard, calcOutCard, PriorityOutCard.ToArray()))
                    {
                        return;
                    }
                }
                else
                {
                    if (bBomb)
                    {
                        //炸弹场按照 三顺＞双顺＞单顺＞三条＞对子＞单张＞炸弹
                        PriorityOutCard = new List<BYTE>(){
                            GLogicDef.CT_THREE_LINE,
                            GLogicDef.CT_DOUBLE_LINE,
                            GLogicDef.CT_SINGLE_LINE,
                            GLogicDef.CT_THREE,
                            GLogicDef.CT_DOUBLE,
                            GLogicDef.CT_SINGLE,
                            GLogicDef.CT_BOMB_CARD,
                        };
                    }
                    else
                    {
                        //普通场按照 四顺＞三顺＞双顺＞单顺＞四条＞三条＞对子＞单张
                        PriorityOutCard = new List<BYTE>(){
                            GLogicDef.CT_FOUR_LINE,
                            GLogicDef.CT_THREE_LINE,
                            GLogicDef.CT_DOUBLE_LINE,
                            GLogicDef.CT_SINGLE_LINE,
                            GLogicDef.CT_FOUR,
                            GLogicDef.CT_THREE,
                            GLogicDef.CT_DOUBLE,
                            GLogicDef.CT_SINGLE,
                        };
                    }
                    if (PriorityOutCardQueue(NoHighCard, calcOutCard, PriorityOutCard.ToArray()))
                    {
                        return;
                    }
                }
            }
        }        
    }
    /// <summary>
    /// 坑主上家出牌
    /// </summary>
    /// <param name="calcOutCard"></param>
    void LastOfBankerOutCard(List<byte> calcOutCard)
    {
        WORD wBankerChairID = wBankerUser;
        WORD wMeChairID = (WORD)((wBankerUser + 2) % wPlayerCount);
        WORD wLastChairID = (WORD)((wBankerUser + 1) % wPlayerCount);

        var meListTmp = GetCutResult(wMeChairID);
        var meHighCard = new Dictionary<byte, List<tagAnalyseCardTypeResult>>();
        var meNoHighCard = new Dictionary<byte, List<tagAnalyseCardTypeResult>>();
        List<byte> ctList = new List<byte>(new byte[]{
			GLogicDef.CT_SINGLE,
			GLogicDef.CT_DOUBLE,
			GLogicDef.CT_SINGLE_LINE,
			GLogicDef.CT_THREE,
			GLogicDef.CT_DOUBLE_LINE,
			GLogicDef.CT_FOUR,
			GLogicDef.CT_THREE_LINE,
			GLogicDef.CT_BOMB_CARD,
			GLogicDef.CT_FOUR_LINE
		}
        );
        List<byte> ctList2 = new List<byte>(new byte[]{
			GLogicDef.CT_BOMB_CARD,
			GLogicDef.CT_FOUR_LINE,
			GLogicDef.CT_THREE_LINE,
			GLogicDef.CT_DOUBLE_LINE,
			GLogicDef.CT_SINGLE_LINE,
			GLogicDef.CT_FOUR,
			GLogicDef.CT_THREE,
			GLogicDef.CT_DOUBLE,
			GLogicDef.CT_SINGLE,
		}
        );
        List<byte> _ctList = new List<byte>();
        if (CommOutCard(wMeChairID, calcOutCard, meHighCard, meNoHighCard) == false)
        {
            if (mAllCards[wBankerChairID].Count == 1)
            {
                _ctList.Clear();
                _ctList.AddRange(ctList.ToArray());
                _ctList.Remove(GLogicDef.CT_SINGLE);
                foreach (byte ct in _ctList)
                {
                    if (GetSpecifiedCardTypeOutCard(meListTmp, ct, calcOutCard))
                    {
                        return;
                    }
                }
                if (meListTmp.ContainsKey(GLogicDef.CT_SINGLE))
                {
                    calcOutCard.Add(meListTmp[GLogicDef.CT_SINGLE][0].CardData[0]);
                }
            }
            else if (mAllCards[wBankerChairID].Count == 2)
            {
                if (GetCutResult(wBankerChairID).ContainsKey(GLogicDef.CT_DOUBLE))
                {
                    _ctList.Clear();
                    _ctList.AddRange(ctList2.ToArray());
                    _ctList.Remove(GLogicDef.CT_DOUBLE);
                    _ctList.Remove(GLogicDef.CT_BOMB_CARD);
                    foreach (byte ct in _ctList)
                    {
                        if (GetSpecifiedCardTypeOutCard(meListTmp, ct, calcOutCard))
                        {
                            return;
                        }
                    }
                    if (meListTmp.ContainsKey(GLogicDef.CT_DOUBLE))
                    {
                        calcOutCard.AddRange(meListTmp[GLogicDef.CT_DOUBLE][0].CardData.ToArray());
                    }
                }
                else
                {
                    _ctList.Clear();
                    _ctList.AddRange(ctList2.ToArray());
                    _ctList.Remove(GLogicDef.CT_SINGLE);
                    _ctList.Remove(GLogicDef.CT_BOMB_CARD);
                    foreach (byte ct in _ctList)
                    {
                        if (GetSpecifiedCardTypeOutCard(meListTmp, ct, calcOutCard))
                        {
                            return;
                        }
                    }
                    if (meListTmp.ContainsKey(GLogicDef.CT_SINGLE))
                    {
                        calcOutCard.Add(meListTmp[GLogicDef.CT_SINGLE][0].CardData[0]);
                    }
                }
            }
            else
            {
                var bankerListTmp = GetCutResult(wMeChairID);
                foreach (byte ct in ctList)
                {
                    if (meListTmp.ContainsKey(ct) && !bankerListTmp.ContainsKey(ct))
                    {
                        GetSpecifiedCardTypeOutCard(meListTmp, ct, calcOutCard);
                        return;
                    }
                }

                var lastHighCard = new Dictionary<byte, List<tagAnalyseCardTypeResult>>();
                var lastNoHighCard = new Dictionary<byte, List<tagAnalyseCardTypeResult>>();
                var bankerHighCard = new Dictionary<byte, List<tagAnalyseCardTypeResult>>();
                var bankerNoHighCard = new Dictionary<byte, List<tagAnalyseCardTypeResult>>();
                GetUserHighCard(wLastChairID, lastHighCard, lastNoHighCard);
                GetUserHighCard(wBankerChairID, bankerHighCard, bankerNoHighCard);

                foreach (byte ct in ctList)
                {
                    if ((meHighCard.ContainsKey(ct) || (lastHighCard.ContainsKey(ct) && meListTmp.ContainsKey(ct))) && !bankerListTmp.ContainsKey(ct))
                    {
                        GetSpecifiedCardTypeOutCard(meListTmp, ct, calcOutCard);
                        return;
                    }
                }
                _ctList.Clear();
                _ctList.AddRange(ctList2.ToArray());
                _ctList.Remove(GLogicDef.CT_BOMB_CARD);
                foreach (byte ct in _ctList)
                {
                    if (GetSpecifiedCardTypeOutCard(meListTmp, ct, calcOutCard))
                    {
                        return;
                    }
                }
            }
        }
    }
    /// <summary>
    /// 坑主下家出牌
    /// </summary>
    /// <param name="calcOutCard"></param>
    void NextOfBankerOutCard(List<byte> calcOutCard)
    {
        WORD wMeChairID = wBankerUser;
        WORD wLastChairID = (WORD)((wBankerUser + 2) % wPlayerCount);
        WORD wNextChairID = (WORD)((wBankerUser + 1) % wPlayerCount);

        //手中天牌
        var dcHighCard = new Dictionary<BYTE, List<tagAnalyseCardTypeResult>>();
        var dcNoHighCard = new Dictionary<byte, List<tagAnalyseCardTypeResult>>();
        if (CommOutCard(wNextChairID, calcOutCard, dcHighCard, dcNoHighCard) == false)
        {
            calcOutCard.Clear();
            var listTemp = GetCutResult(wMeChairID);
            var listTempLast = GetCutResult(wLastChairID);
            var listTempNext = GetCutResult(wNextChairID);
            List<byte> OutCardTypeOrder;
            if (bBomb)
            {
                //炸弹场按照 对子＞单顺＞三条＞双顺＞三顺＞炸弹
                OutCardTypeOrder = new List<BYTE>(){
                        GLogicDef.CT_DOUBLE,
                        GLogicDef.CT_SINGLE_LINE,
                        GLogicDef.CT_THREE,
                        GLogicDef.CT_DOUBLE_LINE,
                        GLogicDef.CT_THREE_LINE,
                        GLogicDef.CT_BOMB_CARD,
                    };
            }
            else
            {
                //普通场按照 对子＞单顺＞三条＞双顺＞四条＞三顺＞四顺
                OutCardTypeOrder = new List<BYTE>(){
                        GLogicDef.CT_DOUBLE,
                        GLogicDef.CT_SINGLE_LINE,
                        GLogicDef.CT_THREE,
                        GLogicDef.CT_DOUBLE_LINE,
                        GLogicDef.CT_FOUR,
                        GLogicDef.CT_THREE_LINE,
                        GLogicDef.CT_FOUR_LINE,
                    };
            }
            //坑主上家和坑主都只剩1张牌时
            if (mAllCards[wLastChairID].Count == 1 && mAllCards[wMeChairID].Count == 1)
            {

                //自己只有两张单牌且有一张是天牌
                if (dcHighCard.ContainsKey(GLogicDef.CT_SINGLE)
                    && listTempNext[GLogicDef.CT_SINGLE][0].CardData.Count < 3)
                {

                    if (PriorityOutCardQueue(listTempNext, calcOutCard, OutCardTypeOrder.ToArray()))
                        return;
                    else
                    {
                        foreach (var c in dcHighCard[GLogicDef.CT_SINGLE][0].CardData)
                        {
                            calcOutCard.Add(c);
                            return;
                        }
                    }
                }
                else
                {
                    //出自己手中最小的一张牌
                    var handCard = GetUserCard(wNextChairID);
                    calcOutCard.Add(handCard[handCard.Count - 1]);
                    return;

                }
            }//坑主上家还有多张牌时,坑主只剩1张牌
            else if (mAllCards[wLastChairID].Count > 1 && mAllCards[wMeChairID].Count == 1)
            {
                //优先出单牌以外的牌型
                if (PriorityOutCardQueue(listTempNext, calcOutCard, OutCardTypeOrder.ToArray()))
                    return;
                else
                {
                    //只剩下单张，则从小到大出牌
                    GetSpecifiedCardTypeOutCard(listTempNext, GLogicDef.CT_SINGLE, calcOutCard);
                    return;

                }


            }//坑主上家只剩1张牌，坑主还剩多张牌时
            else if (mAllCards[wLastChairID].Count == 1 && mAllCards[wMeChairID].Count > 1)
            {

                //先出最小的一张牌，若坑主上家选择“不出”则下次出牌时进入以下步奏\
                byte cardType = 0;
                for (int i = mOutCardRecord.Count - 1; i >= 0; i--)
                {
                    if (mOutCardRecord[i].wChairID == wNextChairID && mOutCardRecord[i].cbAction == 1)
                    {
                        cardType = GetCardType(mOutCardRecord[i].outCard.ToArray(), (byte)mOutCardRecord[i].outCard.Count);
                        if (cardType == GLogicDef.CT_SINGLE
                           && (i == (mOutCardRecord.Count - 1) || mOutCardRecord[i + 1].cbAction == 0))
                        {
                            break;
                        }
                        else
                        {
                            var handCard = GetUserCard(wNextChairID);
                            calcOutCard.Add(handCard[handCard.Count - 1]);
                            return;
                        }
                    }
                }
                //出自己有但是坑主没有的牌型
                foreach (var e in listTempNext)
                {
                    if (listTemp.ContainsKey(e.Key) == false)
                    {
                        if (GetSpecifiedCardTypeOutCard(listTempNext, e.Key, calcOutCard))
                            return;
                        else
                            continue;
                    }
                }

                //然后出自己有天牌，但是坑主没有天牌的牌型
                OutCardTypeOrder.Insert(0, GLogicDef.CT_SINGLE);
                var tempHigh = new Dictionary<byte, List<tagAnalyseCardTypeResult>>();
                foreach (var v in dcHighCard)
                {
                    tempHigh.Add(v.Key, listTempNext[v.Key]);
                }
                if (PriorityOutCardQueue(tempHigh, calcOutCard, OutCardTypeOrder.ToArray()))
                    return;
                //然后出坑主有天牌，自己没有天牌的牌型
                if (bBomb)
                {
                    //炸弹场按照 三顺＞双顺＞单顺＞三条＞对子
                    OutCardTypeOrder = new List<BYTE>(){
                            GLogicDef.CT_THREE_LINE,
                            GLogicDef.CT_DOUBLE_LINE,
                            GLogicDef.CT_SINGLE_LINE,
                            GLogicDef.CT_THREE,
                            GLogicDef.CT_DOUBLE,
                        };
                }
                else
                {
                    //普通场按照 四顺＞三顺＞双顺＞单顺＞四条＞三条＞对子
                    OutCardTypeOrder = new List<BYTE>(){
                            GLogicDef.CT_FOUR_LINE,
                            GLogicDef.CT_THREE_LINE,
                            GLogicDef.CT_DOUBLE_LINE,
                            GLogicDef.CT_SINGLE_LINE,
                            GLogicDef.CT_FOUR,
                            GLogicDef.CT_THREE,
                            GLogicDef.CT_DOUBLE,
                        
                        };
                }
                if (PriorityOutCardQueue(listTempNext, calcOutCard, OutCardTypeOrder.ToArray()))
                    return;


            }//坑主手牌数量＞1
            else if (mAllCards[wMeChairID].Count > 1)
            {

                //1优先出自己有但是坑主没有的牌型
                foreach (var v in listTempNext)
                {
                    if (listTemp.ContainsKey(v.Key) == false)
                    {
                        if (GetSpecifiedCardTypeOutCard(listTempNext, v.Key, calcOutCard))
                            return;
                        else
                            continue;
                    }
                }


                if (bBomb)
                {
                    //炸弹场按照 单张＞对子＞单顺＞三条＞双顺＞三顺＞炸弹
                    OutCardTypeOrder = new List<BYTE>(){
                            GLogicDef.CT_SINGLE,
                            GLogicDef.CT_DOUBLE,
                            GLogicDef.CT_SINGLE_LINE,
                            GLogicDef.CT_THREE,
                            GLogicDef.CT_DOUBLE_LINE,
                            GLogicDef.CT_THREE_LINE,
                            GLogicDef.CT_BOMB_CARD
                            
                        };
                }
                else
                {
                    //普通场按照 单张＞对子＞单顺＞三条＞双顺＞四条＞三顺＞四顺
                    OutCardTypeOrder = new List<BYTE>(){
                            GLogicDef.CT_SINGLE,
                            GLogicDef.CT_DOUBLE,
                            GLogicDef.CT_SINGLE_LINE,
                            GLogicDef.CT_THREE,
                            GLogicDef.CT_DOUBLE_LINE,
                            GLogicDef.CT_FOUR,
                            GLogicDef.CT_THREE_LINE,
                            GLogicDef.CT_FOUR_LINE,
                            
                        };
                }
                //2然后出自己或坑主上家有天牌，但是坑主没有天牌的牌型
                var bankerHighCard = new Dictionary<BYTE, List<tagAnalyseCardTypeResult>>();
                GetUserHighCard(wMeChairID, bankerHighCard, null);
                var dcLastNextHighCard = new Dictionary<BYTE, List<tagAnalyseCardTypeResult>>();
                foreach (var v in listTempNext)
                {
                    if (!bankerHighCard.ContainsKey(v.Key))
                        dcLastNextHighCard.Add(v.Key, listTempNext[v.Key]);
                }
                if (PriorityOutCardQueue(dcLastNextHighCard, calcOutCard, OutCardTypeOrder.ToArray()))
                    return;
                //3然后出坑主有天牌，自己和坑主上家都没有天牌的牌型
                if (bBomb)
                {
                    //炸弹场按照 三顺＞双顺＞单顺＞三条＞对子
                    OutCardTypeOrder = new List<BYTE>(){
                            GLogicDef.CT_THREE_LINE,
                            GLogicDef.CT_DOUBLE_LINE,
                            GLogicDef.CT_SINGLE_LINE,
                            GLogicDef.CT_THREE,
                            GLogicDef.CT_DOUBLE,
                            
                        };
                }
                else
                {
                    //普通场按照 四顺＞三顺＞双顺＞单顺＞四条＞三条＞对子
                    OutCardTypeOrder = new List<BYTE>(){
                        GLogicDef.CT_FOUR_LINE,
                        GLogicDef.CT_THREE_LINE,
                        GLogicDef.CT_DOUBLE_LINE,
                        GLogicDef.CT_SINGLE_LINE,
                        GLogicDef.CT_FOUR,
                        GLogicDef.CT_THREE,
                        GLogicDef.CT_DOUBLE,
                        GLogicDef.CT_SINGLE,
                    };
                }
                if (PriorityOutCardQueue(listTempNext, calcOutCard, OutCardTypeOrder.ToArray()))
                    return;
            }

        }//end 通用出牌
    }//end 地主下家出牌
    /// <summary>
    /// 坑主跟牌
    /// </summary>
    /// <param name="calcOutCard"></param>
    void BankerFollowCard(byte[] outCard, byte outCardCount, List<byte> calcOutCard)
    {
        WORD wMeChairID = wBankerUser;
        WORD wLastChairID = (WORD)((wBankerUser + 2) % wPlayerCount);
        WORD wNextChairID = (WORD)((wBankerUser + 1) % wPlayerCount);

        calcOutCard.Clear();

        byte cbCardType = GetCardType(outCard, outCardCount);
        if(cbCardType == GLogicDef.CT_INVALID)
        {
            Debuger.Instance.LogError("牌型错误");
            return;
        }

        //最后一个出牌
        WORD wLastOutWinner = GetLastOutCardWinner();

        List<tagAnalyseCardTypeResult> result = new List<tagAnalyseCardTypeResult>();
        if (CommFollowCard(wLastChairID, outCard, outCardCount, result,false) == false) return;
        
        //切牌结果
        var dcUserCutCard =  GetCutResult(wMeChairID);
        switch (cbCardType)
        {
            case GLogicDef.CT_SINGLE:
                {
                    #region 单牌处理
                    //两个对家的牌都≥6张时
                    if (mAllCards[wLastChairID].Count >= 6 && mAllCards[wNextChairID].Count >= 6)
                    {
                        if(dcUserCutCard.ContainsKey(cbCardType))
                        {
                            dcUserCutCard[cbCardType][0].CardData.Reverse();
                            for(int i = 0 ; i<dcUserCutCard[cbCardType][0].CardData.Count ; i++ )
                            {
                                if(GetCardValue( dcUserCutCard[cbCardType][0].CardData[i] ) == 3 ) break;
                                if(GetCardLogicValue( dcUserCutCard[cbCardType][0].CardData[i] ) > GetCardLogicValue(outCard[0]) )
                                {
                                    calcOutCard.Add(dcUserCutCard[cbCardType][0].CardData[i]);
                                    return;
                                }                               
                            }
                        }
                        
                        //如果没有单牌但是有222和AAA的三条时，就拆三条跟之（先拆大的）；
                        if (dcUserCutCard.ContainsKey(GLogicDef.CT_THREE))
                        {
                            for (int i = 0; i < dcUserCutCard[GLogicDef.CT_THREE][0].CardData.Count / 3; i += 3)
                            {
                                if (GetCardLogicValue(dcUserCutCard[GLogicDef.CT_THREE][0].CardData[i]) == 16) continue;
                                if (GetCardLogicValue(dcUserCutCard[GLogicDef.CT_THREE][0].CardData[i]) < 14) break;
                                if (GetCardLogicValue(dcUserCutCard[GLogicDef.CT_THREE][0].CardData[i]) <= GetCardLogicValue(outCard[0])) break;
                                dcUserCutCard[GLogicDef.CT_THREE][0].CardData.CopyList(calcOutCard, outCardCount, i);
                                return;
                            }
                        }

                        if (GetCardValue(mAllCards[wMeChairID][0]) == 3)
                        {
                            calcOutCard.Add(mAllCards[wMeChairID][0]);
                            return;
                        }

                    }
                    else
                    {
                        //有单牌就跟一张最大的（切牌后单牌中的最大的一张，3除外）
                        if (dcUserCutCard.ContainsKey(GLogicDef.CT_SINGLE))
                        {
                            for (int i = 0; i < dcUserCutCard[GLogicDef.CT_SINGLE][0].CardData.Count; i++)
                            {
                                if (GetCardLogicValue(dcUserCutCard[GLogicDef.CT_SINGLE][0].CardData[i]) == 16) continue;
                                if (GetCardLogicValue(dcUserCutCard[GLogicDef.CT_SINGLE][0].CardData[i]) <= GetCardLogicValue(outCard[0])) break;
                                dcUserCutCard[GLogicDef.CT_SINGLE][0].CardData.CopyList(calcOutCard, outCardCount, i);
                                return;
                            }

                        }

                        calcOutCard.Add(mAllCards[wMeChairID][0]);
                    }
                    #endregion
                    return;
                }
            case GLogicDef.CT_DOUBLE:
                {
                    #region 对牌处理
                    if (mAllCards[wLastChairID].Count >= 6 && mAllCards[wNextChairID].Count >= 6)
                    {
                        var dcTmp = new Dictionary<byte,List<tagAnalyseCardTypeResult>>(dcUserCutCard);
                        //有对子就跟一个最小的（切牌后对子中的最小的一对，33除外）
                        if(dcTmp.ContainsKey(GLogicDef.CT_DOUBLE))
                        {
                            dcTmp[GLogicDef.CT_DOUBLE][0].CardData.Reverse();

                            for (int i = 0; i < dcTmp[GLogicDef.CT_DOUBLE][0].CardData.Count;i+=2 )
                            {
                                if (GetCardValue(dcTmp[GLogicDef.CT_DOUBLE][0].CardData[i]) == 3) break;
                                if (GetCardLogicValue(dcTmp[GLogicDef.CT_DOUBLE][0].CardData[i]) > GetCardLogicValue(outCard[0]))
                                {
                                    dcTmp[GLogicDef.CT_DOUBLE][0].CardData.CopyList(calcOutCard, outCardCount, i);
                                    return;
                                }                                
                            }
                                                             
                        }
                        //如果没有对子但是有四连以上的双顺（比如KKQQJJ1010），则跟其中最大的一对
                        if(dcTmp.ContainsKey(GLogicDef.CT_DOUBLE_LINE))
                        {
                            //从大到小
                            dcTmp[GLogicDef.CT_DOUBLE_LINE].Sort((a,b)=>
                                {
                                    return GetCardValue( a.CardData[0] ).CompareTo(GetCardValue(b.CardData[0]) );
                                });
                            foreach(var e in dcTmp[GLogicDef.CT_DOUBLE_LINE])
                            {
                                if( e.CardData.Count >= 2*4 && GetCardValue(e.CardData[0])>GetCardValue(outCard[0]) )
                                {
                                    e.CardData.CopyList(calcOutCard,outCardCount);
                                    return;
                                }
                            }
                        }
                        //拆最大的四条
                        if(dcTmp.ContainsKey(GLogicDef.CT_FOUR))
                        {
                            for(int i = 0;i< dcTmp[GLogicDef.CT_FOUR][0].CardData.Count;i+=4)
                            {
                                if( GetCardValue( dcTmp[GLogicDef.CT_FOUR][0].CardData[i] ) == 3 )continue;
                                if (GetCardLogicValue(dcTmp[GLogicDef.CT_FOUR][0].CardData[i]) <= GetCardLogicValue(outCard[0])) break;
                                dcTmp[GLogicDef.CT_FOUR][0].CardData.CopyList(calcOutCard, outCardCount, i);
                                return;
                            }
                        }
                        //拆最大的三条
                        if(dcTmp.ContainsKey(GLogicDef.CT_THREE))
                        {
                            for(int i = 0;i< dcTmp[GLogicDef.CT_THREE][0].CardData.Count;i+=3)
                            {
                                if( GetCardValue( dcTmp[GLogicDef.CT_THREE][0].CardData[i] ) == 3 )continue;
                                if (GetCardLogicValue(dcTmp[GLogicDef.CT_THREE][0].CardData[i]) <= GetCardLogicValue(outCard[0])) break;
                                dcTmp[GLogicDef.CT_THREE][0].CardData.CopyList(calcOutCard,outCardCount,i);
                                return;
                            }
                        }                        
                    }
                    else
                    {
                        var dcTmp = new Dictionary<byte, List<tagAnalyseCardTypeResult>>(dcUserCutCard);
                        if (dcTmp.ContainsKey(GLogicDef.CT_DOUBLE))
                        {   
                            dcTmp[GLogicDef.CT_DOUBLE][0].CardData.Reverse();
                            dcTmp[GLogicDef.CT_DOUBLE][0].CardData.RemoveAll(a => { return GetCardValue(a) == 3; });
                            for (int i = 0; i < dcTmp[GLogicDef.CT_DOUBLE][0].CardData.Count; i+= 2)
                            {
                                if (GetCardValue(dcTmp[GLogicDef.CT_DOUBLE][0].CardData[i]) == 3) break;
                                if (GetCardLogicValue(dcTmp[GLogicDef.CT_DOUBLE][0].CardData[i]) > GetCardLogicValue(outCard[0]))
                                {
                                    dcTmp[GLogicDef.CT_DOUBLE][0].CardData.CopyList(calcOutCard, outCardCount, i);
                                    return;
                                }
                                
                            }
                        }
                        if (dcTmp.ContainsKey(GLogicDef.CT_DOUBLE_LINE))
                        {
                            //从大到小
                            dcTmp[GLogicDef.CT_DOUBLE_LINE].Sort((a, b) =>
                            {
                                return GetCardLogicValue(a.CardData[0]).CompareTo(GetCardLogicValue(b.CardData[0]));
                            });
                            foreach (var e in dcTmp[GLogicDef.CT_DOUBLE_LINE])
                            {
                                for ( int i = e.CardData.Count-1; i>=0 ;i-=2 )
                                {
                                    if (GetCardLogicValue(e.CardData[i]) > GetCardLogicValue(outCard[0]))
                                    {
                                        e.CardData.CopyList(calcOutCard, outCardCount);
                                        return;
                                    }
                                }                                    
                            }
                        }
                        
                        if (dcTmp.ContainsKey(GLogicDef.CT_FOUR))
                        {
                            for (int i = 0; i < dcTmp[GLogicDef.CT_FOUR][0].CardData.Count; i += 4)
                            {
                                if (GetCardValue(dcTmp[GLogicDef.CT_FOUR][0].CardData[i]) == 3) continue;
                                if (GetCardLogicValue(dcTmp[GLogicDef.CT_FOUR][0].CardData[i]) <= GetCardLogicValue(outCard[0])) break;
                                dcTmp[GLogicDef.CT_FOUR][0].CardData.CopyList(calcOutCard, outCardCount, i);
                                return;
                            }
                        }

                        if (dcTmp.ContainsKey(GLogicDef.CT_THREE))
                        {
                            for (int i = 0; i < dcTmp[GLogicDef.CT_THREE][0].CardData.Count; i += 3)
                            {
                                if (GetCardValue(dcTmp[GLogicDef.CT_THREE][0].CardData[i]) == 3) continue;
                                if (GetCardLogicValue(dcTmp[GLogicDef.CT_THREE][0].CardData[i]) <= GetCardLogicValue(outCard[0]))
                                dcTmp[GLogicDef.CT_THREE][0].CardData.CopyList(calcOutCard, outCardCount, i);
                                return;
                            }
                        }
                        if (mAllCards[wMeChairID].Count >= 2 && GetCardValue(mAllCards[wMeChairID][1]) == 3)
                        {
                            mAllCards[wMeChairID].CopyList(calcOutCard, outCardCount);
                            return;
                        }
                    }
                    if (bBomb) goto BombDeal;
                    #endregion
                    return;
                }
            case GLogicDef.CT_THREE:
                {
                    #region 三条处理
                    var dcTmp = new Dictionary<byte, List<tagAnalyseCardTypeResult>>(dcUserCutCard);
                    if(dcTmp.ContainsKey(GLogicDef.CT_THREE))
                    {
                        dcTmp[GLogicDef.CT_THREE][0].CardData.Reverse();
                        for (int i = 0; i < dcTmp[GLogicDef.CT_THREE][0].CardData.Count; i += 3)
                        {
                            if (GetCardValue(dcTmp[GLogicDef.CT_THREE][0].CardData[i]) == 3) break;
                            if (GetCardLogicValue(dcTmp[GLogicDef.CT_THREE][0].CardData[i]) > GetCardLogicValue(outCard[0]))
                            {
                                dcTmp[GLogicDef.CT_THREE][0].CardData.CopyList(calcOutCard, outCardCount, i);
                                return;
                            }
                            
                        }
                    }
                  
                    if (dcTmp.ContainsKey(GLogicDef.CT_THREE_LINE))
                    {
                        //从小到大
                        dcTmp[GLogicDef.CT_THREE_LINE].Sort((a, b) =>
                            {
                                return GetCardLogicValue(a.CardData[a.CardData.Count - 1]).CompareTo(GetCardLogicValue(b.CardData[b.CardData.Count - 1])) * -1;
                            });
                        foreach (var e in dcTmp[GLogicDef.CT_THREE_LINE])
                        {
                            for(int i=e.CardData.Count-3;i>=0;i-=3)
                            {
                                if (GetCardValue(e.CardData[i]) == 3) continue;
                                if (GetCardLogicValue(e.CardData[i]) > GetCardLogicValue(outCard[0]))
                                {
                                   e.CardData.CopyList(calcOutCard, outCardCount, i);
                                    return;
                                }                                
                            }
                        }
                    }
 
                    if (dcTmp.ContainsKey(GLogicDef.CT_FOUR))
                    {
                        for(int i = 0;i<dcTmp[GLogicDef.CT_FOUR][0].CardData.Count;i+=4)
                        {
                            if (GetCardValue(dcTmp[GLogicDef.CT_FOUR][0].CardData[i]) == 3) continue;
                            if (GetCardValue(dcTmp[GLogicDef.CT_FOUR][0].CardData[i]) <= GetCardValue(outCard[0])) break;
                            dcTmp[GLogicDef.CT_FOUR][0].CardData.CopyList(calcOutCard,outCardCount,i);
                            return;
                        }
                    }
 
                    if (mAllCards[wLastChairID].Count < 6 || mAllCards[wNextChairID].Count < 6)
                    {
                        if(mAllCards[wMeChairID].Count>=3 && GetCardValue( mAllCards[wMeChairID][2] ) == 3)
                        {
                            mAllCards[wMeChairID].CopyList(calcOutCard, outCardCount);
                            return;
                        }
                    }
                    if (bBomb) goto BombDeal;
                    
                    #endregion
                    return;
                }
            case GLogicDef.CT_FOUR:
                {
                    #region 四条处理
                    var dcTmp = new Dictionary<byte, List<tagAnalyseCardTypeResult>>(dcUserCutCard);
                    if (dcTmp.ContainsKey(GLogicDef.CT_FOUR))
                    {
                        dcTmp[GLogicDef.CT_FOUR][0].CardData.Reverse();
                        for (int i = 0; i < dcTmp[GLogicDef.CT_FOUR][0].CardData.Count; i += 4)
                        {
                            if (GetCardValue(dcTmp[GLogicDef.CT_FOUR][0].CardData[i]) == 3) break;
                            if (GetCardLogicValue(dcTmp[GLogicDef.CT_FOUR][0].CardData[i]) > GetCardLogicValue(outCard[0]))
                            {
                                dcTmp[GLogicDef.CT_FOUR][0].CardData.CopyList(calcOutCard, outCardCount, i);
                                return;
                            }
                            
                        }
                    }
                    if (mAllCards[wLastChairID].Count < 6 || mAllCards[wNextChairID].Count < 6)
                    {
                        if (mAllCards[wMeChairID].Count>=4 && GetCardValue(mAllCards[wMeChairID][3]) == 3)
                        {
                            mAllCards[wMeChairID].CopyList(calcOutCard, outCardCount);
                            return;
                        }
                    }
                    #endregion
                    return;
                }
            case GLogicDef.CT_SINGLE_LINE:
                {
                    #region 单连处理
                    //两个对家的牌都≥6张时
                    bool EnemyHandNumBothOverSix = (mAllCards[wLastChairID].Count >= 6 && mAllCards[wNextChairID].Count >= 6);

                    var dcTmp = new Dictionary<byte, List<tagAnalyseCardTypeResult>>(dcUserCutCard);
                    if (dcTmp.ContainsKey(GLogicDef.CT_SINGLE_LINE))
                    {
                        List<tagAnalyseCardTypeResult> listSingleLine = new List<tagAnalyseCardTypeResult>();
                        for (int i = 0; i < dcTmp[GLogicDef.CT_SINGLE_LINE].Count; i++)
                        {
                            if (GetCardLogicValue(dcTmp[GLogicDef.CT_SINGLE_LINE][i].CardData[0]) > GetCardLogicValue(outCard[0])
                                && dcTmp[GLogicDef.CT_SINGLE_LINE][i].CardData.Count == outCardCount)
                            {
                                listSingleLine.Add(dcTmp[GLogicDef.CT_SINGLE_LINE][i]);
                            }
                        }
                        if (listSingleLine.Count > 0)
                        {
                            //排序
                            listSingleLine.Sort((a, b) => { return GetCardLogicValue(a.CardData[0]).CompareTo(GetCardLogicValue(b.CardData[0])) * (EnemyHandNumBothOverSix ? -1 : 1); });
                            listSingleLine[0].CardData.CopyList(calcOutCard, outCardCount);
                            return;
                        }
                    }
                    List<tagAnalyseCardTypeResult> follow = new List<tagAnalyseCardTypeResult>();
                    if (CommFollowCard(wMeChairID, outCard, outCardCount, follow))
                    {
                        follow.Sort((a, b) => { return GetCardLogicValue(a.CardData[0]).CompareTo(GetCardLogicValue(b.CardData[0])); });
                        follow[0].CardData.CopyList(calcOutCard, outCardCount);
                        return;
                    }

                    if (EnemyHandNumBothOverSix == false)
                    {
                        if(CommFollowCard(wMeChairID, outCard, outCardCount, follow,false))
                        {
                            List<byte> handCardTmp = null;
                            int MinHandNumIndex = Int32.MaxValue;
                            //List<byte> listMinHand = new List<byte>();

                            foreach(var e in follow)
                            {
                                handCardTmp = mAllCards[wMeChairID].ToList();
                                RemoveCard(e.CardData.ToArray(), (byte)e.CardData.Count, handCardTmp);
                                int HandNumTmp = CutCards(handCardTmp, (byte)handCardTmp.Count);
                                if (HandNumTmp < MinHandNumIndex)
                                {
                                    MinHandNumIndex = HandNumTmp;
                                    calcOutCard.Clear();
                                    calcOutCard.AddRange(e.CardData);
                                }
                                else if (HandNumTmp == MinHandNumIndex)
                                {
                                    if (GetCardLogicValue(calcOutCard[0]) > GetCardLogicValue(e.CardData[0]))
                                    {
                                        calcOutCard.Clear();
                                        calcOutCard.AddRange(e.CardData);
                                    }
                                }
                            }
                            return;
                        }
                    }

                    if (bBomb) goto BombDeal;
                    #endregion
                    return;
                }
            case GLogicDef.CT_DOUBLE_LINE:
            case GLogicDef.CT_THREE_LINE:
            case GLogicDef.CT_FOUR_LINE:
                {
                    #region 双连/三连/四连处理
                    if (GetSpecifiedCardTypeOutCard(dcUserCutCard, cbCardType, calcOutCard, false)) return;
                    if (bBomb) goto BombDeal;
                    #endregion
                    return;
                }           
            case GLogicDef.CT_BOMB_CARD:
                {                    
                    goto BombDeal;                    
                }
        }
    BombDeal:
        #region 炸弹处理
        if ( dcUserCutCard.ContainsKey(GLogicDef.CT_BOMB_CARD) )
        {
            dcUserCutCard[GLogicDef.CT_BOMB_CARD][0].CardData.Reverse();

            Dictionary<byte,List<tagAnalyseCardTypeResult>> HighCard = new Dictionary<BYTE,List<tagAnalyseCardTypeResult>>();
            Dictionary<byte,List<tagAnalyseCardTypeResult>> NoHighCard = new Dictionary<BYTE,List<tagAnalyseCardTypeResult>>();
            for (int i = 0;i< dcUserCutCard[GLogicDef.CT_BOMB_CARD][0].CardData.Count; i+=4)
            {
                byte[] cbCardTmp = new byte[4];
                dcUserCutCard[GLogicDef.CT_BOMB_CARD][0].CardData.CopyTo(i,cbCardTmp,0,4);
               if( GetHighCardType(wMeChairID,cbCardTmp,4) == -1)
               {
                   List<byte> listHandTmp = mAllCards[wMeChairID].ToList();
                   RemoveCard(cbCardTmp, (byte)cbCardTmp.Length, listHandTmp);
                   Dictionary<byte,List<tagAnalyseCardTypeResult>> resAllCardType = new Dictionary<BYTE,List<tagAnalyseCardTypeResult>>();
                   int handnum = CutCards(listHandTmp, (byte)listHandTmp.Count,resAllCardType);
                   
                   DWORD highCardNum = GetHighCard(wMeChairID,resAllCardType,HighCard,NoHighCard);

                   if( (highCardNum&0x0000ffff) <= 1 )
                   {
                       cbCardTmp.CopyList(calcOutCard, 4);
                       return;
                   }
               }
            }
        }
        #endregion
        return;
    }
    /// <summary>
    /// 坑主上家跟牌
    /// </summary>
    /// <param name="outCard">本轮出牌数据</param>
    /// <param name="outCardCount">本轮出牌个数</param>
    /// <param name="calcOutCard">计算后的跟牌</param>
    void LastOfBankerFollowCard(byte[] outCard, byte outCardCount, List<byte> calcOutCard)
    {
        WORD wBankerChairID = wBankerUser;
        WORD wMeChairID = (WORD)((wBankerUser + 2) % wPlayerCount);
        WORD wLastChairID = (WORD)((wBankerUser + 1) % wPlayerCount);
        calcOutCard.Clear();
        byte cbCardType = GetCardType(outCard, outCardCount);
        if (cbCardType == GLogicDef.CT_INVALID)
        {
            Debuger.Instance.LogError("牌型错误");
            return;
        }
        var meCutCards = GetCutResult(wMeChairID);
        var bankerCutCards = GetCutResult(wBankerChairID);
        List<byte> meCards = mAllCards[wMeChairID];
        //		List<byte> ctList = new List<byte> (new byte[]{
        //			GLogicDef.CT_SINGLE,
        //			GLogicDef.CT_DOUBLE,
        //			GLogicDef.CT_SINGLE_LINE,
        //			GLogicDef.CT_THREE,
        //			GLogicDef.CT_DOUBLE_LINE,
        //			GLogicDef.CT_FOUR,
        //			GLogicDef.CT_THREE_LINE,
        //			GLogicDef.CT_BOMB_CARD,
        //			GLogicDef.CT_FOUR_LINE
        //		});
        //		foreach (byte ct in ctList) {
        //		}
        //最后一个出牌
        WORD wLastOutWinner = GetLastOutCardWinner();

        List<tagAnalyseCardTypeResult> result = new List<tagAnalyseCardTypeResult>();
        if (CommFollowCard(wLastChairID, outCard, outCardCount, result, false) == false)
            return;

        List<byte> cardsSingleWithoutThree = meCutCards[GLogicDef.CT_SINGLE][0].CardData;
        cardsSingleWithoutThree.RemoveAll(a =>
        {
            return GetCardValue(a) == 3;
        });

        var meHighCard = new Dictionary<byte, List<tagAnalyseCardTypeResult>>();
        var meNoHighCard = new Dictionary<byte, List<tagAnalyseCardTypeResult>>();
        uint meHighCardHands = GetUserHighCard(wMeChairID, meHighCard, meNoHighCard);
        int bankerCardsNum = mAllCards[wBankerChairID].Count;
        if (cbCardType == GLogicDef.CT_SINGLE)
        {
            if (bankerCardsNum > 2)
            {
                if (wLastOutWinner == wBankerChairID)
                {
                    if (meCutCards.ContainsKey(GLogicDef.CT_SINGLE))
                    {
                        if (cardsSingleWithoutThree.Count <= 2)
                        {
                            calcOutCard.Add(meCutCards[GLogicDef.CT_SINGLE][0].CardData[0]);
                        }
                        else
                        {
                            calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 1]);
                            //GetSpecifiedCardTypeOutCard (meCutCards, GLogicDef.CT_SINGLE, calcOutCard);
                        }
                    }
                    else
                    {
                        calcOutCard.Add(meCards[0]);
                    }
                    return;
                }
                else
                {
                    if (GetHighCardType(wLastChairID, outCard, outCardCount) == -1)
                    {
                        return;
                    }
                    else
                    {
                        if (GetCardValue(outCard[0]) == 1)
                        {
                            if (meCutCards[GLogicDef.CT_SINGLE][0].CardData.Contains(0x02))
                            {
                                calcOutCard.Add(0x02);
                                return;
                            }
                            else if (meCutCards[GLogicDef.CT_SINGLE][0].CardData.Contains(0x12))
                            {
                                calcOutCard.Add(0x12);
                                return;
                            }
                            else if (meCutCards[GLogicDef.CT_SINGLE][0].CardData.Contains(0x22))
                            {
                                calcOutCard.Add(0x22);
                                return;
                            }
                            else if (meCutCards[GLogicDef.CT_SINGLE][0].CardData.Contains(0x32))
                            {
                                calcOutCard.Add(0x32);
                                return;
                            }
                            else
                            {
                                return;
                            }
                        }
                        else if (GetCardValue(outCard[0]) == 2)
                        {
                            if (GetCardValue(meCards[0]) == 3 
                                && (GetHandNumber(wMeChairID) <= 2 
                                || (WORD)((meHighCardHands >> 16)&0x0000ffff) == GetHandNumber(wMeChairID)))
                            {
                                calcOutCard.Add(meCards[0]);
                            }
                            return;
                            /**
                            if (meCutCards [GLogicDef.CT_SINGLE] [0].CardData.Contains (0x03) && (GetHandNumber (wMeChairID) <= 2 || (WORD)(meHighCardHands >> 16) == GetHandNumber (wMeChairID))) {
                                calcOutCard.Add (0x03);
                                return;
                            } else if (meCutCards [GLogicDef.CT_SINGLE] [0].CardData.Contains (0x13) && (GetHandNumber (wMeChairID) <= 2 || (WORD)(meHighCardHands >> 16) == GetHandNumber (wMeChairID))) {
                                calcOutCard.Add (0x13);
                                return;
                            } else if (meCutCards [GLogicDef.CT_SINGLE] [0].CardData.Contains (0x23) && (GetHandNumber (wMeChairID) <= 2 || (WORD)(meHighCardHands >> 16) == GetHandNumber (wMeChairID))) {
                                calcOutCard.Add (0x23);
                                return;
                            } else if (meCutCards [GLogicDef.CT_SINGLE] [0].CardData.Contains (0x33) && (GetHandNumber (wMeChairID) <= 2 || (WORD)(meHighCardHands >> 16) == GetHandNumber (wMeChairID))) {
                                calcOutCard.Add (0x33);
                                return;
                            } else {
                                return;
                            }
                            **/
                        }
                        else
                        {
                            if (cardsSingleWithoutThree.Count == 0)
                            {
                                return;
                            }
                            else if (cardsSingleWithoutThree.Count <= 2)
                            {
                                calcOutCard.Add(meCutCards[GLogicDef.CT_SINGLE][0].CardData[0]);
                                return;
                            }
                            else
                            {
                                //GetSpecifiedCardTypeOutCard (meCutCards, GLogicDef.CT_SINGLE, calcOutCard);
                                calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 1]);
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                if (wLastOutWinner == wBankerChairID)
                {
                    calcOutCard.Add(meCards[0]);
                    return;
                }
                else
                {
                    if (GetHighCardType(wLastChairID, outCard, outCardCount) == -1)
                    {
                        return;
                    }
                    else
                    {
                        if (bankerCutCards.ContainsKey(GLogicDef.CT_DOUBLE))
                        {
                            return;
                        }
                        else
                        {
                            bool isBiggerThanOutCard = true;
                            foreach (byte card in mAllCards[wBankerChairID])
                            {
                                if (GetCardLogicValue(card) <= GetCardLogicValue(outCard[0]))
                                {
                                    isBiggerThanOutCard = false;
                                }
                            }
                            if (isBiggerThanOutCard)
                            {
                                calcOutCard.Add(meCards[0]);
                                return;
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }

        if (cbCardType == GLogicDef.CT_DOUBLE)
        {
            if (bankerCardsNum > 2)
            {
                if (wLastOutWinner == wBankerChairID)
                {
                    if (meCutCards.ContainsKey(GLogicDef.CT_DOUBLE))
                    {
                        if (GetCardValue(outCard[0]) == 2 && GetCardValue(meCards[1]) == 3)
                        {
                            calcOutCard.Add(meCards[0]);
                            calcOutCard.Add(meCards[1]);
                        }
                        else
                        {
                            //GetSpecifiedCardTypeOutCard (meCutCards, GLogicDef.CT_DOUBLE, calcOutCard);
                            calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 1]);
                            calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 2]);
                        }
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (bankerCutCards.ContainsKey(GLogicDef.CT_DOUBLE))
                    {
                        if (bankerCutCards[GLogicDef.CT_DOUBLE][0].CardData.Count * 0.5f == 1 && GetCardValue(bankerCutCards[GLogicDef.CT_DOUBLE][0].CardData[0]) == 3)
                        {
                            return;
                        }
                        else
                        {
                            if (meCutCards.ContainsKey(GLogicDef.CT_DOUBLE))
                            {
                                meCutCards[GLogicDef.CT_DOUBLE][0].CardData.Reverse();
                                for (int i = 0; i < meCutCards[GLogicDef.CT_DOUBLE][0].CardData.Count;i+=2 )
                                {
                                    if (GetCardValue(meCutCards[GLogicDef.CT_DOUBLE][0].CardData[i]) > 10) break;
                                    if (GetCardValue(meCutCards[GLogicDef.CT_DOUBLE][0].CardData[i]) <= GetCardValue(outCard[0])) break;
                                    meCutCards[GLogicDef.CT_DOUBLE][0].CardData.CopyList(calcOutCard,2,i);
                                    return;
                                }                                   
                            }
                            return;
                            
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                if (wLastOutWinner == wBankerChairID)
                {
                    if (meCutCards.ContainsKey(GLogicDef.CT_DOUBLE))
                    {
                        if (meCutCards[GLogicDef.CT_DOUBLE][0].CardData.Count * 0.5f == 1 && GetCardValue(bankerCutCards[GLogicDef.CT_DOUBLE][0].CardData[0]) == 3)
                        {
                            calcOutCard.Add(meCards[0]);
                            calcOutCard.Add(meCards[1]);
                        }
                        else
                        {
                            //GetSpecifiedCardTypeOutCard (meCutCards, GLogicDef.CT_DOUBLE, calcOutCard);
                            calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 1]);
                            calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 2]);
                        }
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (meCutCards[GLogicDef.CT_DOUBLE][0].CardData.Count * 0.5f == 1 && GetCardValue(bankerCutCards[GLogicDef.CT_DOUBLE][0].CardData[0]) == 3)
                    {
                        return;
                    }
                    else
                    {
                        //GetSpecifiedCardTypeOutCard (meCutCards, GLogicDef.CT_DOUBLE, calcOutCard);
                        calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 1]);
                        calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 2]);
                        return;
                    }
                }
            }
        }

        if (cbCardType == GLogicDef.CT_THREE)
        {
            if (bankerCardsNum > 3)
            {
                if (wLastOutWinner == wBankerChairID)
                {
                    if (meCutCards.ContainsKey(GLogicDef.CT_THREE))
                    {
                        if (meCutCards[GLogicDef.CT_THREE][0].CardData.Count == 3 && GetCardValue(bankerCutCards[GLogicDef.CT_THREE][0].CardData[0]) == 3)
                        {
                            return;
                        }
                        else
                        {
                            //GetSpecifiedCardTypeOutCard (meCutCards, GLogicDef.CT_THREE, calcOutCard);
                            calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 1]);
                            calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 2]);
                            calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 3]);
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

            }
            else
            {
                if (wLastOutWinner == wBankerChairID)
                {
                    if (meCutCards.ContainsKey(GLogicDef.CT_THREE))
                    {
                        //GetSpecifiedCardTypeOutCard (meCutCards, GLogicDef.CT_THREE, calcOutCard);
                        calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 1]);
                        calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 2]);
                        calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 3]);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        if (cbCardType == GLogicDef.CT_FOUR)
        {
            if (bankerCardsNum > 3)
            {
                if (wLastOutWinner == wBankerChairID)
                {
                    if (meCutCards.ContainsKey(GLogicDef.CT_FOUR))
                    {
                        if (meCutCards[GLogicDef.CT_FOUR][0].CardData.Count == 4 && GetCardValue(bankerCutCards[GLogicDef.CT_FOUR][0].CardData[0]) == 3)
                        {
                            return;
                        }
                        else
                        {
                            //GetSpecifiedCardTypeOutCard (meCutCards, GLogicDef.CT_FOUR, calcOutCard);
                            calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 1]);
                            calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 2]);
                            calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 3]);
                            calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 4]);
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

            }
            else
            {
                if (wLastOutWinner == wBankerChairID)
                {
                    if (meCutCards.ContainsKey(GLogicDef.CT_FOUR))
                    {
                        //GetSpecifiedCardTypeOutCard (meCutCards, GLogicDef.CT_FOUR, calcOutCard);
                        calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 1]);
                        calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 2]);
                        calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 3]);
                        calcOutCard.Add(result[0].CardData[result[0].CardData.Count - 4]);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        if (cbCardType == GLogicDef.CT_SINGLE_LINE)
        {
            if (bankerCardsNum > 6)
            {
                if (wLastOutWinner == wBankerChairID)
                {
                    calcOutCard.AddRange(result[result.Count - 1].CardData.ToArray());
                    return;
                }
                else
                {
                    if (meCutCards[GLogicDef.CT_SINGLE_LINE].Count == 1 && GetCardValue(bankerCutCards[GLogicDef.CT_SINGLE_LINE][0].CardData[0]) == 13)
                    {
                        return;
                    }
                    else
                    {
                        calcOutCard.AddRange(result[result.Count - 1].CardData.ToArray());
                        return;
                    }
                }
            }
            else
            {
                if (wLastOutWinner == wBankerChairID)
                {
                    calcOutCard.AddRange(result[0].CardData.ToArray());
                    return;
                }
                else
                {
                    if (meCutCards[GLogicDef.CT_SINGLE_LINE].Count == 1 && GetCardValue(bankerCutCards[GLogicDef.CT_SINGLE_LINE][0].CardData[0]) == 13)
                    {
                        return;
                    }
                    else
                    {
                        if (GetCardValue(bankerCutCards[GLogicDef.CT_SINGLE_LINE][0].CardData[0]) == 13)
                        {
                            calcOutCard.AddRange(result[1].CardData.ToArray());
                        }
                        else
                        {
                            calcOutCard.AddRange(result[0].CardData.ToArray());
                        }
                        return;
                    }
                }
            }
        }

        if (cbCardType == GLogicDef.CT_DOUBLE_LINE)
        {
            if (wLastOutWinner == wBankerChairID)
            {
                calcOutCard.AddRange(result[0].CardData.ToArray());
                return;
            }
            else
            {
                return;
            }
        }

        if (cbCardType == GLogicDef.CT_THREE_LINE)
        {
            if (wLastOutWinner == wBankerChairID)
            {
                calcOutCard.AddRange(result[0].CardData.ToArray());
                return;
            }
            else
            {
                return;
            }
        }

        if (cbCardType == GLogicDef.CT_FOUR_LINE)
        {
            if (wLastOutWinner == wBankerChairID)
            {
                calcOutCard.AddRange(result[0].CardData.ToArray());
                return;
            }
            else
            {
                return;
            }
        }

        if (cbCardType == GLogicDef.CT_BOMB_CARD)
        {
            if (wLastOutWinner == wBankerChairID)
            {
                if (GetHandNumber(wMeChairID) <= 2 && (!bankerCutCards.ContainsKey(GLogicDef.CT_BOMB_CARD) || bankerCutCards[GLogicDef.CT_BOMB_CARD][0].CardData[0] < result[0].CardData[0]))
                {
                    calcOutCard.AddRange(result[0].CardData.ToArray());
                    return;
                }
                else
                {
                    if (GetHandNumber(wBankerChairID) <= 2)
                    {
                        calcOutCard.AddRange(result[0].CardData.ToArray());
                        return;
                    }
                    else
                    {
                        if (meHighCardHands == GetHandNumber(wMeChairID) || GetHandNumber(wMeChairID) - meHighCardHands == 1)
                        {
                            calcOutCard.AddRange(result[0].CardData.ToArray());
                            return;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }

    }
    /// <summary>
    /// 坑主下家跟牌
    /// </summary>
    /// <param name="outCard">本轮出牌数据</param>
    /// <param name="outCardCount">本轮出牌个数</param>
    /// <param name="calcOutCard">计算后的跟牌</param>
    void NextOfBankerFollowCard(byte[] outCard, byte outCardCount, List<byte> calcOutCard)
    {
        WORD wMeChairID = wBankerUser;
        WORD wLastChairID = (WORD)((wBankerUser + 2) % wPlayerCount);
        WORD wNextChairID = (WORD)((wBankerUser + 1) % wPlayerCount);
        calcOutCard.Clear();
        byte cbCardType = GetCardType(outCard, outCardCount);
        if (cbCardType == GLogicDef.CT_INVALID)
        {
            Debuger.Instance.LogError("牌型错误");
            return;
        }
        List<tagAnalyseCardTypeResult> result = new List<tagAnalyseCardTypeResult>();
        if (CommFollowCard(wMeChairID, outCard, outCardCount, result, false) == false)
            return;

        var nextNoHighCard = new Dictionary<BYTE, List<tagAnalyseCardTypeResult>>();
        DWORD hNum = GetUserHighCard(wNextChairID, null, nextNoHighCard);
        var lastCard = GetUserCard(wLastChairID);
        var nextCard = GetUserCard(wNextChairID);
        var dcNextUserCutCard = GetCutResult(wNextChairID);

        
        bool isOutBomb = false;//是否可以出炸弹

        //自己有炸弹 每次先判断是否需要出炸弹
        if (dcNextUserCutCard.ContainsKey(GLogicDef.CT_BOMB_CARD))
        {

            var dcBankerCutCard = GetCutResult(wMeChairID);
            //地主有炸弹
            if (dcBankerCutCard.ContainsKey(GLogicDef.CT_BOMB_CARD))
            {

                if (GetCardLogicValue(dcNextUserCutCard[GLogicDef.CT_BOMB_CARD][0].CardData[0])
                    > GetCardLogicValue(dcBankerCutCard[GLogicDef.CT_BOMB_CARD][0].CardData[0]))
                {
                    //除炸弹以外的手数
                    int handNum = GetHandNumber(wNextChairID) - (dcNextUserCutCard[GLogicDef.CT_BOMB_CARD][0].CardData.Count / 4);
                    if (handNum < 2)
                        isOutBomb = true;
                }
            }
            else//没有炸弹
            {
                //获取手上最小的对子
                var doubleCard = GetSpecifiedCard(GLogicDef.CT_DOUBLE, nextCard.ToArray(), (byte)nextCard.Count, new List<BYTE>());
                byte doubleVal = 255;
                if (doubleCard != null)
                { 
                    doubleCard[0].CardData.Reverse();
                    doubleVal = GetCardLogicValue(doubleCard[0].CardData[0]);
                }
                    

                //当坑主没有炸弹，且自己只有一手牌不是天牌或者全是天牌则直接炸之
                int bombNum = nextNoHighCard.ContainsKey(GLogicDef.CT_BOMB_CARD) ?
                    (nextNoHighCard[GLogicDef.CT_BOMB_CARD][0].CardData.Count / 4) : 0;
                if ((hNum & 0x0000FFFF - bombNum) < 2)//需要排除炸弹
                    isOutBomb = true;
                //当坑主上家只剩一张牌时，如果自己手中有更小的单牌且坑主没有炸弹，则炸之
                else if (lastCard.Count == 1
                         && GetCardLogicValue(lastCard[0]) > GetCardLogicValue(nextCard[nextCard.Count - 1]))
                    isOutBomb = true;
                //当坑主上家只剩一个对子时，如果自己手中有更小的对子且坑主没有炸弹，则炸之
                else if (GetCardType(lastCard.ToArray(), (byte)lastCard.Count) == GLogicDef.CT_DOUBLE
                        && GetCardLogicValue(lastCard[0]) > doubleVal)
                    isOutBomb = true;

            }
            //炸弹处理
            if (isOutBomb)
            {
                if (cbCardType == GLogicDef.CT_BOMB_CARD)
                {
                    //出的是炸弹
                    var dcOutResult = new Dictionary<byte, List<tagAnalyseCardTypeResult>>();
                    dcOutResult.Add(cbCardType, result);
                    //从小到大
                    if (GetSpecifiedCardTypeOutCard(dcOutResult, cbCardType, calcOutCard))
                        return;
                }
                else
                {
                    if (GetSpecifiedCardTypeOutCard(dcNextUserCutCard, GLogicDef.CT_BOMB_CARD, calcOutCard))
                        return;
                }
            }
        }

        //跟坑主上家的牌时，直接选择“不出”
        if (GetLastOutCardWinner() == wLastChairID)
            return;

        //跟坑主的牌时
        //如果判断自己只有一手牌不是天牌或者全是天牌则在不拆牌的情况下能跟就直接跟
        List<tagAnalyseCardTypeResult> outResult = new List<tagAnalyseCardTypeResult>();
        if ((hNum & 0x0000FFFF) < 2)
        {
            if (CommFollowCard(wMeChairID, outCard, outCardCount, outResult))
            {
                var dcOutResult = new Dictionary<byte, List<tagAnalyseCardTypeResult>>();
                dcOutResult.Add(cbCardType, outResult);
                //从小到大
                if (GetSpecifiedCardTypeOutCard(dcOutResult, cbCardType, calcOutCard))
                    return;
            }

        }


        //获取手上最大的
        List<byte> remainCard = new List<BYTE>();
        var typeCard = GetSpecifiedCard(cbCardType, nextCard.ToArray(), (byte)nextCard.Count, remainCard);
        var dcMaxOutResult = new Dictionary<byte, List<tagAnalyseCardTypeResult>>();
        dcMaxOutResult.Add(cbCardType, typeCard);
        //坑主上家只剩一张牌时
        if (lastCard.Count == 1)
        {
            if (remainCard != null && remainCard.Count > 0)
            { 
                SortCardList(remainCard, (byte)remainCard.Count);
                remainCard.Reverse();
                if (GetCardLogicValue(lastCard[0]) > GetCardLogicValue(remainCard[0]))
                {
                    //从大到小出
                    if (GetSpecifiedCardTypeOutCard(dcMaxOutResult, cbCardType, calcOutCard, false))
                        return;
                }
            }
            
            //能跟就跟
            outResult.Clear();
            if (CommFollowCard(wMeChairID, outCard, outCardCount, outResult))
            {
                dcMaxOutResult.Clear();
                dcMaxOutResult.Add(cbCardType, outResult);
                GetSpecifiedCardTypeOutCard(dcMaxOutResult, cbCardType, calcOutCard);
                return;
            }

        }
        else if (GetCardType(lastCard.ToArray(), (byte)lastCard.Count) == GLogicDef.CT_DOUBLE)
        {
            if (remainCard != null && remainCard.Count > 0)
            {
                var remianDoubleCard = GetSpecifiedCard(GLogicDef.CT_DOUBLE, remainCard.ToArray(), (byte)remainCard.Count, new List<BYTE>());
                byte doubleVal = 255;
                if (remianDoubleCard != null)
                {
                    remianDoubleCard[0].CardData.Reverse();
                    doubleVal = GetCardLogicValue(remianDoubleCard[0].CardData[0]);
                }
                if (GetCardLogicValue(lastCard[0]) > doubleVal)
                {
                    //从大到小出
                    if (GetSpecifiedCardTypeOutCard(dcMaxOutResult, cbCardType, calcOutCard, false))
                        return;

                }
            }
            
            //能跟就跟
            outResult.Clear();
            if (CommFollowCard(wMeChairID, outCard, outCardCount, outResult))
            {
                dcMaxOutResult.Clear();
                dcMaxOutResult.Add(cbCardType, outResult);
                GetSpecifiedCardTypeOutCard(dcMaxOutResult, cbCardType, calcOutCard);
                return;
            }
        }
        else
        {
            var bankerCard = GetUserCard(wBankerUser);
            if (bankerCard.Count > 5)
            {
                //能跟就跟
                outResult.Clear();
                if (CommFollowCard(wMeChairID, outCard, outCardCount, outResult))
                {
                    dcMaxOutResult.Clear();
                    dcMaxOutResult.Add(cbCardType, outResult);
                    //从小到大
                    GetSpecifiedCardTypeOutCard(dcMaxOutResult, cbCardType, calcOutCard);
                    return;
                }

            }
            else
            {
                //坑主上家在不增加手数的情况下可以跟牌
                outResult.Clear();
                if (CommFollowCard(wNextChairID, outCard, outCardCount, outResult))
                {
                    return;
                }
                else
                {
                    outResult.Clear();
                    //自己跟
                    if (CommFollowCard(wMeChairID, outCard, outCardCount, outResult))
                    {
                        dcMaxOutResult.Clear();
                        dcMaxOutResult.Add(cbCardType, outResult);
                        //从大到小跟
                        if (GetSpecifiedCardTypeOutCard(dcMaxOutResult, cbCardType, calcOutCard, false))
                            return;
                    }
                    else
                    {
                        dcMaxOutResult.Clear();
                        dcMaxOutResult.Add(cbCardType, result);
                        //从大到小跟
                        if (GetSpecifiedCardTypeOutCard(dcMaxOutResult, cbCardType, calcOutCard, false))
                            return;

                    }
                }

            }

        }
    }
    #endregion    

    #region 任务配牌函数
    //添加配牌任务
    public void AddConfigCard(BYTE cbCardType, BYTE cbCardData, WORD wChairID = 0)
    {
        tagConfigCard configCard = new tagConfigCard();
        configCard.cbCardType = cbCardType;
        configCard.cbCardData = cbCardData;
        configCard.wChairID = wChairID;
        mConfigCardType.Add(configCard);
    }

    //清理配牌任务
    public void ClearConfigCard()
    {
        mConfigCardType.Clear();
    }

    //获取配牌结果
    public bool GetConfigCard(BYTE[] cbHandCardData, ref BYTE cbHandCardCount)
    {
        //清理
        cbHandCardData.ArraySetAll((byte)0);
        cbHandCardCount = 0;
        bool bResult = true;
        //确认优先级
        List<tagConfigCard> curConfigCardType = new List<tagConfigCard>();
        //先列出按以下顺序求出指定的,在按以下顺序求出非指定的
        BYTE[] cbCardType = new BYTE[] { GLogicDef.CT_FOUR_LINE, GLogicDef.CT_THREE_LINE,GLogicDef.CT_DOUBLE_LINE, 
           GLogicDef.CT_SINGLE_LINE,GLogicDef.CT_FOUR,GLogicDef.CT_BOMB_CARD,GLogicDef.CT_THREE, GLogicDef.CT_DOUBLE,
           GLogicDef.CT_SINGLE};
        BYTE[] cbAllCard = (BYTE[])GLogicDef.CardData.Clone();
        //指定顺序添加
        for (int i = 0; i < cbCardType.Length; i++)
        {
            for (int j = 0; j < mConfigCardType.Count; j++)
            {
                if (mConfigCardType[j].cbCardType == cbCardType[i] && mConfigCardType[j].cbCardData != 0x00)
                {
                    curConfigCardType.Add(mConfigCardType[j]);
                }
            }
        }
        //非指定顺序添加
        for (int i = 0; i < cbCardType.Length; i++)
        {
            for (int j = 0; j < mConfigCardType.Count; j++)
            {
                if (mConfigCardType[j].cbCardType == cbCardType[i] && mConfigCardType[j].cbCardData == 0x00)
                {
                    curConfigCardType.Add(mConfigCardType[j]);
                }
            }
        }

        //获取配置要求的牌
        BYTE[] cbRemainCard = (BYTE[])cbAllCard.Clone();
        BYTE cbRamainCount = (BYTE)(cbAllCard.Length);
        BYTE[] cbChairCardCount = new BYTE[3] { 0, 16, 32 };
        WORD wChairID = 0;
        for (int i = 0; i < curConfigCardType.Count; i++)
        {
            //椅子号验证失败
            wChairID = curConfigCardType[i].wChairID;
            if (wChairID < 0 || wChairID > 2)
            {
                cbAllCard.Shuffle(0, GLogicDef.FULL_COUNT);
                cbHandCardData.ArraySetAll((byte)0);
                cbAllCard.CopyTo(cbHandCardData, 0);
                cbHandCardCount = GLogicDef.FULL_COUNT;
                return false;
            }
            switch (curConfigCardType[i].cbCardType)
            {
                case GLogicDef.CT_FOUR_LINE:
                case GLogicDef.CT_THREE_LINE:
                case GLogicDef.CT_DOUBLE_LINE:
                case GLogicDef.CT_SINGLE_LINE:
                    {
                        bResult = GetLineCardByConfig(curConfigCardType[i], cbHandCardData, ref cbChairCardCount[wChairID], cbRemainCard, ref cbRamainCount);
                        if (!bResult)    //配牌失败将会采用随机牌算法
                        {
                            cbAllCard.Shuffle(0, GLogicDef.FULL_COUNT);
                            cbHandCardData.ArraySetAll((byte)0);
                            cbAllCard.CopyTo(cbHandCardData, 0);
                            cbHandCardCount = GLogicDef.FULL_COUNT;
                            return bResult;
                        }
                        break;
                    }
                case GLogicDef.CT_FOUR:
                case GLogicDef.CT_BOMB_CARD:
                case GLogicDef.CT_THREE:
                case GLogicDef.CT_DOUBLE:
                case GLogicDef.CT_SINGLE:
                    {
                        bResult = GetNotLineCardByConfig(curConfigCardType[i], cbHandCardData, ref cbChairCardCount[wChairID], cbRemainCard, ref cbRamainCount);
                        if (!bResult)    //配牌失败将会采用随机牌算法
                        {
                            cbAllCard.Shuffle(0, GLogicDef.FULL_COUNT);
                            cbHandCardData.ArraySetAll((byte)0);
                            cbAllCard.CopyTo(cbHandCardData, 0);
                            cbHandCardCount = GLogicDef.FULL_COUNT;
                            return bResult;
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        //随机剩余的牌
        cbRemainCard.Shuffle(0, cbRamainCount);
        BYTE cbRemainCardIndex = 0;
        for (int i = 0; i < GLogicDef.FULL_COUNT; i++)
        {
            if (cbHandCardData[i] == 0) cbHandCardData[i] = cbRemainCard[cbRemainCardIndex++];
        }
        cbHandCardCount = GLogicDef.FULL_COUNT;
        //清理
        ClearConfigCard();
        return bResult;
    }
    //配置连子类型
    private bool GetLineCardByConfig(tagConfigCard tagConfigData, BYTE[] cbHandCardData, ref BYTE cbHandCardCount, BYTE[] cbRemainCardData, ref  BYTE cbRemainCardCount)
    {
        //类型所关联的排数相等个数
        BYTE cbMaxEqual = 0;
        //---------------------------------------------------------------------
        //计算排数相等个数
        if (tagConfigData.cbCardType == GLogicDef.CT_FOUR_LINE) cbMaxEqual = 4;
        else if (tagConfigData.cbCardType == GLogicDef.CT_THREE_LINE) cbMaxEqual = 3;
        else if (tagConfigData.cbCardType == GLogicDef.CT_DOUBLE_LINE) cbMaxEqual = 2;
        else if (tagConfigData.cbCardType == GLogicDef.CT_SINGLE_LINE) cbMaxEqual = 1;
        else return false;
        //---------------------------------------------------------------------
        //排数不够配牌
        if (cbRemainCardCount < cbMaxEqual * 3) return false;
        BYTE[] cbTempCardData = (BYTE[])cbRemainCardData.Clone();
        BYTE cbTempCardCount = cbRemainCardCount;
        SortCardList(cbTempCardData, cbTempCardCount,GLogicDef.FULL_COUNT);

        //打乱花色
        BYTE cbLastCardIndex = 0;
        BYTE cbSameCount = 1;
        BYTE cbCardIndex = 0;
        while (cbLastCardIndex < cbTempCardCount)
        {
            for (cbCardIndex = (BYTE)(cbLastCardIndex + 1); cbCardIndex < cbTempCardCount; cbCardIndex++)
            {
                if (GetCardValue(cbTempCardData[cbLastCardIndex]) == GetCardValue(cbTempCardData[cbCardIndex]))
                {
                    cbSameCount++;
                    continue;
                }
                break;
            }
            if (cbSameCount > 1)
            {
                cbTempCardData.Shuffle(cbLastCardIndex, cbCardIndex - 1);
            }
            cbSameCount = 1;
            cbLastCardIndex = cbCardIndex;
        }
        //去除123
        BYTE cbFirstCardIndex = 0;

        for (BYTE i = 0; i < cbTempCardCount; ++i)
        {
            if (GetCardValue(cbTempCardData[i]) > 3)
            {
                cbFirstCardIndex = i;
                //牌数不够
                if (cbTempCardCount - i < cbMaxEqual * 3) return false;
                break;
            }
        }

        List<tagAnalyseCardTypeResult> cardTypeResultList = new List<tagAnalyseCardTypeResult>();
        BYTE cbCurCardCount = 0;
        BYTE cbEqualCount = 1;
        BYTE[] cbCurCardData = new BYTE[GLogicDef.FULL_COUNT];
        bool IsExist = true;
        BYTE cbLastCard = 0x00;

        //---------------------------------------------------------------------
        //切割牌
        while (cbFirstCardIndex < cbTempCardCount && IsExist)
        {
            IsExist = false;
            cbEqualCount = 1;
            cbLastCard = cbTempCardData[cbFirstCardIndex];
            for (BYTE i = (BYTE)(cbFirstCardIndex + 1); i < cbTempCardCount; i++)
            {
                if (GetCardValue(cbLastCard) == GetCardValue(cbTempCardData[i]))
                {
                    cbEqualCount++;
                }

                if (1 != (GetCardValue(cbLastCard) - GetCardValue(cbTempCardData[i])) && GetCardValue(cbLastCard) != GetCardValue(cbTempCardData[i]))
                {
                    cbLastCard = cbTempCardData[i];
                    if ((cbCurCardCount / cbMaxEqual) < 3)
                    {
                        cbCurCardCount = 0;
                        cbLastCard = cbTempCardData[i];
                        cbEqualCount = 1;
                        continue;
                    }
                    else break;
                }
                //同牌判断
                else if (GetCardValue(cbLastCard) != GetCardValue(cbTempCardData[i]))
                {
                    if (cbEqualCount >= cbMaxEqual)
                    {
                        for (BYTE j = 0; j < cbMaxEqual; j++)
                        {
                            cbCurCardData[cbCurCardCount++] = cbTempCardData[i - cbEqualCount + j];
                        }
                    }
                    else
                    {
                        cbCurCardCount = 0;
                    }
                    cbLastCard = cbTempCardData[i];
                    cbEqualCount = 1;
                }
                else if (i == cbTempCardCount - 1 && cbEqualCount >= cbMaxEqual && cbCurCardCount > 0 && GetCardValue(cbCurCardData[cbCurCardCount - 1]) - 1 == GetCardValue(cbTempCardData[i]))
                {
                    for (BYTE j = 0; j < cbMaxEqual; j++)
                        cbCurCardData[cbCurCardCount++] = cbTempCardData[i - cbEqualCount + 1 + j];
                }
            }
            if (cbCurCardCount / cbMaxEqual > 3)
            {
                tagAnalyseCardTypeResult CardTypeResult = new tagAnalyseCardTypeResult();
                CardTypeResult.cbCardType = tagConfigData.cbCardType;
                cbCurCardData.CopyList(CardTypeResult.CardData, cbCurCardCount);
                cardTypeResultList.Add(CardTypeResult);
                RemoveCard(cbCurCardData, cbCurCardCount, cbTempCardData, cbTempCardCount,GLogicDef.FULL_COUNT);
                cbTempCardCount = (BYTE)(cbTempCardCount - cbCurCardCount);
                cbCurCardCount = 0;
                IsExist = true;
            }
        }
        //---------------------------------------------------------------------

        //抽取配置的牌
        BYTE cbThreeLineNum = 0;
        BYTE cbBeginCard = (BYTE)((tagConfigData.cbCardData & 0xF0) >> 4);
        BYTE cbLinkLength = (BYTE)(tagConfigData.cbCardData & 0x0F);
        BYTE cbEndCard = (BYTE)(cbBeginCard + cbLinkLength - 1);
        if (cbBeginCard == 0x00)  //随机的
        {
            if (cbLinkLength == 0) cbLinkLength = 3;
            //随机四联
            for (int i = 0; i < cardTypeResultList.Count; i++)
            {
                BYTE cbCount = (BYTE)(cardTypeResultList[i].CardData.Count);
                if (cbCount / cbMaxEqual >= cbLinkLength)
                    cbThreeLineNum += (BYTE)((int)(cbCount / cbMaxEqual) - cbLinkLength + 1);

            }
            if (cbThreeLineNum < 1)
                return false;

            System.Random ran = new System.Random();
            BYTE cbRandIndex = (BYTE)(ran.Next(0, cbThreeLineNum));
            BYTE cbPreThreeLineCount = 0;
            cbThreeLineNum = 0;
            for (int i = 0; i < cardTypeResultList.Count; i++)
            {
                if (((cardTypeResultList[i].CardData.Count / cbMaxEqual) - cbLinkLength) < 0)
                    continue;
                cbThreeLineNum += (BYTE)((int)(cardTypeResultList[i].CardData.Count / cbMaxEqual) - cbLinkLength + 1);
                if (cbThreeLineNum > cbRandIndex)
                {
                    BYTE cbIndex = (BYTE)((cbRandIndex - cbPreThreeLineCount) * cbMaxEqual);
                    BYTE[] cbRemoveCard = new BYTE[cbMaxEqual * cbLinkLength];
                    for (int j = 0; j < cbMaxEqual * cbLinkLength; j++)
                    {
                        cbHandCardData[cbHandCardCount++] = cardTypeResultList[i].CardData[j + cbIndex];
                        cbRemoveCard[j] = cbHandCardData[cbHandCardCount - 1];
                    }
                    //删除扑克
                    RemoveCard(cbRemoveCard, (BYTE)(cbMaxEqual * cbLinkLength), cbRemainCardData, cbRemainCardCount,GLogicDef.FULL_COUNT);
                    cbRemainCardCount = (BYTE)(cbRemainCardCount - cbMaxEqual * cbLinkLength);
                    return true;
                }
                cbPreThreeLineCount = cbThreeLineNum;
            }
        }
        else //指定的
        {
            BYTE cbCardCount = 0;
            for (int i = 0; i < cardTypeResultList.Count; i++)
            {
                cbCardCount = (BYTE)(cardTypeResultList[i].CardData.Count);
                if (GetCardValue(cardTypeResultList[i].CardData[0]) >= cbEndCard &&
                   GetCardValue(cardTypeResultList[i].CardData[cbCardCount - 1]) <= cbBeginCard)
                {//存在该顺子

                    BYTE[] cbRemoveCard = new BYTE[(cbEndCard - cbBeginCard + 1) * cbMaxEqual];
                    BYTE cbRemoveCount = 0;
                    for (int j = 0; j < cbCardCount; j++)
                    {
                        if (GetCardValue(cardTypeResultList[i].CardData[j]) <= cbEndCard
                            && GetCardValue(cardTypeResultList[i].CardData[j]) >= cbBeginCard)
                        {
                            cbHandCardData[cbHandCardCount++] = cardTypeResultList[i].CardData[j];
                            cbRemoveCard[cbRemoveCount++] = cardTypeResultList[i].CardData[j];
                        }
                    }
                    //删除扑克
                    RemoveCard(cbRemoveCard, cbRemoveCount, cbRemainCardData, cbRemainCardCount,GLogicDef.FULL_COUNT);
                    cbRemainCardCount = (BYTE)(cbRemainCardCount - cbRemoveCount);
                    return true;
                }
            }
        }
        return false;
    }
    //配置不是连子类型
    public bool GetNotLineCardByConfig(tagConfigCard tagConfigData, BYTE[] cbHandCardData, ref BYTE cbHandCardCount, BYTE[] cbRemainCardData, ref  BYTE cbRemainCardCount)
    {
        //类型所关联的排数相等个数
        BYTE cbMaxEqual = 0;
        //---------------------------------------------------------------------
        //计算排数相等个数
        if (tagConfigData.cbCardType == GLogicDef.CT_FOUR) cbMaxEqual = 4;
        else if (tagConfigData.cbCardType == GLogicDef.CT_BOMB_CARD) cbMaxEqual = 4;
        else if (tagConfigData.cbCardType == GLogicDef.CT_THREE) cbMaxEqual = 3;
        else if (tagConfigData.cbCardType == GLogicDef.CT_DOUBLE) cbMaxEqual = 2;
        else if (tagConfigData.cbCardType == GLogicDef.CT_SINGLE) cbMaxEqual = 1;
        else return false;
        //---------------------------------------------------------------------
        if (cbRemainCardCount < cbMaxEqual) return false;
        BYTE[] cbTempCardData = (BYTE[])cbRemainCardData.Clone();
        BYTE cbTempCardCount = cbRemainCardCount;
        SortCardList(cbTempCardData, cbTempCardCount,GLogicDef.FULL_COUNT);

        //打乱花色
        BYTE cbLastCardIndex = 0;
        BYTE cbSameCount = 1;
        BYTE cbCardIndex = 0;
        while (cbLastCardIndex < cbTempCardCount)
        {
            for (cbCardIndex = (BYTE)(cbLastCardIndex + 1); cbCardIndex < cbTempCardCount; cbCardIndex++)
            {
                if (GetCardValue(cbTempCardData[cbLastCardIndex]) == GetCardValue(cbTempCardData[cbCardIndex]))
                {
                    cbSameCount++;
                    continue;
                }
                break;
            }
            if (cbSameCount > 1)
            {
                cbTempCardData.Shuffle(cbLastCardIndex, cbCardIndex - 1);
            }
            cbSameCount = 1;
            cbLastCardIndex = cbCardIndex;
        }

        //去除123
        BYTE cbFirstCardIndex = 0;
        if (tagConfigData.cbCardData == 0 && tagConfigData.cbCardType == GLogicDef.CT_BOMB_CARD)
        {
            for (BYTE i = 0; i < cbTempCardCount; ++i)
            {
                if (GetCardValue(cbTempCardData[i]) > 3)
                {
                    cbFirstCardIndex = i;
                    //牌数不够
                    if (cbTempCardCount - i < cbMaxEqual * 3) return false;
                    break;
                }
            }
        }

        List<tagAnalyseCardTypeResult> cardTypeResultList = new List<tagAnalyseCardTypeResult>();
        BYTE cbCurCardCount = 0;
        BYTE cbEqualCount = 1;
        BYTE[] cbCurCardData = new BYTE[GLogicDef.FULL_COUNT];
        bool IsExist = true;
        BYTE cbLastCard = 0x00;
        //---------------------------------------------------------------------
        //切割牌
        while (cbFirstCardIndex < cbTempCardCount && IsExist)
        {
            IsExist = false;
            cbEqualCount = 1;
            cbLastCard = cbTempCardData[cbFirstCardIndex];
            for (BYTE i = (BYTE)(cbFirstCardIndex + 1); i < cbTempCardCount; i++)
            {
                if (GetCardValue(cbLastCard) == GetCardValue(cbTempCardData[i]))
                {
                    cbEqualCount++;
                }
                //同牌判断
                if (GetCardValue(cbLastCard) != GetCardValue(cbTempCardData[i]))
                {
                    if (cbEqualCount >= cbMaxEqual)
                    {
                        for (BYTE j = 0; j < cbMaxEqual; j++)
                        {
                            cbCurCardData[cbCurCardCount++] = cbTempCardData[i - cbEqualCount + j];
                        }
                    }
                    else
                    {
                        cbCurCardCount = 0;
                    }
                    cbLastCard = cbTempCardData[i];
                    cbEqualCount = 1;
                }
                else if (i == cbTempCardCount - 1 && cbEqualCount >= cbMaxEqual && cbCurCardCount > 0)
                {
                    for (BYTE j = 0; j < cbMaxEqual; j++)
                        cbCurCardData[cbCurCardCount++] = cbTempCardData[i - cbEqualCount + 1 + j];
                }
            }
            if (cbCurCardCount > 0)
            {
                tagAnalyseCardTypeResult CardTypeResult = new tagAnalyseCardTypeResult();
                CardTypeResult.cbCardType = tagConfigData.cbCardType;
                cbCurCardData.CopyList(CardTypeResult.CardData, cbCurCardCount);
                cardTypeResultList.Add(CardTypeResult);
                RemoveCard(cbCurCardData, cbCurCardCount, cbTempCardData, cbTempCardCount,GLogicDef.FULL_COUNT);
                cbTempCardCount = (BYTE)(cbTempCardCount - cbCurCardCount);
                cbCurCardCount = 0;
                IsExist = true;
            }
        }
        //---------------------------------------------------------------------

        //抽取配置的牌
        BYTE cbCardTypeNum = 0;
        if (tagConfigData.cbCardData == 0x00)  //随机的
        {
            for (int i = 0; i < cardTypeResultList.Count; i++)
            {
                BYTE cbCount = (BYTE)(cardTypeResultList[i].CardData.Count);
                cbCardTypeNum += (BYTE)((int)(cbCount / cbMaxEqual));
            }
            System.Random ran = new System.Random();
            BYTE cbRandIndex = (BYTE)(ran.Next(0, cbCardTypeNum));
            BYTE cbPreThreeLineCount = 0;
            cbCardTypeNum = 0;
            for (int i = 0; i < cardTypeResultList.Count; i++)
            {
                cbCardTypeNum += (BYTE)((int)(cardTypeResultList[i].CardData.Count / cbMaxEqual));
                if (cbCardTypeNum > cbRandIndex)
                {
                    BYTE cbIndex = (BYTE)((cbRandIndex - cbPreThreeLineCount) * cbMaxEqual);
                    BYTE[] cbRemoveCard = new BYTE[cbMaxEqual];
                    for (int j = 0; j < cbMaxEqual; j++)
                    {
                        cbHandCardData[cbHandCardCount++] = cardTypeResultList[i].CardData[j + cbIndex];
                        cbRemoveCard[j] = cbHandCardData[cbHandCardCount - 1];
                    }
                    //删除扑克
                    RemoveCard(cbRemoveCard, (BYTE)(cbMaxEqual), cbRemainCardData, cbRemainCardCount,GLogicDef.FULL_COUNT);
                    cbRemainCardCount = (BYTE)(cbRemainCardCount - cbMaxEqual);
                    return true;
                }
                cbPreThreeLineCount = cbCardTypeNum;
            }
        }
        else //指定的
        {
            BYTE cbCardColor = (BYTE)((tagConfigData.cbCardData & 0xF0) >> 4);
            //0随机花色
            if (cbCardColor == 0)
            {
                System.Random ran = new System.Random();
                cbCardColor = (byte)ran.Next(0, 4);
            }
            else
            {
                cbCardColor -= 1;
            }


            BYTE cbCardValue = (BYTE)(tagConfigData.cbCardData & 0x0F);
            if (cbMaxEqual == 1) cbCardValue = (BYTE)(cbCardColor << 4 | cbCardValue);
            BYTE[] cbRemoveCard = new BYTE[cbMaxEqual];
            BYTE cbRemoveCount = 0;
            BYTE cbCurCardValue = 0x00;
            for (int i = 0; i < cardTypeResultList.Count; i++)
            {
                for (int j = 0; j < cardTypeResultList[i].CardData.Count; j++)
                {
                    if (cbMaxEqual == 1) cbCurCardValue = cardTypeResultList[i].CardData[j];
                    else cbCurCardValue = GetCardValue(cardTypeResultList[i].CardData[j]);
                    if (cbCurCardValue == cbCardValue)
                    {
                        cbHandCardData[cbHandCardCount++] = cardTypeResultList[i].CardData[j];
                        cbRemoveCard[cbRemoveCount++] = cardTypeResultList[i].CardData[j];
                        if (cbRemoveCount >= cbMaxEqual)
                        {
                            //删除扑克
                            RemoveCard(cbRemoveCard, cbRemoveCount, cbRemainCardData, cbRemainCardCount,GLogicDef.FULL_COUNT);
                            cbRemainCardCount = (BYTE)(cbRemainCardCount - cbMaxEqual);
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
    #endregion

}

