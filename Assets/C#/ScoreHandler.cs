using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Collections;

public class ScoreHandler : MonoBehaviour {

    // InputManager sets players
    [HideInInspector]
    public Player[] players;
    private Goal[] _goals;

    public int[] scores;
    public int[] lives;

    public int[] p1Kills;
    public int[] p2Kills;
    public int[] p3Kills;
    public int[] p4Kills;

    private ContinuesCurve _leaderSpotLight;
    private GameManager _gameManager;

    private void Start()
    {
        Goal[] unorderedGoals = FindObjectsOfType<Goal>();
        _goals = new Goal[unorderedGoals.Length];
        foreach (Goal goal in unorderedGoals)
        {
            _goals[(int)goal.currentPlayer - 1] = goal;
        }

        scores = new int[4] { 0, 0, 0, 0 };
        _gameManager = FindObjectOfType<GameManager>();
        int playerlives = _gameManager.playerLives;
        lives = new int[4] { playerlives, playerlives, playerlives, playerlives };

        p1Kills = new int[4] { 0, 0, 0, 0 };
        p2Kills = new int[4] { 0, 0, 0, 0 };
        p3Kills = new int[4] { 0, 0, 0, 0 };
        p4Kills = new int[4] { 0, 0, 0, 0 };
    }

    public void StageStart()
    {
        Start();
        _leaderSpotLight = GameObject.FindGameObjectWithTag("Player").GetComponent<ContinuesCurve>();
        StartCoroutine(StartCountdown());
    }

    public void LostLive(int player)
    {
        lives[player - 1] = _goals[player - 1].GetCurrentLives();
    }

    public void KillPlayer(int player)
    {
        players[player - 1].Die();

        int livingPLayerCount = 0;
        int winner = -1;
        foreach (Player p in players)
        {
            if (p.isLive)
            {
                livingPLayerCount++;
                winner = (int)p.currentPlayer;
            }
        }

        if (livingPLayerCount == 1)
            StartCoroutine(Restart(winner));
    }

    public void AddScore(int player)
    {
        if (player < 1)
            return;
        _goals[player - 1].AddToScore();
        scores[player - 1] = _goals[player - 1].GetCurrentScore();
    }

    public void ScoreReached(int player)
    {
        foreach (Player p in players)
        {
            if (player == (int)p.currentPlayer)
                continue;

            p.Die();
        }

        StartCoroutine(Restart(player));
    }

    public void AddKill(int killer, int victim)
    {
        if (killer == 1)
            p1Kills[victim - 1]++;
        if (killer == 2)
            p2Kills[victim - 1]++;
        if (killer == 3)
            p3Kills[victim - 1]++;
        if (killer == 4)
            p4Kills[victim - 1]++;
    }

    public void UpdateSpotLight()
    {
        int temp = 0;
        int playerToLight = 1;

        if (_gameManager.currentMode == GameManager.GameMode.Elimination)
        {
            for (int i = 0; i < lives.Length; i++)
            {
                if (lives[i] > temp)
                {
                    temp = lives[i];
                    playerToLight = i + 1;
                }
            }
        }
        if (_gameManager.currentMode == GameManager.GameMode.ScoreRun)
        {
            for (int i = 0; i < scores.Length; i++)
            {
                if (scores[i] > temp)
                {
                    temp = scores[i];
                    playerToLight = i + 1;
                }
            }
        }

        _leaderSpotLight.MoveToPlayer(playerToLight, true);
        return;
    }

    IEnumerator StartCountdown()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(1f);
        yield return new WaitForSecondsRealtime(1f);
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        StopCoroutine(StartCountdown());
    }

    IEnumerator Restart(int winner)
    {
        Debug.Log("Winner is p" + winner);

        yield return new WaitForSeconds(2);

        // Loads active scene
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        SceneManager.LoadScene(0);
        StopAllCoroutines();
    }
}
