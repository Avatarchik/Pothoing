using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using UnityEngineEx.LogInterface;

public enum CardType
{
	None,
	Spade,          //♠
	Heart,          //♥
	Diamond,        //♦
	Club,           //♣
	RedJoker,
	BlackJoker,
	Back
}

public class Card : MonoBehaviour
{
	[ContextMenuItem ("Refresh It","Refresh")]
	public CardType
		type;
	[Header("0:Joker 1:A 13:K")]
	public int
		num;
	[SerializeField]
	CardData
		data;
	[SerializeField]
	Image
		typeMarkJoker;
	[SerializeField]
	Image
		typeMarkBig;
	[SerializeField]
	Image
		typeMarkSmall;
	[SerializeField]
	Image
		typeNum;
	[SerializeField]
	Image
		jokerLabel;
	[SerializeField]
	Image
		background;
	[SerializeField]
	Text
		debugText;
	[SerializeField]
	Image
		mask;
	[SerializeField]
	ScaleTweener
		bomb;
	[SerializeField]
	Button
		btn;
	CardType cacheType;//for resume
	byte cacheNum = 255;
	Action<Card> onSelect;
	//Action<Card> unSelect;
	Action onTouchUp;
	bool isPointerOn = false;

	void Start ()
	{
		Refresh ();
	}
	
	void Refresh ()
	{
		if (typeMarkJoker) {
			typeMarkJoker.gameObject.SetActive (true);
		}
		typeMarkSmall.gameObject.SetActive (true);
		typeMarkBig.gameObject.SetActive (true);
		jokerLabel.gameObject.SetActive (true);
		typeNum.gameObject.SetActive (true);
		bomb.gameObject.SetActive (false);
		mask.color = Color.clear;
		mask.gameObject.SetActive (false);
		btn.gameObject.SetActive (false);
		background.sprite = data.GetCardBackground (true);
		typeMarkBig.sprite = data.GetMark (type);
		typeMarkSmall.sprite = data.GetMark (type);
		typeMarkJoker.sprite = data.GetMark (type);
		debugText.text = type.ToString () + num;
		switch (type) {
		case CardType.Spade:
		case CardType.Club:
			typeNum.sprite = data.GetNum (num, false);
			jokerLabel.gameObject.SetActive (false);
			typeMarkJoker.gameObject.SetActive (false);
			break;
		case CardType.Heart:
		case CardType.Diamond:
			typeNum.sprite = data.GetNum (num, true);
			jokerLabel.gameObject.SetActive (false);
			typeMarkJoker.gameObject.SetActive (false);
			break;
		case CardType.RedJoker:
			typeNum.gameObject.SetActive (false);
			typeMarkBig.gameObject.SetActive (false);
			typeMarkSmall.gameObject.SetActive (false);
			jokerLabel.sprite = data.GetNum (0, true);
			break;
		case CardType.BlackJoker:
			typeNum.gameObject.SetActive (false);
			typeMarkBig.gameObject.SetActive (false);
			typeMarkSmall.gameObject.SetActive (false);
			jokerLabel.sprite = data.GetNum (0, false);
			break;
		case CardType.Back:
			jokerLabel.gameObject.SetActive (false);
			typeNum.gameObject.SetActive (false);
			typeMarkSmall.gameObject.SetActive (false);
			typeMarkBig.gameObject.SetActive (false);
			typeMarkJoker.gameObject.SetActive (false);
			background.sprite = data.GetCardBackground (false);
			break;
		default:
			Debug.Log ("wrong type");
			break;
		}
	}

	public void SetCard (CardType cardType, int cardNum)
	{
		cardNum = Mathf.Clamp (cardNum, 1, 13);
		if (cardType == CardType.Back) {
			cacheType = type;
			cacheNum = (byte)num;
		}
		type = cardType;
		num = cardNum;
		Refresh ();
	}

