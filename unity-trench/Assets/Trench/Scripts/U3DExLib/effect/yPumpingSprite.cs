using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CheckItemsNew : MonoBehaviour
{

    private Dictionary<string, bool> NewStates = new Dictionary<string, bool>();
    public GameObject NewIcon = null;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bool bNew = false;
        foreach (var e in NewStates)
        {
            bNew |= e.Value;
        }

        if (bNew)
        {
            NewIcon.GetComponent<yPumpingSprite>().bAuto = true;
            if (NewIcon.activeSelf == false)
                NewIcon.SetActive(true);
        }
        else
        {
            if (NewIcon.activeSelf == true)
                NewIcon.SetActive(false);
        }
    }

    public void AddItemNew(string name, bool v)
    {
        NewStates[name] = v;
    }
}

public class yPumpingSprite : MonoBehaviour 
{
	public bool	bAuto	= false;

	Vector3	MyOrgScale;
	bool	bStart		= false;
	public float	fScaleAni	= 1.0f;
	public float	fDirAni		= 2.0f;
	public float	fMaxScale	= 1.5f;
	public float	fUpSpeed	= 2.0f;
	public float	fDownSpeed	= -1.0f;

    public string   SendObjName = null;
    public CheckItemsNew ItemNewObj = null;

	private bool	m_isInitialized	= false;

	// Use this for initialization
	void Start () 
	{
		if(bAuto)
		{
            OnStart(fMaxScale, fUpSpeed, fDownSpeed, true);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(bStart)
		{
			if (Time.timeScale == 0.0f) return;

			fScaleAni += fDirAni*Time.deltaTime*(1.0f/Time.timeScale);
			if(fScaleAni >= fMaxScale)
			{
				fScaleAni = fMaxScale;
				fDirAni = fDownSpeed;
			}
			if(fScaleAni <= 1.0f)
			{
				fScaleAni = 1.0f;
				fDirAni = fUpSpeed;
			}
			this.transform.localScale = MyOrgScale*fScaleAni;
			
		}
	}

	//	# Public methods
	
	public void OnStart(float Max = 1.5f, float up = 2.0f, float Down = -1.0f, bool isOn = true)
	{
        if (SendObjName == null)
        {
            Debug.LogWarning("yPumpingSprite SendObjName!");
        }
        else
        {
            if(ItemNewObj!=null)
            {
                ItemNewObj.AddItemNew(SendObjName, isOn);
            }
        }

		if(isOn && m_isInitialized)
		{
			turnOn ();
			return;
		}
		
		fMaxScale	= Max;
		MyOrgScale	= this.transform.localScale;
		bStart		= isOn;
		fDirAni		= fUpSpeed = up;
        fDownSpeed	= Down;

		m_isInitialized	= true;
    }

	public void turnOn ()
	{
		if(m_isInitialized == false)
		{
			OnStart();
		}
		bStart = true;
	}

	public void turnOff ()
	{
		bStart = false;
	}
}
