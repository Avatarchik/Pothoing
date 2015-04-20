using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 牌堆组件
/// 功能描述：用于在玩家发牌时的动画表示
/// </summary>
public class CardStack : MonoBehaviour
{
	public delegate void CompleteCallBack ();

	public CompleteCallBack     callback;       //完成发牌后回调接口
	public Card                 mCard;          //单张牌Perfab
	[Tooltip("0:last,1:self ,2:next")]
	public PlayerPanel[]
		playerPanels;//玩家坐标位置
	public Transform[]          pitCardPositions;//pit坐标位置
	public float                mTotoalTime = 27f;    //整个过程时间

	//-----------------------------------------

	List<Card>                  mAllCard;
	int                         mCardIndex;
	int                         playerPositionIndex;
	int                         pitPositionIndex = 0;
	float                       singleCardTime = 0.5f;
	float                       dispatchInterval = 0.066f;
	float                       cardScaleParam = 0.175f;
	CardStackState              mStackState;

	enum CardStackState
	{
		STATE_INIT,
		STATE_READY,
		STATE_RUN,
		STATE_DONE
	}

	/// <summary>
	/// 初始化未分配的牌
	/// </summary>
	public void InitCardList ()
	{

		pitPositionIndex = 0;    

		mAllCard = new List<Card> ();
		if (mStackState != CardStackState.STATE_INIT) {
			return;
		}

		Logger.getLogger.Log ("initCardList");

		//--------------------------------

		const int cartSize = 52;
		for (int i = 0; i < cartSize; i++) {
			Card card = Instantiate (mCard, new Vector3 (0, 0, -i * 0.1f), Quaternion.identity) as Card;
			card.SetCard (CardType.Back, i + 1);

			card.transform.SetParent (this.transform);
			mAllCard.Add (card);
		}

		//------------------------        
		
		singleCardTime = mTotoalTime / (float)cartSize;
		mStackState = CardStackState.STATE_READY;
	}
    
	/// <summary>
	/// 发牌
	/// </summary>
	public void Run ()
	{

		if (mStackState != CardStackState.STATE_READY) {
			return;
		}
		
		mStackState = CardStackState.STATE_RUN;
		StartCoroutine (DispacthCard ());
	}

	public void DispacthCardsTo (List<Card> cards, int player)
	{
		StartCoroutine (DoDispacthCardsTo (cards, player));
	}

	IEnumerator DoDispacthCardsTo (List<Card> cards, int player)
	{
		playerPositionIndex = player;
		Vector3 tEndPostion = playerPanels [player].GetRestCardStackPos ().position;
		//Debug.Log ("DoDispacthCardsTo"+cards.Count);
		foreach (Card theCard in cards) {
			//Debug.Log ("DoDispacthCardsTo"+theCard.type+player);
			//theCard.transform.SetParent (this.transform);
			iTween.MoveTo (theCard.gameObject, iTween.Hash ("position", tEndPostion,
		                                                              "islocal", false,
		                                                              "easetype", iTween.EaseType.easeOutQuad,
		                                                              "time", singleCardTime));
		
			iTween.ScaleTo (theCard.gameObject, iTween.Hash ("scale", theCard.transform.localScale * cardScaleParam,
		                                                               "islocal", true,
		                                                               "easetype", iTween.EaseType.easeOutQuad,
		                                                               "time", singleCardTime));
			iTween.RotateAdd (theCard.gameObject, iTween.Hash ("amount", Vector3.forward * 360f,
		                                                                 "islocal", true,
		                                                                 "easetype", iTween.EaseType.easeOutQuad,
		                                                                 "time", singleCardTime));
			SOUND.Instance.OneShotSound (Sfx.inst.sendCard);
			yield return new WaitForSeconds (dispatchInterval);
			OnPlayerGetCard ();
		}
		yield return new WaitForSeconds (1f);
		foreach (Card ob in cards) {            
			ob.transform.SetParent (null);
			Destroy (ob.gameObject);
		}
		cards.Clear ();
	}
    
