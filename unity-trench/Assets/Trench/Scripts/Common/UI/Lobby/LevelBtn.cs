using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelBtn : MonoBehaviour
{
	[SerializeField]
	Image[]
		stars;
	[SerializeField]
	Sprite
		star;
	[SerializeField]
	Sprite
		grayStar;

	public void SetStar (int starNum)
	{
		for(int i=0;i<starNum;i++){
			stars[i].sprite=star;
			stars[i].gameObject.SendMessage("Show");
		}
	}
}
