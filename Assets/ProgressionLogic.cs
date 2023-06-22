using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionLogic : MonoBehaviour
{
    public int objectivesCompleted = 0;
    private int numObjectives = 0;

    private GameStateManager manager;

    public void Start()
    {
        manager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
    }

    public void SetNumObjectives(int numObjectives)
    {
        this.numObjectives = numObjectives;
    }

    public void MarkCompleted()
    {
        objectivesCompleted++;
        if (objectivesCompleted == numObjectives)
        {
            manager.WonGame();
        }
    }

    public void Reset()
    {
        objectivesCompleted = 0;
    }
}
