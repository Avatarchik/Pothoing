using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class EconomicSystem
{

    
    /// <summary>
    /// 炸弹场游戏结算 分数 = 底分 × （叫分 + 炸弹倍数） × 2；
    /// </summary>
    /// <param name="allParm">
    /// 0: hasMasterWin 1 yes 0 no
    /// 1: MasterIndex
    /// 2: CallScore
    /// 3: BomNum
    /// 4: BaseNum
    /// 5: Revenue 
    /// </param>
    /// <param name="outAllScore">输出分数</param>
    public static void RulerClearBomGame(int[] allParm, int[] outAllScore)
    {
        if (allParm.Length != 6)
        {
            return;
        }

        //出炸弹个数
        int bomNum = allParm[3] > 2 ? 2 : allParm[3];

        for (int i = 0; i < outAllScore.Length;i++ )
        {
            int addNum = 0;

            if (allParm[1] == i )   //是否坑主
            {
                addNum = (allParm[0] == 1 ? 2 : -2) ;
            }
            else                    //农民
            {
                addNum = (allParm[0] == 1 ? -1 : 1);
            }
            outAllScore[i] = allParm[4] * (bomNum * 2 + allParm[2]) * addNum;

            //减去税收
            if (outAllScore[i] >= 100L)
            {
                int lRevenue = (int)(outAllScore[i] * allParm[5] * 1.0f / 100.0f);
                outAllScore[i] -= lRevenue;
            }
        }
        
    }

    /// <summary>
    /// 普通场结算 
    /// ----------------------------------
    /// 坑主胜利时，  坑主加分，分数=底分×倍数×2；
    ///             平民减分，分数=底分×倍数。
    /// 平民胜利时，  坑主减分，分数=底分×倍数×2；
    ///             平民加分，分数=底分×倍数
    /// ------------------------------------
    /// </summary>
    /// <param name="allParm">
    /// 0: hasMasterWin 1 yes 0 no
    /// 1: MasterIndex
    /// 2: CallScore
    /// 3: BaseNum
    /// 4: Revenue  
    /// </param>
    /// <param name="outAllScore"></param>
    public static void RulerClearNormal(int[] allParm, int[] outAllScore)
    {
        if(allParm.Length != 5)
        {
            return;
        }

        for (int i = 0; i < outAllScore.Length; i++)
        {
            int addNum = 0;

            if (allParm[1] == i)   //是否坑主
            {
                addNum = (allParm[0] == 1 ? 2 : -2);
            }
            else                    //农民
            {
                addNum = (allParm[0] == 1 ? -1 : 1);
            }

            outAllScore[i] = allParm[3] * allParm[2] * addNum;

            //减去税收
            if (outAllScore[i] >= 100L)
            {
                int lRevenue = (int)(outAllScore[i] * allParm[4] * 1.0f / 100.0f);
                outAllScore[i] -= lRevenue;
            }
        }
    }

    /// <summary>
    /// 游戏强退 扣分 = 房间倍率 × 3
    /// </summary>
    /// <param name="inData">
    /// 0,房间倍率
    /// </param>
    public static int GameBreakScore(int[] inData)
    {
        return inData[0] * 3;
    }

}

