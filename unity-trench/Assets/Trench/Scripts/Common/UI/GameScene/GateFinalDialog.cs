using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public enum GateResult
{
    RESULT_SUCCEE,
    RESULT_FAIL
}


public class GateFinalDialog : Dialog {

	// Use this for initialization

    [SerializeField]
    StartComponet[] mStarts;

    [SerializeField]
    Button ConfirmationBut;

    [SerializeField]
    Button CancelBut;

    [SerializeField]
    Text ResultText;

    [SerializeField]
    Text StageTarget;

    [SerializeField]
    Text StageScore;

    void Start()
    {
        SetStart(1);
    }

    public void InitFinal(bool isSuccess,int starNum,string target,int score)
    {
        if (isSuccess)
            ResultText.text = TextManager.Get("StageResultSuccess");
        else
            ResultText.text = TextManager.Get("StageResultFailure");

        SetStart(starNum);
        StageTarget.text = target;
        StageScore.text = score.ToString();
    }

    public void SetFinalConfirmation(string butTitle,UnityAction butAction)
    {
        ConfirmationBut.GetComponentInChildren<Text>().text = butTitle;
        ConfirmationBut.onClick.RemoveAllListeners();
        if (butAction != null)
            ConfirmationBut.onClick.AddListener(butAction);
        else {
            ConfirmationBut.onClick.AddListener(Hide);
        }

    }

    public void SetFinalCancel(string butTitle, UnityAction butAction)
    {
        CancelBut.GetComponentInChildren<Text>().text = butTitle;
        CancelBut.onClick.RemoveAllListeners();
        if (butAction != null)
        {
            
            CancelBut.onClick.AddListener(butAction);
        }
            
        else
        {
            CancelBut.onClick.AddListener(Hide);
        }
        
    }

    public void SetStart(int num)
    {
        if(num < 0 || num > 3)
        {
            return;
        }

        for(int i = 0;i < mStarts.Length;i++)
        {
            if(i <= num - 1)
            {
                mStarts[i].SetState(StartState.OFF);                
            }
            else
            {
                mStarts[i].SetState(StartState.ON);
            }

        }

    }


    public void SetResult(GateResult res)
    {
        switch(res)
        {
            case GateResult.RESULT_SUCCEE:

                break;
            case GateResult.RESULT_FAIL:

                break;
        }
    }


    public override void Hide()
    {
        base.Hide();
    }

    public override void Show()
    {
        base.Show();
    }

    

}


