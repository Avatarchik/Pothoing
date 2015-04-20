using UnityEngine;
using System.Collections.Generic;

public class SceneMgr {

    //场景名称
    public const string SC_Logo = "Logo";
    public const string SC_Loading = "Loading";
    public const string SC_Hall = "Lobby";
    public const string SC_Game = "Game";

    private string currScene;

    public void SetScene(string str)
    {
        if (str.Equals(currScene))
            return;
        else
            currScene = str;
            
        switch(str)
        {
            case SC_Game:
                Application.LoadLevel(SC_Game);
                break;
            case SC_Loading:
                Application.LoadLevel(SC_Loading);
                break;
            case SC_Logo:
                Application.LoadLevel(SC_Logo);
                break;
            case SC_Hall:
                Application.LoadLevel(SC_Hall);
                break;
        }
    }
}
