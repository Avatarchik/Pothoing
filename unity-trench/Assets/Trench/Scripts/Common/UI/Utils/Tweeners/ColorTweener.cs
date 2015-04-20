using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ColorTweener : Tweener
{

//	[SerializeField]
//	iTween.EaseType
//		fromEaseType = iTween.EaseType.linear;
//	[SerializeField]
//	iTween.EaseType
//		toEaseType = iTween.EaseType.linear;
//	[SerializeField]
//	float
//		tweenTime = 0.5f;
	[SerializeField]
	Color
		targetColor = Color.clear;
	Color originalColor;
	Action onFinish;

	void Awake ()
	{
		originalColor = GetComponent<Image> ().color;
	}

//	public void ColorTo (Action onComplete)
//	{
//		onFinish = onComplete;
//		iTween.ColorTo (gameObject, iTween.Hash ("color", targetColor,
//		                                         "easetype", toEaseType,
//		                                         "oncomplete", "OnFinish",
//		                                         "time", tweenTime));
//	}

//	public void ColorTo ()
//	{
//		iTween.ColorTo (gameObject, iTween.Hash ("color", targetColor,
//		                                         "easetype", toEaseType,
//		                                         "time", tweenTime));
//	}

	public void ColorFrom ()
	{
		FromTarget ();
	}

	public override void ToTarget (Action onComplete=null, float time=-1f)
	{
		float moveTime = time < 0 ? tweenTime : time;
		onFinish = onComplete;
		iTween.ColorTo (gameObject, iTween.Hash ("color", targetColor,
				                                         "easetype", toEaseType,
				                                         "oncomplete", "OnFinish",
		                                         "time", moveTime));
	}
	
	public override void FromTarget (Action onComplete=null, float time=-1f)
	{
		float moveTime = time < 0 ? tweenTime : time;
		GetComponent<Image> ().color = originalColor;
		iTween.ColorFrom (gameObject, iTween.Hash ("color", targetColor,
		                                           "easetype", fromEaseType,
		                                           "time", moveTime));
	}

	void OnFinish ()
	{
		if (onFinish != null) {
			onFinish ();
		}
	}
}
