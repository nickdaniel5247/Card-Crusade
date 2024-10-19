using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chip : MonoBehaviour
{
    private Camera main;
    public float speed = 20f;

    void Awake()
    {
        main = Camera.main;
    }

    //Change current gameObject from Bank to Chip and respawn Bank
    void OnMouseDown()
    {
        gameObject.name = "Chip";
        GameObject bank = Instantiate(gameObject);
        bank.name = "Bank";
    }

    void OnMouseDrag()
    {
        transform.position = Vector3.Lerp(transform.position, getMousePos(), speed * Time.deltaTime);
    }

    Vector3 getMousePos()
    {
        Vector3 pos = main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;
        return pos;
    }
}
