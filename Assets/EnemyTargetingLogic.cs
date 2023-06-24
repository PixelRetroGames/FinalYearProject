using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetingLogic : MonoBehaviour
{
    public class Target
    {
        public GameObject obj;
        public float priority;

        public Target(GameObject obj, float priority)
        {
            this.obj = obj;
            this.priority = priority;
        }
    }

    public List<Target> targets = new List<Target>();

    public void SetTargets(List<GameObject> objTargets)
    {
        targets = new List<Target>();
        foreach (var obj in objTargets)
        {
            targets.Add(new Target(obj, 0));
        }
    }

    public void AddTarget(GameObject target, float priority = 0f)
    {
        targets.Add(new Target(target, priority));
    }

    public Transform GetTarget()
    {
        targets.Sort((x, y) => -x.priority.CompareTo(y.priority));

        List<Target> possibleTargets = targets.FindAll(x => x.priority == targets[0].priority);

        return targets[UnityEngine.Random.Range(0, possibleTargets.Count)].obj.transform;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            var player = collision.gameObject;
            SetMaxPriorityForTarget(player);
        }
    }

    public int FindTargetIndex(GameObject obj)
    {
        int i = 0;
        foreach (var target in targets)
        {
            if (target.obj.Equals(obj))
            {
                return i;
            }
            i++;
        }
        return i;
    }

    public void SetMaxPriorityForTarget(GameObject obj)
    {
        var i = FindTargetIndex(obj);
        targets[i].priority = 1;
    }

    public void IncreasePriorityForTarget(GameObject obj, float amount)
    {
        var i = FindTargetIndex(obj);
        targets[i].priority = Mathf.Min(1f, Mathf.Max(0f, targets[i].priority + amount));
    }
}
