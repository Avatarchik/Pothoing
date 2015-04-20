using UnityEngine;
using System.Collections;

public class yScreenSizing : MonoBehaviour 
{
	public enum ArrangeType {
		HALF,
		FULL,
		FULL_WH,
		FULL_PLUS,
		HALF_PLUS,
		FULL_KEEP_W,
	}

	public ArrangeType MyType = ArrangeType.HALF;
	public int MyDir = -1;
	public MonoBehaviour CtrlScript;

	public	int	FULL_PLUSX	= 20;
	public	int FULL_PLUSY	= 20;

	// Use this for initialization
	void Start () 
	{
		switch(MyType)
		{
		case ArrangeType.HALF:
			InitHalfBG(0.0f);
			break;
		case ArrangeType.FULL:
			InitFullBG();
			break;
		case ArrangeType.FULL_WH:
			InitFullBG_WH();
			break;
		case ArrangeType.FULL_PLUS:
			InitFullBgPlusLittleMore();
			break;
		case ArrangeType.HALF_PLUS:
			InitHalfBG(2.0f);
			break;
		case ArrangeType.FULL_KEEP_W:
			InitFullBG_KeepWidth();
			break;
		}
		
		if(CtrlScript != null)
		{
			CtrlScript.Invoke("OnResizing",0.0f);
		}
	}
	
	void InitFullBG()
	{
		float fHVaule = COMMON_CONST.CameraHeight/COMMON_FUNC.GetCamearaRealHeight();
		Vector3 Size = this.transform.localScale;
        Size.x = COMMON_FUNC.GetCamearaRealWidth() * fHVaule;
		this.transform.localScale = Size;
		
		Vector3 Pos = this.transform.localPosition;
		Pos.x = MyDir*(fHVaule*(float)Screen.width)/2.0f;
		this.transform.localPosition = Pos;
	}

	void InitFullBG_WH()
	{
        float fHVaule = COMMON_CONST.CameraHeight / COMMON_FUNC.GetCamearaRealHeight();
		Vector3 Size = this.transform.localScale;
		float ratio	= Size.y/Size.x;
        Size.x = COMMON_FUNC.GetCamearaRealWidth() * fHVaule;
		Size.y	= Size.x * ratio;
		this.transform.localScale = Size;
		
		Vector3 Pos = this.transform.localPosition;
		Pos.x = MyDir*(fHVaule*(float)Screen.width)/2.0f;
		this.transform.localPosition = Pos;
	}

	void InitFullBG_KeepWidth()
	{
        float fHVaule = COMMON_CONST.CameraHeight / COMMON_FUNC.GetCamearaRealHeight();
		Vector3 Size = this.transform.localScale;
		Size.x = (float)(Size.x) * fHVaule;
		this.transform.localScale = Size;
		
		Vector3 Pos = this.transform.localPosition;
		Pos.x = MyDir*(Size.x)/2.0f;
		this.transform.localPosition = Pos;
	}
	
	void InitHalfBG(float AddSize)
	{
        float fHVaule = COMMON_CONST.CameraHeight / COMMON_FUNC.GetCamearaRealHeight();
		Vector3 Size = this.transform.localScale;
        Size.x = (AddSize + (COMMON_FUNC.GetCamearaRealWidth() / 2.0f)) * fHVaule;
		this.transform.localScale = Size;
		
		Vector3 Pos = this.transform.localPosition;
		Pos.x = MyDir*(fHVaule*(float)Screen.width)/2.0f;
		this.transform.localPosition = Pos;
	}
	
	void InitFullBgPlusLittleMore ()
	{
        float fHVaule = COMMON_CONST.CameraHeight / COMMON_FUNC.GetCamearaRealHeight();
		Vector3 Size = this.transform.localScale;
        Size.x = COMMON_FUNC.GetCamearaRealWidth() * fHVaule;
		Size.x = Size.x + FULL_PLUSX;
		Size.y = Size.y + FULL_PLUSY;
		this.transform.localScale = Size;
		
		Vector3 Pos = this.transform.localPosition;
        Pos.x = MyDir * (fHVaule * COMMON_FUNC.GetCamearaRealWidth()) / 2.0f - 1.0f;
		this.transform.localPosition = Pos;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
