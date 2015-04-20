using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class RestCard : MonoBehaviour
{

	[SerializeField]
	ScaleTweener
		back;
	[SerializeField]
	Text
		SurplusText;
	int restCardNum = 0;
	float numTweenInterval = 0.01f;

	public int RestCardNum {
		get {
			return restCardNum;
		}set {
			restCardNum = value;
			SurplusText.text = restCardNum.ToString ();
			SurplusText.gameObject.SendMessage("Show");
		}
	}
	
	//设置剩余牌数
	public void SetRestNum (int num)
	{
		iTween.ValueTo (gameObject, iTween.Hash ("from", RestCardNum, "to", num, "time", Mathf.Abs (RestCardNum - num) * numTweenInterval, "onupdate", "SetRestCardNum"));
	}

	public void PlusRestNum ()
	{
		RestCardNum++;
	}

	void SetRestCardNum (int num)
	{
		RestCardNum = num;
	}

	public Transform GetCardBackPos ()
	{
		return back.transform;
	}

	public void Toggle (bool isShow)
	{
		if (isShow) {
			SurplusText.GetComponent<ScaleTweener> ().Show ();
			back.Show ();
		} else {
			SurplusText.GetComponent<ScaleTweener> ().Hide (null);
			back.Hide (null);
		}
	}
}
