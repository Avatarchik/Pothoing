using UnityEngine;
using System.Collections;

public class yShadow : MonoBehaviour 
{
	public Transform Follow;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		Vector3 vFo = Follow.localPosition;
		Vector3 vS;
		vS.x = 0.5f - vFo.y/10.0f;
		vS.y = 0.5f - vFo.y/10.0f;
		vS.z = 1.0f;
		this.transform.localScale = vS;
		vFo.y = 0.0f;
		this.transform.localPosition = vFo;
	}
}
