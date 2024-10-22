using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackjack_Controller : MonoBehaviour
{
    //Editor chosen list of participants, for now
    public List<GameObject> players = new List<GameObject>();
    public GameObject dealer;
    public Blackjack_Player.Action playerChoice = Blackjack_Player.Action.None;
    public bool continueGame = true;

    public void droppedChip(GameObject chip, Collider2D collider, int value)
    {
        foreach (GameObject player in players)
        {
            foreach (Transform transform in player.transform)
            {
                if (transform.name != "Bets")
                {
                    continue;
                }

                if (!transform.GetComponent<Collider2D>().IsTouching(collider))
                {
                    continue;
                }

                //We are in a slot
                
                player.GetComponent<Blackjack_Player>().initalBet += value;

                if (transform.childCount == 0)
                {
                    chip.transform.SetParent(transform);
                    chip.transform.localPosition = new Vector3();
                    chip.GetComponent<Chip>().enabled = false;
                    chips.Add(chip);
                    return;
                }

                break;
            }
        }

        //Didn't find slot for bets
        Destroy(chip);
    }

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

    private void endRound()
    {
        for (int i = 0; i < players.Count; ++i)
        {
            players[i].GetComponent<Blackjack_Player>().destroyHands();
        }

        dealer.GetComponent<Blackjack_Dealer>().destroyHand();
    }

    private void evaluateHands(int dealerHandValue, List<List<int>> playerHandValues)
    {
        if (players.Count != playerHandValues.Count)
        {
            Debug.LogError("CONTROLLER: players.Count != playerHandValues.Count");
            return;
        }

        for (int i = 0; i < players.Count; ++i)
        {
            for (int j = 0; j < playerHandValues[i].Count; ++j)
            {
                if (playerHandValues[i][j] > 21)
                {
                    Debug.Log("Player " + i + " lost on hand " + j);
                    continue;
                }

                if (dealerHandValue > 21 || playerHandValues[i][j] > dealerHandValue)
                {
                    Debug.Log("Player " + i + " won on hand " + j);
                }
                else if (playerHandValues[i][j] < dealerHandValue)
                {
                    Debug.Log("Player " + i + " lost on hand " + j);
                }
                else //playerHandValues[i][j] == dealerHandValue
                {
                    Debug.Log("Player " + i + " pushed on hand " + j);
                }
            }
        }
    }

    private IEnumerator gameLoop()
    {
        Blackjack_Dealer blackjack_Dealer = dealer.GetComponent<Blackjack_Dealer>();

        while (continueGame)
        {
            blackjack_Dealer.shuffleDeck();
            
            dealCards();

            //Play turns

            List<List<int>> playerHandValues = new List<List<int>>();

            for (int i = 0; i < players.Count; ++i)
            {
                Blackjack_Player player = players[i].GetComponent<Blackjack_Player>();
                IEnumerator playTurn = player.playTurn((ret) => { playerHandValues.Add(ret); });

                while (playTurn.MoveNext())
                {
                    playerChoice = Blackjack_Player.Action.None;
                    yield return new WaitUntil(() => playerChoice != Blackjack_Player.Action.None);
                    player.playerChoice = playerChoice;
                }
            }

            blackjack_Dealer.revealFirstCard();
            int dealerHandValue = blackjack_Dealer.playTurn();

            yield return new WaitForSeconds(10f); //Need to see results

            //Turns over

            evaluateHands(dealerHandValue, playerHandValues);

            endRound();
        }
    }

    void Start()
    {
        StartCoroutine(gameLoop());
    }
}
