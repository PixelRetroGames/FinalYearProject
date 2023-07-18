using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class DirectionScript : MonoBehaviour
{
    public GameObject target = null;
    public GameObject player;
    public float timeToLive;
    public float timeRemaining = 0f;

    public void Activate()
    {
        timeRemaining = timeToLive;
        FindRune();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeRemaining < 0 || target == null)
        {
            gameObject.SetActive(false);
            return;
        }

        transform.position = player.transform.position;
        /*Vector3 relative = transform.position - target.transform.position;
        float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, angle, 0);*/

        timeRemaining -= Time.deltaTime;

        transform.LookAt(target.transform);
    }

    void FindRune()
    {
        var runes = GameObject.FindGameObjectsWithTag("Runestone");
        foreach (var rune in runes)
        {
            if (!rune.GetComponentInChildren<RuneStoneLogic>().IsCompleted())
            {
                target = rune;
                return;
            }
        }
    }
}
