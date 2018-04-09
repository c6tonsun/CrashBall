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

    private void Start()
    {
        Goal[] unorderedGoals = FindObjectsOfType<Goal>();
        _goals = new Goal[unorderedGoals.Length];
        foreach (Goal goal in unorderedGoals)
        {
            _goals[(int)goal.currentPlayer - 1] = goal;
        }

        scores = new int[4] { 0, 0, 0, 0 };
        int playerlives = FindObjectOfType<GameManager>().playerLives;
        lives = new int[4] { playerlives, playerlives, playerlives, playerlives };
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

    public void AddToScore(int player)
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

    IEnumerator Restart(int winner)
    {
        Debug.Log("Winner is p" + winner);

        yield return new WaitForSeconds(2);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        StopAllCoroutines();
    }
}
