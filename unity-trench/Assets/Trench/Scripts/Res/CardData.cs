using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardData : MonoBehaviour
{
	[SerializeField]
	CardType[]
		cardTypes;
	[SerializeField]
	Sprite[]
		cardTypeMarks;
	[Tooltip("0:joker,13:K")]
	[SerializeField]
	Sprite[]
		cardRedNums;
	[SerializeField]
	Sprite[]
		cardBlackNums;
	[SerializeField]
	Sprite
		cardFront;
	[SerializeField]
	Sprite
		cardBack;

	public Sprite GetMark (CardType type)
	{
		if (type != CardType.None && type != CardType.Back) {
			return cardTypeMarks [(int)type - 1];
		} else {
			return null;
		}
	}

	public Sprite GetNum (int num, bool isRed)
	{
		if (num < cardRedNums.Length && num >= 0) {
			if (isRed) {
				return cardRedNums [num];
			} else {
				return cardBlackNums [num];
			}
		} else {
			return null;
		}
	}

	public Sprite GetCardBackground (bool isFront)
	{
		if (isFront) {
			return cardFront;
		} else {
			return cardBack;
		}
	}
}
