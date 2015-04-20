using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum OutCardsType
{
	Pass,
	Single,
	Pair,
	Triple,
	Bomb,
	Straight,
	PairStraight,
	TripleStraight,
	BombStraight
}

class GameLogicUtil
{
	//得到牌 值
	public static int GetCardScore (byte card)
	{
		return card & 0x0f;
	}

	public static int GetVoiceIndex(byte card){
		int score=GetCardScore (card);
		if (score <= 3) {
			return 9+score;
		}else{
			return score-4;
		}
	}


//	public const int VFX_NONE   = 0;
//    public const int VFX_BOM    = 1;
//    public const int VFX_ORDER  = 2;
   

	public static OutCardsType GetOutCardType (byte[] cardList)
	{


//        if (HasSameCardList(cardList))
//        {
//			if (cardList.Length < 4)
//			{
//				return VFX_NONE;
//			}
//        }
//        else if (HasStraight(cardList))
//        {
//            return VFX_ORDER;
//        }
//
//        return VFX_NONE;

		if (cardList.Length > 4) {
			if (HasStraight (cardList)) {
				return OutCardsType.Straight;
			} else {
				int sameCount = -HasSameCardList (cardList);
				if (sameCount == 2) {
					return OutCardsType.PairStraight;
				} else if (sameCount == 3) {
					return OutCardsType.TripleStraight;
				} else if (sameCount == 4) {
					return OutCardsType.BombStraight;
				} else {
					return OutCardsType.Pass;
				}
			}
		} else if (cardList.Length == 4) {
			if (HasSameCardList (cardList) > 0) {
				return OutCardsType.Bomb;
			} else {
				return OutCardsType.Straight;
			}
		} else if (cardList.Length == 3) {
			if (HasSameCardList (cardList) > 0) {
				return OutCardsType.Triple;
			} else {
				return OutCardsType.Straight;
			}

		} else if (cardList.Length == 2) {
			return OutCardsType.Pair;
		} else if (cardList.Length == 1) {
			return OutCardsType.Single;
		} else {
			return OutCardsType.Pass;
		}
	}

	public static int HasSameCardList (byte[] cardList)
	{
		for (int i = 1; i < cardList.Length; i++) {
			if (GetCardScore (cardList [i]) != GetCardScore (cardList [0])) {
				return -i;
			}
		}
		return GetCardScore (cardList [0]);
	}

	public static bool HasStraight (byte[] cardList)
	{
//        if (cardList.Length < 5)
//        {
//            return false;
//        }

		for (int i = 0; i < cardList.Length - 1; i++) {
			int current = GetCardScore (cardList [i]);
			int next = GetCardScore (cardList [i + 1]);

			if (current - 1 != next) {
				return false;
			}
		}

		return true;
	}


    public static bool AddStageCondCard(GameLogic gameLogic,ushort wuser)
    {
        if (DataBase.Instance.IsStageRoom)
        {
            uint curStage = DataBase.Instance.CurStage.Id;
            List<INFO_STAGE_COND_LINK> stageCondTwo = 
                DataBase.Instance.STAGE_MGR.GetStageCondInfoByGroup(curStage, StageCond.COND_GROUPTWO);
            
            if (DataBase.Instance.PLAYER.Property != PropertyMgr.PROPERTY_BOM &&
                stageCondTwo.Count < 1)
                return false;
            //有起手出牌的任务需要发红桃4
            bool IsRed4 = false;
            var condhand = stageCondTwo.Find((x) => x.FixedHand == 1);
            if (condhand != null)
            {
                //发一张红桃4
                gameLogic.AddConfigCard(GLogicDef.CT_SINGLE, (byte)0x34, wuser);
                IsRed4 = true;
            }
            

            foreach (INFO_STAGE_COND_LINK cond in stageCondTwo)
            {
                switch (cond.StageConditionId)
                {
                    case StageCond.COND_OUTSINGLE: //出单张
                        if ((cond.Condition & 0x0F) == 4 && IsRed4)
                            break;
                        gameLogic.AddConfigCard(GLogicDef.CT_SINGLE, (byte)cond.Condition, wuser);
                        break;
                    case StageCond.COND_OUTDOUBLE: //出对子
                        gameLogic.AddConfigCard(GLogicDef.CT_DOUBLE, (byte)cond.Condition, wuser);
                        break;
                    case StageCond.COND_OUTTHREE:  //出三条
                        gameLogic.AddConfigCard(GLogicDef.CT_THREE, (byte)cond.Condition, wuser);
                        break;
                    case StageCond.COND_SINGLELINK://出单连
                        gameLogic.AddConfigCard(GLogicDef.CT_SINGLE_LINE, (byte)cond.Condition, wuser);
                        break;
                    case StageCond.COND_DOUBLELINK://双连
                        gameLogic.AddConfigCard(GLogicDef.CT_DOUBLE_LINE, (byte)cond.Condition, wuser);
                        break;
                    case StageCond.COND_THREELINK://三连
                        gameLogic.AddConfigCard(GLogicDef.CT_THREE_LINE, (byte)cond.Condition, wuser);
                        break;
                    case StageCond.COND_FOURLINK://四连
                        gameLogic.AddConfigCard(GLogicDef.CT_FOUR_LINE, (byte)cond.Condition, wuser);
                        break;
                    case StageCond.COND_OUTBOMB://出炸弹
                        gameLogic.AddConfigCard(GLogicDef.CT_BOMB_CARD, (byte)cond.Condition, wuser);
                        break;
                    default:
                        break;
                }
            }

            
            var bomCon = stageCondTwo.Find((x) => x.StageConditionId == StageCond.COND_OUTBOMB);
            if (DataBase.Instance.PLAYER.Property == PropertyMgr.PROPERTY_BOM &&
                bomCon == null)
            {
                gameLogic.AddConfigCard(GLogicDef.CT_BOMB_CARD, 0, wuser);
            }
            else if (DataBase.Instance.PLAYER.Property != PropertyMgr.PROPERTY_BOM
                && bomCon != null)
            {
                for (ushort i = 0; i < 3; i++)
                { 
                    if(i!= wuser)
                        gameLogic.AddConfigCard(GLogicDef.CT_BOMB_CARD, 0, i);
                }
            }

            return true;

        }

        return false;
    }

    ///////////////////////////////////////////////////////
    /// <summary>
    /// 是否必叫 三个3，或者四个3，或者两个3＋两个2
    /// </summary>
    /// <param name="allCard"></param>
    /// <returns></returns>
    public static bool HasMustCallScore(byte[] allCard)
    {
        const byte num1 = 3;
        const byte num2 = 2;

        int num1count = StatSum(allCard, num1);
        int num2count = StatSum(allCard, num2);

        if(num1count > 2)
        {
            return true;
        }else if(num1count > 1 && num2count > 1)
        {
            return true;
        }

        return false;
    }

    public static int StatSum(byte[] allData, byte num)
    {
        int count = 0;
        for (int i = 0; i < allData.Length; i++)
        {
            if (GetCardScore(allData[i]) == num)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="allData"></param>
    /// <returns></returns>
    public static byte[] ConverListToByte(List<byte> allData)
    {
        byte[] temp = new byte[allData.Count];

        for(int i = 0;i < allData.Count;i++)
        {
            temp[i] = allData[i];
        }

        return temp;
    }
}
