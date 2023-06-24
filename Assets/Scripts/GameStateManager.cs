using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameObject menuWorld;
    public GameObject terrain;
    public GameObject enemy;

    public MenuRuneStone exitRune;
    public MenuRuneStone retryRune;

    private enum GameStates
    {
        StillPlaying,
        PlayerWon,
        PlayerLost
    }

    private GameStates state;

    public void Start()
    {
        menuWorld.GetComponentInChildren<TerrainTool>().TriggerRebuild();
    }

    public void StartGame()
    {
        //menuWorld.GetComponentInChildren<TerrainCollider>().enabled = false;
        //menuWorld.GetComponentInChildren<Terrain>().enabled = false;
        menuWorld.SetActive(false);

        terrain.SetActive(true);
        var safezone = GameObject.Find("SafeZone").GetComponent<SafeZoneLogic>();
        safezone.Reset();
        safezone.DisableTrigger();
        enemy.SetActive(true);
        terrain.GetComponent<TerrainTool>().TriggerRebuild();
        safezone.EnableTrigger();

        GameObject.Find("Player").transform.position = new Vector3(0, 0.5f, 0);
        GameObject.Find("Progression").GetComponent<ProgressionLogic>().Reset();
        enemy.GetComponent<EnemyMovement>().Spawn();
    }

    private void Update()
    {
        if (state == GameStates.StillPlaying) {
            return;
        }

        bool won = state== GameStates.PlayerWon;
        state = GameStates.StillPlaying;
        EndGame(won);
    }

    public void WonGame()
    {
        state = GameStates.PlayerWon;
    }

    public void LostGame()
    {
        state = GameStates.PlayerLost;
    }

    private void EndGame(bool won)
    {
        terrain.SetActive(false);

        GameObject.Find("Player").transform.position = new Vector3(0, 0.5f, 0);
        GameObject.Find("Progression").GetComponent<ProgressionLogic>().Reset();

        enemy.SetActive(false);

        menuWorld.SetActive(true);
        menuWorld.GetComponentInChildren<TerrainTool>().TriggerRebuild();

        GameObject.Find("MenuWorld/MenuElements/RetryRune/RetryText").SetActive(!won);
        GameObject.Find("MenuWorld/MenuElements/RetryRune/PlayText").SetActive(won);

        GameObject.Find("MenuWorld/MenuElements/Texts/WinText").SetActive(won);
        GameObject.Find("MenuWorld/MenuElements/Texts/DeathText").SetActive(!won);
        GameObject.Find("MenuWorld/MenuElements/Texts/MenuText").SetActive(false);

        exitRune.Reset();
        retryRune.Reset();
    }
}
