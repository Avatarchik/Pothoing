using System;
using UnityEngineEx.CMD.i3778;


public interface IGameSink
{
    void OnGameMessage(GlobalDef.CMD_Head cmd, object o);//游戏处理函数
    void ResetData();//新开一局 重置游戏数据
    //void OnBlendTrench(object o);       //黑挖
    //void OnGameStart(object o);         //游戏开始回调
    //void OnSendCard(object o);          //发牌
    //void OnGameEnd(object o);           //游戏结束
    //void OnCallScore(object o);    //叫分
    //void OnOutCard(object o);  //出牌
    //void OnNOOutCard(object o);         //不出
    //void OnPrompt(object o);            //提示
    //void OnNextTurn(object o);          //再来一局
    //void OnTimeOut(object o); //定时器超时

}