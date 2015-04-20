using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UserHud : MonoBehaviour
{
	public static UserHud inst;
 	[SerializeField]
	UserData
		data;
	[SerializeField]
	Image
		head;
	[SerializeField]
	Text
		userName;
	[SerializeField]
	Text
		coin;
	[SerializeField]
	Text
		diamond;

	void Awake(){
		inst = this;
	}

	public void Init ()
	{
		head.sprite = data.head;
		userName.text = data.uiName;
		coin.text = data.coin.ToString ();
		diamond.text = data.diamond.ToString ();
	}

	public void RefreshMoney ()
	{
		data.RefreshMoney ();
		coin.text = data.coin.ToString ();
		diamond.text = data.diamond.ToString ();
	}
}
