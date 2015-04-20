using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RewardItem : MonoBehaviour {

    [SerializeField]
    Text RewardName;

    public void SetReward(string name)
    {
        this.RewardName.text = name;
    }
}
