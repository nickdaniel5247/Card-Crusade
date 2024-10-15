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

        hitButton.interactable = true;
        doubleDownButton.interactable = false;
        splitButton.interactable = false;
        standButton.interactable = true;
    }

    public void disableButtons()
    {
        hitButton.interactable = false;
        doubleDownButton.interactable = false;
        splitButton.interactable = false;
        standButton.interactable = false;
    }

    public void hit()
    {
        controller.playerChoice = Blackjack_Player.Action.Hit;
        disableButtons();
    }

    public void doubleDown()
    {
        controller.playerChoice = Blackjack_Player.Action.Double;
        disableButtons();
    }

    public void split()
    {
        controller.playerChoice = Blackjack_Player.Action.Split;
        disableButtons();
    }

    public void stand()
    {
        controller.playerChoice = Blackjack_Player.Action.Stand;
        disableButtons();
    }
}
