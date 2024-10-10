using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackjack_Controller : MonoBehaviour
{
    //Editor chosen list of participants, for now
    public List<GameObject> players = new List<GameObject>();
    public GameObject dealer;

    public bool continueGame = true;

    private void dealCards()
    {
        Blackjack_Dealer blackjack_Dealer = dealer.GetComponent<Blackjack_Dealer>();

        for (int i = 0; i < players.Count; ++i)
        {
            players[i].GetComponent<Blackjack_Player>().dealFirstCard();
        }

        blackjack_Dealer.dealFirstCard();

        for (int i = 0; i < players.Count; ++i)
        {
            players[i].GetComponent<Blackjack_Player>().dealSecondCard();
        }

        blackjack_Dealer.dealSecondCard();
    }

    private IEnumerator playTurns()
    {
        List<List<int>> playerHandValues = new List<List<int>>();

        for (int i = 0; i < players.Count; ++i)
        {
            yield return players[i].GetComponent<Blackjack_Player>().playTurn((ret) => { playerHandValues.Add(ret); });
        }

        dealer.GetComponent<Blackjack_Dealer>().revealFirstCard();
        //TODO: Finish dealer turn here
    }

    private void endRound()
    {
        for (int i = 0; i < players.Count; ++i)
        {
            players[i].GetComponent<Blackjack_Player>().destroyHands();
        }

        dealer.GetComponent<Blackjack_Dealer>().destroyHand();
    }

    private IEnumerator gameLoop()
    {
        while (continueGame)
        {
            dealCards();

            yield return playTurns();
            yield return new WaitForSeconds(2f); //Need to see results

            //TODO: Evaluate hands against dealer

            endRound();
        }
    }

    void Start()
    {
        StartCoroutine(gameLoop());
    }
}
