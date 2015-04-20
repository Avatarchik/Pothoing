using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class StageController : MonoBehaviour {

    [SerializeField]
    WindowData winData;

    [SerializeField]
    RectTransform DrawParent;
    [SerializeField]
    RectTransform StageDraw; //拖动对象

    [SerializeField]
    Transform Decoration; //地图装饰

    [SerializeField]
    Transform Route; //路径

    [SerializeField]
    Transform Stages; //关卡组

    [SerializeField]
    StageBtn stage;   //关卡对象

    [SerializeField]
    Text StarCount;

    [SerializeField]
    LobbyDialogManager DialogMar;
    //当前用户


    //地图宽、高度
    private float mapWidth = 0;
    private float mapHeight = 480;

    private StageMgr sMgr;
    private Player player;

    void Awake()
    {
        sMgr = DataBase.Instance.STAGE_MGR;
        player = DataBase.Instance.PLAYER;
    }
    //初始化关卡地图
    public void InitStageMap()
    {
        float beginX = 0;
        float endX = 0;
        //读取所有关卡
        List<INFO_STAGE> stageList = sMgr.GetAllStage();
        //读取用户完成关卡状态
        StarCount.text = player.GetTotalStar().ToString();
        
        int stageNum = stageList.Count;
        float parentWidth = DrawParent.rect.width;
        if (stageNum > 0)
        {
            beginX = stageList[0].X;
            endX = stageList[stageNum-1].X;
            //计算地图宽度
            mapWidth = beginX + endX;
            //float posx = -mapWidth / 2;
            float posy = -mapHeight / 2;
            Rect rc = StageDraw.rect;
            //StageDraw.rect.Set(rc.xMin, rc.yMin, mapWidth, rc.height);
            StageDraw.sizeDelta = new Vector2(mapWidth,480);
            //Vector3 drawLocal = StageDraw.localPosition;
            INFO_STAGE curStage = DataBase.Instance.STAGE_MGR.GetStage(player.CurStageId);
            //drawLocal.x = posx + curStage.X;
            //StageDraw.localPosition = drawLocal;
            StageDraw.anchoredPosition = new Vector2(-curStage.X + (parentWidth*0.5f), 0);
            foreach (INFO_STAGE stage in stageList)
            {
               
                StageBtn stageBut = Instantiate(this.stage) as StageBtn;
                StageStatus status = player.GetStageStatus(stage.Id);
                UserStageRecord ustage = player.GetUserStageRecord(stage.Id);
                if (status == StageStatus.Compelet)
                {
                    stageBut.clickEvent += PopStageInfo;
                    int starNum = COMMON_FUNC.GetStarByScore(stage.Id, ustage.BestScore);
                    stageBut.SetStar(starNum);
                }
                stageBut.clickEvent += StageButClick;
                stageBut.transform.SetParent(Stages);
                Vector3 v3 = new Vector3(stage.X,posy+stage.Y);
                stageBut.transform.localScale = Vector3.one;
                stageBut.InitStage(stage.Id, stage.Name, v3);

            }
        }
        
        


    
    }


    //关卡按钮处理
    void StageButClick(uint stageId)
    {
        INFO_STAGE stage = DataBase.Instance.STAGE_MGR.GetStage(stageId);
        StageStatus status = player.GetStageStatus(stageId);
        if (status == StageStatus.CanPlay)
        {
            PopStageInfo(stageId);
        }
        else if (status == StageStatus.NotUnlock)
        {
            //提示
            LobbyDialogManager.inst.ShowConfirmDialog(
                    string.Format(TextManager.Get("StageUnlockTip"), stage.NeedStar),
                    null
            );
        }
        else if (status == StageStatus.Unlocked)
        {
            //开启 前置关卡未完成
            LobbyDialogManager.inst.ShowConfirmDialog(
                    string.Format(TextManager.Get("StageNotCanPlayTip"), stage.OpenCondition),
                    null
            );
        }
        
    }
    //弹出关卡信息
    INFO_STAGE openStage;
    public void PopStageInfo(uint stageId)
    {
        openStage = DataBase.Instance.STAGE_MGR.GetStage(stageId);
        UserStageRecord record = DataBase.Instance.PLAYER.GetUserStageRecord(stageId);
        if (record != null && record.LastStatus == (int)StageRecordStatus.Progress)
        {
            //正在进行的关卡，显示关卡进度
            List<INFO_STAGE_COND_LINK> stageConds = 
                DataBase.Instance.STAGE_MGR.GetStageCondInfoByGroup(stageId, StageCond.COND_GROUPTHREE);
            INFO_STAGE_COND_LINK stageCondThree = null;
            if (stageConds.Count > 0) stageCondThree = stageConds[0];

            //作弊效果
            string propStr = "无";
            int propId = DataBase.Instance.PLAYER.Property;
            if (propId != 0)
            {
                INFO_PROPERTY prop = DataBase.Instance.PROP_MGR.GetProperty(propId);
                propStr = prop.Name;
            }
            //弹出关卡进度
            DialogMar.ShowConfirmDialog(
                    string.Format(TextManager.Get("StageProgressInfo"),
                    "(" + record.CondVal3 + "/" + stageCondThree.Condition + ")", propStr),
                    rePlayStage, TextManager.Get("StageReplayBut"), true, rePlayStage,
                    TextManager.Get("StageContinueBut"),
                    string.Format(TextManager.Get("StageProgressTitle"), stageId)
                );
            return;
            
        }
        Dialog stageDialog = DialogMar.GetDialog(LobbyDialog.LevelInfo);
        StageInfo stageInfo = stageDialog.GetComponent<StageInfo>();

        if (stageInfo != null)
        {
            stageInfo.InitStage(openStage, record);
            stageInfo.BeginStageClick = rePlayStage;
            DialogMar.ShowDialog(LobbyDialog.LevelInfo);
        }
    }
    //重玩关卡
    void rePlayStage()
    {
        DataBase.Instance.STAGE_MGR.ReplayStage(openStage.Id);
    }

	void Start () {
        //StartCoroutine(init());
        InitStageMap();
        if (winData.StageId > 0)
            StageButClick(winData.StageId);
	}
	
}
