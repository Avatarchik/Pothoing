using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{

	Vector3 oriPos;
	float interval = 0.01f;
	float shakePower = 5f;

	public void Shake ()
	{
		StartCoroutine (DoShake (0.2f));
		//Debug.Log("DoShake");
	}

	public void Shake(float time){
		StartCoroutine (DoShake (time));
	}
//
//	public void ShakeLoop(){
//		
//	}

	IEnumerator DoShake (float time)
	{
		oriPos = transform.position;
		float timer = 0;
		while (timer<time) {
			transform.position = oriPos + new Vector3 (Random.Range (-shakePower, shakePower), Random.Range (-shakePower, shakePower), Random.Range (-shakePower, shakePower));
			yield return new WaitForSeconds (interval);
			timer += interval;
		}
		transform.position = oriPos;
	}
}
