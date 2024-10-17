using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Money : MonoBehaviour
{
    static private TextMeshProUGUI ui_Text;
    static private float _balance = 1000f;

    void Awake()
    {
        ui_Text = this.GetComponent<TextMeshProUGUI>();

        //if (!PlayerPrefs.HasKey("Balance"))
        {
            PlayerPrefs.SetFloat("Balance", balance);
        }

        balance = PlayerPrefs.GetFloat("Balance");
    }

    static public float balance 
    {
        get { return _balance; }
        set 
        {
            _balance = value;
            PlayerPrefs.SetFloat("Balance", value);

            if (ui_Text)
            {
                ui_Text.text = "Balance $" + Math.Truncate(value).ToString("#,0");
            }
        }
    }
}
