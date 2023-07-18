using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : AgentMovement
{
    private GameStateManager manager;
    private EnemyTargetingLogic targetingLogic;

    [SerializeField]
    private float changeTargetTime = 0f;
    [SerializeField]
    private float changeTargetTimeElapsed = 0f;

    [SerializeField]
    private float updateTargetPosTime = 0f;
    [SerializeField]
    private float updateTargetPosTimeElapsed = 0f;

    public float abandonTargetTreshold;
    public float huntTargetTreshold;

    private float targetPriority;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        manager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
        targetingLogic = GetComponentInChildren<EnemyTargetingLogic>();
        //targetingLogic.AddTarget(GameObject.Find("Player"));
    }

    private void Reset()
    {
        changeTargetTimeElapsed = 0f;
        updateTargetPosTimeElapsed = 0f;
    }

    private void UpdateTargetPos()
    {
        if (updateTargetPosTimeElapsed > 0)
        {
            updateTargetPosTimeElapsed -= Time.deltaTime;
            return;
        }

        updateTargetPosTimeElapsed = updateTargetPosTime;

        if (target != null)
        {
            targetPosition = target.position;
        }

        NavMeshPath path = new NavMeshPath();

        agent.CalculatePath(targetPosition, path);
        agent.SetPath(path);

        //agent.SetDestination(targetPosition);
    }

    private void Update()
    {
        if (target != null)
        {
            targetPriority = targetingLogic.GetTargetPriority(target.gameObject);

            if (targetingLogic.GetTargetPriority() > huntTargetTreshold)
            {
                targetPriority = 0f;
            }
        }

        if (changeTargetTimeElapsed > 0 && targetPriority > abandonTargetTreshold)
        {
            UpdateTargetPos();
            changeTargetTimeElapsed -= Time.deltaTime;
            return;
        }

        changeTargetTimeElapsed = changeTargetTime;

        target = targetingLogic.GetTarget();
        targetPosition = targetingLogic.GetTargetPosition();
        targetPriority = targetingLogic.GetTargetPriority();

        UpdateTargetPos();

        //agent.SetDestination(targetPosition);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            manager.LostGame();
            return;
        }

        if (collision.gameObject.GetComponent<NPCLogic>() != null)
        {
            targetingLogic.RemoveTarget(collision.gameObject);
            Destroy(collision.gameObject);
            changeTargetTimeElapsed = 0f;
            return;
        }
    }

    public void Spawn()
    {
        var pos = GetReachablePosition();

        agent.Warp(pos);
        Debug.Log("position = " + pos.ToString());
        Reset();
    }
}
