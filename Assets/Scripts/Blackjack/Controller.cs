using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackjack_Controller : MonoBehaviour
{
    //Editor chosen list of participants (should be used as slots)
    public List<GameObject> players = new List<GameObject>();
    public GameObject dealer;
    public Blackjack_Player.Action playerChoice = Blackjack_Player.Action.None;
    public float endRoundTime = 5f;

    public AudioClip loseSound;
    private AudioSource audioSource;

    private List<GameObject> chips = new List<GameObject>();
    private Blackjack_Dealer blackjack_Dealer;

    void Awake()
    {
        blackjack_Dealer = dealer.GetComponent<Blackjack_Dealer>();
        audioSource = GetComponent<AudioSource>();
    }

    public void droppedChip(GameObject chip, Collider2D collider, int value)
    {
        if (Money.balance < value)
        {
            Destroy(chip);
            return;
        }

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
                Money.balance -= value;

                if (transform.childCount == 0)
                {
                    chip.transform.SetParent(transform);
                    chip.transform.localPosition = new Vector3();
                    chip.GetComponent<Collider2D>().enabled = false;
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
            Blackjack_Player blackjack_Player = players[i].GetComponent<Blackjack_Player>();

            if (blackjack_Player.initalBet != 0)
            {
                blackjack_Player.dealFirstCard();
            }
        }

        blackjack_Dealer.dealFirstCard();

        for (int i = 0; i < players.Count; ++i)
        {
            Blackjack_Player blackjack_Player = players[i].GetComponent<Blackjack_Player>();

            if (blackjack_Player.initalBet != 0)
            {
                blackjack_Player.dealSecondCard();
            }
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

        for (int i = 0; i < chips.Count; ++i)
        {
            Destroy(chips[i]);
        }
        
        chips.Clear();
    }

    //Evaluate hands against dealer and payout accordingly
    private void evaluateHands(int dealerHandValue, List<List<(int,int)>> playerHandValues)
    {
        //playerHandValues item1 is hand value
        //playerHandValues item2 is amount bet for that hand

        if (players.Count != playerHandValues.Count)
        {
            Debug.LogError("CONTROLLER: players.Count != playerHandValues.Count");
            return;
        }

        bool onlyBusted = true;

        //Payout any wins here
        for (int i = 0; i < players.Count; ++i)
        {
            for (int j = 0; j < playerHandValues[i].Count; ++j)
            {
                //Player busted, no return needed
                if (playerHandValues[i][j].Item1 > 21)
                {
                    continue;
                }

                onlyBusted = false;

                //At this point if either dealer busted or player is higher they win
                if (dealerHandValue > 21 || playerHandValues[i][j].Item1 > dealerHandValue)
                {
                    //Payouts for blackjack are 3/2 not double
                    if (playerHandValues[i][j].Item1 == 21)
                    {
                        Money.balance += playerHandValues[i][j].Item2 * 3/2;
                    }
                    else
                        Money.balance += playerHandValues[i][j].Item2 * 2;
                }
                else if (playerHandValues[i][j].Item1 == dealerHandValue)
                {
                    //Push gets player exact bet back
                    Money.balance += playerHandValues[i][j].Item2;
                }
            }
        }

        if (onlyBusted)
        {
            //Loser busted all their cards
            audioSource.PlayOneShot(loseSound);
        }
    }

    //Plays a round of Blacjack; expects bets to be set once called
    public IEnumerator playRound(System.Action retCallback)
    {
        //No bets placed, don't play round
        if (chips.Count == 0)
        {
            retCallback();
            yield break;
        }

        blackjack_Dealer.shuffleDeck();
        
        dealCards();

        //Play turns

        List<List<(int, int)>> playerHandValues = new List<List<(int,int)>>();

        for (int i = 0; i < players.Count; ++i)
        {
            Blackjack_Player player = players[i].GetComponent<Blackjack_Player>();

            //Don't play turns of those who didn't bet
            if (player.initalBet == 0)
            {
                //Add empty list to allow later functions to work easy
                //This keeps players.Count == playerHandValues.Count
                playerHandValues.Add(new List<(int,int)>());
                continue;
            }

            IEnumerator playTurn = player.playTurn((ret) => { playerHandValues.Add(ret); });

            while (playTurn.MoveNext())
            {
                //Wait for our playerChoice to change and then propogate changes down to Player
                playerChoice = Blackjack_Player.Action.None;
                yield return new WaitUntil(() => playerChoice != Blackjack_Player.Action.None);
                player.playerChoice = playerChoice;
            }
        }

        blackjack_Dealer.revealFirstCard();
        int dealerHandValue = blackjack_Dealer.playTurn();

        //Need to see results
        yield return new WaitForSeconds(endRoundTime);

        //Playing is over

        evaluateHands(dealerHandValue, playerHandValues);

        endRound();

        retCallback();
    }
}