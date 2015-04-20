using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tip : MonoBehaviour
{
	[SerializeField]
	Tweener
		tweener;
	[SerializeField]
	Text
		label;
	float time=3;

	public void Show (string content)
	{
		StartCoroutine (DoShow (content));
	}

	IEnumerator DoShow (string content)
	{
		label.text = content;
		//tweener.gameObject.SetActive (true);
		tweener.FromTarget ();
		yield return new WaitForSeconds (time);
		tweener.ToTarget ();
	}
}

