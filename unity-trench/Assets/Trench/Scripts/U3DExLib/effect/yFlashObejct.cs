using UnityEngine;
using System.Collections;

public class yFlashObejct : MonoBehaviour 
{
	public float ShowTime = 1.0f;
	public float HiddenTime = 0.5f;
	float CurTime = 0.0f;
	bool bShow = true;
	public MonoBehaviour CtrlScript;
	public bool bAuto = true;
	bool bStart = false;
	public int Type = 0;
	// Use this for initialization
	void Awake()
	{
		bStart = bAuto;
	}

	void Start () 
	{
	
	}

	public void OnStart()
	{
		bStart = true;
	}

	public void OnStop()
	{
		bStart = false;
		bShow = false;
		if(Type == 1)
		{
			this.GetComponent<MeshRenderer>().enabled = bShow;
		}
		else
		{
			CtrlScript.enabled = bShow;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(bStart == false) return;
		CurTime += Time.deltaTime;
		if(bShow && CurTime >= ShowTime)
		{
			CurTime = 0.0f;
			bShow = false;
			if(Type == 1)
			{
				this.GetComponent<MeshRenderer>().enabled = bShow;
			}
			else
			{
				CtrlScript.enabled = bShow;
			}
		}
		else if(!bShow && CurTime >= HiddenTime)
		{
			CurTime = 0.0f;
			bShow = true;
			if(Type == 1)
			{
				this.GetComponent<MeshRenderer>().enabled = bShow;
			}
			else
			{
				CtrlScript.enabled = bShow;
			}
		}
	}
}
