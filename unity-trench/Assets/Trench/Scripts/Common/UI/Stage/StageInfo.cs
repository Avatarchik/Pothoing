using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/// <summary>
/// 关卡信息
/// wll
/// 2015-03-16
/// </summary>
public class StageInfo : MonoBehaviour {

    [SerializeField]
    Text StageName; //关卡名称
    [SerializeField]
    Text[] StarText;     //星星分数
    [SerializeField]
    Image[] StarImg;     //星星 
    [SerializeField]
    Sprite redStar;
    [SerializeField]
    Sprite blankStar;
    [SerializeField]
    Text MinScore;  //底分
    [SerializeField]
    Text Condition;   //条件描述

    [SerializeField]
    Slider scoreSlider; //分数进度

    [SerializeField]
    Button BeginStage;

    [SerializeField]
    LuckDraw luckDraw; //关卡抽奖

    [SerializeField]
    Text StageScore; //闯关费用

    //开始关卡事件
    //public delegate void BeginStageHandler();
    public System.Action BeginStageClick;

    

    //初始化关卡
    public void InitStage(INFO_STAGE stage, UserStageRecord record)
    {
        
        StageName.text = stage.Name;
        scoreSlider.maxValue = stage.Star3;
        if (StarText.Length == 3)
        {
            StarText[0].text = TextManager.Get("StageComplete");
            StarText[1].text = stage.Star2.ToString();
            StarText[2].text = stage.Star3.ToString();
        }
        if (record != null && record.Status == (int)StageRecordStatus.Success)
        {
            scoreSlider.value = record.BestScore;
            int starNum = COMMON_FUNC.GetStarByScore(stage.Id, record.BestScore);
            for (int i = 0; i < starNum; i++)
                StarImg[i].sprite = redStar;
        }
        else
        {
            scoreSlider.value = 0;
            for (int i = 0; i < 3; i++)
                StarImg[i].sprite = blankStar;
        }
            
        MinScore.text = stage.RoomRate.ToString();
        Condition.text = stage.Describe;
        StageScore.text = stage.StageCost.ToString();
        //设置关卡抽奖
        luckDraw.SetDrawStage(stage);
    }

    void Start()
    { 
        //监听按钮事件
        BeginStage.onClick.AddListener(delegate() {
            if (BeginStageClick != null)
                BeginStageClick();
        });
    }

}
