package com.Gamezz.toolbox;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.util.List;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.ActivityManager;
import android.app.ActivityManager.MemoryInfo;
import android.content.Context;
import android.content.Intent;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.Build;
import android.os.Handler;
import android.os.Looper;
import android.telephony.TelephonyManager;
import android.text.format.Formatter;
import android.util.Log;
import android.widget.Toast;

import com.unity3d.player.UnityPlayer;

public class DeviceInformer {
	private static final String TAG = "DeviceInformer";
	private static final String	RECEIVE_OBJECT_NAME	= "DeviceManager";
	
	private static final String	SET_DEVICE_INFO = "SetDeviceInfo";
	private static final String SET_IS_ROOTED	= "SetIsRooted";
	private static final String SET_IS_CHEATING	= "SetIsCheating";

	private static final int BAD_DEVICE_ID_LENGTH = 1;
	private static final String EMPTY_STRING = "";
	private static final String EMPTY_DEVICE_ID = "-1";
	private static final String EMPTY_PHONE_NUMBER = "-1";
	private static final String BAD_DEVICE_ID_PATTERN = "000000000000000";
	
	private static final String DELIMITER	= ",";
	
	private Context		m_context		= null;
	private Activity	mMainActivity	= null;
	
	public static DeviceInformer getInstance() {
		
		return DeviceInformerHolder.instance;
	}
	
	private static final class DeviceInformerHolder {
		
		public static final DeviceInformer instance = new DeviceInformer();
	}
	
	public void init (Context appContext, Activity mainActivity) {
		
		m_context		= appContext;
		mMainActivity	= mainActivity;
	}
	
	public void testMethod () {
		
		UnityPlayer.UnitySendMessage(RECEIVE_OBJECT_NAME, "OnReceive", "test");
	}
	
	public void rateApplication () {
		// naver
		// uri scheme : appstore://store?packageName=XXXXXX
		
		// google
		Uri uri = Uri.parse("market://details?id=" + m_context.getPackageName());
		Intent goToMarket = new Intent(Intent.ACTION_VIEW, uri);
		m_context.startActivity(goToMarket);
	}
	
	public void getDeviceInfo () {
		
		TelephonyManager manager = null;
		
		try {
			manager = (TelephonyManager) m_context.getSystemService(Context.TELEPHONY_SERVICE);
		} catch (Exception exception) {
			Log.e(TAG, exception.toString());
		}
		
		String	strRooted		= isRooted();
		String	deviceId		= (manager.getDeviceId() == null ? EMPTY_DEVICE_ID : manager.getDeviceId());
		String	phoneNumber		= EMPTY_PHONE_NUMBER; // (manager.getLine1Number() == null ? EMPTY_PHONE_NUMBER : manager.getLine1Number().replace("+82", "0"));
		String	vendor			= manager.getNetworkOperatorName();
		String	locale			= manager.getNetworkCountryIso();
		String	language		= m_context.getResources().getConfiguration().locale.getLanguage();
		String	OSVersion		= Build.VERSION.RELEASE;
		String	model			= Build.DEVICE;
		String	maker			= Build.MANUFACTURER;
		@SuppressWarnings("deprecation")
		String	processor		= Build.CPU_ABI;
		String	resistrationID	= "";//GCMManager.getInstance().getRegistrationId();
		String  memTotal        = Long.toString(getTotalMemory());
		String memAvalid        = Long.toString(getAvailMemory());
		
		if(deviceId.equals(EMPTY_STRING) || deviceId.length() <= BAD_DEVICE_ID_LENGTH || deviceId.equals(BAD_DEVICE_ID_PATTERN)) {
			
			try {
			
				WifiManager m = (WifiManager) m_context.getSystemService(Context.WIFI_SERVICE);
				WifiInfo info = m.getConnectionInfo();
				
				deviceId = info.getMacAddress();
			
			} catch (Exception exception) {
				Log.e(TAG, exception.toString());
			}
		}
		
		String infoPack	= strRooted + 
				DELIMITER + deviceId + 
				DELIMITER + phoneNumber + 
				DELIMITER + vendor + 
				DELIMITER + locale + 
				DELIMITER + language + 
				DELIMITER + OSVersion + 
				DELIMITER + model + 
				DELIMITER + maker + 
				DELIMITER + processor + 
				DELIMITER + resistrationID+
				DELIMITER + memTotal+
				DELIMITER + memAvalid;
		
		UnityPlayer.UnitySendMessage(RECEIVE_OBJECT_NAME, SET_DEVICE_INFO, infoPack);
		Log.i(TAG, "getDeviceInfo(): " + infoPack);
		//showToast(infoPack);
	}
	
