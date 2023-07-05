using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{
    [SerializeReference] protected Transform target;
    [SerializeReference] protected Vector3 targetPosition;
    protected NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected Vector3 GetReachablePosition()
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

        Vector3 agentPos = agent.transform.position;
        agent.transform.position = illegalPos;

        if (NavMesh.SamplePosition(illegalPos + illegalRange * new Vector3(1, 0, 0), out hit, 30f, 0))
        {
            agent.transform.position = hit.position;
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

        agent.transform.position = agentPos;

        return pos;
    }
}
