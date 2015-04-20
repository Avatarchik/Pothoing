using UnityEngine;
using System.Collections;

public class AutoFitCanvasToScreen : MonoBehaviour {

	[SerializeField]
	RectTransform canvas;

	void Awake () {
		//Camera.main.orthographicSize *= Screen.width>=Screen.height? Screen.width / 800f:Screen.height / 480f;//Screen.width *Screen.height / (800f*480f);
		//print (Screen.width+" "+Screen.height+" "+(float)Screen.width/(float)Screen.height);
		float newWidth = (float)Screen.width / (float)Screen.height * 480f;
		canvas.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal,newWidth);
		//canvas.transform.localScale = new Vector3 (newWidth/800f,newWidth/800f,canvas.transform.localScale.z);
		//canvas.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, Screen.height);
	}

}
