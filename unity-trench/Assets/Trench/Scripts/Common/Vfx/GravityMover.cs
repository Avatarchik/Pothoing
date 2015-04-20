using UnityEngine;
using System.Collections;

public class GravityMover : MonoBehaviour {

	void Update () {
		transform.eulerAngles = Input.acceleration;
	}
}
