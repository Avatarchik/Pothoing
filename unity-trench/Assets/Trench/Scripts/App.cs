using UnityEngine;
using UnityEngineEx.Net;

//using System;
//using System.Runtime.InteropServices;
//using System.Text;

/// <summary>
/// 全局游戏启动入口
/// 请将此脚本挂载于游戏启动时的对象
/// </summary>

//public struct aaaa { }
//[Serializable]
//[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
//public class tagUserDataTest : aaaa
//{
//    //用户属性
//    public ushort wFaceID;							//头像索引
//    public ushort wUserType;						//用户类型
//    public uint dwUserID;							//用户 I D
//    public uint dwGroupID;							//社团索引
//    public uint dwGameID;							//用户 I D
//    public uint dwUserRight;						//用户等级
//    public uint dwMasterRight;						//管理权限
//    public uint dwAreaType;						//地区 I D
//    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
//    public byte[] szName;					        //用户名字
//    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
//    public byte[] szGroupName;				        //社团名字

//}
public class App : MonoBehaviour
{
	[SerializeField]
	UnityEngine.UI.Text
		txtVer;
	[SerializeField]
	WindowData
		navigationData;
	/// <summary>
	/// 
	/// </summary>
	DataBase DataCenter;             //数据中心
	DeviceManager DeviceManager;    //设备信息
	Timer mTimer;
    
	void Awake ()
	{
		DeviceManager = DeviceManager.Instance;
		mTimer = Timer.Instance;
		PlayerPrefs.SetInt ("DebugMode", 1);
		navigationData.Init ();
	}

	void Start ()
	{
		//tagUserDataTest sss = new tagUserDataTest();
        
		//sss.szName = new sbyte[32];
		//sss.szGroupName = new sbyte[32];

		//string sname = Encoding.ASCII.GetString(sss.szName);

		//string sjson = sss.ToJSON();
		//tagUserDataTest user = sjson.FromJSON<tagUserDataTest>();
		//Debug.Log(user.ToString());

		//PlayerData ssss = new PlayerData();
		//ssss.mName = "1111";
		//ssss.DeviceID = null;
		//ssss.cbGender = (byte)1;

		//string ssssss = ssss.ToJSON();

		//Debug.Log(ssssss);
		//Debug.Log(ssss.ToJSON());
		//Debug.Log(DeviceManager.ToJSON());
		DataCenter = DataBase.Instance;


		txtVer.text = COMMON_CONST.Version;
		SOUND.Instance.PlayBGM (Bgm.inst.logo, false);
	}
}
