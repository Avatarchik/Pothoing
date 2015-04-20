using UnityEngine;
using System.Collections;
using UnityEngineEx.CMD.i3778;

public class LobbyData : MonoBehaviour
{
	public UserData user;
	public SettingData setting;
	public GameData arena;
	public ShopData shop;
	public SignData sign;
	public WindowData navigation;
	Player player;

	public void Init ()
	{
		user.Init ();
		setting.Init ();
		arena.Init ();
		shop.Init ();
		sign.Init ();
		//navigation.Init ();
	}
	
}
