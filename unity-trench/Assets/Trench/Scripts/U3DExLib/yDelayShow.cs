using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class yDelayShow : MonoBehaviour 
{
	public float FirstDealy = 0.5f;
	public GameObject [] FirstGroupObjects;
	public float SecondDealy = 0.5f;
	public GameObject [] SecondGroupObjects;
	public float ThirdDelay = 0.5f;
	public Image ThirdPanel;

	bool bThirdAction = false;
	// Use this for initialization
	void Start () 
	{
		Invoke("OnFistAction",FirstDealy);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(bThirdAction)
		{
            Color cr = ThirdPanel.color;
            cr.a += Time.deltaTime * 3.0f;
            ThirdPanel.color = cr;
			if(ThirdPanel.color.a >= 1.0f)
			{
                cr.a = 1.0f;
				ThirdPanel.color = cr;
				bThirdAction = false;;
			}
		}
	}

	void OnFistAction()
	{
		for(int i=0;i<FirstGroupObjects.Length;++i)
		{
			FirstGroupObjects[i].SetActive(true);
		}

		Invoke("OnSecondAction",SecondDealy);
	}

	void OnSecondAction()
	{
		for(int i=0;i<SecondGroupObjects.Length;++i)
		{
			SecondGroupObjects[i].SetActive(true);
		}

		Invoke("OnThirdAction",ThirdDelay);
	}

	void OnThirdAction()
	{
		ThirdPanel.gameObject.SetActive(true);
		bThirdAction = true;
	}
}
