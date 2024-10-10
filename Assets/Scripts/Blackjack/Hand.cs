using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Hand : MonoBehaviour
{
    public List<GameObject> cardObjects = new List<GameObject>();
    protected Deck deck;

    void Awake()
    {
        deck = GameObject.Find("Deck").GetComponent<Deck>();

        if (!deck)
        {
            Debug.LogError("HAND: Cannot find Deck GameObject w/ Deck script.");
            return;
        }
    }

    public abstract void init(params object[] handInitParams);

    //Draw card with correct transformations
    protected void spawnCard(Card card, bool rotate = false)
    {
        var gameObject = new GameObject("Card");
        gameObject.transform.SetParent(this.transform);
        gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        gameObject.transform.localPosition = cardObjects.Last().transform.localPosition + new Vector3(0.25f, 0.25f, 0f);

        if (rotate)
        {
            gameObject.transform.Rotate(0f, 0f, 90f);
        }

        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = card.card;
        spriteRenderer.sortingLayerName = "Cards";
        spriteRenderer.sortingOrder = cardObjects.Count;

        var cardData = gameObject.AddComponent<CardData>();
        cardData.value = card.value;

        cardObjects.Add(gameObject);
    }
}
