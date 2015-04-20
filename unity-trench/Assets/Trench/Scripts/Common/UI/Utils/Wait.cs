using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Wait : MonoBehaviour
{
	public UnityEvent callBack;
	public float waitTime = 0.5f;

	IEnumerator Start ()
	{
		yield return new  WaitForSeconds (waitTime);
		callBack.Invoke ();
	}

}
