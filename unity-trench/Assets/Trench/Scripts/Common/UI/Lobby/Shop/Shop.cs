using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Shop : MonoBehaviour
{
	[SerializeField]
	GoodItem
		goodItem;
	[Header("0:Diamond,1:Coin,2:Item")]
	[SerializeField]
	Transform[]
		parents;
	[SerializeField]
	ShopData
		datas;
	[SerializeField]
	UserData
		user;
	List<GoodItem> diamondGoods = new List<GoodItem> ();
	List<GoodItem> coinGoods = new List<GoodItem> ();
	List<GoodItem> itemGoods = new List<GoodItem> ();
	LobbyDialogManager dialogs;
	[SerializeField]
	PageManager
		pages;

	void Start ()
	{
		dialogs = LobbyDialogManager.inst;
	}

	public void Init ()
	{
		//Debug.Log (datas.goodData.Count);
		if (coinGoods.Count > 0) {
			foreach (GoodItem item in coinGoods) {
				Destroy (item.gameObject);
			}
			coinGoods.Clear ();
		}
		if (diamondGoods.Count > 0) {
			foreach (GoodItem item in diamondGoods) {
				Destroy (item.gameObject);
			}
			diamondGoods.Clear ();
		}
		if (itemGoods.Count > 0) {
			foreach (GoodItem item in itemGoods) {
				Destroy (item.gameObject);
			}
			itemGoods.Clear ();
		}
		foreach (GoodData data in datas.goodData) {
			GoodItem item = Instantiate (goodItem)as GoodItem;
			item.Init (data, OnPurchaseBtn);
			if (data.type == GoodType.Coin) {
				coinGoods.Add (item);
				item.transform.SetParent (parents [1]);
			} else if (data.type == GoodType.Diamond) {
				diamondGoods.Add (item);
				item.transform.SetParent (parents [0]);
			} else {
				itemGoods.Add (item);
				item.transform.SetParent (parents [2]);
			}
		}
		//Debug.Log ("SHOP INITED!");
	}

	void OnPurchaseBtn (GoodData good)
	{
		if (good.priceType == PriceType.Coin) {
			if (user.coin < good.price) {
				dialogs.ShowConfirmDialog (TextManager.Get ("lowCoinTip"), ShowCoinPage);
			} else {
				OnBuyItem (good);
			}
		} else if (good.priceType == PriceType.Diamond) {
			if (user.diamond < good.price) {
				dialogs.ShowConfirmDialog (TextManager.Get ("lowDiamondTip"), ShowDiamondPage);
			} else {
				OnBuyCoin (good);
			}
		} else {
			if (PlayerPrefs.GetInt ("DebugMode", 0) != 1) {
				PluginManager.Purchase (good.pid);
			} else {
				Camera.main.gameObject.SendMessage ("OnPurchaseSuccess", good.pid);
			}
		}
		SOUND.Instance.OneShotSound (Sfx.inst.btn);
	}

	public void ShowDiamondPage ()
	{
		pages.SwitchPage (0);
	}

	public void ShowCoinPage ()
	{
		pages.SwitchPage (1);
	}

	public void ShowItemPage ()
	{
		pages.SwitchPage (2);
	}

	public void OnBuyItem (GoodData good)
	{
		user.coin -= (int)good.price;
		//TODO add item
		user.SaveMoney ();
		UserHud.inst.RefreshMoney ();
		SOUND.Instance.OneShotSound (Sfx.inst.buy);
	}

	public void OnBuyCoin (GoodData good)
	{
		user.diamond -= (int)good.price;
		user.coin += good.plusNum;
		user.SaveMoney ();
		UserHud.inst.RefreshMoney ();
		SOUND.Instance.OneShotSound (Sfx.inst.reward);
	}

	public void OnBuyDiamond (GoodData good)
	{
		user.diamond += good.plusNum;
		user.SaveMoney ();
		UserHud.inst.RefreshMoney ();
		SOUND.Instance.OneShotSound (Sfx.inst.buy);
	}

	public GoodData GetGoodData (string pid)
	{
		foreach (GoodData data in datas.goodData) {
			if (data.pid == pid) {
				return data;
			}
		}
		return null;
	}

//	public void TestInit(){
//		GoodData data = new GoodData ();
//		data.goodName="Good";
//		data.type = GoodType.Coin;
//		data.desc="good info";
//		data.price="$100";
//		data.isOnSale = true;
//		Init (new GoodData[8]{data,data,data,data,data,data,data,data});
//	}
}
