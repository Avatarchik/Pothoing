using UnityEngine;
using System.Collections;


public class sAndroidPlugin : sNativePlugin {

    private static readonly string ANDROID_DEVICEINFORMER = "com.Gamezz.toolbox.DeviceInformer";

	#if UNITY_ANDROID
	private AndroidJavaObject m_activity = null;
	#endif

	public sAndroidPlugin ()
	{
		#if UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass(ANDROID_DEVICEINFORMER);
		m_activity = jc.CallStatic<AndroidJavaObject>("getInstance");
		#endif
	}

	override public void CallMethod (string method, params object[] args)
	{
		#if UNITY_ANDROID
		if (m_activity != null)
		{
			m_activity.Call(method, args);
		}
		else
		{
			DeviceManager manager = GameObject.FindObjectOfType(typeof(DeviceManager)) as DeviceManager;
			manager.SendMessage("OnReceive", "getting activity failed");
		}
		#endif
	}
}
