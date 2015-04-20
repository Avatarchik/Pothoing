using UnityEngine;
using System.Collections;

public class yCameraSize : MonoBehaviour 
{
	public Camera MyCamera;
	// Use this for initialization
	//1280 960
	void Awake()
	{
		float fSmall = (float)Screen.height;
		float fBig = (float)Screen.width;

		if(fSmall > fBig)
		{
			float Temp = fSmall;
			fSmall = fBig;
			fBig = Temp;
		}
		float fOriginalRatio = COMMON_CONST.CameraHeight / COMMON_CONST.CameraWidth;
		float v = fSmall/fBig;
		if(v > 0.7f)
		{
			MyCamera.rect = new Rect(0,(1.0f - v)/2.0f,1,v);
			COMMON_FUNC.SetCameraWidthHeight(MyCamera.rect.width,MyCamera.rect.height);
		}
#if UNITY_ANDROID
		DeviceManager.Instance.ShowToast(v.ToString());
#endif

//		if( v > fOriginalRatio)
//		{
//			//float fHRatio = fSmall - COMMON_CONST.CameraHeight;
//			float fControlRatio = v - fOriginalRatio;
//			MyCamera.rect = new Rect(0,(1.0f - v)/2.0f,1,v);
//			COMMON_CONST.SetCameraWidthHeight(MyCamera.rect.width,MyCamera.rect.height);
//		}



	}

	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
