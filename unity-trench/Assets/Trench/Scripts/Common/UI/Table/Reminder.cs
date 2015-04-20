using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Reminder : MonoBehaviour
{
	public static Reminder inst;
	[SerializeField]
	Text[]
		cardNums;
	[SerializeField]
	MoveTweener
		tweener;
	string cards = "32AKQJT987654";
	Dictionary<char,int> numDict = new Dictionary<char, int> ();
	bool isHidden = false;

	void Awake ()
	{
		inst = this;
		Init ();
	}

	public void Init ()
	{
		numDict.Clear ();
		foreach (char charecter in cards) {
			numDict.Add (charecter, 4);
		}
		Refresh ();
		tweener.ToTarget (null, 0f);
		//gameObject.SetActive (false);
		isHidden = true;
	}

	public void Decrease (byte[] cards)
	{
		foreach (byte card in cards) {
			Decrease (Card.GetTranslatedDataNum (Card.TranslateData (card)));
		}
	}

	public void Decrease (byte card, int num=1)
	{
		Decrease (Card.GetTranslatedDataNum (Card.TranslateData (card)), num);
	}

	public void Decrease (int card, int num=1)
	{
		char key = 'J';
		switch (card) {
		case 0:
			Debug.Log ("Joker!?");
			break;
		case 1:
			key = 'A';
			break;
		case 10:
			key = 'T';
			break;
		case 11:
			key = 'J';
			break;
		case 12:
			key = 'Q';
			break;
		case 13:
			key = 'K';
			break;
		default:
			key = card.ToString () [0];
			break;
		}
		Decrease (key, num);
	}

	void Decrease (char card, int num=1)
	{
		numDict [card] -= num;
		numDict [card] = Mathf.Clamp (numDict [card], 0, 4);
		Refresh ();
	}

	void Refresh ()
	{
		int i = 0;
		foreach (Text label in cardNums) {
			if (label.text != numDict [cards [i]].ToString ()) {
				label.text = numDict [cards [i]].ToString ();
				label.SendMessage("Show");
			}
			i++;
		}
	}

	public void Toggle ()
	{
		if (isHidden) {
			tweener.FromTarget ();
			isHidden = false;
		} else {
			tweener.ToTarget ();
			isHidden = true;
		}
	}

	public void Toggle (bool isShow)
	{
		if (isShow) {
			tweener.FromTarget ();
			isHidden = false;
		} else {
			tweener.ToTarget ();
			isHidden = true;
		}
	}
}