	public void showToast (final String message) {
		
		runOnUiThread(new Runnable() {
			
			public void run() {
				
				Toast.makeText(mMainActivity, message, Toast.LENGTH_LONG).show();
			}
		});
	}
	
	private void runOnUiThread(Runnable runnable) {
		
		Looper looper = Looper.getMainLooper();
		
		if (looper.getThread() == Thread.currentThread())
			runnable.run();
		else {
			Handler handler = new Handler(looper);
			handler.post(runnable);
		}
	}
	
	public void getIsRooted () {
		
		String strRooted = isRooted();
		UnityPlayer.UnitySendMessage(RECEIVE_OBJECT_NAME, SET_IS_ROOTED, strRooted);
	//	Log.i(TAG, "getIsRooted(): " + strRooted);
	}
	
	private String isRooted() {
		
		boolean isRooted = false;
		
		// first try
//		if (isRooted == false) {
//			Process	process	= null;
//			try {
//				process	= Runtime.getRuntime().exec("su");
//				isRooted = true;
//			} catch (IOException e1) {
//				// TODO Auto-generated catch block
//				e1.printStackTrace();
//			} finally {
//				if (process != null) {
//					process.destroy();
//				}
//				Runtime.getRuntime().freeMemory();
//			}
//		}
		
        
		// second try
//		if (isRooted == false) {
//			
//			Process				process		= null;
//			InputStreamReader	input		= null;
//			BufferedReader		reader		= null;
//			
//			try {
//				
//				process		= Runtime.getRuntime().exec("find / -name su");
//				input		= new InputStreamReader(process.getInputStream());
//				reader		= new BufferedReader(input);
//				
//				if (reader.ready()) {
//					isRooted = true;
//		        }
//				
//				String	result	= reader.readLine();
//		        if (result != null && result.contains("/su") == true) {
//		        	isRooted = true;
//		        }
//		        
//			} catch (IOException e) {
//				
//				// TODO Auto-generated catch block
//				e.printStackTrace();
//			} finally {
//				if (reader != null) {
//					try {
//						reader.close();
//					} catch (IOException e) {
//						// TODO Auto-generated catch block
//						e.printStackTrace();
//					}
//				}
//				if (input != null) {
//					try {
//						input.close();
//					} catch (IOException e) {
//						// TODO Auto-generated catch block
//						e.printStackTrace();
//					}
//				}
//				if (process != null) {
//					process.destroy();
//				}
//		        
//		        Runtime.getRuntime().freeMemory();
//			}
//		}
		
		// third try
		if (isRooted == false) {
			isRooted = findBinary("su");
		}
		
		String strRooted = (isRooted == true) ? "true" : "false";
		
		return strRooted;
	}
	
	@SuppressLint("FindBinary")
	private boolean findBinary(String binaryName) {
		
	    boolean found = false;
	    if (!found) {
	        String[] places = {"/sbin/", "/system/bin/", "/system/xbin/", "/data/local/xbin/",
	                "/data/local/bin/", "/system/sd/xbin/", "/system/bin/failsafe/", "/data/local/"};
	        for (String where : places) {
	            if ( new File( where + binaryName ).exists() ) {
	                found = true;
	                break;
	            }
	        }
	    }
	    return found;
	}
	
