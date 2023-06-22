using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform target;

    private GameStateManager manager;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        manager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
    }

    private void Update()
    {
        agent.destination = target.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            manager.LostGame();
        }
    }
}
