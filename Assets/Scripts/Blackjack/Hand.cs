using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool canSplit = false;
    public List<int> cards = new List<int>();
    public List<GameObject> cardObjects = new List<GameObject>();

    private Deck deck;

    void Awake()
    {
        deck = GameObject.Find("Deck").GetComponent<Deck>();

        if (!deck)
        {
            Debug.LogError("Cannot find Deck GameObject w/ Deck script.");
            return;
        }
    }

    //Draw card with given sprite and correct transformations
    private void spawnCard(Sprite card)
    {
        var gameObject = new GameObject("Card");
        gameObject.transform.SetParent(this.transform);
        gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        gameObject.transform.position = cardObjects.Last().transform.position + new Vector3(0.25f, 0.25f, 0f);

        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = card;
        spriteRenderer.sortingLayerName = "Cards";
    }

    void hit()
    {
        
    }

    void split()
    {

    }

    void doubleDown()
    {

    }

    void Start()
    {
        
    }
}
