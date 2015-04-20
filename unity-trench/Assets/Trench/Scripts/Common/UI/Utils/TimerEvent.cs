using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TimerEvent : MonoBehaviour
{

	[SerializeField]
	UnityEvent
		events;
	[SerializeField]
	float
		interval = 5f;
	[Header("-1:Eternal")]
	[SerializeField]
	int
		loop = -1;

	IEnumerator Start ()
	{
		while (loop!=0) {
			yield return new WaitForSeconds (interval);
			events.Invoke ();
			if (loop > 0) {
				loop--;
			}
		}
	}
}
