using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UserInfoDlg : Dialog
{
	[SerializeField]
	UserData
		data;
	[SerializeField]
	InputField
		input;
	[SerializeField]
	Toggle
		sexToggle;
	[SerializeField]
	Button
		changeBtn;

	public override void Init ()
	{
		if (!string.IsNullOrEmpty (data.uiName)) {
			input.text = data.uiName;
		}
		sexToggle.isOn = data.isFemale;
		changeBtn.interactable = false;
	}

	public void JudgeChangeBtn ()
	{
		if (data.uiName != input.text || data.isFemale != sexToggle.isOn) {
			changeBtn.interactable = true;
		} else {
			changeBtn.interactable = false;
		}
	}

	public void OnChangeBtn ()
	{
		data.SetUserInfo (input.text, sexToggle.isOn);
		JudgeChangeBtn ();
		//changeBtn.interactable = false;
		SOUND.Instance.OneShotSound (Sfx.inst.btn);
	}

	public void OnSexToggle ()
	{
		JudgeChangeBtn ();
		SOUND.Instance.OneShotSound (Sfx.inst.btn);
	}

}
