using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PropertyStopButton : MonoBehaviour {

    [SerializeField]
    Button mStopOut;
    [SerializeField]
    Sprite[] mAllButtonState;
	// Use this for initialization

    State mState;


    public enum State
    {
        STATE_DISABLED,//不可用
        STATE_ENABLE,//可选
        STATE_SELECTED,//已经选择
    }


	void Start () {

        mStopOut.onClick.AddListener(ChangeButtonState);
        //SetState(State.STATE_ENABLE);
	}

    public void SetState( State state)
    {
        switch(state)
        {
            case State.STATE_DISABLED:
                mStopOut.interactable = false;
                break;
            case State.STATE_ENABLE:
                mStopOut.image.sprite = mAllButtonState[1];
                mStopOut.interactable = true;
                break;
            case State.STATE_SELECTED:
                mStopOut.image.sprite = mAllButtonState[0];
                break;
          
        }
        this.mState = state;
    }

    public State GetState()
    {
        return this.mState;
    }
    
    public void ChangeButtonState()
    {
        switch (mState)
        {
            case State.STATE_ENABLE:
                SetState(State.STATE_SELECTED);               
                break;
            case State.STATE_SELECTED:
                SetState(State.STATE_ENABLE);
                break;
                
        }

        Debug.Log("STATE:" + mState);
    }    
}
