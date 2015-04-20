using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngineEx.LogInterface;

class SelectedCard
{
	public Card card;
	public Vector3 oldV3;

}

public enum CardControlLevel
{
	Locked,
	CanDragButCannotPlay,
	CanPlay
}

public class HandCardMgr : MonoBehaviour
{
	public List<Card> HandCards = new List<Card> ();
	public Card card;
	[SerializeField]
	Transform
		dragCardsParent;
	[SerializeField]
	Transform
		playedCardsPos;
	[SerializeField]
	RuleTip
		tip;
	[SerializeField]
	CardDeck
		deck;
	float dragHeightParam = 0.4f;
	List<Card> selectedCards = new List<Card> (); //选中的牌
	List<Card> readyCards = new List<Card> ();
	List<Card> copiedCards = new List<Card> ();
	float maxDistance = 56f; //最大牌距
	float minDistance = 20f; //最小牌距

	private float distance = 0; //实际牌距

	CardControlLevel lockControlLevel = CardControlLevel.Locked;
	Vector3 defaultDragCardsParentPos;
	float cardWidth = 0;
	bool isDragging = false;
	bool isDragStarted = false;
	float sortTime = 0.5f;
	float sortInterval = 0.2f;
	Action OnACardBeSelected;

	bool IsDragging {
		get {
			return isDragging;
		}
		set {
			if (isDragging != value) {
				isDragging = value;
				if (isDragging) {
					OnStartDragging ();
				} else {
					OnExitDragging ();
				}
			}
		}
	}

	bool IsDragStarted {
		get {
			return isDragStarted;
		}
		set {
			if (isDragStarted != value) {
				isDragStarted = value;
				if (isDragStarted) {
					OnDragStart ();
				} else {
					OnDragEnd ();
				}
			}
		}
	}

	void Start ()
	{
		defaultDragCardsParentPos = dragCardsParent.localPosition;
		cardWidth = card.GetComponent<RectTransform> ().rect.width;
	}

	public void Run (byte[] data)
	{
		StartCoroutine (AddHandCard (data));
	}

	public void InitOnSelected (Action onSelected)
	{
		OnACardBeSelected = onSelected;
	}

	IEnumerator AddHandCard (byte[] data)
	{
		for (int i = 0; i < data.Length; i++) {
			Card obj = Instantiate (card) as Card;
			obj.SetCard (data [i]);
			HandCards.Add (obj);
			SortHands (this.GetComponent<RectTransform> ().rect.width, transform, HandCards);
			yield return new WaitForSeconds (sortInterval);
		}
		yield return 0;
		InitCards ();

	}

	//清除手牌
	public void ClearHandCards ()
	{
		foreach (Card obj in HandCards) {
			Destroy (obj.gameObject);
		}

		HandCards.Clear ();
		selectedCards.Clear ();
		readyCards.Clear ();
		copiedCards.Clear ();
		//sequencedCards.Clear();
	}

	//清除打出的牌
	public void ClearOutCards ()
	{
		deck.ClearAllCards ();
	}

	//清除单个玩家的出牌显示
	public void ClearOutCards (PlayerPanelPosition p)
	{
		deck.CleanCards (p);
	}
	//处理手牌炸弹显示
	void showHandCardsBomb (List<Card> cards)
	{
		int cardNum = 1;
		for (int i = 0; i < cards.Count; i++) {
			Card obj = cards [i];
			if (obj.IsShowingBomb ())
				obj.ToggleBombMark (false);
			if (i != 0) {
				if (obj.num == cards [i - 1].num)
					cardNum++;
				else
					cardNum = 1;
				if (cardNum == 4) {
					for (int n = 3; n >= 0; n--) {
						cards [i - n].ToggleBombMark (true);
					}
					SOUND.Instance.OneShotSound (Sfx.inst.show);
				}
			}
		}
        

	}

	public void SequenceHands (byte[] data)
	{
		List<Card> sequencedCards = new List<Card> ();
		ToggleCards (CardControlLevel.Locked);
		foreach (byte byteData in data) {
			foreach (Card theCard in HandCards) {
				if (byteData == theCard.GetCardByteData ()) {
					sequencedCards.Add (theCard);
					break;
				}
			}
		}
		HandCards = sequencedCards;
		SortHands (this.GetComponent<RectTransform> ().rect.width, transform, HandCards);
		InitCards ();
	}

