using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TaskAlert : Dialog {

    [SerializeField]
    Text mText;

	// Use this for initialization
	void Start () {
	
	}
	
    public void ShowText(string contentstr)
    {
        mText.text = contentstr;
        base.Show();
    }
}
