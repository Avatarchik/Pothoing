using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum RuleTipType{
	None,
	PlayWithoutRule,
	NoBiggerCard
}

public class RuleTip : MonoBehaviour
{
	[SerializeField]
	Image
		label;
	[SerializeField]
	Sprite
		playWithoutRuleTip;
	[SerializeField]
	Sprite
		noBiggerCardTip;
	[SerializeField]
	ScaleTweener
		tweener;

	public void ShowRuleTip (RuleTipType tip)
	{
		if (tip == RuleTipType.PlayWithoutRule) {
			label.sprite = playWithoutRuleTip;
			tweener.Show();
			SOUND.Instance.OneShotSound (Sfx.inst.show);
		} else if (tip == RuleTipType.NoBiggerCard) {
			label.sprite = noBiggerCardTip;
			tweener.Show();
			SOUND.Instance.OneShotSound (Sfx.inst.show);
		} else {
			tweener.Hide(null);
			SOUND.Instance.OneShotSound (Sfx.inst.hide);
		}
	}

//	public void ShowRuleTip (RuleTipType tip,float elipse){
//		StartCoroutine (DoShowTip(tip,elipse));
//	}
//
//	IEnumerator DoShowTip(RuleTipType tip,float elipse){
//		ShowRuleTip (tip);
//		yield return new WaitForSeconds(elipse);
//		ShowRuleTip (RuleTipType.None);
//	}
}