	public void SortHands (float width, Transform parent, List<Card> cards)
	{
		//计算牌距
		if (cards.Count > 0) {
			if (cards.Count > 1) {
				distance = (width - cardWidth) / (float)(cards.Count + 1);
				//Debug.Log(distance);
			}
			distance = Mathf.Clamp (distance, minDistance, maxDistance);
			float handsWidth = (cards.Count - 1) * distance;

			//牌位置
			float firstPoint = -handsWidth * 0.5f;
			for (int i = 0; i < cards.Count; i++) {
				Card obj = cards [i];
                
				obj.transform.SetParent (parent);
				//obj.transform.localPosition = new Vector3 ((firstPoint + i * distance), 0, 0);
				if (i != 0) {
					obj.transform.SetSiblingIndex (cards [i - 1].transform.GetSiblingIndex () + 1);
				}
				iTween.MoveTo (obj.gameObject, iTween.Hash ("position", new Vector3 ((firstPoint + i * distance), 0, 0),
				                                            "islocal", true,
				                                            "easetype", iTween.EaseType.easeOutQuad,
				                                            "time", sortTime));
			}
		}
	}

	void InitCards ()
	{
		for (int i = 0; i < HandCards.Count; i++) {
			Card obj = HandCards [i];
			obj.ActivateTrigger (false);
			obj.InitOnSelectedAction (OnCardSelected);
			//obj.InitUnSelectedAction (OnCardSelected);
			obj.InitOnTouchUpAction (OnTouchUp);
		}
		lockControlLevel = CardControlLevel.CanDragButCannotPlay;

        switch(GameHelper.Instance.GetStage())
        {
            case GameHelper.STAGE_BOM:
            case GameHelper.STAGE_GATE:
		        showHandCardsBomb (HandCards);
                break;         
        }
	}

	public void ToggleCards (CardControlLevel controlLevel)
	{
		lockControlLevel = controlLevel;
		bool isActive = lockControlLevel != CardControlLevel.Locked ? true : false;
		for (int i = 0; i < HandCards.Count; i++) {
			Card obj = HandCards [i];
			obj.ActivateTrigger (isActive);
		}
	}

	void OnCardSelected (Card selected)
	{
		if (selectedCards.Contains (selected)) {
//			selectedCards.Remove (selected);
//			selected.ShowMask (false);
			selectedCards [selectedCards.Count - 1].ShowMask (false);
			selectedCards.Remove (selectedCards [selectedCards.Count - 1]);
		} else {
			selectedCards.Add (selected);
			selected.ShowMask (true);
			OnACardBeSelected ();
		}
		SOUND.Instance.OneShotSound (Sfx.inst.chooseCard);
	}

	void OnTouchUp ()
	{
		byte[] removableCardData = new byte[0];
		List<byte> removableCardDataList = new List<byte> ();
		removableCardDataList.AddRange (removableCardData);
		foreach (Card cardObj in selectedCards) {
			Vector3 oriPos = new Vector3 (cardObj.transform.localPosition.x, 0, cardObj.transform.localPosition.z);
			cardObj.ShowMask (false);
			if (readyCards.Contains (cardObj) && !removableCardDataList.Contains (cardObj.GetCardByteData ())) {
				//card.SetAlpha(0.5f);
				iTween.MoveTo (cardObj.gameObject, iTween.Hash ("position", oriPos,
						                                 "islocal", true,
				                                         "easetype", iTween.EaseType.easeOutQuad,
						                                 "time", 0.1f));
				readyCards.Remove (cardObj);

			} else {
				iTween.MoveTo (cardObj.gameObject, iTween.Hash ("position", oriPos + Vector3.up * 20,
				                                             "islocal", true,
				                                             "easetype", iTween.EaseType.easeOutQuad,
				                                             "time", 0.1f));
				readyCards.Add (cardObj);
				//card.SetAlpha(0.5f);
			}
		}
		MainGame.inst.OnSelectCard (GetCardsByteData (selectedCards), ref removableCardData);
		selectedCards.Clear ();
		OnACardBeSelected ();
	}

