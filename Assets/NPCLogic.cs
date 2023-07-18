using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class NPCLogic : AgentMovement
{
    public enum AIState
    {
        StateRoam,
        StateGoToSafezone,
        StateWaitAtSafezone
    }

    public float retreatTreshold;

    private AIState state = AIState.StateRoam;
    private EnemyTargetingLogic enemyTargeting;
    private Vector3 safezonePos;

    public void SetSafezone(Vector3 pos)
    {
        safezonePos = pos;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        state = AIState.StateRoam;
        enemyTargeting = GameObject.Find("TargetingSystem").GetComponent<EnemyTargetingLogic>();
    }

    private void Update()
    {
        CheckStateChange();
        if (state == AIState.StateRoam)
        {
            Roam();
            return;
        }

        if (state == AIState.StateGoToSafezone)
        {
            GoToSafezone();
            return;
        }
    }

    private void MoveTowardsTarget()
    {
        if (target != null)
        {
            targetPosition = target.position;
        }

        agent.destination = targetPosition;
    }

    private void Roam()
    {
        MoveTowardsTarget();

        if (Vector3.Distance(targetPosition, gameObject.transform.position) <= 5f)
        {
            FindNewTarget();
        }
    }

    private void GoToSafezone()
    {
        target = null;
        targetPosition = safezonePos;

        MoveTowardsTarget();
    }

    private void CheckStateChange()
    {
        if (CheckRoamStart())
        {
            return;
        }

        if (CheckEnemyTargeting())
        {
            return;
        }
    }

    private bool CheckRoamStart()
    {
        if (enemyTargeting.GetTargetPriority(gameObject) == 0f)
        {
            state = AIState.StateRoam;
            return true;
        }
        return false;
    }

    private bool CheckEnemyTargeting()
    {
        if (enemyTargeting.GetTargetPriority(gameObject) >= retreatTreshold)
        {
            state = AIState.StateGoToSafezone;
            return true;
        }
        return false;
    }

    public void FindNewTarget()
    {
        var pos = GetReachablePosition();
        target = null;
        targetPosition = pos;
    }
}
