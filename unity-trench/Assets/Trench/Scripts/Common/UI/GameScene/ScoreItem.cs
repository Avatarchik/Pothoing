using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreItem : MonoBehaviour {

    [SerializeField]
    Text mText;


    public void SetData(int num)
    {
        mText.text = num.ToString();
    }


}
