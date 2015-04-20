using UnityEngine;
using System.Collections;

public class Rotater : MonoBehaviour
{
	public Vector3 rotateSpeed = Vector3.forward;

	void Update ()
	{
		transform.eulerAngles += rotateSpeed * Time.deltaTime;
	}
}
