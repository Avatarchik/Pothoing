using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MissionInfoItem : MonoBehaviour
{

	[SerializeField]
	Text
		title;
	[SerializeField]
	Text
		desc;
	[SerializeField]
	Text
		progressLabel;
	[SerializeField]
	Slider
		progressBar;
	[SerializeField]
	Text
		state;
	//[Header("Locked,Progressing,Completed,Failed,Invalid,Skipped")]
	[SerializeField]
	string[]
		stateMarks = {
		"Locked",
		"Progressing",
		"Completed",
		"Failed",
		"Invalid",
		"Skipped"
	};

//	string completeDesc;
//	string failedDesc;

	public void Init (StageMissionData data)
	{
		title.text = data.title;
		desc.text = data.desc;
		progressLabel.text = data.currentProgress + "/" + data.targetProgress;
		progressBar.value = (float)data.currentProgress / (float)data.targetProgress;
		SwitchState (data.state);
	}

	public void SwitchState (MissionProgressState missionState)
	{
		state.text = stateMarks [(int)missionState];
		switch (missionState) {
		case MissionProgressState.Locked:
			state.gameObject.SetActive (true);
			break;
		case MissionProgressState.Progressing:
			state.gameObject.SetActive (false);
			break;
		case MissionProgressState.Completed:
			state.gameObject.SetActive (true);
			break;
		case MissionProgressState.Failed:
			state.gameObject.SetActive (true);
			break;
		case MissionProgressState.Invalid:
			state.gameObject.SetActive (true);
			break;
		case MissionProgressState.Skipped:
			state.gameObject.SetActive (true);
			break;
		}
	}

//	public void Progress(){
//		SwitchState (data.state);
//	}

//	public void OnCompleted(){
//	
//	}

//	public void OnFailed(){
//		
//	}
//
//	public void OnProgress(){
//		
//	}
}

