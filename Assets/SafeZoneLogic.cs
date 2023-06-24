using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SafeZoneLogic : MonoBehaviour
{
    public float priorityDecreaseRate;
    public float rotationSpeed = 0;
    private float angle = 0;
    private List<GameObject> playersSafe = new List<GameObject>();
    EnemyTargetingLogic enemyTargeting;
    public TerrainTool terrain;
    private Collider collider;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = terrain.safeZoneRadius * new Vector3(1, 0, 1) * 0.2f;
        transform.position = new Vector3(terrain.safeZonePosition.x, transform.position.y, terrain.safeZonePosition.z);
        angle = 0;
        //enemyTargeting = GameObject.Find("TargetingSystem").GetComponent<EnemyTargetingLogic>();
        collider = GetComponent<Collider>();
    }

    public void Reset()
    {
        Start();
    }

    public void DisableTrigger()
    {
        collider.isTrigger = false;
    }

    public void EnableTrigger()
    {
        collider.isTrigger = true;
    }

    public void OnValidate()
    {
        Start();
    }

    // Update is called once per frame
    void Update()
    {
        angle += rotationSpeed + Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, angle, 0);

        foreach (var player in playersSafe)
        {
            //enemyTargeting.IncreasePriorityForTarget(player, priorityDecreaseRate * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            playersSafe.Add(collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            playersSafe.Remove(collision.gameObject);
        }
    }
}
