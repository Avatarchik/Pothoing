using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum PlayerStateText
{
    ScorePass,
    Score1,
    Score2,
    Score3,
    Ready,
	PlayPass,
	Black,
    None
}

/// <summary>
/// 出牌选择枚举
/// </summary>
public enum ChoiceCommand
{
    CHOICE_PASS,    //不出
    CHOICE_OUTCARD, //出牌
    CHOICE_TIPE     //提示
}

interface IGameView
{
    /// <summary>
    /// 更新玩家panel
    /// </summary>
    /// <param name="allPlayer">所有玩家数据</param>
    void SetInforPanel(int id, Player player);
    
    /// <summary>
    /// 倒计时时钟
    /// </summary>
    /// <param name="id">座位号</param>
    /// <param name="times">倒计时秒数</param>
    /// <param name="callback">完成回调</param>
	void ClockStart( int times, TableController.ClockCallBack callback,int id,bool isShow=false);

    /// <summary>
    /// 停止计时，放弃回调
    /// </summary>
    void ClockStop();

    /// <summary>
    /// 设置坑主
    /// </summary>
    /// <param name="id"></param>
    void SetMaster(int id);

    /// <summary>
    /// 设置玩家面板状态
    /// </summary>
    /// <param name="id"></param>
    /// <param name="stateText"></param>
    void SetState(int id, PlayerStateText stateText);
    
}
