using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class NPCSpawner
{
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private int numNPC;
    [SerializeField] private GameObject npcParent;

    public void Spawn(Vector3 center, float radius)
    {
        for (int i = 0; i < npcParent.transform.childCount; i++)
        {
            var child = npcParent.transform.GetChild(i);
            GameObject.Destroy(child.gameObject);
        }

        float deltaAngle = 2 * Mathf.PI / numNPC;

        for (int i = 0; i < numNPC; i++) {
            float angleRad = numNPC * deltaAngle;
            Vector3 angle = new Vector3(Mathf.Sin(angleRad), 0, Mathf.Cos(angleRad));
            angle.Normalize();
            Vector3 pos = center + 0.5f * radius * angle;
            var npc = GameObject.Instantiate(npcPrefab, npcParent.transform);
            npc.transform.position = pos;
            npc.GetComponent<NavMeshAgent>().Warp(pos);
            var npcLogic = npc.GetComponent<NPCLogic>();
            npcLogic.FindNewTarget();
            npcLogic.SetSafezone(center);
        }
    }
}
