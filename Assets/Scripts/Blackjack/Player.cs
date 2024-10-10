using System.Collections;
using System.Collections.Generic;
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

    //Split hand at provided index
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

    public IEnumerator playTurn()
    {
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
        }
    }
}
