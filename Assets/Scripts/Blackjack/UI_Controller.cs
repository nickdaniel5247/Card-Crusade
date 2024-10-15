using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    private Blackjack_Controller controller;
    public Button hitButton;
    public Button doubleDownButton;
    public Button splitButton;
    public Button standButton;

    void Awake()
    {
        controller = GameObject.Find("Controller").GetComponent<Blackjack_Controller>();

        if (!controller)
        {
            Debug.LogError("UI_CONTROLLER: Cannot find Controller GameObject w/ Blackjack_Controller script.");
            return;
        }

        //Probably should switch to automatically finding them
        if (!hitButton || !splitButton || !doubleDownButton || !standButton)
        {
            Debug.LogError("UI_CONTROLLER: Button(s) not set.");
            return;
        }

        setButtons(true, false, false, true);
    }

    public void setButtons(bool hit, bool doubleDown, bool split, bool stand)
    {
        hitButton.interactable = hit;
        doubleDownButton.interactable = doubleDown;
        splitButton.interactable = split;
        standButton.interactable = stand;
    }

    public void hit()
    {
        controller.playerChoice = Blackjack_Player.Action.Hit;
        setButtons(false, false, false, false);
    }

    public void doubleDown()
    {
        controller.playerChoice = Blackjack_Player.Action.Double;
        setButtons(false, false, false, false);
    }

    public void split()
    {
        controller.playerChoice = Blackjack_Player.Action.Split;
        setButtons(false, false, false, false);
    }

    public void stand()
    {
        controller.playerChoice = Blackjack_Player.Action.Stand;
        setButtons(false, false, false, false);
    }
}