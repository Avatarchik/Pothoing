using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class DeviceManager : SingleClass<DeviceManager>
{

    //private static readonly string CONTAINER_NAME = "DeviceManager";

    //private	static sDeviceManager mInstance = null;
    //public	static sDeviceManager Instance {
    //    get {
    //        if(mInstance == null ) {
    //            mInstance = GameObject.FindObjectOfType(typeof(sDeviceManager)) as sDeviceManager;
    //            if (!mInstance) {
    //                GameObject container = null;
    //                if ((container = GameObject.Find(CONTAINER_NAME)) == null)
    //                {
    //                    container = new GameObject();
    //                    container.name = CONTAINER_NAME;
    //                }
    //                mInstance = container.AddComponent(typeof(sDeviceManager)) as sDeviceManager;
    //                DontDestroyOnLoad(mInstance);
    //            }
    //        }
    //        return mInstance;
    //    }
    //}
	
	#region : Public methods =====================================================================

	public	RuntimePlatform	Platform	{private set; get;}
	private	sNativePlugin mNativePlugin = null;
	public void Init() {

		Platform = Application.platform;

		if(mNativePlugin == null) {

			#if UNITY_EDITOR
			// editor
			mNativePlugin = ScriptableObject.CreateInstance<sEditorPlugin>();
			#elif UNITY_ANDROID
			// android
			mNativePlugin = ScriptableObject.CreateInstance<sAndroidPlugin>();
			#elif UNITY_IPHONE
			// iphone
			mNativePlugin = ScriptableObject.CreateInstance<sEditorPlugin>();
			#endif

		//	if (Platform == RuntimePlatform.WindowsEditor ||
		//	    Platform == RuntimePlatform.OSXEditor) 
		//	{
		//		// editor
		//		mNativePlugin = ScriptableObject.CreateInstance<sEditorPlugin>();
		//	} else if (Platform == RuntimePlatform.Android) 
		//	{
		//		// android
		//		mNativePlugin = ScriptableObject.CreateInstance<sAndroidPlugin>();
		//	} else if (Platform == RuntimePlatform.IPhonePlayer) 
		//	{
		//		// iphone
		//		mNativePlugin = null;
		//	}
		}	
		ShowToast(this.ToString() + ", Init(), value: " + Application.platform.ToString());
	}

	#endregion : Public methods ==================================================================

	private enum INFO
	{
		IS_ROOTED,
		DEVICE_ID,
		PHONE_NUMBER,
		VENDOR,
		LOCALE,
		LANGUAGE,
		OS_VERSION,
		MODEL,
		MAKER,
		PROCESSOR,
		RESISTRATION_ID,
        MEMTOTAL,
        MEMAVALID,

		COUNT_MAX
	}

	private string[]	m_info			= new string[(int)INFO.COUNT_MAX];
	public	bool		isRooted		{get{return (isRooted_str == "false") ? false : true;}}
	public	string		isRooted_str	{get{return m_info[(int)INFO.IS_ROOTED];}}
    public string       deviceId        { get { if (Application.platform == RuntimePlatform.WindowsEditor)return getDeviceID(); else return m_info[(int)INFO.DEVICE_ID]; } }
	public	string		phoneNumber		{get{return m_info[(int)INFO.PHONE_NUMBER];}}
	public	string		vendor			{get{return m_info[(int)INFO.VENDOR];}}
	public	string		locale			{get{return m_info[(int)INFO.LOCALE];}}
	public	string		language		{get{return m_info[(int)INFO.LANGUAGE];}}
	public	string		OSVersion		{get{return m_info[(int)INFO.OS_VERSION];}}
	public	string		model			{get{return SystemInfo.deviceModel;}}	//{get{return m_info[(int)INFO.MODEL];}}
	public	string		maker			{get{return m_info[(int)INFO.MAKER];}}
	public	string		processor		{get{return m_info[(int)INFO.PROCESSOR];}}
	public	string		resistrationID	{get{return m_info[(int)INFO.RESISTRATION_ID];}}
    public  string      memtotal        {get{return m_info[(int)INFO.MEMTOTAL];} }
    public  string      memavalid       {get{ return m_info[(int)INFO.MEMAVALID]; } }

	public event Action onSetDeviceInfo = null;
	private void SetDeviceInfo (string infoPack)
	{
		string[] received	= infoPack.Split(',');
		for (int i = 0 ; i < received.Length ; i++)
		{
			m_info[i] = received[i];
		}

		if (onSetDeviceInfo != null) {onSetDeviceInfo();}
		onSetDeviceInfo = null;

		ShowToast(this.ToString() + ", SetDeviceInfo(), value: " + infoPack);

		ShowToast("LEEHJ DEVICEID" + m_info[(int)INFO.DEVICE_ID]);

		ShowToast("LEEHJ RESISTRATIONID" + m_info[(int)INFO.RESISTRATION_ID]);

	}

	public void GetDeviceInfo ()
	{
		mNativePlugin.CallMethod(sNativePlugin.GET_DEVICE_INFO);
	}

	public event Action onSetIsRooted = null;
	private void SetIsRooted (string isRooted) 
	{
		m_info[(int)INFO.IS_ROOTED] = isRooted;

		if (onSetIsRooted != null) {onSetIsRooted();}
		onSetIsRooted = null;
		
		ShowToast(this.ToString() + ", SetIsRooted(), value: " + isRooted);
	}

	public void GetIsRooted () 
	{
		mNativePlugin.CallMethod(sNativePlugin.GET_IS_ROOTED);
	}

	public event Action onSetIsCheating = null;
	public	string	ToolName		{private set; get;}
	public	bool	IsCheating		{private set; get;}	
	private void SetIsCheating (string toolName)
	{
		IsCheating	= (toolName != "false");
		ToolName	= toolName;

		if (onSetIsCheating != null) {onSetIsCheating();}
		onSetIsCheating = null;
		
		ShowToast(this.ToString() + ", SetIsCheating(), value: " + toolName);
	}

	public void GetIsCheating ()
	{
		mNativePlugin.CallMethod(sNativePlugin.GET_IS_CHEATING);
	}

	public bool IsToastOn	= false;
	public void ShowToast (string msg) 
	{
		if (IsToastOn == false) return;
		#if UNITY_EDITOR
		Debug.Log("Toast: " + msg);
		#elif UNITY_ANDROID
		mNativePlugin.CallMethod(sNativePlugin.SHOW_TOAST, msg);
		#elif UNITY_IPHONE

		#endif
	}

    public void ShowToast(string msg,bool isOn)
    {
        if (isOn == false) 
            return;
#if UNITY_EDITOR
        Debug.Log("Toast: " + msg);
#elif UNITY_ANDROID
		mNativePlugin.CallMethod(sNativePlugin.SHOW_TOAST, msg);
#elif UNITY_IPHONE

#endif
    }

	public void TestMethod ()
	{
		mNativePlugin.CallMethod(sNativePlugin.TEST_METHOD);
	}

	public void RateApplication ()
	{
		mNativePlugin.CallMethod(sNativePlugin.RATE_APPLICATION);
	}

    string getDeviceID()
    {
        string macAddress = "";
#if UNITY_EDITOR
        System.Net.NetworkInformation.NetworkInterface[] ins = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
        foreach (System.Net.NetworkInformation.NetworkInterface adapter in ins)
        {
            if (!adapter.GetPhysicalAddress().ToString().Equals(""))
            {
                macAddress = adapter.GetPhysicalAddress().ToString();
                for (int i = 1; i < 6; i++)
                {
                    macAddress = macAddress.Insert(3 * i - 1, ":");
                }
                break;
            }
        }
#elif UNITY_ANDROID
		//TODO ???
        //macAddress = sDeviceManager.Instance.deviceId;
#elif UNITY_IOS

#endif
        Debug.Log("MAC address:" + macAddress);
        return macAddress;
    }
}
