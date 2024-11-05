using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Blackjack_Player : Participant
{
    public float handDist = 1.5f;
    public int maxSplits = 3;
    public int initalBet = 0;
    public Vector3 cardSpawnOffset = new Vector3(0.25f, 0.25f, 0f);
    public Vector3 cardSpawnPos = new Vector3(-0.25f, 0f, 0f);

    public enum Action
    {
        None,
        Hit,
        Stand,
        Double,
        Split
    };

    public Action playerChoice = Action.None;

    private UI_Controller ui_Controller;

    void Awake()
    {
        ui_Controller = GameObject.Find("Canvas").GetComponent<UI_Controller>();

        if (!ui_Controller)
        {
            Debug.LogError("BLACKJACK_PLAYER: Cannot find Canvas GameObject w/ UI_Controller script.");
            return;
        }
    }

    //Split hand at provided index, no sanity checking performed
    private void splitHand(int idx)
    {
        //Take second card ownership from previous hand
        GameObject card = hands[idx].GetComponent<Blackjack_Hand>().split();

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

        createHand(idx + 1, (idx + 1) * handDist - furthestHandDist, card, cardSpawnOffset, cardSpawnPos);
        hands[idx + 1].GetComponent<Blackjack_Hand>().hit();
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

        Blackjack_Hand hand = hands.First().GetComponent<Blackjack_Hand>();

        if (hand.getCardObjects().Count != 1)
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

    private bool canDouble(Blackjack_Hand hand)
    {
        return (hand.getCardObjects().Count == 2) && (Money.balance >= initalBet);
    }

    private bool canSplit(Blackjack_Hand hand)
    {
        List<GameObject> cardObjects = hand.getCardObjects();

        if (cardObjects.Count != 2 || cardObjects[0].GetComponent<CardData>().value != cardObjects[1].GetComponent<CardData>().value
            || hands.Count >= (maxSplits + 1) || Money.balance < initalBet)
        {
            return false;
        }

        return true;
    }

    /*
     * Play hand using playerChoice values; expects caller to iterate this function
     *
     * playerChoice will be reset to Action.None each time it is processed
     * Expects caller to continually assign playerChoice until this function ends
     *
     * Returns final hand values and bet value for each hand, in that respective order, 
     * through the retCallback paramter
     */
    public IEnumerator playTurn(System.Action<List<(int,int)>> retCallback)
    {
        List<(int,int)> handValues = new List<(int,int)>();

        for (int i = hands.Count - 1; i >= 0; --i)
        {
            Blackjack_Hand hand = hands[i].GetComponent<Blackjack_Hand>();

            //Hand requires hit to start, came from a split
            if (hand.getCardObjects().Count == 1)
            {
                hand.hit();
            }

            bool endHandTurn = false;
            int betValue = initalBet;

            while (!endHandTurn && hand.getHandValue() < 21)
            {
                ui_Controller.setButtons(true, canDouble(hand), canSplit(hand), true);
                yield return null;

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
                    Money.balance -= betValue;
                    betValue *= 2;
                    endHandTurn = true;
                    break;
                case Action.Split:
                    splitHand(i);
                    Money.balance -= initalBet;
                    ++i; //New hand becomes one to the right
                    hand = hands[i].GetComponent<Blackjack_Hand>(); //Switch to new hand now
                    break;
                }

                playerChoice = Action.None;
            }

            handValues.Add((hand.getHandValue(), betValue));
        }

        retCallback(handValues);
    }
}