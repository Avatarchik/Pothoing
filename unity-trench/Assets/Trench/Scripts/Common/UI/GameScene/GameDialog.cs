using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;

enum GameDialogIndex
{
	SETTING,    //设置
	FINAL,      //结算
	TASKALERT,  //任务提示
	STAGEMARK,  //关卡进度
	STAGERESULT //关卡结果
}

public class GameDialog : DialogManager
{

	[SerializeField]
	ConfirmDialog
		mConfigDialog;
	[SerializeField]
	MoveTweener
		curtain;
	//---------------------------------------

	/// <summary>
	/// 结算面板类型 普通场，炸弹场
	/// </summary>
	public const int CATEGORY_NORMALE = 1;
	/// <summary>
	/// 结算面板类型 关卡
	/// </summary>
	public const int CATEGORY_STAGE = 2;
	public static GameDialog inst;

	void Awake ()
	{
		inst = this;
	}
	
	void Start ()
	{
		curtain.FromTarget ();
	}

	public void OnConfirmExit ()
	{        
        mConfigDialog.Init(MainGame.inst.GetBreakButtonTip(), ExitGameScene);
		mConfigDialog.gameObject.SetActive (true);
		mConfigDialog.Show ();
	}



	void ExitGameScene ()
	{
		curtain.ToTarget (OnExitGame);
	}

	public void OnExitGame ()
	{
		Debug.Log ("exit Game!!!!!!!!");
		MainGame.inst.OnExitGame ();
		
	}

	public void AlertTaskText (string contentStr)
	{
		TaskAlert ta = dialogs [(int)GameDialogIndex.TASKALERT] as TaskAlert;
		ta.gameObject.SetActive (true);
		ta.ShowText (contentStr);
	}
    
	/// <summary>
	/// 设置结算面板 
	/// </summary>
	/// <param name="category">CATEGORY_STAGE | CATEGORY_NORMALE</param>
	/// <param name="isWin">是否 获胜</param>
	/// <param name="allData">显示分数</param>
	/// <param name="action">继续回调</param>
	public void SetFinalWin (int category, bool isWin, int[] allData, System.Action action)
	{
		if (currentDialog is FinalDialog) {
			FinalDialog finaldlg = currentDialog as FinalDialog;
            
			finaldlg.SetWin (isWin);
			finaldlg.SetConuite (action);

			switch (category) {
			case CATEGORY_NORMALE:
				finaldlg.SetStatisticsData (allData);
				break;
			case CATEGORY_STAGE:
				finaldlg.SetStageData (allData);
				break;
			}
		}
	}

	public void ShowStageResult (bool isSuccess, int starNum, string target, int score,
                                string butTitle, UnityAction butAction,
                                string butTitle2, UnityAction butAction2)
	{
		ShowDialog ((int)GameDialogIndex.STAGERESULT);
		GateFinalDialog dig = currentDialog as GateFinalDialog;
		if (dig != null) {
			dig.InitFinal (isSuccess, starNum, target, score);
            dig.SetFinalConfirmation(butTitle, delegate
            {
                HideDialog((int)GameDialogIndex.STAGERESULT);
                butAction();
            });
            dig.SetFinalCancel(butTitle2, delegate
            {
                HideDialog((int)GameDialogIndex.STAGERESULT);
                butAction2();
            });
		}

	}

    


}
