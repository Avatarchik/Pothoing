using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SlotMachine : MonoBehaviour
{

	[SerializeField]
	Scrollbar
		scroller;
	double scrollSpeed = 0.01d;
	bool isSpinning = false;
	bool isSlowing = false;

	public void Spin ()
	{
		StartCoroutine (DoSpin ());
	}

	IEnumerator DoSpin ()
	{
		isSpinning = true;
		isSlowing = false;
		while (isSpinning) {
			scroller.value += (float)scrollSpeed;
			yield return 0;
			if (scroller.value >= 1) {
				scroller.value = 0;
			}
			if (scrollSpeed > 0) {
				if (!isSlowing) {
					scrollSpeed += 0.0005d;
					if (scrollSpeed >= 0.05) {
						isSlowing = true;
					}
				} else {
					scrollSpeed -= 0.0005d;
				}
			} else {
				isSpinning = false;
			}
		}
	}
}
