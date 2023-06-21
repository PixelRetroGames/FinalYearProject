using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameObject menuWorld;
    public GameObject terrain;
    public GameObject enemy;

    public void StartGame()
    {
        menuWorld.GetComponentInChildren<TerrainCollider>().enabled = false;
        menuWorld.GetComponentInChildren<Terrain>().enabled = false;
        menuWorld.SetActive(false);

        terrain.SetActive(true);
        terrain.GetComponent<TerrainTool>().TriggerRebuild();

        GameObject.Find("Player").transform.position = new Vector3(0, 0.5f, 0);
        GameObject.Find("Progression").GetComponent<ProgressionLogic>().Reset();

        enemy.SetActive(true);
        enemy.transform.position = new Vector3(20, 0, 20);
    }
}
