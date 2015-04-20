using UnityEngine;
using System.Collections;

public class yScreenModify : MonoBehaviour 
{
	public int MyDir = -1;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	void Awake()
	{
        float fWidth = COMMON_CONST.CameraWidthRatio * Screen.width;
        float fHeight = COMMON_CONST.CameraHeightRatio * Screen.height;

		float fHVaule = COMMON_CONST.CameraHeight/COMMON_FUNC.GetCamearaRealHeight();
		Vector3 Pos = this.transform.localPosition;
        Pos.x = (float)MyDir * (fHVaule * COMMON_FUNC.GetCamearaRealWidth()) / 2.0f;
				
		this.transform.localPosition = Pos;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
}
