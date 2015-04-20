using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PageManager : MonoBehaviour
{

	[SerializeField]
	GameObject[]
		pages;
	[SerializeField]
	Button[]
		pageBtns;

	public void SwitchPage (int pageIndex)
	{
		for (int i=0; i<pages.Length; i++) {
			pages [i].SetActive (false);
			pageBtns [i].interactable = true;
		}
		pages [pageIndex].SetActive (true);
		pageBtns [pageIndex].interactable = false;
		SOUND.Instance.OneShotSound (Sfx.inst.btn);
	}
}
