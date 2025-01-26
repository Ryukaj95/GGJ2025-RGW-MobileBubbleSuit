using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{


    [Header("Areas")]
    [SerializeField] public bool isPaused = false;

    [SerializeField] public Collider2D playerArena;

    [SerializeField] public Collider2D bulletArena;

    [SerializeField] public Collider2D gameArena;

    [SerializeField] public List<Collider2D> spawnAreas = new List<Collider2D>();

    [SerializeField] public List<Collider2D> landingAreas = new List<Collider2D>();

    public Collider2D enemySpawnArea => spawnAreas[UnityEngine.Random.Range(0, spawnAreas.Count)];

    [Header("Stage")]
    [SerializeField] public Points points;

    [SerializeField] public bool stopShooting = false;

    [SerializeField] public List<StageData> stages;

    private int stageIndex = 0;

    public StageData CurrentStage => stages[stageIndex];

    [SerializeField] public bool spacePressed = false;

    [Header("RATING")]
    [SerializeField] public float globalTime = 0;
    [SerializeField] public int hitChain = 0;
    [SerializeField] public int score = 0;
    [SerializeField] public int graze = 0;
    [SerializeField] public int kills = 0;

    [SerializeField] public Canvas endScoreCanvas;
    [SerializeField] public EndGameScore endGameScore;

    public void Start()
    {
        endScoreCanvas.enabled = false;
        isPaused = true;
        stopShooting = true;
        StartCoroutine(StartStage());
    }


    public IEnumerator StartStage()
    {
        isPaused = true;
        ResetScore();
        endGameScore.ClearAll();
        UIManager.Instance.TurnOffLights();
        if (CurrentStage.startDialogue.dialogueBG != null) DialogueManager.Instance.UpdateBackground(CurrentStage.startDialogue.dialogueBG);
        if (CurrentStage.stageBG != null) BackgroundManager.Instance.UpdateBackground(CurrentStage.stageBG);
        if (CurrentStage.startDialogue != null)
        {
            yield return CutsceneManager.Instance.JumpstartDialogue(CurrentStage.startDialogue);
        }
        stopShooting = false;
        isPaused = false;
        WaveManager.Instance.waves = CurrentStage.waves;
        WaveManager.Instance.StartWave();

        yield return new WaitUntil(() => WaveManager.Instance.wavesFinished);
        StartCoroutine(EndStage());
    }

    public void Update()
    {
        PlayerController.Instance.playerControls.Combat.Shoot.performed += _ => spacePressed = true;
        PlayerController.Instance.playerControls.Combat.Shoot.canceled += _ => spacePressed = false;
        UIManager.Instance.SetScore(score);
        UIManager.Instance.SetChainScore(hitChain);
        CalculateFunkoScore();
    }

    public void FixedUpdate()
    {
        if (!isPaused)
        {
            globalTime += Time.deltaTime;
        }
    }

    public IEnumerator EndStage()
    {
        UIManager.Instance.TurnOnLeavingLight();
        isPaused = true;
        yield return new WaitForSeconds(2f);
        if (CurrentStage.endDialogue != null)
        {
            if (CurrentStage.endDialogue.dialogueBG != null) DialogueManager.Instance.UpdateBackground(CurrentStage.endDialogue.dialogueBG);
            yield return CutsceneManager.Instance.JumpstartDialogue(CurrentStage.endDialogue);
        }
        stopShooting = true;
        yield return StartCoroutine(ShowScores());
        WaveManager.Instance.Reset();
        Debug.Log("Ended Stage: " + stageIndex);
        stageIndex++;
        if (stageIndex < stages.Count)
        {
            StartCoroutine(StartStage());
        }
        else
        {
            Win();
        }
    }

    public void Lose()
    {
        Debug.Log("LOOOOOOOOSE");
    }
    public void Win()
    {
        Debug.Log("WIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIN");
    }

    public IEnumerator ShowScores()
    {
        endScoreCanvas.enabled = true;
        string rating = "F";
        int currentFunkoLvl = UIManager.Instance.GetCurrentFunko();
        if (currentFunkoLvl == 0)
        {
            rating = "F";
        }
        else if (currentFunkoLvl == 1)
        {
            rating = "D";
        }
        else if (currentFunkoLvl == 2)
        {
            rating = "C";
        }
        else if (currentFunkoLvl == 3)
        {
            rating = "B";
        }
        else if (currentFunkoLvl == 4)
        {
            rating = "A";
        }
        else if (currentFunkoLvl == 5)
        {
            rating = "S";
        }
        int time = (int)globalTime * 1000;
        endGameScore.SetAll(kills, PlayerController.Instance.health, graze, time, rating, score);
        yield return new WaitUntil(() => spacePressed);
        endScoreCanvas.enabled = false;
    }

    public void DropPoints(UnityEngine.Vector2 pos)
    {
        Instantiate(points, pos, UnityEngine.Quaternion.identity);
    }

    public void AddPoints(int points)
    {
        score += points;
        UIManager.Instance.SetScore(score);
    }

    public void AddKill()
    {
        kills++;
    }

    public void ResetKills()
    {
        kills = 0;
    }

    public void ResetScore()
    {
        score = 0;
        graze = 0;
        hitChain = 0;
        kills = 0;
        globalTime = 0;
    }

    public void CalculateFunkoScore()
    {
        double funkoScore = Math.Truncate(score * 2 + graze + (300 - globalTime) * 100 + kills * 100 + hitChain * 500) * PlayerController.Instance.health;

        if (funkoScore < 25000)
        {
            UIManager.Instance.SetFunko(1);
        }
        else if (funkoScore > 25000 && funkoScore < 50000)
        {
            UIManager.Instance.SetFunko(2);
        }
        else if (funkoScore > 50000 && funkoScore < 70000)
        {
            UIManager.Instance.SetFunko(2);
        }
        else if (funkoScore > 70000 && funkoScore < 100000)
        {
            UIManager.Instance.SetFunko(3);
        }
        else if (funkoScore > 100000 && funkoScore < 150000)
        {
            UIManager.Instance.SetFunko(4);
        }
        else if (funkoScore > 150000)
        {
            UIManager.Instance.SetFunko(5);
        }
        else
        {
            UIManager.Instance.SetFunko(0);
        }
    }


}
