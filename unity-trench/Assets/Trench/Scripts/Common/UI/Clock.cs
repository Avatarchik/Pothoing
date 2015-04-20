using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public enum ClockPosition
{
	Identify,
	Play,
	NextPlayer,
	LastPlayer
}

public class Clock : MonoBehaviour
{
	[SerializeField]
	Image
		image;
	[SerializeField]
	Text
		timeLabel;
	[SerializeField]
	Animator
		animtor;
	[Tooltip("0:Identify,1:Play,2:NextPlayer,3:LastPlayer")]
	[SerializeField]
	Transform[]
		positions;
//	[SerializeField]
//	audio clockWarningSfx;
	[SerializeField]
	ParticleSystem 
		goneVfx;
	int timer;

	IEnumerator DoCountDown (int from, int warningTime, Action callback)
	{
//		image.color = Color.white;
//		timeLabel.color = Color.black;
		timer = from;
		while (timer > 0) {
			timer--;
			timeLabel.text = (timer + 1).ToString ();
			if (timer <= warningTime) {
				timeLabel.gameObject.SendMessage ("Show");
//			}
//			if (timer <= warningTime) {
				animtor.Play ("ClockJump");
				//audio.Play();
				SOUND.Instance.OneShotSound (Sfx.inst.clock);
			}
			yield return new WaitForSeconds (1f);
		}
		if (callback != null) {
			callback ();
		}
		yield return new WaitForSeconds (0.1f);
		image.color = Color.clear;
		timeLabel.color = Color.clear;
		goneVfx.Play ();
	}

	public void CountDown (int from, int warningTime, Action timeUpCallBack)
	{
		StartCoroutine (DoCountDown (from, warningTime, timeUpCallBack));
	}

	public void StopCountDown ()
	{
		//StopCoroutine ("DoCountDown");
		//timer = 0;
		StopAllCoroutines ();
		if (image.color != Color.clear) {
			goneVfx.Play ();
			image.color = Color.clear;
			timeLabel.color = Color.clear;
		}

	}

	public void TestCountDown ()
	{
		CountDown (10, 5, delegate() {
			Debug.Log ("Time Up!");
		});
	}

	public void SetClockPosition (ClockPosition pos)
	{
		SetClockPosition (pos,0.5f);
	}

	public void SetClockPosition (ClockPosition pos,float time)
	{
		goneVfx.Play ();
		SOUND.Instance.OneShotSound (Sfx.inst.show);
		image.color = Color.white;
		timeLabel.color = Color.black;
		if (time > 0) {
			iTween.MoveTo (gameObject, positions [(int)pos].position, time);
		} else {
			transform.position=positions[(int)pos].position;
		}
	}

}
