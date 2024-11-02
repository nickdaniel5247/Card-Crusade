using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Hand : MonoBehaviour
{
    protected List<GameObject> cardObjects = new List<GameObject>();
    protected Deck deck;
    protected Vector3 cardScale = new Vector3(0.75f, 0.75f, 0.75f);

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
        gameObject.transform.localScale = cardScale;
        gameObject.transform.localPosition = cardObjects.Last().transform.localPosition + new Vector3(0.25f, 0.25f, 0f);

        if (rotate)
        {
            gameObject.transform.localPosition += new Vector3(0.25f, 0.25f, 0f);
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
