using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignData : MonoBehaviour
{
	public List<INFO_SIGN> signInfo = new List<INFO_SIGN> ();
	public int continueSignedDays = 0;
	public bool isTodaySigned = false;

	public void Init ()
	{
		signInfo = DataBase.Instance.SIGN_MGR.GetAllSignData ();
		continueSignedDays = DataBase.Instance.PLAYER.GetContinueSignedDays ();
		isTodaySigned = DataBase.Instance.PLAYER.IsSignedToday ();
		Debug.Log ("continueSignedDays:"+continueSignedDays);
		Debug.Log ("isTodaySigned:"+isTodaySigned);
	}

	public void Sign ()
	{
		DataBase.Instance.PLAYER.Sign ();
		Init ();
	}
}
