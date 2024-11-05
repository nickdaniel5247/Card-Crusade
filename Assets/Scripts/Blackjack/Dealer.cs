using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Blackjack_Dealer : Participant
{
    //Editor assigned prefab
    public GameObject faceDownCard;
    public Vector3 cardSpawnOffset = new Vector3(1f, 0f, 0f);
    public float hitWait = 1f;

    private Sprite firstCard;
    private Deck deck;

    private AudioSource audioSource;
    public AudioClip cardSound;

    void Awake()
    {
        deck = GameObject.Find("Deck").GetComponent<Deck>();

        if (!deck)
        {
            Debug.LogError("BLACKJACK_DEALER: Cannot find Deck GameObject w/ Deck script.");
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    //Comfort function allowing Controller to easily shuffle
    public void shuffleDeck()
    {
        deck.resetAndShuffle();
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

        createHand(0, 0f, cardObject, cardSpawnOffset, new Vector3());
    }

    //Deals second card; expects starting conditions created from calling dealFirstCard otherwise it's no-op
    public void dealSecondCard()
    {
        if (hands.Count != 1)
        {
            return;
        }

        Blackjack_Hand hand = hands.First().GetComponent<Blackjack_Hand>();

        if (hand.getCardObjects().Count != 1)
        {
            return;
        }

        hand.hit();
    }

    public void revealFirstCard()
    {
        List<GameObject> cardObjects = hands.First().GetComponent<Blackjack_Hand>().getCardObjects();

        if (hands.Count < 1 || cardObjects.Count < 1)
        {
            return;
        }

        cardObjects.First().GetComponent<SpriteRenderer>().sprite = firstCard;
        audioSource.PlayOneShot(cardSound);
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
     * Play hand using dealer rules (hit while under 17, hit on soft 17 too)
     *
     * Returns final hand value
     */
    public IEnumerator playTurn(System.Action<int> retCallback)
    {
        Blackjack_Hand hand = hands.First().GetComponent<Blackjack_Hand>();

        while (hand.getHandValue(false) < 17)
        {
            hand.hit();
            hand.transform.localPosition -= cardSpawnOffset/2;
            audioSource.PlayOneShot(cardSound);
            yield return new WaitForSeconds(hitWait);
        }

        retCallback(hand.getHandValue(false));
    }
}