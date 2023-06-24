using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform target;

    private GameStateManager manager;
    private EnemyTargetingLogic targetingLogic;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        manager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
        //targetingLogic = GetComponentInChildren<EnemyTargetingLogic>();
    }

    private void Update()
    {
        //target = targetingLogic.GetTarget();
        agent.destination = target.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            manager.LostGame();
        }
    }

    public void Spawn()
    {
        var terrain = GameObject.Find("Terrain");
        float w = -terrain.transform.position.x;
        float h = -terrain.transform.position.z;

        var safeZone = GameObject.Find("SafeZone");
        var illegalPos = safeZone.transform.position;
        var illegalRange = safeZone.transform.localScale.x;

        Vector3 pos;
        NavMeshPath path = new NavMeshPath();
        NavMeshHit hit = new NavMeshHit();

        transform.position = illegalPos;

        if (NavMesh.SamplePosition(transform.position, out hit, 30f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }

        do
        {
            var x = UnityEngine.Random.Range(-w, w);
            var z = UnityEngine.Random.Range(-h, h);
            pos = new Vector3(x, 0, z);

            if (Vector3.Distance(illegalPos, pos) < illegalRange)
            {
                continue;
            }

            if (NavMesh.SamplePosition(pos, out hit, 30f, NavMesh.AllAreas))
            {
                pos = hit.position;
            }

            agent.CalculatePath(pos, path);
            break;
        } while (path.status != NavMeshPathStatus.PathComplete);

        transform.position = pos;

        //targetingLogic.AddTarget(GameObject.Find("Player"));
    }
}
