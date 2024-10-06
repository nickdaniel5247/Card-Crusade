using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{
    public Sprite card;
    public int value;
}

public class Deck : MonoBehaviour
{
    private static int deckSize = 52;
    private int currentCard = 0;
    
    // This script expects deck to be populated by the editor
    [Header("NOTE: ace should have value 1")] 
    public Card[] deck = new Card[deckSize];

    void shuffle()
    {
        for (int i = 0; i < deckSize; ++i)
        {
            //Randomly chosen index ahead/equal of i
            int idx = i + (int)Random.Range(0, deckSize - i - 1);
            
            Card temp = deck[idx];
            deck[idx] = deck[i];
            deck[i] = temp;
        }
    }

    void Awake()
    {
        foreach (Card card in deck) 
        {
            if (card.card == null) 
            {
                Debug.LogError("Deck is not populated.");
                return;
            }
        }

        shuffle();
    }

    public void resetAndShuffle()
    {
        currentCard = 0;
        shuffle();
    }

    public Card takeCard()
    {
        if (currentCard >= 52)
        {
            Debug.LogError("currentCard index too big, deck exhausted.");
            return null;
        }

        return deck[currentCard++];
    }
}
