using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TablePit : MonoBehaviour
{
	[SerializeField]
	Card[]
		pitCards;
	[SerializeField]
	Transform[]
	pitCardsPos;

	[SerializeField]
	MoveTweener tweener;

	Vector3[] originalCardsPos;
	Vector3[] originalCardsLocalScale;

	List<Card> trenchedCards=new List<Card>();

	void Start(){
		originalCardsPos = new Vector3[]{pitCards[0].transform.localPosition,pitCards[1].transform.localPosition,pitCards[2].transform.localPosition,pitCards[3].transform.localPosition};
		originalCardsLocalScale = new Vector3[]{pitCards[0].transform.localScale,pitCards[1].transform.localScale,pitCards[2].transform.localScale,pitCards[3].transform.localScale};
	}

	public void Show(){
		tweener.FromTarget ();
	}

	public void ShowCards (byte[] byteData)
	{
		if (byteData.Length != 4) {
			Debug.LogError ("pit card data error!");
		} else {
			int i = 0;
			foreach (Card card in pitCards) {
				card.gameObject.SetActive(true);
				card.SetCard (byteData [i]);
				card.SetCard(CardType.Back,0);
				i++;
			}
			StartCoroutine(FlipCards(0.8f,0.4f));
		}
	}

	public void HideCards(){
		foreach (Card card in pitCards) {
            card.SetCard(CardType.Back, 1);
			card.gameObject.SetActive(false);
		}
	}

	IEnumerator FlipCards(float flipTime,float interval){
		int i = 0;
		foreach (Card card in pitCards) {
			iTween.MoveTo(card.gameObject,iTween.Hash("position", pitCardsPos[i].position,"time",interval,"easetype",iTween.EaseType.easeOutQuad));
			iTween.ScaleTo(card.gameObject,iTween.Hash("scale",pitCardsPos[i].localScale,"time",interval,"easetype",iTween.EaseType.easeOutQuad));
			card.ChangeNumScale(false);
			yield return new WaitForSeconds(0.1f);
			i++;
		}
		yield return new WaitForSeconds(0.1f);
		foreach (Card card in pitCards) {
			card.FlipForward(flipTime);
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(1f);
		i = 0;
		trenchedCards.Clear ();
		foreach (Card card in pitCards) {
			Card copiedCard=Instantiate(card,card.transform.position,card.transform.rotation) as Card;
			copiedCard.transform.SetParent(transform);
			trenchedCards.Add(copiedCard);
			iTween.MoveTo(card.gameObject,iTween.Hash("position", originalCardsPos[i],"time",interval,"islocal",true,"easetype",iTween.EaseType.easeOutQuad));
			iTween.ScaleTo(card.gameObject,iTween.Hash("scale",originalCardsLocalScale[i],"time",interval,"easetype",iTween.EaseType.easeOutQuad));
			card.ChangeNumScale(true);
			i++;
		}
	}

	public List<Card> GetTrenchedCards(){
		return trenchedCards;
	}

//	public void TestShowCard(){
//		ShowCards (new byte[]{0x01,0x02,0x03,0x04});
//	}


}
