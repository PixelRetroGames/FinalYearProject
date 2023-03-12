using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_movement: MonoBehaviour
{
    public float speed;

    private Vector3 velocity;

    private void Start()
    {

    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        //transform.Translate(2.0f * Vector3.forward * Time.deltaTime);
        transform.Translate(speed * new Vector3(h, 0, v) * Time.deltaTime);
    }
}