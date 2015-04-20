using UnityEngine;
using System.Collections;

public class yScaleAnimation : MonoBehaviour 
{
	Vector3 MyBaseScale;
	float fMyScale = 0.0f;
	float fMyAccel = 1.0f;
	bool bStart = false;
	float [] fStoplist;
	int MyStop = 0;
	float fMyDir = 1.0f;
	// Use this for initialization
	void Awake()
	{
		MyBaseScale = this.transform.localScale;
		this.transform.localScale = MyBaseScale*fMyScale;

		fStoplist = new float[5];
		fStoplist[0] = 1.2f;
		fStoplist[1] = 0.8f;
		fStoplist[2] = 1.1f;
		fStoplist[3] = 0.9f;
		fStoplist[4] = 1.0f;
	}

	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(bStart)
		{
			fMyScale += fMyDir*Time.deltaTime*fMyAccel;
			fMyAccel += 5.0f*Time.deltaTime;
			if(fMyDir > 0.0f)
			{
				if(fMyScale >= fStoplist[MyStop])
				{
					fMyAccel /= 5.0f;
					fMyScale = fStoplist[MyStop];
					++MyStop;
					if(MyStop == fStoplist.Length)
					{
						bStart = false;
					}
					else
					{
						if(fMyScale > fStoplist[MyStop])
						{
							fMyDir = -1.0f;
						}
						else
						{
							fMyDir = 1.0f;
						}
					}
				}
			}
			else
			{
				if(fMyScale <= fStoplist[MyStop])
				{
					fMyAccel /= 5.0f;
					fMyScale = fStoplist[MyStop];
					++MyStop;
					if(MyStop == fStoplist.Length)
					{
						bStart = false;
					}
					else
					{
						if(fMyScale > fStoplist[MyStop])
						{
							fMyDir = -1.0f;
						}
						else
						{
							fMyDir = 1.0f;
						}
					}
				}
			}

			this.transform.localScale = MyBaseScale*fMyScale;
		}
	}

	public void OnStart()
	{
		bStart = true;
		//this.GetComponent<UISprite>().enabled = true;
	}
}