	public static string TranslateData (byte byteData)
	{
		CardType tCardtype;
		
		byte cardtype = (byte)(byteData & 0xf0);
		
		switch (cardtype) {
		case 0x00:
			tCardtype = CardType.Diamond;
			break;
		case 0x10:
			tCardtype = CardType.Club;
			break;
		case 0x20:
			tCardtype = CardType.Heart;
			break;
		case 0x30:
			tCardtype = CardType.Spade;
			break;
		default:
			Debuger.Instance.LogError ("类型错误");
			return null;
		}
		
		byte cardscore = (byte)(byteData & 0x0f);
		
		if (cardscore > 13 || cardscore < 1) {
			Debuger.Instance.LogError ("分值错误" + cardscore);
			return null;
		}

		return tCardtype + "#" + cardscore;
	}

	public static int GetTranslatedDataNum (string translatedData)
	{
		return int.Parse (translatedData.Split ('#') [1]);
	}

	public static CardType GetTranslatedDataCardType (string translatedData)
	{
		return  (CardType)Enum.Parse (typeof(CardType), translatedData.Split ('#') [0]);
	}

	public void SetCard (byte byteData)
	{
		CardType tCardtype;

		byte cardtype = (byte)(byteData & 0xf0);

		switch (cardtype) {
		case 0x00:
			tCardtype = CardType.Diamond;
			break;
		case 0x10:
			tCardtype = CardType.Club;
			break;
		case 0x20:
			tCardtype = CardType.Heart;
			break;
		case 0x30:
			tCardtype = CardType.Spade;
			break;
		default:
			Debuger.Instance.LogError ("类型错误");
			return;
		}

		byte cardscore = (byte)(byteData & 0x0f);

		if (cardscore > 13 || cardscore < 1) {
			Debuger.Instance.LogError ("分值错误" + cardscore);
			return;
		}

		SetCard (tCardtype, cardscore);
	}

	/// <summary>
	/// 返回 card 对应的 byte数据
	/// </summary>
	/// <returns> byte 数据 </returns>
	public byte GetCardByteData ()
	{
		byte tByteResult = 0;

		switch (type) {
		case CardType.Diamond:
			tByteResult = 0x00;
			break;
		case CardType.Club:
			tByteResult = 0x10;
			break;
		case CardType.Heart:
			tByteResult = 0x20;
			break;
		case CardType.Spade:
			tByteResult = 0x30;
			break;
		}

		tByteResult |= (byte)(cacheNum == 255 ? (byte)num : cacheNum);

		//Debug.Log (tByteResult+" "+(tByteResult & 0x0f));

		return tByteResult;

	}

	const float defaultFlipTime = 0.5f;

	public void FlipBackward (float flipTime=defaultFlipTime)
	{
		//print ("Flip Backward!");
		transform.eulerAngles = new Vector3 (transform.eulerAngles.x, 0, transform.eulerAngles.z);
		iTween.RotateTo (gameObject, iTween.Hash ("rotation", transform.eulerAngles + Vector3.up * 180f, "time", flipTime, "easetype", iTween.EaseType.easeOutQuad));
		StartCoroutine (JudgeCardBackward ());
		SOUND.Instance.OneShotSound (Sfx.inst.chooseCard);
	}

	public void FlipForward (float flipTime=defaultFlipTime)
	{
		//print ("Flip Forward!");
		transform.eulerAngles = new Vector3 (transform.eulerAngles.x, 180f, transform.eulerAngles.z);
		iTween.RotateTo (gameObject, iTween.Hash ("rotation", transform.eulerAngles - Vector3.up * 180f, "time", flipTime, "easetype", iTween.EaseType.easeOutQuad));
		StartCoroutine (JudgeCardForward ());
		SOUND.Instance.OneShotSound (Sfx.inst.chooseCard);
	}

	IEnumerator JudgeCardBackward ()
	{
		while (transform.eulerAngles.y < 90) {
			yield return 0;
		}
		SetCard (CardType.Back, 0);
		yield return 0;
	}

