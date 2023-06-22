using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;

    private Rigidbody rb;
    private Vector3 movement;
    private Quaternion rotation;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Rotation
        Vector3 mousePosition = Input.mousePosition;

        // Force ScreenToWorld to return the the y coordinate of the player
        // instead of the distance from the camera plane
        mousePosition.z = mainCamera.transform.position.y - transform.position.y;

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        Vector3 direction = mouseWorldPosition - transform.position;
        direction.y = 0;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        rotation = Quaternion.Euler(0, angle, 0);
        rotation.Normalize();

        // Translation
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Normalized movement vector
        movement = new Vector3(h, 0, v).normalized;
        movement *= speed;

    }

    private void FixedUpdate()
    {
        rb.rotation = rotation.normalized;
        rb.velocity = movement;
    }
}
