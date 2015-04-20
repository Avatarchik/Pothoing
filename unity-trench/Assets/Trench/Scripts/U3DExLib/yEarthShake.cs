using UnityEngine;
using System.Collections;

public class yEarthShake : MonoBehaviour 
{
	bool 	m_bShake = false;
	float	m_fShakeTime = 0.0f;
	float	m_fShakeTick = 0.0f;
	Vector3 MyOrgpos;
	float MyDir = 0.0f;
	// Use this for initialization
	void Start () 
	{
		MyOrgpos = this.transform.localPosition;
	}

	float GetRandomTick()
	{
		return Random.Range(0.05f,0.1f);
	}

	public void OnShake()
	{
		m_bShake = true;
		m_fShakeTime = Random.Range(0.5f,0.7f);
		m_fShakeTick = GetRandomTick();
		if(Random.Range(0,2) == 0)
		{
			MyDir = 1.0f;
		}
		else
		{
			MyDir = -1.0f;
		}
	}

	// Update is called once per frame
	void Update () 
	{
		/*
		if(Input.GetKeyDown(KeyCode.F5))
		{
			OnShake();
		}
		*/

		if(m_bShake)
		{
			m_fShakeTime -= Time.deltaTime;
			if(m_fShakeTime <= 0.0f)
			{
				m_fShakeTime = 0.0f;
				m_bShake = false;
				this.transform.localPosition = MyOrgpos;
			}
			else
			{
				m_fShakeTick -= Time.deltaTime;
				if(m_fShakeTick <= 0.0f)
				{
					m_fShakeTick = GetRandomTick();
					float fShake = Random.Range(5.0f,15.0f);
					Vector3 Temp = MyOrgpos;
					Temp.x += fShake*MyDir;
					this.transform.localPosition = Temp;
					MyDir *= -1.0f;
				}
			}
		}
	}
}
