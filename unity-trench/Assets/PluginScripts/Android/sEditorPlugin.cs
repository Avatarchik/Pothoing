using UnityEngine;
using System.Collections;

public class sEditorPlugin : sNativePlugin {

	public sEditorPlugin ()
	{

	}

	override public void CallMethod (string method, params object[] args)
	{
		Debug.Log("Called: " + method + ", with: " + args);
	}
}
