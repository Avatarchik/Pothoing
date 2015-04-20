using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResultTip : MonoBehaviour
{
	[SerializeField]
	Image
		label;
	[SerializeField]
	Animator
		anim;
	[SerializeField]
	Sprite
		winTip;
	[SerializeField]
	Sprite
		loseTip;
	[SerializeField]
	ScaleTweener
		tweener;
//	[SerializeField]
//	ParticleSystem winFirework;
//	[SerializeField]
//	ParticleSystem loseVfx;
	public void SetResultTip (bool isWin)
	{
		if (isWin) {
			label.sprite = winTip;
		} else {
			label.sprite = loseTip;
		}
	}

	public void ShowResult(bool isWin){
		tweener.Show ();
		if (isWin) {
			//winFirework.Play();
			anim.Play("WinLabel");
			SOUND.Instance.OneShotSound (Sfx.inst.win);
		} else {
			//loseVfx.Play();
			//anim.Play("Idle");
			anim.Play("LoseLabel");
			SOUND.Instance.OneShotSound (Sfx.inst.hitTable);
		}
	}

	public void Hide(System.Action action){
//		winFirework.Stop ();
//		loseVfx.Stop ();
		tweener.Hide (action);
	}
}
