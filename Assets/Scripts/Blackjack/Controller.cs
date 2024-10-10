using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    //Editor chosen list of players, for now
    public List<GameObject> players = new List<GameObject>();
    public bool continueGame = true;

    private void dealCards()
    {
        for (int i = 0; i < players.Count; ++i)
        {
            players[i].GetComponent<Player>().dealFirstCard();
        }

        for (int i = 0; i < players.Count; ++i)
        {
            players[i].GetComponent<Player>().dealSecondCard();
        }
    }

    private IEnumerator gameLoop()
    {
        while (continueGame)
        {
            List<List<int>> playerHandValues = new List<List<int>>();

            dealCards();

            //TODO: Add dealer begin round

            for (int i = 0; i < players.Count; ++i)
            {
                yield return players[i].GetComponent<Player>().playTurn((ret) => { playerHandValues.Add(ret); });
            }

            //TODO: Add dealer turn here

            //TODO: Evaluate hands against dealer

            for (int i = 0; i < players.Count; ++i)
            {
                players[i].GetComponent<Player>().destroyHands();
            }

            //TODO: Add dealer end round
        }
    }

    void Start()
    {
        StartCoroutine(gameLoop());
    }
}
