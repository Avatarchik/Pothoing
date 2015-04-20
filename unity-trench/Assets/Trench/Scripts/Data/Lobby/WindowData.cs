using UnityEngine;
using System.Collections;

public class WindowData : MonoBehaviour
{

	public LobbyState LobbyWin = LobbyState.MainMenu; //大厅默认打开界面
	public uint StageId;        //打开关卡Id

	public void Init ()
	{
		LobbyWin = LobbyState.MainMenu;
	}
}
