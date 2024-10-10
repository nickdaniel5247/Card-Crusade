using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Blackjack_Hand : Hand
{
    public override void init(params object[] handInitParams)
    {
        //Initialize first card in hand using supplied card GameObject
        if (handInitParams.Count() == 1)
        {
            GameObject givenCard = (GameObject)handInitParams[0];

            givenCard.transform.SetParent(this.transform);
            givenCard.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            givenCard.transform.localPosition = new Vector3(-0.25f, 0f, 0f);
            cardObjects.Add(givenCard);

            return;
        }

        //Initialize first card in hand by drawing random card
        Card card = deck.takeCard();

        var gameObject = new GameObject("Card");
        gameObject.transform.SetParent(this.transform);
        gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        gameObject.transform.localPosition = new Vector3(-0.25f, 0f, 0f);

        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = card.card;
        spriteRenderer.sortingLayerName = "Cards";
        spriteRenderer.sortingOrder = cardObjects.Count;

        var cardData = gameObject.AddComponent<CardData>();
        cardData.value = card.value;

        cardObjects.Add(gameObject);
    }

    public void hit()
    {
        Card card = deck.takeCard();
        spawnCard(card);
    }

    //Splits hand by surrendering ownership of second card to caller
    public GameObject split()
    {
        if (cardObjects.Count != 2)
        {
            Debug.Log("HAND: cardObjects' count doesn't equal 0.");
            return null;
        }

        GameObject card = cardObjects.Last();
        cardObjects.RemoveAt(cardObjects.Count - 1);

        return card;
    }

    public void doubleDown()
    {
        Card card = deck.takeCard();
        spawnCard(card, true);
    }

    public int getHandValue()
    {
        int value = 0;
        bool seenAce = false;

        foreach (GameObject card in cardObjects)
        {
            int cardVal = card.GetComponent<CardData>().value;

            //If we haven't seen an ace and now encounter one, use it as value 11
            if (!seenAce && cardVal == 1)
            {
                cardVal = 11;

                //seenAce is used since only ace can be 11
                seenAce = true;
            }

            value += cardVal;
        }

        //If we are above 21, we can try to lower by changing ace back to 1
        if (seenAce && value > 21)
        {
            value -= 10;
        }

        return value;
    }
}