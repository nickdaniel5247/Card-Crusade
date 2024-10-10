using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Participant : MonoBehaviour
{
    protected List<GameObject> hands = new List<GameObject>();
    public float handVerticalOffset = 2f;
    public MonoScript Hand;

    private void addHand(GameObject gameObject, System.Type type, params object[] handInitParams)
    {
        Hand hand = (Hand)gameObject.AddComponent(type);
        hand.init(handInitParams);
    }

    /*
     *  Creates hand at specified idx in the hands array and at a specified xPos
     *  
     *  A card GameObject can be supplied to start with an initial card; otherwise, 
     *  a random card pulled from the deck becomes the starting card
     */
    protected void createHand(int idx = 0, float xPos = 0f, params object[] handInitParams)
    {
        var gameObject = new GameObject("Hand");
        gameObject.transform.SetParent(this.transform);
        gameObject.transform.localPosition = new Vector3(xPos, handVerticalOffset, 0f);

        addHand(gameObject, Hand.GetClass(), handInitParams);
        hands.Insert(idx, gameObject); //Not O(N) only O(4), can't split more than 4 times
    }
}
