using UnityEngine;
using System;
using System.Collections;

public class Tweener : MonoBehaviour {

	[SerializeField]
	protected iTween.EaseType
		fromEaseType = iTween.EaseType.easeOutElastic;
	[SerializeField]
	protected iTween.EaseType
		toEaseType = iTween.EaseType.easeInBack;
	[SerializeField]
	protected float
		tweenTime = 0.5f;

	public virtual void ToTarget (Action onComplete=null,float time=-1f)
	{
	}
	
	public virtual void FromTarget (Action onComplete=null,float time=-1f)
	{
	}
}
