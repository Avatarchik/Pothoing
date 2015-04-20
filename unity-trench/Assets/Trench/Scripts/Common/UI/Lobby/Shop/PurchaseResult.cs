using UnityEngine;
using System.Collections;

public class PurchaseResult : MonoBehaviour
{
	[SerializeField]
	Shop
		shop;

	void OnPurchaseSuccess (string pid)
	{
		LobbyDialogManager.inst.ShowConfirmDialog (TextManager.Get ("purchaseSuccessTip"), null, null, false);
		GoodData data = shop.GetGoodData (pid);
		if (data != null) {
			shop.OnBuyDiamond (data);
		} else {
			Debug.Log ("pid is wrong!");
			OnPurchaseFail (pid);
		}
	}

	void OnPurchaseFail (string pid)
	{
		LobbyDialogManager.inst.ShowConfirmDialog (TextManager.Get ("purchaseFailTip"), null, null, false);
	}

	void OnPurchaseCancel (string pid)
	{
		LobbyDialogManager.inst.ShowConfirmDialog (TextManager.Get ("purchaseCancelTip"), null, null, false);
	}
}
