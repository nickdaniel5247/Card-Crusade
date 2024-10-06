using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Deck : MonoBehaviour
{
    /* This script expects deck to be populated by the editor
    /* The indicies representing cards follow this format:
    /*
    /* 0-12 Clubs
    /* 13-25 Diamonds
    /* 26-38 Hearts
    /* 39-51 Spades
    /*
    /* Inside each of these suit ranges, the ordering goes A, 2, 3, ... 10, J, Q, K
    */

    private static int deckSize = 52;
    private int currentCard = 0;

    public Sprite[] deck = new Sprite[deckSize];

    void Awake()
    {
        foreach (Sprite card in deck) 
        {
            if (card == null) 
            {
                Debug.LogError("Deck is not populated.");
                return;
            }
        }

        shuffle();
    }

    private void shuffle()
    {
        for (int i = 0; i < deckSize; ++i)
        {
            //Randomly chosen index ahead/equal of i
            int idx = i + (int)Random.Range(0, deckSize - i - 1);
            
            Sprite temp = deck[idx];
            deck[idx] = deck[i];
            deck[i] = temp;
        }
    }

    void resetAndShuffle()
    {
        currentCard = 0;
        shuffle();
    }

    Sprite takeCard()
    {
        if (currentCard >= 52)
        {
            Debug.LogError("currentCard index too big, deck exhausted.");
            return null;
        }

        return deck[currentCard++];
    }
}