	void Update ()
	{
		if (lockControlLevel != CardControlLevel.Locked && (selectedCards.Count > 0 || readyCards.Count > 0)) {
			if (IsPointerOnAnySelectedCard () || IsPointerOnAnyReadyCard ()) {
				IsDragStarted = true;
			}
#if UNITY_EDITOR
			if (Input.GetMouseButton (0)) {
#else
			if (Input.touchCount > 0) {
#endif
				if (IsDragStarted && Input.mousePosition.y > Screen.height * dragHeightParam) {
					IsDragging = true;
					dragCardsParent.position = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				} else {
					IsDragging = false;
				}
			} else {
				IsDragging = false;
				IsDragStarted = false;
			}
		} else {
			IsDragging = false;
			IsDragStarted = false;
		}
	}

	bool IsPointerOnAnyReadyCard ()
	{
		foreach (Card readyCard in readyCards) {
			if (readyCard.IsPointerOn ()) {
				return true;
			}
		}
		return false;
	}

	bool IsPointerOnAnySelectedCard ()
	{
		if (selectedCards.Count > 0) {
			foreach (Card selectedCard in selectedCards) {
				if (selectedCard.IsPointerOn ()) {
					return true;
				}
			}
		}
		return false;
	}

	public void AddCardsToReadyCards (List<byte> data)
	{
		//readyCards.Clear ();

		foreach (Card cardObj in HandCards) {
			Vector3 oriPos = new Vector3 (cardObj.transform.localPosition.x, 0, cardObj.transform.localPosition.z);
			if (data.Contains (cardObj.GetCardByteData ())) {
				if (!readyCards.Contains (cardObj)) {
					readyCards.Add (cardObj);
				}
				iTween.MoveTo (cardObj.gameObject, iTween.Hash ("position", oriPos + Vector3.up * 20,
					                                                "islocal", true,
					                                                "easetype", iTween.EaseType.easeOutQuad,
					                                                "time", 0.1f));
			} else {
				if (readyCards.Contains (cardObj)) {
					readyCards.Remove (cardObj);
				}
				iTween.MoveTo (cardObj.gameObject, iTween.Hash ("position", oriPos,
						                                                "islocal", true,
						                                                "easetype", iTween.EaseType.easeOutQuad,
						                                                "time", 0.1f));

			}
		}
	}

	public Card[] GetReadyCards ()
	{
		return readyCards.ToArray ();
	}

	public byte[] GetReadyCardsData ()
	{
		return GetCardsByteData (readyCards);
	}

	byte[] GetCardsByteData (List<Card> cards)
	{
		byte[] byteData = new byte[cards.Count];
		int i = 0;
		foreach (Card theCard in cards) {
			byteData [i] = theCard.GetCardByteData ();
			i++;
		}
		return byteData;
	}

	void OnStartDragging ()
	{
		if (copiedCards.Count > 0) {
			foreach (Card copyCard in copiedCards) {
				if (copyCard.gameObject) {
					Destroy (copyCard.gameObject);
				}
			}
		}
		copiedCards.Clear ();
		foreach (Card cardObj in selectedCards) {
			Vector3 oriPos = new Vector3 (cardObj.transform.localPosition.x, 0, cardObj.transform.localPosition.z);
			if (readyCards.Contains (cardObj)) {
//				readyCards.Remove (cardObj);
			} else {
				readyCards.Add (cardObj);
				iTween.MoveTo (cardObj.gameObject, iTween.Hash ("position", oriPos + Vector3.up * 20,
					                                                "islocal", true,
					                                                "easetype", iTween.EaseType.easeOutQuad,
					                                                "time", 0.1f));
			}
		}
		foreach (Card cardObj in HandCards) {
			cardObj.ActivateTrigger (false);
			cardObj.ShowMask (false);
			if (readyCards.Contains (cardObj)) {//for sequence//|| selectedCards.Contains (cardObj)
				cardObj.SetAlpha (0.5f);
				Card copyCard = Instantiate (card, dragCardsParent.position, cardObj.transform.rotation) as Card;
				copyCard.SetCard (cardObj.GetCardByteData ());
				copiedCards.Add (copyCard);
				copyCard.transform.localScale *= 0.5f;
				copyCard.transform.SetParent (dragCardsParent);
			}
		}
		SortHands (copiedCards.Count * cardWidth * 0.5f, dragCardsParent, copiedCards);
		selectedCards.Clear ();
		OnACardBeSelected ();
	}

	void OnExitDragging ()
	{
		if (Input.mousePosition.y <= Screen.height * dragHeightParam || lockControlLevel != CardControlLevel.CanPlay) {
			OnPlaySelfCardFail ();
		} else {
			OnACardBeSelected ();
			MainGame.inst.OnPlayCard (GetCardsByteData (readyCards));
		}
	}

	//float selectStartX;
	void OnDragStart ()
	{
		//selectStartX = Input.mousePosition.x;
	}

	void OnDragEnd ()
	{
		foreach (Card cardObj in HandCards) {
			cardObj.ActivateTrigger (true);
		}
	}

	public void PlayCard (List<Card> cards, PlayerPanelPosition player)
	{
		deck.PlayCard (cards, player);
	}

	public void OnPlaySelfCardSuccess (byte[] data)//bool isDrag
	{
		if (copiedCards.Count == 0) {
			foreach (Card cardObj in HandCards) {
				if (readyCards.Contains (cardObj)) {//for sequence//|| selectedCards.Contains (cardObj)
					cardObj.ActivateTrigger (false);
					cardObj.ShowMask (false);
					cardObj.SetAlpha (1f);
					Card copyCard = Instantiate (card, cardObj.transform.position, cardObj.transform.rotation) as Card;
					copyCard.SetCard (cardObj.GetCardByteData ());
					copiedCards.Add (copyCard);
					copyCard.transform.localScale *= 0.5f;
					copyCard.transform.SetParent (dragCardsParent);
				}
			}
		} else {
			List<Card> refCopiedCards = new List<Card> ();
			refCopiedCards.AddRange (copiedCards.ToArray ());
			foreach (Card copiedCardObj in refCopiedCards) {
				bool isPlayedCard = false;
				foreach (byte byteData in data) {
					if (copiedCardObj.GetCardByteData () == byteData) {
						isPlayedCard = true;
						break;
					}
				}
				if (!isPlayedCard) {
					Destroy (copiedCardObj.gameObject);
					copiedCards.Remove (copiedCardObj);
				}
			}
		}
		deck.PlayCard (copiedCards, PlayerPanelPosition.BOTTOM);
		foreach (Card cardObj in readyCards) {
			HandCards.Remove (cardObj);
			Destroy (cardObj.gameObject);
		}
		readyCards.Clear ();
		foreach (Card restCard in HandCards) {
			restCard.ShowMask (false);
		}
		SortHands (this.GetComponent<RectTransform> ().rect.width, transform, HandCards);
		ToggleCards (CardControlLevel.CanDragButCannotPlay);
		copiedCards.Clear ();
		showHandCardsBomb (HandCards);
		//clear copiedCards
	}

	public void OnPlaySelfCardFail ()
	{
		dragCardsParent.localPosition = defaultDragCardsParentPos;
		foreach (Card cardObj in HandCards) {
			cardObj.SetAlpha (1f);
		}
		foreach (Card copyCard in copiedCards) {
			Destroy (copyCard.gameObject);
		}
		copiedCards.Clear ();

		foreach (Card cardObj in HandCards) {
			if (readyCards.Contains (cardObj)) {
				Vector3 oriPos = new Vector3 (cardObj.transform.localPosition.x, 0, cardObj.transform.localPosition.z);
				cardObj.ShowMask (false);
				readyCards.Remove (cardObj);
				iTween.MoveTo (cardObj.gameObject, iTween.Hash ("position", oriPos,
			                                                "islocal", true,
			                                                "easetype", iTween.EaseType.easeOutQuad,
			                                                "time", 0.1f));
			}
		}
		selectedCards.Clear ();
		OnACardBeSelected ();
	}

	public void RewriteCardData (Card card, byte data)
	{
		card.SetCard (data);
		if (readyCards.Contains (card)) {
			readyCards.Remove (card);
		}
		if (selectedCards.Contains (card)) {
			selectedCards.Remove (card);
		}
	}

	public void RewriteCardsData (Card[] cards, byte[] data)
	{
		for (int i=0; i<cards.Length; i++) {
			RewriteCardData (cards [i], data [i]);
		}
	}
	
	public void ShowTip (RuleTipType tipType, float elipse=2f)
	{
		StartCoroutine (DoShowTip (tipType, elipse));
	}
	
	IEnumerator DoShowTip (RuleTipType tipType, float elipse=2f)
	{
		foreach (Card cardObj in HandCards) {
			cardObj.ActivateTrigger (false);
			cardObj.ShowMask (true);
		}
		tip.gameObject.SetActive (true);
		tip.ShowRuleTip (tipType);
		yield return new WaitForSeconds (elipse);
		tip.ShowRuleTip (RuleTipType.None);
		foreach (Card cardObj in HandCards) {
			cardObj.ActivateTrigger (true);
			cardObj.ShowMask (false);
		}
		
	}

	List<Card> GetCardsbByByteData (byte[] data, List<Card> cards)
	{
		List<Card> gotCards = new List<Card> ();
		List<byte> dataList = new List<byte> ();
		dataList.AddRange (data);
		foreach (Card theCard in cards) {
			if (dataList.Contains (theCard.GetCardByteData ())) {
				gotCards.Add (theCard);
			}
		}
		return gotCards;
	}

	void MarkAsBomb (List<Card> cards, bool isShowBomb)
	{
		if (cards.Count > 0) {
			foreach (Card bombCard in cards) {
				bombCard.ToggleBombMark (isShowBomb);
			}
		}
	}

	public void MarkAsBomb (byte[] data)
	{
		if (data.Length >= 4 && HandCards.Count >= data.Length) {
			List<byte> dataList = new List<byte> ();
			dataList.AddRange (data);
			foreach (Card bombCard in HandCards) {
				if (dataList.Contains (bombCard.GetCardByteData ())) {
					bombCard.ToggleBombMark (true);
				} else if (bombCard.IsShowingBomb ()) {
					bombCard.ToggleBombMark (false);
				}
			}
			SOUND.Instance.OneShotSound (Sfx.inst.show);
		} else {
			foreach (Card bombCard in HandCards) {
				if (bombCard.IsShowingBomb ()) {
					bombCard.ToggleBombMark (false);
				}
			}
			//SOUND.Instance.OneShotSound (Sfx.inst.outCard);
		}
	}
}
