using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GoodItem : MonoBehaviour
{
	[SerializeField]
	Text
		nameLabel;
	[SerializeField]
	Image
		icon;
	[SerializeField]
	Text
		desc;
	[SerializeField]
	Text
		price;
	[SerializeField]
	Button
		btn;
	GoodData data;
	Action<GoodData> onPurchaseBtn;

	public void Init (GoodData goodData, Action<GoodData> onBtn)
	{
		data = goodData;
		nameLabel.text = data.goodName;
		onPurchaseBtn = onBtn;
		if (icon != null) {
			icon.sprite = data.icon;
		}
		desc.text = data.desc;
		price.text = data.price.ToString ();
		//btn.onClick.AddListener (OnPurchaseBtn);
		ToggleGood (data.isOnSale);
	}

	public void ToggleGood (bool isActive)
	{
		btn.interactable = isActive;
	}

	public void OnPurchaseBtn ()
	{
		onPurchaseBtn (data);
	}

	public GoodData GetData ()
	{
		return data;
	}

//	public void ToggleBtn(bool isActive){
//		btn.interactable = isActive;
//	}
}
