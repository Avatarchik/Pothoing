using UnityEngine;
using System;
using System.Collections;

public class MoveTweener : Tweener
{

//	[SerializeField]
//	iTween.EaseType
//		fromEaseType = iTween.EaseType.easeInQuad;
//	[SerializeField]
//	iTween.EaseType
//		toEaseType = iTween.EaseType.easeOutQuad;
//	[SerializeField]
//	float
//		tweenTime = 0.5f;
	[SerializeField]
	Vector3
		targetPos = Vector3.zero;
	Vector3 originalPos;
	Action onFromCompleted;
	Action onToCompleted;

	void Awake ()
	{
		originalPos = transform.transform.localPosition;
		//Debug.Log (originalPos);
	}

	public override void ToTarget (Action onComplete=null, float time=-1f)
	{
		float moveTime = time < 0 ? tweenTime : time;
		if (onComplete != null) {
			onToCompleted = onComplete;
		}
		iTween.MoveTo (gameObject, iTween.Hash ("position", transform.position + targetPos,
		                                          "islocal", false,
		                                        "oncomplete", "OnToComplete",
		                                        "easetype", toEaseType,
		                                        "time", moveTime));
	}

	public override void FromTarget (Action onComplete=null, float time=-1f)
	{
		float moveTime = time < 0 ? tweenTime : time;
		onFromCompleted = onComplete;
		transform.localPosition = originalPos;
		iTween.MoveFrom (gameObject, iTween.Hash ("position", transform.position + targetPos,
		                                           "islocal", false,
		                                          "oncomplete", "OnFromComplete",
		                                           "easetype", fromEaseType,
		                                          "time", moveTime));
	}

	public void MoveFrom ()
	{
		transform.localPosition = originalPos;
		iTween.MoveFrom (gameObject, iTween.Hash ("position", transform.position + targetPos,
		                                          "islocal", false,
		                                          "oncomplete", "OnFromComplete",
		                                          "easetype", fromEaseType,
		                                          "time", tweenTime));
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
