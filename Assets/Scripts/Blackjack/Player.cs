using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    private List<GameObject> hands = new List<GameObject>();
    public float handVerticalOffset = 2f;
    public float handDist = 1f;

    public enum Action
    {
        None,
        Hit,
        Stand,
        Double,
        Split
    };

    public Action playerChoice = Action.None;

    /*
     *  Creates hand at specified idx in the hands array and at a specified xPos
     *  
     *  A card GameObject can be supplied to start with an initial card; otherwise, 
     *  a random card pulled from the deck becomes the starting card
     */
    private void createHand(int idx = 0, GameObject card = null, float xPos = 0f)
    {
        var gameObject = new GameObject("Hand");
        gameObject.transform.SetParent(this.transform);
        gameObject.transform.localPosition = new Vector3(xPos, handVerticalOffset, 0f);

        var hand = gameObject.AddComponent<Hand>();

        if (card)
        {
            hand.init(card);
        }
        else
        {
            hand.init();
        }

        hands.Insert(idx, gameObject); //Not O(N) only O(4), can't split more than 4 times
    }

    //Split hand at provided index, no sanity checking performed
    private void splitHand(int idx)
    {
        //Take second card ownership from previous hand
        GameObject card = hands[idx].GetComponent<Hand>().split();

        /* Calculate new card positions centered around local position (0, handVerticalOffset, 0) */

        //New hand count for calculating positions
        int handCount = hands.Count + 1;
        float furthestHandDist = (handCount - 1) * handDist / 2;
        
        for (int i = 0, j = 0; i < hands.Count; ++i, ++j)
        {
            //idx + 1 belongs to new hand's spot
            if (i == idx + 1)
            {
                //We're still missing new hand in hands array so skip it's corresponding value
                ++j;
            }

            float xPos = j * handDist - furthestHandDist;
            hands[i].transform.localPosition = new Vector3(xPos, handVerticalOffset, 0f);
        }

        createHand(idx + 1, card, (idx + 1) * handDist - furthestHandDist);
    }

    //Deals first card by creating inital hand; no-op if first card has been dealt already
    public void dealFirstCard()
    {
        if (hands.Count != 0)
        {
            return;
        }

        createHand();
    }

    //Deals second card; expects starting conditions created from calling dealFirstCard otherwise it's no-op
    public void dealSecondCard()
    {
        if (hands.Count != 1)
        {
            return;
        }

        Hand hand = hands.First().GetComponent<Hand>();

        if (hand.cardObjects.Count != 1)
        {
            return;
        }

        hand.hit();
    }

    //Destroys all hands
    public void destroyHands()
    {
        for (int i = 0; i < hands.Count; ++i)
        {
            Destroy(hands[i]);
        }

        hands.Clear();
    }

    /*
     * Play hand using playerChoice values; will wait for changes to playerChoice to continue
     *
     * playerChoice will be reset to Action.None each time it is processed
     * Expects caller to continually assign playerChoice until this function ends
     *
     * Returns final hand values through the retCallback paramter
     */
    public IEnumerator playTurn(System.Action<List<int>> retCallback)
    {
        List<int> handValues = new List<int>();

        for (int i = hands.Count - 1; i >= 0; --i)
        {
            Hand hand = hands[i].GetComponent<Hand>();
            bool endHandTurn = false;

            while (!endHandTurn && hand.getHandValue() < 21)
            {
                yield return new WaitUntil(() => playerChoice != Action.None);

                switch (playerChoice)
                {
                case Action.Hit:
                    hand.hit();
                    break;
                case Action.Stand:
                    endHandTurn = true;
                    break;
                case Action.Double:
                    hand.doubleDown();
                    endHandTurn = true;
                    break;
                case Action.Split:
                    splitHand(i);
                    ++i; //New hand becomes one to the right
                    hand = hands[i].GetComponent<Hand>(); //Switch to new hand now
                    break;
                }

                playerChoice = Action.None;
            }

            handValues.Add(hand.getHandValue());
        }

        retCallback(handValues);
    }
}