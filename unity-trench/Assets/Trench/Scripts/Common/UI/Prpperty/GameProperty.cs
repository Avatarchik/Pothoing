using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class GameProperty : MonoBehaviour {

    [SerializeField]
    PropertyBottomCards mPropertyBottomCards;

    [SerializeField]
    PropertyNextPlayerCards mNextPlayerCards;

    [SerializeField]
    PropertyStopButton mStopButton;

    [SerializeField]
    PropertyChange mChange;

    [SerializeField]
    ParticleSystem mBom;



    /// //////////////////////////////////////////////


    public void SetBottomCard(byte[] cards)
    {
        mPropertyBottomCards.SetBottomCard(cards);
    }

    public void SetBottomDisable()
    {
        mPropertyBottomCards.SetBottomDisable();
    }



    public IEnumerator SetNextPlayerCards(List<byte> cards)
    {

        byte[] temp = new byte[cards.Count];
        for (int i = 0; i < cards.Count;i++ )
        {
            temp[i] = cards[i];
        }

        mNextPlayerCards.SetCardData(temp);

        yield return new WaitForSeconds(5f);

        mNextPlayerCards.ClearCard();
    }

    public void SetStopButton(bool hasvis)
    {
        mStopButton.gameObject.SetActive(hasvis);
    }

    public void SetStopButtonState(PropertyStopButton.State state)
    {
        mStopButton.SetState(state);
    }

    public PropertyStopButton.State GetStopButtonState()
    {
        return mStopButton.GetState();
    }


    //---------------------------------------------------

    public void ShowChangeDialog(bool onoff)
    {

        mChange.ShowChangeDialog(onoff);
    }

    public void SetChangeButton(bool enable)
    {
        mChange.ChanageButton(enable);
    }

    public void SetChangeButtonAction(Action action,Action action1)
    {
        mChange.SetCallBack(action, action1);
    }

    //---------------------

    public void StartSendBom()
    {
        mBom.Play();
    }
}
