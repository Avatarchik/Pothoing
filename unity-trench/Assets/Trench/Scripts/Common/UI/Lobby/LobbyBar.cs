using UnityEngine;
using System.Collections;

public class LobbyBar : MonoBehaviour
{
	[SerializeField]
	MoveTweener[]
		btns;
	[SerializeField]
	float
		tweenTime = 1f;

	public void HideBtns ()
	{
		StartCoroutine (DoHideBtns ());
	}

	IEnumerator DoHideBtns ()
	{
		foreach (MoveTweener btn in btns) {
			if (btn.gameObject.activeInHierarchy) {
				btn.ToTarget ();
				yield return new WaitForSeconds ((float)tweenTime / (float)btns.Length);
			}
		}
		yield return 0;
	}

	public void ShowBtns(){
		StartCoroutine (DoShowBtns ());
	}

	IEnumerator DoShowBtns ()
	{
		foreach (MoveTweener btn in btns) {
			if (btn.gameObject.activeInHierarchy) {
				btn.FromTarget();
				yield return new WaitForSeconds ((float)tweenTime / (float)btns.Length);
			}
		}
		yield return 0;
	}
}
