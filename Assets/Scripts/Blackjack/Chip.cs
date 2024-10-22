using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackjack_Chip : Chip
{
    private Blackjack_Controller controller;
    public GameObject test;
    public int value = 25;

    public override void Awake()
    {
        main = Camera.main;
        controller = GameObject.Find("Controller").GetComponent<Blackjack_Controller>();

        if (!controller)
        {
            Debug.LogError("BLACKJACK_CHIP: Cannot find Controller GameObject w/ Blackjack_Controller script.");
            return;
        }
    }

    public override void OnMouseUp()
    {
        controller.droppedChip(gameObject, gameObject.GetComponent<Collider2D>(), value);
    }
}
