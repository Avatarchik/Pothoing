using UnityEngine;
using System.Collections;

public class CameraOrbit : MonoBehaviour 
{
	public GameObject m_MySoundMng;
	public GameObject m_MyFocus;
	public Vector3 m_vBoost;
	public float m_fMoveBoost;
	public float m_fMaxDist = 100.0f;
	public float m_fMinDist = 10.0f;
	public float m_fDist = 100.0f;
	
	public Vector3 m_vTarget;
	public float m_fXRot = 0.0f;
	public float m_fYRot = 45.0f;
	
	
	const float yMinLimit = 25.0f;
	const float yMaxLimit = 89.0f;
	
	public bool m_bMove = true;
	public bool m_bRotate = true;
	public bool m_bZoom = true;
	
	bool 	m_bShake = false;
	float	m_fShakeTime = 0.0f;
	float 	m_fShake = 0.0f;
	// Use this for initialization
	void Start () 
	{
		SetTargetPosition();
		SetCamera();
	}
	
	void SetTargetPosition()
	{
		m_vTarget = m_MyFocus.transform.position;
	}
	
	void Update()
	{

	}
	// Update is called once per frame
	void FixedUpdate () 
	{
		//InputProcess();
		TargetMove();
		SetCamera();
		
		Vector3 vPos = this.transform.position;
		if(vPos.z > 9.5f) vPos.z = 9.5f;
		if(vPos.z < -9.5f) vPos.z = -9.5f;
		if(vPos.x < -8.0f) vPos.x = -8.0f;
		if(vPos.x > 8.0f) vPos.x = 8.0f;
		this.transform.position = vPos;
		
		m_fXRot += 360;
		m_fYRot += 360;
		m_fXRot %= 360;
		m_fYRot %= 360;
		
		if(m_bShake)
		{
			m_fShakeTime -= Time.deltaTime;
			if(m_fShakeTime <= 0.0f)
			{
				m_fShakeTime = 0.0f;
				m_bShake = false;
				m_fShake = 0.0f;
			}
			else
			{
				m_fShake = Random.Range(-0.25f,0.25f);
			}
		}
	}
	
	void InputProcess()
	{
		float fZdelta = -Input.GetAxis("Mouse ScrollWheel");
		if(m_bZoom && fZdelta != 0.0f)
		{
			m_fDist += fZdelta*m_vBoost.z;
			if(m_fDist > m_fMaxDist) m_fDist = m_fMaxDist;
			if(m_fDist < m_fMinDist) m_fDist = m_fMinDist;
			
			this.transform.position = m_vTarget - this.transform.forward*m_fDist;
		}
		
		if(m_bRotate && Input.GetMouseButton(1))
		{
			m_fYRot += Input.GetAxis("Mouse X")*m_vBoost.y;
			m_fXRot -= Input.GetAxis("Mouse Y")*m_vBoost.x;
			m_fXRot = ClampAngle(m_fXRot, yMinLimit, yMaxLimit);
			SetCamera();
		}
		
		if(Input.GetKeyDown(KeyCode.F5))
		{
			EarthShake();
		}
	}
	
	public void EarthShake()
	{
		m_bShake = true;
		m_fShakeTime = 0.15f;
	}
	
	void TargetMove()
	{
		Vector3 vForward = transform.forward;
		vForward.y = 0.0f;
		vForward.Normalize();
		
		Vector3 vRight = transform.right;
		vRight.y = 0.0f;
		vRight.Normalize();
		
		Vector3 vMove;
		vMove.x =  -Input.GetAxis("Mouse X");
		vMove.y = 0;
		vMove.z =  -Input.GetAxis("Mouse Y");
		
		//m_MyFocus.transform.Translate(vMove*m_fMoveBoost);
		m_vTarget += vForward*vMove.z*m_fMoveBoost;
		m_vTarget += vRight*vMove.x*m_fMoveBoost;
	}
	
	public void SetCamera()
	{
		m_vTarget = m_MyFocus.transform.position;
		m_vTarget.x += m_fShake;
		Quaternion rotation = Quaternion.Euler(m_fXRot, m_fYRot, 0);
		
		Vector3 vDist = new Vector3(0.0f, 0.0f, -m_fDist);
		Vector3 position = rotation * vDist + m_vTarget;
		
		transform.rotation = rotation;
		transform.position = position;
	}
	
	float ClampAngle (float angle,float min,float max) 
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
	
	public void MoveSleep()
	{
		m_bMove = false;
	}
	
	public void MoveWakeUp()
	{
		m_bMove = true;
	}
	
	public void RotateSleep()
	{
		m_bRotate = false;
	}
	
	public void RotateWakeUp()
	{
		m_bRotate = true;
	}
	
	public void AllSleep()
	{
		m_bMove = false;
		m_bRotate = false;
		m_bZoom = false;
	}
	
	public void AllWakeUp()
	{
		m_bMove = true;
		m_bRotate = true;
		m_bZoom = true;
	}
}
