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

        if (NavMesh.SamplePosition(illegalPos + illegalRange * new Vector3(1, 0, 0), out hit, 30f, 0))
        {
            transform.position = hit.position;
        }

        int maxRetries = 30;

        do
        {
            float angleRad = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
            Vector3 angle = new Vector3(Mathf.Sin(angleRad), 0, Mathf.Cos(angleRad));

            pos = illegalPos + angle * UnityEngine.Random.Range(illegalRange + w / 4, w / 2);

            var x = pos.x;
            var z = pos.z;

            //var x = UnityEngine.Random.Range(-w, w);
            //var z = UnityEngine.Random.Range(-h, h);
            pos = new Vector3(x, 0, z);

            /*if (Vector3.Distance(illegalPos, pos) < illegalRange)
            {
                continue;
            }*/

            if (NavMesh.SamplePosition(pos, out hit, 30f, 0))
            {
                pos = hit.position;
            }

            agent.CalculatePath(pos, path);
            maxRetries--;
        } while (path.status != NavMeshPathStatus.PathComplete && maxRetries > 0);

        agent.Warp(pos);
        Debug.Log("position = " + pos.ToString());

        //targetingLogic.AddTarget(GameObject.Find("Player"));
    }
}
