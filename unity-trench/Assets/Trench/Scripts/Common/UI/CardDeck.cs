using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CardDeck : MonoBehaviour
{

	[SerializeField]
	Transform
		LeftDeck;
	[SerializeField]
	Transform
		RightDeck;
	[SerializeField]
	Transform
		MyDeck;

    public void ClearAllCards()
    {
        CleanCards(PlayerPanelPosition.LEFT);
        CleanCards(PlayerPanelPosition.RIGHT);
        CleanCards(PlayerPanelPosition.BOTTOM);
    }
	public void CleanCards (PlayerPanelPosition p)
	{
		Transform currTransfrom = MyDeck;
		if (p == PlayerPanelPosition.BOTTOM)
			currTransfrom = MyDeck;
		else if (p == PlayerPanelPosition.LEFT)
			currTransfrom = LeftDeck;
		else if (p == PlayerPanelPosition.RIGHT)
			currTransfrom = RightDeck;
		foreach (Transform child in currTransfrom) {
			Destroy (child.gameObject);
		}
	}

	public void PlayCard (List<Card> Cards, PlayerPanelPosition p)
	{ 
        
		//计算牌位置起点
		if (Cards.Count > 0) {
			//牌实际宽度 牌缩放到0.625;
			float cardWidth = Cards [0].GetComponent<RectTransform> ().rect.width * 0.625f;
			float cardHeight = Cards [0].GetComponent<RectTransform> ().rect.height * 0.625f;
			//牌间距
			float distance = cardWidth * 0.35f;

			float cardDeckWidth = 0;
			//大于10张牌分两行显示
			if (Cards.Count > 10)
				cardDeckWidth = distance * 9 + cardWidth;
			else
				cardDeckWidth = distance * (Cards.Count - 1) + cardWidth;


			//牌堆起始位置
			float begin = 0;
			Transform currTransfrom = MyDeck;
			if (p == PlayerPanelPosition.BOTTOM) {
				begin = -cardDeckWidth / 2 + 0.5f * cardWidth;
				currTransfrom = MyDeck;
			} else if (p == PlayerPanelPosition.LEFT) {
				begin = 0.5f * cardWidth;
				currTransfrom = LeftDeck;
                
			} else if (p == PlayerPanelPosition.RIGHT) {
				begin = -cardDeckWidth + 0.5f * cardWidth;
				currTransfrom = RightDeck;
			}
			Vector3 beginV3 = new Vector3 (0f, 0f, 0f);
            
			//删除所有子对象
			foreach (Transform child in currTransfrom) {
				Destroy (child.gameObject);
			}
			//排列牌堆
			for (int i = 0; i < Cards.Count; i++) {
				beginV3.x = begin;
				Cards [i].transform.SetParent (currTransfrom);
				if (i < 10) {
					beginV3.x += i * distance;
				} else {
					beginV3.x += (i - 10) * distance;
					beginV3.y = -0.5f * cardHeight;
				}
				iTween.MoveTo (Cards [i].gameObject, iTween.Hash ("position", beginV3,
                                                         "islocal", true,
                                                         "easetype", iTween.EaseType.easeOutQuad,
                                                         "time", 0.5f));
				iTween.ScaleTo (Cards [i].gameObject, iTween.Hash ("scale", new Vector3 (0.625f, 0.625f),
				                                         "easetype", iTween.EaseType.easeOutQuad,
                                                         "time", 0.5f));
				SOUND.Instance.OneShotSound (Sfx.inst.outCard);
			}
		}

	}
}
