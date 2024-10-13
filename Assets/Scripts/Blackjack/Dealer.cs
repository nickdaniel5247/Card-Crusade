using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Blackjack_Dealer : Participant
{
    //Editor assigned prefab
    public GameObject faceDownCard;
    private Sprite firstCard;
    private Deck deck;

    void Awake()
    {
        deck = GameObject.Find("Deck").GetComponent<Deck>();

        if (!deck)
        {
            Debug.LogError("BLACKJACK_DEALER: Cannot find Deck GameObject w/ Deck script.");
            return;
        }
    }

    //Deals first card face-down by creating inital hand; no-op if first card has been dealt already
    public void dealFirstCard()
    {
        if (hands.Count != 0)
        {
            return;
        }

        Card card = deck.takeCard();
        firstCard = card.card;

        GameObject cardObject = Instantiate(faceDownCard);
        cardObject.GetComponent<CardData>().value = card.value;

        createHand(0, 0f, cardObject);
    }

    //Deals second card; expects starting conditions created from calling dealFirstCard otherwise it's no-op
    public void dealSecondCard()
    {
        if (hands.Count != 1)
        {
            return;
        }

        Blackjack_Hand hand = hands.First().GetComponent<Blackjack_Hand>();

        if (hand.cardObjects.Count != 1)
        {
            return;
        }

        hand.hit();
    }

    public void revealFirstCard()
    {
        List<GameObject> cardObjects = hands.First().GetComponent<Blackjack_Hand>().cardObjects;

        if (hands.Count < 1 || cardObjects.Count < 1)
        {
            return;
        }

        cardObjects.First().GetComponent<SpriteRenderer>().sprite = firstCard;
    }

    //Destroys hand
    public void destroyHand()
    {
        for (int i = 0; i < hands.Count; ++i)
        {
            Destroy(hands[i]);
        }

        hands.Clear();
    }

    /*
     * Play hand using dealer rules
     *
     * Returns final hand value
     */
    public int playTurn()
    {
        Debug.LogError("BLACKJACK_DEALER: playTurn is not implemented.");
        return 0;
    }
}