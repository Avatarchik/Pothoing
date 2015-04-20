using UnityEngine;
using System.Collections;

public class VfxPlayer : MonoBehaviour {

	[SerializeField]
	ScaleTweener
		tweener;
	[SerializeField]
	ParticleSystem
		vfx;
	[SerializeField]
	float showTime=2f;

	public void Play(){
		StartCoroutine (DoPlay());
	}

	IEnumerator DoPlay(){
		tweener.Show ();
		vfx.Play ();
		yield return new WaitForSeconds (showTime);
		Hide ();
	}

	void Hide(){
		tweener.Hide (null);
		vfx.Stop ();
	}
}