	/// <summary>
	/// 发牌核心逻辑
	/// </summary>
	IEnumerator DispacthCard ()
	{
		int cardsCount = mAllCard.Count;
		//Debug.Log ("Dispacth Cards!" + singleCardTime);
		while (cardsCount>0 && mStackState==CardStackState.STATE_RUN) {
			if (cardsCount <= 4) {
				Vector3 tEndPostion = pitCardPositions [pitPositionIndex].position;
				iTween.MoveTo (mAllCard [mCardIndex].gameObject, iTween.Hash ("position", tEndPostion,
				                                                              "islocal", false,
				                                                              "easetype", iTween.EaseType.easeOutQuad,
				                                                              "time", singleCardTime));
				iTween.ScaleTo (mAllCard [mCardIndex].gameObject, iTween.Hash ("scale", pitCardPositions [pitPositionIndex].localScale,
				                                                               "islocal", true,
				                                                               "easetype", iTween.EaseType.easeOutQuad,
				                                                               "time", singleCardTime));
				iTween.RotateAdd (mAllCard [mCardIndex].gameObject, iTween.Hash ("amount", Vector3.forward * 360f,
				                                                                "islocal", true,
								                                                "easetype", iTween.EaseType.easeOutQuad,
				                                                                "time", singleCardTime));
				SOUND.Instance.OneShotSound (Sfx.inst.sendCard);
				pitPositionIndex++;
			} else {
				if (playerPositionIndex != 1) {
					Vector3 tEndPostion = playerPanels [playerPositionIndex].GetRestCardStackPos ().position;//GetOffSetPosition(playerCardPositions[mPositionIndex].position);

					iTween.MoveTo (mAllCard [mCardIndex].gameObject, iTween.Hash ("position", tEndPostion,
		                                                "islocal", false,
					                                      "easetype", iTween.EaseType.easeOutQuad,
		                                                  "time", singleCardTime));

					iTween.ScaleTo (mAllCard [mCardIndex].gameObject, iTween.Hash ("scale", mAllCard [mCardIndex].transform.localScale * cardScaleParam,
			                                                   "islocal", true,
					                                            "easetype", iTween.EaseType.easeOutQuad,
			                                                   "time", singleCardTime));
					iTween.RotateAdd (mAllCard [mCardIndex].gameObject, iTween.Hash ("amount", Vector3.forward * 360f,
					                                                               "islocal", true,
										                                           "easetype", iTween.EaseType.easeOutQuad,
					                                                               "time", singleCardTime));
					SOUND.Instance.OneShotSound (Sfx.inst.sendCard);
				} else {
					mAllCard [mCardIndex].gameObject.SetActive (false);
				}
			}
			mCardIndex++;
			playerPositionIndex++;
			if (playerPositionIndex >= playerPanels.Length) {
				playerPositionIndex = 0;
			}
			yield return new WaitForSeconds (dispatchInterval);
			cardsCount--;
			if (cardsCount >= 4) {
				OnPlayerGetCard ();
			}
		}
		yield return new WaitForSeconds (singleCardTime);
		OnDispatchComplete ();
	}

	void OnDispatchComplete ()
	{
		foreach (Transform pitCard in pitCardPositions) {
			pitCard.gameObject.SetActive (true);
		}
		mStackState = CardStackState.STATE_DONE;
		RemoveChild ();
		if (callback != null) {
			callback ();
		}
	}

	void OnPlayerGetCard ()
	{
		//print ("OnPlayerGetCard");
		playerPanels [playerPositionIndex].PlusRestCardNum ();
	}

	private void RemoveChild ()
	{
		mCardIndex = 0;
		playerPositionIndex = 0;
		mStackState = CardStackState.STATE_INIT;    

		foreach (Card ob in mAllCard) {            
			ob.transform.SetParent (null);
			Destroy (ob.gameObject);
		}

		mAllCard.Clear ();
	}    
}
