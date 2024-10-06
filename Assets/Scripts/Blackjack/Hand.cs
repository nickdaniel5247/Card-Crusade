using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<int> cards = new List<int>();
    public List<GameObject> cardObjects = new List<GameObject>();

    private Deck deck;

    void Awake()
    {
        deck = GameObject.Find("Deck").GetComponent<Deck>();

        if (!deck)
        {
            Debug.LogError("HAND: Cannot find Deck GameObject w/ Deck script.");
            return;
        }

        Card card = deck.takeCard();
        cards.Add(card.value);

        var gameObject = new GameObject("Card");
        gameObject.transform.SetParent(this.transform);
        gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        gameObject.transform.localPosition = new Vector3(-0.25f, 0f, 0f);

        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = card.card;
        spriteRenderer.sortingLayerName = "Cards";
        spriteRenderer.sortingOrder = cardObjects.Count;

        cardObjects.Add(gameObject);
    }

    //Draw card with given sprite and correct transformations
    private void spawnCard(Sprite card, bool rotate = false)
    {
        var gameObject = new GameObject("Card");
        gameObject.transform.SetParent(this.transform);
        gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        gameObject.transform.localPosition = cardObjects.Last().transform.position + new Vector3(0.25f, 0.25f, 0f);

        if (rotate)
        {
            gameObject.transform.Rotate(0f, 0f, 90f);
        }

        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = card;
        spriteRenderer.sortingLayerName = "Cards";
        spriteRenderer.sortingOrder = cardObjects.Count;

        cardObjects.Add(gameObject);
    }

    void hit()
    {
        Card card = deck.takeCard();
        spawnCard(card.card);
        cards.Add(card.value);
    }

    void split()
    {
        Debug.LogError("Split not implemented yet.");
    }

    void doubleDown()
    {
        Card card = deck.takeCard();
        spawnCard(card.card, true);
        cards.Add(card.value);
    }
}
