using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class LuckDraw : MonoBehaviour {


    [SerializeField]
    RectTransform RewardsBox;

    [SerializeField]
    RewardItem Reward;

    [SerializeField]
    float ItemHeight;

    [SerializeField]
    Text DrawScore;

    [SerializeField]
    Button BeginLuckBut;

    int rewardsNum = 0;
    float drawHeight = 0;

    private List<INFO_PROPERTY> rewards;


    private INFO_STAGE stageDraw; //关卡抽奖
    public void SetDrawStage(INFO_STAGE stage)
    {
        this.stageDraw = stage;
        DrawScore.text = stage.LotteryCost.ToString();
    }
    //初始化奖励项
    public void InitRewards()
    {
        rewards = DataBase.Instance.LOTTERY_MGR.GetLotteryProp();
        rewardsNum = rewards.Count;
        //滚动区高度
        drawHeight = rewardsNum * ItemHeight * 2;
        Rect rc = RewardsBox.rect;
        RewardsBox.sizeDelta = new Vector2(rc.width, drawHeight);

        int temp = 0;
        for (int i = 0; i < rewardsNum * 2; i++)
        {
            temp = i;
            if (i >= rewardsNum) temp = i - rewardsNum;
            RewardItem item = Instantiate(Reward) as RewardItem;
            INFO_PROPERTY prop = rewards[temp];
            item.SetReward(rewards[temp].Name);
            item.transform.SetParent(RewardsBox);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = new Vector3(0, i * ItemHeight);
             
        }
        
    }

    //摇奖动画
    IEnumerator Scroll()
    {
        if (stageDraw != null)
        {
            //获取抽奖结果
            INFO_PROPERTY luckResult = DataBase.Instance.LOTTERY_MGR.GetLotteryResult(stageDraw);
            if (luckResult != null)
            {
                int renum = rewards.IndexOf(luckResult);
                renum -= 1;
                if (renum < 0)
                    renum = rewardsNum - 1;
                int scrollNum = Random.Range(6, 10);
                Vector2 v2 = RewardsBox.anchoredPosition;
                //计算总共要滚动的距离 
                float tutor = (scrollNum * rewardsNum + renum) * ItemHeight;
                tutor += v2.y;
                float sleep = 0f;
                while (tutor > 0.1)
                {
                    sleep = tutor / (scrollNum * rewardsNum + renum);
                    v2 = RewardsBox.anchoredPosition;
                    if (v2.y <= -drawHeight / 2)
                    {
                        v2.y += drawHeight / 2;
                        RewardsBox.anchoredPosition = v2;
                    }
                    v2.y -= sleep;
                    tutor -= sleep;
                    RewardsBox.anchoredPosition = v2;
                    yield return new WaitForSeconds(0.001f);
                }
            }
            
            
        }
        LobbyDialogManager.inst.ShowCover(false);
        BeginLuckBut.interactable = true;
        yield return 0;
    }
    // 摇奖
    public void BeginLuckDraw()
    {
        LobbyDialogManager.inst.ShowCover(true);
        BeginLuckBut.interactable = false;
        StartCoroutine(Scroll());
    }
	void Start () {
        InitRewards();
	}
	
	void Update () {
	
	}
}
