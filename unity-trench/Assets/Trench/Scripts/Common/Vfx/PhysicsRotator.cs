using UnityEngine;
using System.Collections;

public class PhysicsRotator : MonoBehaviour {

	public Vector3 torque=Vector3.one;
	void Start () {
		rigidbody.AddTorque(torque);
	}
}
