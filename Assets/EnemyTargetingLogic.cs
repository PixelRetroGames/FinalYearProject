using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTargetingLogic : MonoBehaviour
{
    [System.Serializable]
    public class Target
    {
        public bool isStatic;
        public Vector3 staticPos;
        public GameObject obj;
        public float priority;

        public Target(GameObject obj, float priority)
        {
            isStatic = false;
            this.obj = obj;
            this.priority = priority;
        }

        public Target(float priority, Vector3 staticPos)
        {
            isStatic = true;
            this.staticPos = staticPos;
            this.obj = null;
            this.priority = priority;
        }
    }

    public void Start()
    {
        targets.Clear();
        AddRunestones();
        GatherTargets();
    }

    public void Reset()
    {
        Start();
    }

    public void GatherTargets()
    {
        foreach (var target in GameObject.FindObjectsOfType<EnemyTarget>())
        {
            AddTarget(target.gameObject, 0);
        }
    }

    public void AddRunestones()
    {
        foreach (var rune in GameObject.FindObjectsOfType<RuneStoneLogic>()) {
            NavMeshHit hit = new NavMeshHit();
            //Transform transform = Instantiate(rune.transform.parent.transform);
            Vector3 pos = rune.transform.parent.transform.position - 10f * new Vector3(1, 0, 1);
            pos = new Vector3(pos.x, 0, pos.z);
            //transform.position -= 290f * new Vector3(1, 0, 1); 

            if (NavMesh.SamplePosition(pos, out hit, 40f, 0))
            {
                pos = hit.position;
            }

            AddTarget(new Target(0.2f, pos));
            //transform.gameObject.SetActive(false);
        }
    }

    [SerializeField]
    public List<Target> targets = new List<Target>();

    public void SetTargets(List<GameObject> objTargets)
    {
        targets = new List<Target>();
        foreach (var obj in objTargets)
        {
            targets.Add(new Target(obj, 0));
        }
    }

    private void AddTarget(Target target)
    {
        targets.Add(target);
    }

    public void AddTarget(GameObject target, float priority = 0f)
    {
        targets.Add(new Target(target, priority));
    }

    public Transform GetTarget()
    {
        targets.Sort((x, y) => -x.priority.CompareTo(y.priority));

        List<Target> possibleTargets = targets.FindAll(x => x.priority == targets[0].priority);

        var target = targets[UnityEngine.Random.Range(0, possibleTargets.Count - 1)];

        if (!target.isStatic)
        {
            return target.obj.transform;
        }

        return null;
    }

    public float GetTargetPriority()
    {
        targets.Sort((x, y) => -x.priority.CompareTo(y.priority));

        List<Target> possibleTargets = targets.FindAll(x => x.priority == targets[0].priority);

        var target = targets[UnityEngine.Random.Range(0, possibleTargets.Count - 1)];
        
        return target.priority;
    }

    public Vector3 GetTargetPosition()
    {
        targets.Sort((x, y) => -x.priority.CompareTo(y.priority));

        List<Target> possibleTargets = targets.FindAll(x => x.priority == targets[0].priority);

        var target = targets[UnityEngine.Random.Range(0, possibleTargets.Count - 1)];

        if (!target.isStatic)
        {
            return target.obj.transform.position;
        }

        return target.staticPos;
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
        int foundIndex = -1;
        foreach (var target in targets)
        {
            if (target.obj != null && target.obj.Equals(obj))
            {
                foundIndex = i;
                break;
            }
            i++;
        }

        return foundIndex;
    }

    public void SetMaxPriorityForTarget(GameObject obj)
    {
        var i = FindTargetIndex(obj);
        if (i == -1)
        {
            return;
        }
        targets[i].priority = 1;
    }

    public void IncreasePriorityForTarget(GameObject obj, float amount)
    {
        var i = FindTargetIndex(obj);
        if (i == -1)
        {
            return;
        }
        targets[i].priority = Mathf.Min(1f, Mathf.Max(0f, targets[i].priority + amount));
    }

    public void RemoveTarget(GameObject obj)
    {
        var i = FindTargetIndex(obj);
        if (i == -1)
        {
            return;
        }
        targets.RemoveAt(i);
    }

    public float GetTargetPriority(GameObject obj)
    {
        var i = FindTargetIndex(obj);
        if (i == -1)
        {
            return 0f;
        }
        return targets[i].priority;
    }
}
