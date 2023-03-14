using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Rotation
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.nearClipPlane; 
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        transform.LookAt(mouseWorldPosition);

        // Translation
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        transform.position += speed * new Vector3(h, 0, v) * Time.deltaTime;
        mainCamera.transform.position = new Vector3(transform.position.x, mainCamera.transform.position.y, transform.position.z);
    }
}