	IEnumerator JudgeCardForward ()
	{
		while (transform.eulerAngles.y > 90) {
			yield return 0;
		}
		SetCard (cacheType, cacheNum);
		yield return 0;
	}

	public bool IsPointerOn ()
	{
		return isPointerOn;
	}

	public void ToggleBombMark (bool isShow)
	{
		if (isShow) {
			bomb.gameObject.SetActive (true);
			bomb.Show ();
		} else {
			bomb.Hide (delegate() {
				bomb.gameObject.SetActive (false);
			});
		}

	}

	public void ActivateTrigger (bool isActive)
	{
		btn.gameObject.SetActive (isActive);
	}

	public void InitOnSelectedAction (Action<Card> function)
	{
		onSelect = function;
	}

	public void InitOnTouchUpAction (Action function)
	{
		onTouchUp = function;
	}

	public void ToggleMask ()
	{
		if (mask.gameObject.activeInHierarchy) {
			ShowMask (false);
		} else {
			ShowMask (true);
		}
	}

	public void OnEnterCard ()
	{
#if UNITY_EDITOR
		if (Input.GetMouseButton(0)) {
			OnSelected ();
		}
#else
		OnSelected ();
#endif
		isPointerOn = true;

	}

	public void OnExitCard ()
	{
		isPointerOn = false;

	}
	
	public void OnPointerDown ()
	{
#if UNITY_EDITOR
		OnSelected ();
		//OnTouchUp ();
#endif
	}
	
	void OnSelected ()
	{
		if (onSelect != null) {
			onSelect (this);
		}
	}

	public void OnTouchUp ()
	{
		if (onTouchUp != null) {
			onTouchUp ();
		}
	}

	float defaultShowMaskTime = 0.1f;

	public void ShowMask (bool isShow)
	{
		if (isShow) {
			mask.gameObject.SetActive (true);
			iTween.ColorTo (mask.gameObject, iTween.Hash ("color", new Color (0, 0, 0, 0.5f), "time", defaultShowMaskTime));//"oncomplete",""
			//print("showMask");
		} else {
			iTween.ColorTo (mask.gameObject, iTween.Hash ("color", new Color (0, 0, 0, 0.0f), "time", defaultShowMaskTime));//,"oncomplete","close"
			//print("hideMask");
		}
	}

	public void SetAlpha (float alpha)
	{
		if (typeMarkJoker.gameObject.activeInHierarchy) {
			SetAlpha (typeMarkJoker, alpha);
		}
		if (typeMarkBig.gameObject.activeInHierarchy) {
			SetAlpha (typeMarkBig, alpha);
		}
		if (typeMarkSmall.gameObject.activeInHierarchy) {
			SetAlpha (typeMarkSmall, alpha);
		}
		if (typeNum.gameObject.activeInHierarchy) {
			SetAlpha (typeNum, alpha);
		}
		if (jokerLabel.gameObject.activeInHierarchy) {
			SetAlpha (jokerLabel, alpha);
		}
		if (background.gameObject.activeInHierarchy) {
			SetAlpha (background, alpha);
		}
		if (bomb.gameObject.activeInHierarchy) {
			SetAlpha (bomb.GetComponent<Image> (), alpha);
		}
	}

	void SetAlpha (Image image, float alpha)
	{
		iTween.ColorTo (image.gameObject, iTween.Hash ("color", new Color (image.color.r, image.color.g, image.color.b, alpha), "time", defaultShowMaskTime));
	}

	public bool IsShowingBomb ()
	{
		return bomb.gameObject.activeInHierarchy;
	}

	public void ChangeNumScale (bool isScaleToBigNum)
	{
		Animator anim = GetComponent<Animator> ();
		if (isScaleToBigNum) {
			anim.CrossFade ("CardChange", 0.1f);
		} else {
			anim.CrossFade ("CardChangeBack", 0.1f);
		}
	}
}
