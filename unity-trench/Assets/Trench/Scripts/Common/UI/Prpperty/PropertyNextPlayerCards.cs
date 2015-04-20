using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PropertyNextPlayerCards : MonoBehaviour {

    [SerializeField]
    Card mCard;

    [SerializeField]
    ParticleSystem mParticle;

    List<Card> mAllCard;





	// Use this for initialization
	void Start () 
    {       

              
	}
	
	public void SetCardData(byte[] allCard)
    {
        if(mAllCard == null)
        {
            mAllCard = new List<Card>(); 
        }


        this.gameObject.SetActive(true);
        mParticle.Play();
        for(int i = 0;i < allCard.Length;i++)
        {
            Card card = Instantiate(mCard) as Card;

            card.transform.SetParent(this.transform);

            card.transform.localScale = new Vector3(0.5f,0.5f,1);
            card.transform.localPosition = new Vector3(i * 15, 0 , 0);

            card.SetCard(allCard[i]);

            mAllCard.Add(card);
        }
    }


    public void ClearCard()
    {
        this.gameObject.SetActive(false);
        for (int i = 0; i < mAllCard.Count; i++)
        {           
            mAllCard[i].transform.SetParent(null);

            Destroy(mAllCard[i].gameObject);
        }

        mAllCard.Clear();
    }
}
