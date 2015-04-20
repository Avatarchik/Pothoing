using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum TableState
{
	None,
	Black,
	Ready,
	Identify,
	Play,
	Result
}

public enum DisableButton
{
	Score1,
	Score2,
	Hint,
	Play,
	PlayPass,
    IdentifyPass
}

public class TableButtonManager : MonoBehaviour
{
	[ContextMenuItem ("Refresh State","OnChangeState")]
	[SerializeField]
	ScaleTweener[]
	blackBtns;
	[SerializeField]
	ScaleTweener[]
		readyBtns;
	[SerializeField]
	ScaleTweener[]
		identifyBtns;
	[SerializeField]
	ScaleTweener[]
		playBtns;
	[SerializeField]
	ScaleTweener[]
		resultBtns;
	[Tooltip("0:score 1,1:score 2,2:hint,3:play,4:play pass,5:identify pass")]
	[SerializeField]
	TableButton[]
		disableBtns;

   
	ScaleTweener[][] stateBtns;

	public TableState State {
		get {
			return state;
		}set {
			if (state != value) {
				state = value;
				OnChangeState ();
			}
		}
	}

	TableState state;

	void Start ()
	{
		stateBtns = new ScaleTweener[][]{blackBtns, readyBtns,identifyBtns,playBtns,resultBtns};
	}

	void OnChangeState ()
	{
		ToggleBtns (blackBtns, false, false);
		ToggleBtns (readyBtns, false, false);
		ToggleBtns (identifyBtns, false, false);
		ToggleBtns (playBtns, false, false);
		ToggleBtns (resultBtns, false, false);
		switch (state) {
		case TableState.Black:
			ToggleBtns (blackBtns, true, true);
			break;
		case TableState.Ready:
			ToggleBtns (readyBtns, true, true);
			break;
		case TableState.Identify:
			ToggleBtns (identifyBtns, true, true);
			break;
		case TableState.Play:
			ToggleBtns (playBtns, true, true);
			break;
		case TableState.Result:
			ToggleBtns (resultBtns, true, true);
			break;
		default:
			break;
		}
	}

	public void TogglePlayButtons (bool isShow)
	{
		ToggleBtns (playBtns, isShow, true);
	}

	public void ToggleBtns (TableState tableState, bool isShow)
	{
		state = tableState;
		ToggleBtns (stateBtns [(int)tableState - 1], isShow, true);
	}

	void ToggleBtns (ScaleTweener[] btns, bool isShow, bool isAnimated)
	{
		foreach (ScaleTweener btn in btns) {
			if (isShow) {
				btn.gameObject.SetActive (true);
				if (isAnimated) {
					btn.Show ();
				}
				btn.GetComponent<Button> ().interactable = true;
				SOUND.Instance.OneShotSound (Sfx.inst.show);
			} else {
				//Debug.Log(btn.GetComponent<Button> ().enabled );
				btn.GetComponent<Button> ().interactable = false;
				if (isAnimated) {
					btn.Hide (delegate() {
						btn.gameObject.SetActive (false);
					});
					SOUND.Instance.OneShotSound (Sfx.inst.hide);
				} else {
					btn.gameObject.SetActive (false);
				}
			}
		}
	}

	public void DisableBtn (DisableButton btn, bool isDisable=true)
	{
		disableBtns [(int)btn].ChangeSprite (isDisable);
	}

    public void SetBlackCallBack(System.Action blackCall, System.Action cancal)
    {
        Button bublack = blackBtns[0].gameObject.GetComponent<Button>();
        Button bucancal = blackBtns[1].gameObject.GetComponent<Button>();

        bublack.onClick.RemoveAllListeners();
        bucancal.onClick.RemoveAllListeners();

        if (blackCall == null)
        {
            bublack.onClick.AddListener(null);
        }
        else
        {
             bublack.onClick.AddListener(blackCall.Invoke);
        }

        if (cancal == null)
        {
            bucancal.onClick.AddListener(null);
        }
        else
        {
            bucancal.onClick.AddListener(cancal.Invoke);
        }
        
    }
}