	@SuppressLint("CheatingToolProjectNames")
	private static final String[] CHEATING_TOOLS = {
		"com.cih.gamecih2",
		"com.cih.game_cih",
		"cn.maocai.gamekiller",
		"idv.aqua.bulldog",
		"com.google.android.xyz",
		"com.google.android.kkk",
		"com.cih.gamecih",
		"cn.luomao.gamekiller",
		"com.android.xxx",
		//"com.android.mms",
		"cn.maocai.gameki11er",
		"cn.mc.sq",
		"org.aqua.gg",
		"cc.cz.madkite.freedom",
		//"com.rsupport.mvagent",
		"org.sbtools.gamehack",
		"cc.madkite.freedom"
    };
	
	public void getIsCheating () {
		
		PackageManager pm	= m_context.getPackageManager();
		List<ApplicationInfo> appList = pm.getInstalledApplications(0);
		String tool = "false";
		int nSize = appList.size();
		for (int i = 0; i < nSize; i++) {
			for (int j = 0; j < CHEATING_TOOLS.length; j++) {
			//	Log.i(TAG, "appList: " + appList.get(i).packageName);
				if (appList.get(i).packageName.equalsIgnoreCase(CHEATING_TOOLS[j]) == true) {
					tool = CHEATING_TOOLS[j];
					break;
				} else {
					continue;
				}
			}
		}
		UnityPlayer.UnitySendMessage(RECEIVE_OBJECT_NAME, SET_IS_CHEATING, tool);
	//	Log.i(TAG, "getIsCheating(): " + tool);
	}
	
	public static final String  PREFS	= "";
	public void setGcmState (String state) {
		/*
		SharedPreferences myPrefs = c.getSharedPreferences("myPrefs", 0);
		String savedUser = myPrefs.getString("user", null);
		if(savedUser == null) {
			user = UUID.randomUUID().toString();
		    String hashedUser = md5(user);
		    SharedPreferences.Editor myPrefsEditor = myPrefs.edit();

		    myPrefsEditor.putString("user", hashedUser);
		    myPrefsEditor.commit();

		    return hashedUser;
		    } else {
		    return savedUser;
		    }
		    */
	}
	
	public void getGcmState () {
		
	}
	
	private long getAvailMemory() {// 获取android当前可用内存大小  
		  
        ActivityManager am = (ActivityManager) mMainActivity.getSystemService(Context.ACTIVITY_SERVICE);  
        MemoryInfo mi = new MemoryInfo();  
        am.getMemoryInfo(mi);  
        //mi.availMem; 当前系统的可用内存  
  
        return mi.availMem;
        //return Formatter.formatFileSize(mMainActivity.getBaseContext(), mi.availMem);// 将获取的内存大小规格化  
    }  
  
    private long getTotalMemory() {  
        String str1 = "/proc/meminfo";// 系统内存信息文件  
        String str2;  
        String[] arrayOfString;  
        long initial_memory = 0;  
  
        try {  
            FileReader localFileReader = new FileReader(str1);  
            BufferedReader localBufferedReader = new BufferedReader(  
                    localFileReader, 8192);  
            str2 = localBufferedReader.readLine();// 读取meminfo第一行，系统总内存大小  
  
            arrayOfString = str2.split("\\s+");  
            for (String num : arrayOfString) {  
                Log.i(str2, num + "\t");  
            }  
  
            initial_memory = Integer.valueOf(arrayOfString[1]).intValue() ;// 获得系统总内存，单位是KB，乘以1024转换为Byte  
            localBufferedReader.close();  
  
        } catch (IOException e) {  
        }  
        
        //showToast(Long.toString(initial_memory));
        return initial_memory;
        //return Formatter.formatFileSize(mMainActivity.getBaseContext(), initial_memory);// Byte转换为KB或者MB，内存大小规格化  
    } 
}
