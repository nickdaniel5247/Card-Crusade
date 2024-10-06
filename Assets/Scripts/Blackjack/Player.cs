using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private List<GameObject> hands = new List<GameObject>();
    public float handVerticalOffset = 2f;

    private void createHand(int idx = 0)
    {
        var gameObject = new GameObject("Hand");
        gameObject.transform.SetParent(this.transform);
        gameObject.transform.localPosition = new Vector3(0f, 0f + handVerticalOffset, 0f);

        var hand = gameObject.AddComponent<Hand>();

        hands.Insert(idx, gameObject); //Not O(N) only O(4), can't split more than 4 times
    }

    void splitHand(int idx)
    {
        if (idx < 0 || idx >= hands.Count)
        {
            Debug.LogError("PLAYER: Bad index for split.");
            return;
        }

        Debug.LogError("PLAYER: Split hand operation not implemented.");
    }
}
