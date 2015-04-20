using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public enum StartState
{
    ON,
    OFF
}

public class StartComponet : MonoBehaviour {

    [SerializeField]
    Sprite[] mAllImage;

	// Use this for initialization
	void Start () {        
        
       
	}
	
	// Update is called once per frame
	public void SetState (StartState state) {

        int index = (int)state;
        Image current = GetComponent<Image>();
        current.sprite = mAllImage[index];
	}
}
