using UnityEngine;
using System.Collections;


public abstract class sNativePlugin : ScriptableObject {

	static public readonly string RATE_APPLICATION	= "rateApplication";
	static public readonly string GET_DEVICE_INFO	= "getDeviceInfo";
	static public readonly string GET_IS_ROOTED		= "getIsRooted";
	static public readonly string GET_IS_CHEATING	= "getIsCheating";
	static public readonly string SHOW_TOAST		= "showToast";
	static public readonly string TEST_METHOD		= "testMethod";

	abstract public void CallMethod (string method, params object[] args);

}
