using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PropertyBottomCards : MonoBehaviour
{
  


    [SerializeField]
    Card mCard;

    [SerializeField]
    ParticleSystem mStart;

    List<Card> mAllCards;

  


    public void SetBottomCard(byte[] cards)
    {
       


        if (mAllCards == null)
        {
            mAllCards = new List<Card>();
        }
        this.gameObject.SetActive(true);
        mStart.Play();
        for (int i = 0; i < cards.Length; i++)
        {

            Card card = Instantiate(mCard, new Vector3(i * 30, 0, 0), Quaternion.identity) as Card;
            card.SetCard(cards[i]);
            card.transform.SetParent(this.transform);
            mAllCards.Add(card);

        }
    }

    public void SetBottomDisable()
    {
        this.gameObject.SetActive(false);
        for (int i = 0; i < mAllCards.Count; i++)
        {
            mAllCards[i].transform.SetParent(null);
            Destroy(mAllCards[i].gameObject);
        }

        mAllCards.Clear();
    }
}
