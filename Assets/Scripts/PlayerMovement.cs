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
    [SerializeField] private GameObject signPrefab;
    [SerializeField] private int maxPlacedSigns;

    private bool canPlaceSign = false;
    private Queue<GameObject> placedSigns = new Queue<GameObject>();

    public void SetSignPlacement(bool status)
    {
        canPlaceSign = status;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    public void Reset()
    {
        var signGroup = GameObject.Find("Signs");
        for (int i = 0; i < signGroup.transform.childCount; i++)
        {
            Destroy(signGroup.transform.GetChild(i).gameObject);
        }

        placedSigns.Clear();
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

        if (canPlaceSign && Input.GetMouseButtonDown(0))
        {
            SpawnSign(mouseWorldPosition);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Reset();
        }
    }

    private void SpawnSign(Vector3 pos)
    {
        var sign = Instantiate(signPrefab, GameObject.Find("Signs").transform);
        sign.transform.position = pos;

        placedSigns.Enqueue(sign);

        if (placedSigns.Count > maxPlacedSigns)
        {
            Destroy(placedSigns.Dequeue());
        }
    }

    private void FixedUpdate()
    {
        rb.rotation = rotation.normalized;
        rb.velocity = movement;
    }
}
