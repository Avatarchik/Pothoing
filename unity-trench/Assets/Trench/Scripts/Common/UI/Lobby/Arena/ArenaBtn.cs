using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ArenaBtn : MonoBehaviour
{
	[SerializeField]
	Text
		roomName;
	[SerializeField]
	Button
		btn;
	INFO_ROOM info;
	Action<INFO_ROOM,ArenaBtn> onBtnClick;

	public void Init (INFO_ROOM roomInfo, Action<INFO_ROOM,ArenaBtn> onBtn)
	{
		info = roomInfo;
		roomName.text = roomInfo.szServerName;
		onBtnClick = onBtn;
	}

	public void OnBtnClick ()
	{
		if (onBtnClick != null) {
			onBtnClick (info, this);
		}
	}

	public void ToggleButton (bool isActive)
	{
		btn.interactable = isActive;
	}

	public INFO_ROOM GetRoomData ()
	{
		return info;
	}
}
