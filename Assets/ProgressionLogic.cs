using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionLogic : MonoBehaviour
{
    public float gameTime;
    public float timeRemaining;

    public int objectivesCompleted = 0;
    private int numObjectives = 0;

    private GameStateManager manager;
    public EnemyTargetingLogic enemyTargeting;
    public GameObject player;

    public GameObject timeText;
    public GameObject sanityBar;
    private bool started = false;

    public void Start()
    {
        manager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
    }

    public void Update()
    {
        if (!started)
        {
            return;
        }

        timeRemaining = Mathf.Max(0, timeRemaining - Time.deltaTime);

        var text = timeText.GetComponent<TextMeshProUGUI>();
        string mins = (((int)timeRemaining) / 60).ToString();
        string secs = (((int)timeRemaining) % 60).ToString();

        // if it's one digit
        if (int.Parse(secs) < 10)
        {
            secs = "0" + secs;
        }

        text.SetText("Time remaining: " + mins.ToString() + ":" + secs.ToString());

        sanityBar.GetComponent<Image>().fillAmount = enemyTargeting.GetTargetPriority(player);

        if (timeRemaining == 0f)
        {
            manager.LostGame();
        }
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
        timeRemaining = gameTime;
        timeText.SetActive(true);
        sanityBar.SetActive(true);
        started = true;
    }

    public void Stop()
    {
        timeText.SetActive(false);
        sanityBar.SetActive(false);
        started = false;
    }
}
