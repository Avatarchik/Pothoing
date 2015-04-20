using UnityEngine;
using System;
using System.Collections;

public class ScaleTweener : Tweener
{

//	[SerializeField]
//	iTween.EaseType fromEaseType=iTween.EaseType.easeOutElastic;
//	[SerializeField]
//	iTween.EaseType toEaseType=iTween.EaseType.easeInElastic;
	[SerializeField]
	Vector3 targetScale=Vector3.zero;
	Vector3 originalScale;
	Action onFromCompleted;
	Action onToCompleted;

	void Awake ()
	{
		originalScale = transform.localScale;
	}

	public void Hide (Action onHidden)
	{
		onToCompleted = onHidden;
		ToTarget ();
	}

	public void Show ()
	{
		FromTarget ();
	}

	public override void ToTarget (Action onComplete=null, float time=-1f)
	{
		float moveTime = time < 0 ? tweenTime : time;
		onToCompleted = onComplete;
		iTween.ScaleTo (gameObject, iTween.Hash ("scale", targetScale,
		                                         "islocal", true,
		                                         "easetype", toEaseType,
		                                         "oncomplete", "OnToComplete",
		                                         "time", moveTime));
	}
	
	public override void FromTarget (Action onComplete=null, float time=-1f)
	{
		float moveTime = time < 0 ? tweenTime : time;
		onFromCompleted = onComplete;
		transform.localScale = originalScale;
		iTween.ScaleFrom (gameObject, iTween.Hash ("scale", targetScale,
		                                           "islocal", true,
		                                           "easetype", fromEaseType,
		                                           "oncomplete", "OnFromComplete",
		                                           "time", moveTime));
		//Debug.Log (fromEaseType);
	}

	void OnFromComplete ()
	{
		if (onFromCompleted != null) {
			onFromCompleted ();
		}
	}

	void OnToComplete ()
	{
		if (onToCompleted != null) {
			onToCompleted ();
		}
	}
}
