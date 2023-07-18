using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

[ExecuteInEditMode]
public class SafeZoneLogic : MonoBehaviour
{
    public float priorityDecreaseRate;
    public float priorityIncreaseRate;
    public float rotationSpeed = 0;
    private float angle = 0;
    private List<GameObject> playersSafe = new List<GameObject>();
    private List<GameObject> playersUnsafe = new List<GameObject>();
    EnemyTargetingLogic enemyTargeting;
    public TerrainTool terrain;
    private Collider collider;

    [SerializeField]
    private NPCSpawner npcSpawner;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = terrain.safeZoneRadius * new Vector3(1, 0, 1);
        transform.position = new Vector3(terrain.safeZonePosition.x, transform.position.y, terrain.safeZonePosition.z);
        angle = 0;
        enemyTargeting = GameObject.Find("TargetingSystem").GetComponent<EnemyTargetingLogic>();
        collider = GetComponent<Collider>();
        npcSpawner.Spawn(transform.position, transform.localScale.x);
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
        transform.GetChild(0).transform.rotation = Quaternion.Euler(90, angle, 0);

        SanitizePlayerList(playersSafe);
        SanitizePlayerList(playersUnsafe);

        foreach (var player in playersSafe)
        {
            enemyTargeting.IncreasePriorityForTarget(player, -priorityDecreaseRate * Time.deltaTime);
        }

        foreach (var player in playersUnsafe)
        {
            enemyTargeting.IncreasePriorityForTarget(player, priorityIncreaseRate * Time.deltaTime);
        }
    }

    private void SanitizePlayerList(List<GameObject> playerList)
    {
        for (int i = playerList.Count - 1; i >= 0; i--)
        {
            if (WasDestroyed(playerList[i]))
            {
                playerList.RemoveAt(i);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var obj = other.gameObject;

        if (obj.gameObject.GetComponent<EnemyTarget>() != null)
        {
            playersSafe.Add(obj.gameObject);
            playersUnsafe.Remove(obj.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var obj = other.gameObject;

        if (obj.gameObject.GetComponent<EnemyTarget>() != null)
        {
            playersSafe.Remove(obj.gameObject);
            playersUnsafe.Add(obj.gameObject);
        }
    }

    private static bool WasDestroyed(GameObject obj)
    {
        try
        {
            if (obj.gameObject == null) return true;
        }
        catch (Exception)
        {
            return true;
        }
        return false;
    }
}
